using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopOnDapr.Services.Catalog.API.Model;

namespace Microsoft.eShopOnDapr.Services.Catalog.API.Infrastructure.EntityConfigurations
{
    public class CatalogItemEntityTypeConfiguration : IEntityTypeConfiguration<CatalogItem>
    {
        public void Configure(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("CatalogItem");

            builder.Property(item => item.Id)
                .UseHiLo("catalog_hilo")
                .IsRequired();

            builder.Property(item => item.Name)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(item => item.Price)
                .HasPrecision(4, 2)
                .IsRequired(true);

            builder.Property(item => item.PictureFileName)
                .IsRequired(true);

            builder.HasOne(item => item.CatalogBrand)
                .WithMany()
                .HasForeignKey(item => item.CatalogBrandId);

            builder.HasOne(item => item.CatalogType)
                .WithMany()
                .HasForeignKey(item => item.CatalogTypeId);

            builder.HasData(
                new CatalogItem
                {
                    Id = 1,
                    Name = ".NET Bot Black Hoodie",
                    Price = 19.5M,
                    PictureFileName = "1.png",
                    CatalogBrandId = 1,
                    CatalogTypeId = 5,
                    AvailableStock = 100
                },
                new CatalogItem
                {
                    Id = 2,
                    Name = ".NET Black & White Mug",
                    Price = 8.5M,
                    PictureFileName = "2.png",
                    CatalogBrandId = 1,
                    CatalogTypeId = 2,
                    AvailableStock = 100
                },
                new CatalogItem
                {
                    Id = 3,
                    Name = "Prism White T-Shirt",
                    Price = 12,
                    PictureFileName = "3.png",
                    CatalogBrandId = 3,
                    CatalogTypeId = 5,
                    AvailableStock = 100
                },
                new CatalogItem
                {
                    Id = 4,
                    Name = ".NET Foundation T-shirt",
                    Price = 14.99M,
                    PictureFileName = "4.png",
                    CatalogBrandId = 1,
                    CatalogTypeId = 5,
                    AvailableStock = 100
                },
                new CatalogItem
                {
                    Id = 5,
                    Name = "Roslyn Red Pin",
                    Price = 8.5M,
                    PictureFileName = "5.png",
                    CatalogBrandId = 3,
                    CatalogTypeId = 3,
                    AvailableStock = 100
                },
                new CatalogItem
                {
                    Id = 6,
                    Name = ".NET Blue Hoodie",
                    Price = 12,
                    PictureFileName = "6.png",
                    CatalogBrandId = 1,
                    CatalogTypeId = 5,
                    AvailableStock = 100
                },
                new CatalogItem
                {
                    Id = 7,
                    Name = "Roslyn Red T-Shirt",
                    Price = 12,
                    PictureFileName = "7.png",
                    CatalogBrandId = 3,
                    CatalogTypeId = 5,
                    AvailableStock = 100
                },
                new CatalogItem
                {
                    Id = 8,
                    Name = "Kudu Purple Hoodie",
                    Price = 8.5M,
                    PictureFileName = "8.png",
                    CatalogBrandId = 3,
                    CatalogTypeId = 5,
                    AvailableStock = 100
                },
                new CatalogItem
                {
                    Id = 9,
                    Name = "Cup<T> White Mug",
                    Price = 12,
                    PictureFileName = "9.png",
                    CatalogBrandId = 3,
                    CatalogTypeId = 2,
                    AvailableStock = 100
                },
                new CatalogItem
                {
                    Id = 10,
                    Name = ".NET Foundation Pin",
                    Price = 9,
                    PictureFileName = "10.png",
                    CatalogBrandId = 1,
                    CatalogTypeId = 3,
                    AvailableStock = 100
                },
                new CatalogItem
                {
                    Id = 11,
                    Name = "Cup<T> Pin",
                    Price = 8.5M,
                    PictureFileName = "11.png",
                    CatalogBrandId = 1,
                    CatalogTypeId = 3,
                    AvailableStock = 100
                },
                new CatalogItem
                {
                    Id = 12,
                    Name = "Prism White TShirt",
                    Price = 12,
                    PictureFileName = "12.png",
                    CatalogBrandId = 3,
                    CatalogTypeId = 5,
                    AvailableStock = 100
                },
                new CatalogItem
                {
                    Id = 13,
                    Name = "Modern .NET Black & White Mug",
                    Price = 8.5M,
                    PictureFileName = "13.png",
                    CatalogBrandId = 1,
                    CatalogTypeId = 2,
                    AvailableStock = 100
                },
                new CatalogItem
                {
                    Id = 14,
                    Name = "Modern Cup<T> White Mug",
                    Price = 12,
                    PictureFileName = "14.png",
                    CatalogBrandId = 1,
                    CatalogTypeId = 2,
                    AvailableStock = 100
                },
                new CatalogItem
                {
                    Id = 15,
                    Name = "Dapr Cap",
                    Price = 9.99M,
                    PictureFileName = "15.png",
                    CatalogBrandId = 2,
                    CatalogTypeId = 1,
                    AvailableStock = 100
                },
                new CatalogItem
                {
                    Id = 16,
                    Name = "Dapr Zipper Hoodie",
                    Price = 14.99M,
                    PictureFileName = "16.png",
                    CatalogBrandId = 2,
                    CatalogTypeId = 5,
                    AvailableStock = 100
                },
                new CatalogItem
                {
                    Id = 17,
                    Name = "Dapr Logo Sticker",
                    Price = 1.99M,
                    PictureFileName = "17.png",
                    CatalogBrandId = 2,
                    CatalogTypeId = 4,
                    AvailableStock = 100
                });
        }
    }
}