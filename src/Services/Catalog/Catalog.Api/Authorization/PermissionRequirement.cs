using Microsoft.AspNetCore.Authorization;

namespace Catalog.Api.Authorization;

public sealed record PermissionRequirement(string Permission)
    : IAuthorizationRequirement;