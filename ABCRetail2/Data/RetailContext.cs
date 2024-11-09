using Microsoft.EntityFrameworkCore;
using ABCRetail2.Models;

namespace ABCRetail2.Data
{
    public class RetailContext : DbContext
    {
        public RetailContext(DbContextOptions<RetailContext> options) : base(options) { }

        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CustomerProfile> CustomerProfiles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderMessage> OrderMessages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Contract> Contracts { get; set; }  // New DbSet for Contracts

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Primary key configurations
            modelBuilder.Entity<CartItem>().HasKey(c => c.Id);
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<CustomerProfile>().HasKey(cp => cp.Id);
            modelBuilder.Entity<UserAccount>().HasKey(ua => ua.Id);
            modelBuilder.Entity<Contract>().HasKey(c => c.Id);

            // Property configurations
            modelBuilder.Entity<UserAccount>()
                .Property(ua => ua.Email)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<UserAccount>()
                .Property(ua => ua.Username)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<UserAccount>()
                .Property(ua => ua.Password)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .Property(p => p.Description)
                .HasMaxLength(500);
        }
    }
}
