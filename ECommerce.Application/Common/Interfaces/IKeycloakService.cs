using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Common.Interfaces
{
    public interface IKeycloakService
    {
        // Authentication methods
        Task<Result<KeycloakTokenResponse>> LoginAsync(string username, string password);
        Task<Result<bool>> ValidateTokenAsync(string token);
        Task<Result<bool>> LogoutAsync(string refreshToken);

        // User management methods
        Task<Result<string>> CreateUserAsync(string username, string email, string firstName, string lastName, string password);
        Task<Result<bool>> UpdateUserAsync(string userId, string email, string firstName, string lastName);
        Task<Result<bool>> DeleteUserAsync(string userId);

        // Role management
        Task<Result<bool>> AssignRolesAsync(string userId, List<string> roles);
        Task<Result<bool>> RemoveRolesAsync(string userId, List<string> roles);
        Task<Result<List<KeycloakRoleDto>>> GetRealmRolesAsync();

        // Group management  
        Task<Result<bool>> AssignToGroupAsync(string userId, string groupId);
        Task<Result<bool>> RemoveFromGroupAsync(string userId, string groupId);
        Task<Result<List<KeycloakGroupDto>>> GetGroupsAsync();

        // User queries
        Task<Result<KeycloakUserDto>> GetUserByIdAsync(string userId);
        Task<Result<KeycloakUserDto>> GetUserByUsernameAsync(string username);
        Task<Result<KeycloakUserDto>> GetUserByEmailAsync(string email);
    }

    // Corrected to match Keycloak's actual JSON response
    public record KeycloakTokenResponse(
        string access_token,
        string refresh_token,
        int expires_in,
        int refresh_expires_in,
        string token_type,
        string session_state,
        string scope
    );

    public class KeycloakUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
        public long CreatedTimestamp { get; set; }
    }

    public class KeycloakRoleDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Composite { get; set; }
        public bool ClientRole { get; set; }
        public string ContainerId { get; set; } = string.Empty;
    }

    public class KeycloakGroupDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public List<KeycloakGroupDto>? SubGroups { get; set; }
    }
}
