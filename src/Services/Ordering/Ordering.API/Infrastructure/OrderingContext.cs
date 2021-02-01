using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.EntityConfigurations;
using Microsoft.eShopOnContainers.Services.Ordering.API.Model;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure
{
    public class OrderingContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "ordering";

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CardType> CardTypes { get; set; }

        public OrderingContext(DbContextOptions<OrderingContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().OwnsOne(o => o.Address);

            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CardTypeEntityTypeConfiguration());
        }
    }
}
