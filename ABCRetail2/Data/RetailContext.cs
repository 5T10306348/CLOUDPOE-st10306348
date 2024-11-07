using Microsoft.EntityFrameworkCore;
using ABCRetail2.Models;

namespace ABCRetail2.Data
{
    public class RetailContext : DbContext
    {
        public RetailContext(DbContextOptions<RetailContext> options) : base(options)
        {
        }

        // Define DbSets for each model you want to include in the database
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CustomerProfile> CustomerProfiles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderMessage> OrderMessages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
    }
}
