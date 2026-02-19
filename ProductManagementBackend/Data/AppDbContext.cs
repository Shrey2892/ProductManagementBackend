using global::ProductManagementBackend.Models;
using Microsoft.EntityFrameworkCore;
using ProductManagementBackend.Models;
namespace ProductManagementBackend.Data
{



    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
    }
}