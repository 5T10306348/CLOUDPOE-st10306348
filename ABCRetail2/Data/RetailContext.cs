using Microsoft.EntityFrameworkCore;
using ABCRetail2.Models;

namespace ABCRetail2.Data
{
    public class RetailContext : DbContext
    {
        public RetailContext(DbContextOptions<RetailContext> options) : base(options)
        {
        }

        // Define DbSets for each model to represent the tables in SQL Server
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CustomerProfile> CustomerProfiles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderMessage> OrderMessages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite keys where needed
            modelBuilder.Entity<CartItem>()
                .HasKey(c => new { c.PartitionKey, c.RowKey });

            modelBuilder.Entity<Order>()
                .HasKey(o => new { o.PartitionKey, o.RowKey });

            modelBuilder.Entity<Product>()
                .HasKey(p => new { p.PartitionKey, p.RowKey });

            modelBuilder.Entity<CustomerProfile>()
                .HasKey(cp => new { cp.PartitionKey, cp.RowKey });

            modelBuilder.Entity<UserAccount>()
                .HasKey(ua => new { ua.PartitionKey, ua.RowKey });

            // Configuring properties for each entity if needed
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .IsRequired();

            modelBuilder.Entity<UserAccount>()
                .Property(ua => ua.Password)
                .IsRequired();

            // Set maximum length for certain properties to mimic original Azure Table Storage limitations
            modelBuilder.Entity<CartItem>().Property(c => c.PartitionKey).HasMaxLength(100);
            modelBuilder.Entity<CartItem>().Property(c => c.RowKey).HasMaxLength(100);
            modelBuilder.Entity<CustomerProfile>().Property(cp => cp.PartitionKey).HasMaxLength(100);
            modelBuilder.Entity<CustomerProfile>().Property(cp => cp.RowKey).HasMaxLength(100);
            modelBuilder.Entity<Order>().Property(o => o.PartitionKey).HasMaxLength(100);
            modelBuilder.Entity<Order>().Property(o => o.RowKey).HasMaxLength(100);
            modelBuilder.Entity<Product>().Property(p => p.PartitionKey).HasMaxLength(100);
            modelBuilder.Entity<Product>().Property(p => p.RowKey).HasMaxLength(100);
            modelBuilder.Entity<UserAccount>().Property(ua => ua.PartitionKey).HasMaxLength(100);
            modelBuilder.Entity<UserAccount>().Property(ua => ua.RowKey).HasMaxLength(100);

            // Further configurations can be added here if needed
        }
    }
}
