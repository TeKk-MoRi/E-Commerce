using ECommerce.Application.Common;
using ECommerce.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Web;

namespace ECommerce.Infrastructure.Authentication
{
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
                    return Result<KeycloakTokenResponse>.Failure("Invalid credentials");

                var content = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();
                return Result<KeycloakTokenResponse>.Success(content!);
            }
            catch (Exception ex)
            {
                return Result<KeycloakTokenResponse>.Failure($"Login failed: {ex.Message}");
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
                    return Result<bool>.Failure("Token validation failed");

                var content = await response.Content.ReadFromJsonAsync<JsonElement>();
                var isValid = content.GetProperty("active").GetBoolean();
                return Result<bool>.Success(isValid);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Token validation error: {ex.Message}");
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
                    ? Result<bool>.Success(true)
                    : Result<bool>.Failure("Logout failed");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Logout error: {ex.Message}");
            }
        }

        public async Task<Result<string>> CreateUserAsync(string username, string email, string firstName, string lastName, string password)
        {
            try
            {
                await EnsureAdminTokenAsync();

                var userRepresentation = new
                {
                    username = username,
                    email = email,
                    firstName = firstName,
                    lastName = lastName,
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

                var request = CreateAdminRequest(HttpMethod.Post, $"/admin/realms/{_config.Realm}/users");
                request.Content = JsonContent.Create(userRepresentation);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return Result<string>.Failure($"User creation failed: {error}");
                }

                // Extract user ID from location header
                var location = response.Headers.Location?.ToString();
                var userId = location?.Split('/').Last() ?? throw new Exception("Failed to get user ID from response");
                return Result<string>.Success(userId);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"User creation error: {ex.Message}");
            }
        }

        public async Task<Result<bool>> UpdateUserAsync(string userId, string email, string firstName, string lastName)
        {
            try
            {
                await EnsureAdminTokenAsync();

                var userRepresentation = new
                {
                    email = email,
                    firstName = firstName,
                    lastName = lastName
                };

                var request = CreateAdminRequest(HttpMethod.Put, $"/admin/realms/{_config.Realm}/users/{userId}");
                request.Content = JsonContent.Create(userRepresentation);

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode
                    ? Result<bool>.Success(true)
                    : Result<bool>.Failure("User update failed");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"User update error: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteUserAsync(string userId)
        {
            try
            {
                await EnsureAdminTokenAsync();

                var request = CreateAdminRequest(HttpMethod.Delete, $"/admin/realms/{_config.Realm}/users/{userId}");
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode
                    ? Result<bool>.Success(true)
                    : Result<bool>.Failure("User deletion failed");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"User deletion error: {ex.Message}");
            }
        }

        public async Task<Result<bool>> AssignRolesAsync(string userId, List<string> roles)
        {
            try
            {
                await EnsureAdminTokenAsync();

                var availableRolesResult = await GetRealmRolesAsync();
                if (availableRolesResult.IsFailure)
                    return Result<bool>.Failure(availableRolesResult.Message);

                var rolesToAssign = availableRolesResult.Data
                    .Where(r => roles.Contains(r.Name))
                    .Select(r => new { id = r.Id, name = r.Name })
                    .ToList();

                var request = CreateAdminRequest(HttpMethod.Post,
                    $"/admin/realms/{_config.Realm}/users/{userId}/role-mappings/realm");
                request.Content = JsonContent.Create(rolesToAssign);

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode
                    ? Result<bool>.Success(true)
                    : Result<bool>.Failure("Role assignment failed");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Role assignment error: {ex.Message}");
            }
        }

        public async Task<Result<bool>> RemoveRolesAsync(string userId, List<string> roles)
        {
            try
            {
                await EnsureAdminTokenAsync();

                var availableRolesResult = await GetRealmRolesAsync();
                if (availableRolesResult.IsFailure)
                    return Result<bool>.Failure(availableRolesResult.Message);

                var rolesToRemove = availableRolesResult.Data
                    .Where(r => roles.Contains(r.Name))
                    .Select(r => new { id = r.Id, name = r.Name })
                    .ToList();

                var request = CreateAdminRequest(HttpMethod.Delete,
                    $"/admin/realms/{_config.Realm}/users/{userId}/role-mappings/realm");
                request.Content = JsonContent.Create(rolesToRemove);

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode
                    ? Result<bool>.Success(true)
                    : Result<bool>.Failure("Role removal failed");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Role removal error: {ex.Message}");
            }
        }

        public async Task<Result<List<KeycloakRoleDto>>> GetRealmRolesAsync()
        {
            try
            {
                await EnsureAdminTokenAsync();

                var request = CreateAdminRequest(HttpMethod.Get, $"/admin/realms/{_config.Realm}/roles");
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return Result<List<KeycloakRoleDto>>.Failure("Failed to get roles");

                var roles = await response.Content.ReadFromJsonAsync<List<KeycloakRoleDto>>();
                return Result<List<KeycloakRoleDto>>.Success(roles ?? new List<KeycloakRoleDto>());
            }
            catch (Exception ex)
            {
                return Result<List<KeycloakRoleDto>>.Failure($"Get roles error: {ex.Message}");
            }
        }

        public async Task<Result<bool>> AssignToGroupAsync(string userId, string groupId)
        {
            try
            {
                await EnsureAdminTokenAsync();

                var request = CreateAdminRequest(HttpMethod.Put,
                    $"/admin/realms/{_config.Realm}/users/{userId}/groups/{groupId}");
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode
                    ? Result<bool>.Success(true)
                    : Result<bool>.Failure("Group assignment failed");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Group assignment error: {ex.Message}");
            }
        }

        public async Task<Result<bool>> RemoveFromGroupAsync(string userId, string groupId)
        {
            try
            {
                await EnsureAdminTokenAsync();

                var request = CreateAdminRequest(HttpMethod.Delete,
                    $"/admin/realms/{_config.Realm}/users/{userId}/groups/{groupId}");
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode
                    ? Result<bool>.Success(true)
                    : Result<bool>.Failure("Group removal failed");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Group removal error: {ex.Message}");
            }
        }

        public async Task<Result<List<KeycloakGroupDto>>> GetGroupsAsync()
        {
            try
            {
                await EnsureAdminTokenAsync();

                var request = CreateAdminRequest(HttpMethod.Get, $"/admin/realms/{_config.Realm}/groups");
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return Result<List<KeycloakGroupDto>>.Failure("Failed to get groups");

                var groups = await response.Content.ReadFromJsonAsync<List<KeycloakGroupDto>>();
                return Result<List<KeycloakGroupDto>>.Success(groups ?? new List<KeycloakGroupDto>());
            }
            catch (Exception ex)
            {
                return Result<List<KeycloakGroupDto>>.Failure($"Get groups error: {ex.Message}");
            }
        }

        public async Task<Result<KeycloakUserDto>> GetUserByIdAsync(string userId)
        {
            try
            {
                await EnsureAdminTokenAsync();

                var request = CreateAdminRequest(HttpMethod.Get, $"/admin/realms/{_config.Realm}/users/{userId}");
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return Result<KeycloakUserDto>.Failure("User not found");

                var user = await response.Content.ReadFromJsonAsync<KeycloakUserDto>();
                return user != null
                    ? Result<KeycloakUserDto>.Success(user)
                    : Result<KeycloakUserDto>.Failure("User not found");
            }
            catch (Exception ex)
            {
                return Result<KeycloakUserDto>.Failure($"Get user error: {ex.Message}");
            }
        }

        public async Task<Result<KeycloakUserDto>> GetUserByUsernameAsync(string username)
        {
            try
            {
                await EnsureAdminTokenAsync();

                var encodedUsername = HttpUtility.UrlEncode(username);
                var request = CreateAdminRequest(HttpMethod.Get,
                    $"/admin/realms/{_config.Realm}/users?username={encodedUsername}");
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return Result<KeycloakUserDto>.Failure("User not found");

                var users = await response.Content.ReadFromJsonAsync<List<KeycloakUserDto>>();
                var user = users?.FirstOrDefault();
                return user != null
                    ? Result<KeycloakUserDto>.Success(user)
                    : Result<KeycloakUserDto>.Failure("User not found");
            }
            catch (Exception ex)
            {
                return Result<KeycloakUserDto>.Failure($"Get user error: {ex.Message}");
            }
        }

        public async Task<Result<KeycloakUserDto>> GetUserByEmailAsync(string email)
        {
            try
            {
                await EnsureAdminTokenAsync();

                var encodedEmail = HttpUtility.UrlEncode(email);
                var request = CreateAdminRequest(HttpMethod.Get,
                    $"/admin/realms/{_config.Realm}/users?email={encodedEmail}");
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return Result<KeycloakUserDto>.Failure("User not found");

                var users = await response.Content.ReadFromJsonAsync<List<KeycloakUserDto>>();
                var user = users?.FirstOrDefault();
                return user != null
                    ? Result<KeycloakUserDto>.Success(user)
                    : Result<KeycloakUserDto>.Failure("User not found");
            }
            catch (Exception ex)
            {
                return Result<KeycloakUserDto>.Failure($"Get user error: {ex.Message}");
            }
        }

        private async Task EnsureAdminTokenAsync()
        {
            if (_adminToken != null && DateTime.UtcNow < _tokenExpiry) return;

            var formData = new List<KeyValuePair<string, string>>
            {
                new("client_id", _config.ClientId), // Use main client instead of admin-cli
                new("client_secret", _config.ClientSecret), // Include client secret
                new("username", _config.AdminUsername),
                new("password", _config.AdminPassword),
                new("grant_type", "password")
            };
            var response = await _httpClient.PostAsync(
                $"/realms/{_config.Realm}/protocol/openid-connect/token", // Use your realm, not master
                new FormUrlEncodedContent(formData));



            if (!response.IsSuccessStatusCode)
                throw new UnauthorizedAccessException("Admin login failed");

            var content = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();
            _adminToken = content!.access_token;
            _tokenExpiry = DateTime.UtcNow.AddSeconds(content.expires_in - 30);
        }

        private HttpRequestMessage CreateAdminRequest(HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);
            return request;
        }
    }
}
