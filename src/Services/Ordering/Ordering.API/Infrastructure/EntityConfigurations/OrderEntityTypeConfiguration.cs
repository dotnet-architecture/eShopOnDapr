using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopOnDapr.Services.Ordering.API.Model;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.Infrastructure.EntityConfigurations
{
    class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> orderConfiguration)
        {
            orderConfiguration.ToTable("Orders");

            orderConfiguration.HasKey(o => o.Id);

            orderConfiguration.HasAlternateKey(o => o.OrderNumber);

            orderConfiguration.Property(o => o.OrderNumber)
                .UseHiLo("orderseq");

            orderConfiguration
                .OwnsOne(o => o.Address, a =>
                {
                    a.WithOwner();
                });
        }
    }
}
