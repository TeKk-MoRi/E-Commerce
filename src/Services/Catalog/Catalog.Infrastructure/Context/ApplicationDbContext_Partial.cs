using Catalog.Domain.Entities.Products;
using Catalog.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Context
{
    public partial class ApplicationDbContext
    {
        //public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
    }
}
