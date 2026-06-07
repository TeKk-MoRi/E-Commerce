
namespace ECommerce.Infrastructure.Authentication
{
    public class KeycloakConfig
    {
        public string Authority { get; set; } = string.Empty;  // http://localhost:8081
        public string Realm { get; set; } = string.Empty;      // dnsforyou
        public string AdminClientId { get; set; } = "admin-cli";
        public string AdminUsername { get; set; } = string.Empty;
        public string AdminPassword { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}
