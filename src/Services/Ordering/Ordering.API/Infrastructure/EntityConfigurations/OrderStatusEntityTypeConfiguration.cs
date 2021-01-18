//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure;
//using Microsoft.eShopOnContainers.Services.Ordering.API.Model;

//namespace Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.EntityConfigurations
//{
//    class OrderStatusEntityTypeConfiguration : IEntityTypeConfiguration<OrderStatusState>
//    {
//        public void Configure(EntityTypeBuilder<OrderStatusState> orderStatusConfiguration)
//        {
//            orderStatusConfiguration.ToTable("orderstatus", OrderingContext.DEFAULT_SCHEMA);

//            orderStatusConfiguration.HasKey(o => o.Id);

//            orderStatusConfiguration.Property(o => o.Id)
//                .HasDefaultValue(1)
//                .ValueGeneratedNever()
//                .IsRequired();

//            orderStatusConfiguration.Property(o => o.Name)
//                .HasMaxLength(200)
//                .IsRequired();
//        }
//    }
//}
