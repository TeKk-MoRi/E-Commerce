using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Context
{
    public partial class ApplicationDbContext
    {
        //public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
    }
}
