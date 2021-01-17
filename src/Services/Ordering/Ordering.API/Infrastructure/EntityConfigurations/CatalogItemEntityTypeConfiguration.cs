//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Microsoft.eShopOnContainers.Services.Ordering.API.Model;

//namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.EntityConfigurations
//{
//    class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
//    {
//        public void Configure(EntityTypeBuilder<Order> builder)
//        {
//            builder.ToTable("Order");

//            builder.Property(o => o.Id)
//                .UseHiLo("catalog_hilo")
//                .IsRequired();

//            //builder.Property(o => o.BuyerId)
//            //    .Has
//            //    .IsRequired(true)
//            //    .HasMaxLength(50);

//            //builder.Property(o => o.BuyerName)
//            //    .IsRequired(true)
//            //    .HasMaxLength(50);

//            //builder.Property(ci => ci.Price)
//            //    .IsRequired(true);

//            //builder.Property(ci => ci.PictureFileName)
//            //    .IsRequired(false);

//            //builder.Ignore(ci => ci.PictureUri);

//            //builder.HasOne(ci => ci.CatalogBrand)
//            //    .WithMany()
//            //    .HasForeignKey(ci => ci.CatalogBrandId);

//            //builder.HasOne(ci => ci.CatalogType)
//            //    .WithMany()
//            //    .HasForeignKey(ci => ci.CatalogTypeId);
//        }
//    }
//}
