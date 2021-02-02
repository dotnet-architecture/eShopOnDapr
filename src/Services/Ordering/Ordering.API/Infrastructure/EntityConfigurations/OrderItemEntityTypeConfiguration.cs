using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopOnContainers.Services.Ordering.API.Model;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.EntityConfigurations
{
    class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> orderItemConfiguration)
        {
            orderItemConfiguration.ToTable("orderItems", OrderingContext.DEFAULT_SCHEMA);

            orderItemConfiguration.HasKey(o => o.Id);

            orderItemConfiguration.Property(o => o.Id)
                .UseHiLo("orderitemseq");
        }
    }
}
