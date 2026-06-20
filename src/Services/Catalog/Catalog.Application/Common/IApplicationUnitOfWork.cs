using Catalog.Domain.Entities.Products;
using Catalog.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Common;
public interface IUnitOfWork
{
    Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IApplicationUnitOfWork : IUnitOfWork
{
   // DbSet<User> Users { get; }
    DbSet<Product> Products { get; }
}
