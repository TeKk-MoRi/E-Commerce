using IdentityAccess.Application.Authorization;

namespace IdentityAccess.Application.Common.Interfaces;

public interface IPermissionReadService
{
    Task<IReadOnlyCollection<PermissionDto>> GetAllAsync(
        CancellationToken cancellationToken = default);
}