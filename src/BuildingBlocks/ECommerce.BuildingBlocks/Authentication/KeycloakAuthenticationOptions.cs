namespace ECommerce.BuildingBlocks.Authentication;

public sealed class KeycloakAuthenticationOptions
{
    public string Authority { get; init; } = string.Empty;

    public string Realm { get; init; } = string.Empty;

    public string ClientId { get; init; } = string.Empty;

    public bool RequireHttpsMetadata { get; init; }
}