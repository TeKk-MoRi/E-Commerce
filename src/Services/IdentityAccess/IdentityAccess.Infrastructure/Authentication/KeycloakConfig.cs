
namespace IdentityAccess.Infrastructure.Authentication
{

    public sealed class KeycloakConfig
    {
        public string BaseUrl { get; init; } = string.Empty;
        public string Authority { get; init; } = string.Empty;
        public string Realm { get; init; } = string.Empty;

        public string AdminRealm { get; init; } = "master";
        public string AdminClientId { get; init; } = "admin-cli";
        public string AdminUsername { get; init; } = string.Empty;
        public string AdminPassword { get; init; } = string.Empty;

        public string ClientId { get; init; } = string.Empty;
        public string ClientSecret { get; init; } = string.Empty;
    }
}
