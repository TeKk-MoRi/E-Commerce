using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Common;
public interface IUnitOfWork
{
    Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IApplicationUnitOfWork : IUnitOfWork
{
   // DbSet<User> Users { get; }
    DbSet<Product> Products { get; }
}
