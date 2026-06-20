using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using IdentityAccess.Application.Common;
using IdentityAccess.Application.Common.Errors;
using IdentityAccess.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace IdentityAccess.Infrastructure.Authentication;

public class KeycloakService : IKeycloakService
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakConfig _config;
    private string? _adminToken;
    private DateTime _tokenExpiry;

    public KeycloakService(HttpClient httpClient, IOptions<KeycloakConfig> config)
    {
        _httpClient = httpClient;
        _config = config.Value;
        _httpClient.BaseAddress = new Uri(_config.Authority);
    }

    public async Task<Result<KeycloakTokenResponse>> LoginAsync(string username, string password)
    {
        try
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new("client_id", _config.ClientId),
                new("client_secret", _config.ClientSecret),
                new("username", username),
                new("password", password),
                new("grant_type", "password")
            };

            var response = await _httpClient.PostAsync(
                $"/realms/{_config.Realm}/protocol/openid-connect/token",
                new FormUrlEncodedContent(formData));

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized<KeycloakTokenResponse>(
                    "Keycloak.InvalidCredentials",
                    "Invalid username or password.");
            }

            var content = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();

            return content is null
                ? Failure<KeycloakTokenResponse>(
                    "Keycloak.EmptyTokenResponse",
                    "Keycloak returned an empty token response.")
                : Result<KeycloakTokenResponse>.Success(content);
        }
        catch (Exception ex)
        {
            return Failure<KeycloakTokenResponse>(
                "Keycloak.LoginFailed",
                $"Login failed: {ex.Message}");
        }
    }
    
    public async Task<Result<KeycloakTokenResponse>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new("client_id", _config.ClientId),
                new("client_secret", _config.ClientSecret),
                new("grant_type", "refresh_token"),
                new("refresh_token", refreshToken)
            };

            var response = await _httpClient.PostAsync(
                $"/realms/{_config.Realm}/protocol/openid-connect/token",
                new FormUrlEncodedContent(formData));

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized<KeycloakTokenResponse>(
                    "Keycloak.InvalidRefreshToken",
                    "Refresh token is invalid or expired.");
            }

            var content = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();

            return content is null
                ? Failure<KeycloakTokenResponse>(
                    "Keycloak.EmptyTokenResponse",
                    "Keycloak returned an empty token response.")
                : Result<KeycloakTokenResponse>.Success(
                    content,
                    "Token refreshed successfully.");
        }
        catch (Exception ex)
        {
            return Failure<KeycloakTokenResponse>(
                "Keycloak.RefreshTokenFailed",
                $"Refresh token failed: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ValidateTokenAsync(string token)
    {
        try
        {
            await EnsureAdminTokenAsync();

            var formData = new List<KeyValuePair<string, string>>
            {
                new("token", token),
                new("client_id", _config.ClientId),
                new("client_secret", _config.ClientSecret)
            };

            var response = await _httpClient.PostAsync(
                $"/realms/{_config.Realm}/protocol/openid-connect/token/introspect",
                new FormUrlEncodedContent(formData));

            if (!response.IsSuccessStatusCode)
            {
                return Failure<bool>(
                    "Keycloak.TokenValidationFailed",
                    "Token validation failed.");
            }

            var content = await response.Content.ReadFromJsonAsync<JsonElement>();
            var isValid = content.GetProperty("active").GetBoolean();

            return Result<bool>.Success(isValid);
        }
        catch (Exception ex)
        {
            return Failure<bool>(
                "Keycloak.TokenValidationError",
                $"Token validation error: {ex.Message}");
        }
    }

    public async Task<Result<bool>> LogoutAsync(string refreshToken)
    {
        try
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new("client_id", _config.ClientId),
                new("client_secret", _config.ClientSecret),
                new("refresh_token", refreshToken)
            };

            var response = await _httpClient.PostAsync(
                $"/realms/{_config.Realm}/protocol/openid-connect/logout",
                new FormUrlEncodedContent(formData));

            return response.IsSuccessStatusCode
                ? Result<bool>.Success(true, "User logged out successfully.")
                : Failure<bool>(
                    "Keycloak.LogoutFailed",
                    "Logout failed.");
        }
        catch (Exception ex)
        {
            return Failure<bool>(
                "Keycloak.LogoutError",
                $"Logout error: {ex.Message}");
        }
    }

    public async Task<Result<string>> CreateUserAsync(
        string username,
        string email,
        string firstName,
        string lastName,
        string password)
    {
        try
        {
            await EnsureAdminTokenAsync();

            var userRepresentation = new
            {
                username,
                email,
                firstName,
                lastName,
                enabled = true,
                credentials = new[]
                {
                    new
                    {
                        type = "password",
                        value = password,
                        temporary = false
                    }
                }
            };

            var request = CreateAdminRequest(
                HttpMethod.Post,
                $"/admin/realms/{_config.Realm}/users");

            request.Content = JsonContent.Create(userRepresentation);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                return Failure<string>(
                    "Keycloak.UserCreationFailed",
                    $"User creation failed: {error}");
            }

            var location = response.Headers.Location?.ToString();
            var userId = location?.Split('/').Last();

            return string.IsNullOrWhiteSpace(userId)
                ? Failure<string>(
                    "Keycloak.UserIdMissing",
                    "Failed to get user id from Keycloak response.")
                : Result<string>.Success(userId);
        }
        catch (Exception ex)
        {
            return Failure<string>(
                "Keycloak.UserCreationError",
                $"User creation error: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UpdateUserAsync(
        string userId,
        string email,
        string firstName,
        string lastName)
    {
        try
        {
            await EnsureAdminTokenAsync();

            var userRepresentation = new
            {
                email,
                firstName,
                lastName
            };

            var request = CreateAdminRequest(
                HttpMethod.Put,
                $"/admin/realms/{_config.Realm}/users/{userId}");

            request.Content = JsonContent.Create(userRepresentation);

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode
                ? Result<bool>.Success(true)
                : Failure<bool>(
                    "Keycloak.UserUpdateFailed",
                    "User update failed.");
        }
        catch (Exception ex)
        {
            return Failure<bool>(
                "Keycloak.UserUpdateError",
                $"User update error: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteUserAsync(string userId)
    {
        try
        {
            await EnsureAdminTokenAsync();

            var request = CreateAdminRequest(
                HttpMethod.Delete,
                $"/admin/realms/{_config.Realm}/users/{userId}");

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode
                ? Result<bool>.Success(true)
                : Failure<bool>(
                    "Keycloak.UserDeletionFailed",
                    "User deletion failed.");
        }
        catch (Exception ex)
        {
            return Failure<bool>(
                "Keycloak.UserDeletionError",
                $"User deletion error: {ex.Message}");
        }
    }

    public async Task<Result<bool>> AssignRolesAsync(string userId, List<string> roles)
    {
        try
        {
            await EnsureAdminTokenAsync();

            var availableRolesResult = await GetRealmRolesAsync();

            if (availableRolesResult.IsFailure)
                return Result<bool>.Failure(availableRolesResult.Error);

            var rolesToAssign = availableRolesResult.Data!
                .Where(role => roles.Contains(role.Name))
                .Select(role => new
                {
                    id = role.Id,
                    name = role.Name
                })
                .ToList();

            var request = CreateAdminRequest(
                HttpMethod.Post,
                $"/admin/realms/{_config.Realm}/users/{userId}/role-mappings/realm");

            request.Content = JsonContent.Create(rolesToAssign);

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode
                ? Result<bool>.Success(true)
                : Failure<bool>(
                    "Keycloak.RoleAssignmentFailed",
                    "Role assignment failed.");
        }
        catch (Exception ex)
        {
            return Failure<bool>(
                "Keycloak.RoleAssignmentError",
                $"Role assignment error: {ex.Message}");
        }
    }

    public async Task<Result<bool>> RemoveRolesAsync(string userId, List<string> roles)
    {
        try
        {
            await EnsureAdminTokenAsync();

            var availableRolesResult = await GetRealmRolesAsync();

            if (availableRolesResult.IsFailure)
                return Result<bool>.Failure(availableRolesResult.Error);

            var rolesToRemove = availableRolesResult.Data!
                .Where(role => roles.Contains(role.Name))
                .Select(role => new
                {
                    id = role.Id,
                    name = role.Name
                })
                .ToList();

            var request = CreateAdminRequest(
                HttpMethod.Delete,
                $"/admin/realms/{_config.Realm}/users/{userId}/role-mappings/realm");

            request.Content = JsonContent.Create(rolesToRemove);

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode
                ? Result<bool>.Success(true)
                : Failure<bool>(
                    "Keycloak.RoleRemovalFailed",
                    "Role removal failed.");
        }
        catch (Exception ex)
        {
            return Failure<bool>(
                "Keycloak.RoleRemovalError",
                $"Role removal error: {ex.Message}");
        }
    }

    public async Task<Result<List<KeycloakRoleDto>>> GetRealmRolesAsync()
    {
        try
        {
            await EnsureAdminTokenAsync();

            var request = CreateAdminRequest(
                HttpMethod.Get,
                $"/admin/realms/{_config.Realm}/roles");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return Failure<List<KeycloakRoleDto>>(
                    "Keycloak.GetRolesFailed",
                    "Failed to get realm roles.");
            }

            var roles = await response.Content.ReadFromJsonAsync<List<KeycloakRoleDto>>();

            return Result<List<KeycloakRoleDto>>.Success(
                roles ?? new List<KeycloakRoleDto>());
        }
        catch (Exception ex)
        {
            return Failure<List<KeycloakRoleDto>>(
                "Keycloak.GetRolesError",
                $"Get roles error: {ex.Message}");
        }
    }

    public async Task<Result<bool>> AssignToGroupAsync(string userId, string groupId)
    {
        try
        {
            await EnsureAdminTokenAsync();

            var request = CreateAdminRequest(
                HttpMethod.Put,
                $"/admin/realms/{_config.Realm}/users/{userId}/groups/{groupId}");

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode
                ? Result<bool>.Success(true)
                : Failure<bool>(
                    "Keycloak.GroupAssignmentFailed",
                    "Group assignment failed.");
        }
        catch (Exception ex)
        {
            return Failure<bool>(
                "Keycloak.GroupAssignmentError",
                $"Group assignment error: {ex.Message}");
        }
    }

    public async Task<Result<bool>> RemoveFromGroupAsync(string userId, string groupId)
    {
        try
        {
            await EnsureAdminTokenAsync();

            var request = CreateAdminRequest(
                HttpMethod.Delete,
                $"/admin/realms/{_config.Realm}/users/{userId}/groups/{groupId}");

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode
                ? Result<bool>.Success(true)
                : Failure<bool>(
                    "Keycloak.GroupRemovalFailed",
                    "Group removal failed.");
        }
        catch (Exception ex)
        {
            return Failure<bool>(
                "Keycloak.GroupRemovalError",
                $"Group removal error: {ex.Message}");
        }
    }

    public async Task<Result<List<KeycloakGroupDto>>> GetGroupsAsync()
    {
        try
        {
            await EnsureAdminTokenAsync();

            var request = CreateAdminRequest(
                HttpMethod.Get,
                $"/admin/realms/{_config.Realm}/groups");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return Failure<List<KeycloakGroupDto>>(
                    "Keycloak.GetGroupsFailed",
                    "Failed to get groups.");
            }

            var groups = await response.Content.ReadFromJsonAsync<List<KeycloakGroupDto>>();

            return Result<List<KeycloakGroupDto>>.Success(
                groups ?? new List<KeycloakGroupDto>());
        }
        catch (Exception ex)
        {
            return Failure<List<KeycloakGroupDto>>(
                "Keycloak.GetGroupsError",
                $"Get groups error: {ex.Message}");
        }
    }

    public async Task<Result<KeycloakUserDto>> GetUserByIdAsync(string userId)
    {
        try
        {
            await EnsureAdminTokenAsync();

            var request = CreateAdminRequest(
                HttpMethod.Get,
                $"/admin/realms/{_config.Realm}/users/{userId}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound<KeycloakUserDto>(
                    "Keycloak.UserNotFound",
                    "User was not found.");
            }

            var user = await response.Content.ReadFromJsonAsync<KeycloakUserDto>();

            return user is null
                ? NotFound<KeycloakUserDto>(
                    "Keycloak.UserNotFound",
                    "User was not found.")
                : Result<KeycloakUserDto>.Success(user);
        }
        catch (Exception ex)
        {
            return Failure<KeycloakUserDto>(
                "Keycloak.GetUserError",
                $"Get user error: {ex.Message}");
        }
    }

    public async Task<Result<KeycloakUserDto>> GetUserByUsernameAsync(string username)
    {
        try
        {
            await EnsureAdminTokenAsync();

            var encodedUsername = Uri.EscapeDataString(username);

            var request = CreateAdminRequest(
                HttpMethod.Get,
                $"/admin/realms/{_config.Realm}/users?username={encodedUsername}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound<KeycloakUserDto>(
                    "Keycloak.UserNotFound",
                    "User was not found.");
            }

            var users = await response.Content.ReadFromJsonAsync<List<KeycloakUserDto>>();
            var user = users?.FirstOrDefault();

            return user is null
                ? NotFound<KeycloakUserDto>(
                    "Keycloak.UserNotFound",
                    "User was not found.")
                : Result<KeycloakUserDto>.Success(user);
        }
        catch (Exception ex)
        {
            return Failure<KeycloakUserDto>(
                "Keycloak.GetUserError",
                $"Get user error: {ex.Message}");
        }
    }

    public async Task<Result<KeycloakUserDto>> GetUserByEmailAsync(string email)
    {
        try
        {
            await EnsureAdminTokenAsync();

            var encodedEmail = Uri.EscapeDataString(email);

            var request = CreateAdminRequest(
                HttpMethod.Get,
                $"/admin/realms/{_config.Realm}/users?email={encodedEmail}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound<KeycloakUserDto>(
                    "Keycloak.UserNotFound",
                    "User was not found.");
            }

            var users = await response.Content.ReadFromJsonAsync<List<KeycloakUserDto>>();
            var user = users?.FirstOrDefault();

            return user is null
                ? NotFound<KeycloakUserDto>(
                    "Keycloak.UserNotFound",
                    "User was not found.")
                : Result<KeycloakUserDto>.Success(user);
        }
        catch (Exception ex)
        {
            return Failure<KeycloakUserDto>(
                "Keycloak.GetUserError",
                $"Get user error: {ex.Message}");
        }
    }

    private async Task EnsureAdminTokenAsync()
    {
        if (_adminToken is not null && DateTime.UtcNow < _tokenExpiry)
            return;

        var formData = new List<KeyValuePair<string, string>>
        {
            new("client_id", _config.AdminClientId),
            new("username", _config.AdminUsername),
            new("password", _config.AdminPassword),
            new("grant_type", "password")
        };

        var response = await _httpClient.PostAsync(
            $"/realms/master/protocol/openid-connect/token",
            new FormUrlEncodedContent(formData));

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            throw new UnauthorizedAccessException(
                $"Keycloak admin login failed: {error}");
        }

        var content = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();

        if (content is null)
            throw new InvalidOperationException("Keycloak returned an empty admin token response.");

        _adminToken = content.access_token;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(content.expires_in - 30);
    }

    private HttpRequestMessage CreateAdminRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);
        return request;
    }

    private static Result<T> Failure<T>(string code, string message)
    {
        return Result<T>.Failure(Error.Failure(code, message));
    }

    private static Result<T> Unauthorized<T>(string code, string message)
    {
        return Result<T>.Failure(Error.Unauthorized(code, message));
    }

    private static Result<T> NotFound<T>(string code, string message)
    {
        return Result<T>.Failure(Error.NotFound(code, message));
    }
}