namespace Catalog.Application.Common.Interfaces;

public interface IIdentityAccessPermissionClient
{
    Task<bool> HasPermissionAsync(
        string permission,
        CancellationToken cancellationToken = default);
}