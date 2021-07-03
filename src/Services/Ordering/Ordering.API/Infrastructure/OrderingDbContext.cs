using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.EntityConfigurations;
using Microsoft.eShopOnContainers.Services.Ordering.API.Model;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure
{
    public sealed class OrderingDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public OrderingDbContext(DbContextOptions<OrderingDbContext> options)
            : base(options)
        {
            // No need for change tracking.
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().OwnsOne(o => o.Address);

            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
        }
    }
}
