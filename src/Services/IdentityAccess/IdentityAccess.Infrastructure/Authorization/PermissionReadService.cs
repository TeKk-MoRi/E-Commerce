using IdentityAccess.Application.Authorization.DTOs;
using IdentityAccess.Application.Common.Interfaces;
using IdentityAccess.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace IdentityAccess.Infrastructure.Authorization;

public sealed class PermissionReadService(
    IdentityAccessDbContext dbContext)
    : IPermissionReadService
{
    public async Task<IReadOnlyCollection<PermissionDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Permissions
            .AsNoTracking()
            .OrderBy(permission => permission.Code)
            .Select(permission => new PermissionDto(
                permission.Id,
                permission.Code,
                permission.Description))
            .ToListAsync(cancellationToken);
    }
}