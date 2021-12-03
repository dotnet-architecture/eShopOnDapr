namespace Microsoft.eShopOnDapr.Services.Ordering.API.Infrastructure.EntityConfigurations;

public class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> orderItemConfiguration)
    {
        orderItemConfiguration.ToTable("OrderItems");

        orderItemConfiguration.HasKey(o => o.Id);

        orderItemConfiguration.Property(o => o.Id)
            .UseHiLo("orderitemseq");

        orderItemConfiguration.Property(item => item.UnitPrice)
            .HasPrecision(4, 2);
    }
}
