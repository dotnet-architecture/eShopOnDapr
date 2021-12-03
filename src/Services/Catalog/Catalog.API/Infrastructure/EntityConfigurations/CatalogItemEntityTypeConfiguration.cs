namespace Microsoft.eShopOnDapr.Services.Catalog.API.Infrastructure.EntityConfigurations;

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
            new CatalogItem(1, ".NET Bot Black Hoodie", 19.5M, "1.png", 1, 5, 100),
            new CatalogItem(2, ".NET Black & White Mug", 8.5M, "2.png", 1, 2, 100),
            new CatalogItem(3, "Prism White T-Shirt", 12, "3.png", 3, 5, 100),
            new CatalogItem(4, ".NET Foundation T-shirt", 14.99M, "4.png", 1, 5, 100),
            new CatalogItem(5, "Roslyn Red Pin", 8.5M, "5.png", 3, 3, 100),
            new CatalogItem(6, ".NET Blue Hoodie", 12, "6.png", 1, 5, 100),
            new CatalogItem(7, "Roslyn Red T-Shirt", 12, "7.png", 3, 5, 100),
            new CatalogItem(8, "Kudu Purple Hoodie", 8.5M, "8.png", 3, 5, 100),
            new CatalogItem(9, "Cup<T> White Mug", 12, "9.png", 3, 2, 100),
            new CatalogItem(10, ".NET Foundation Pin", 9, "10.png", 1, 3, 100),
            new CatalogItem(11, "Cup<T> Pin", 8.5M, "11.png", 1, 3, 100),
            new CatalogItem(12, "Prism White TShirt", 12, "12.png", 3, 5, 100),
            new CatalogItem(13, "Modern .NET Black & White Mug", 8.5M, "13.png", 1, 2, 100),
            new CatalogItem(14, "Modern Cup<T> White Mug", 12, "14.png", 1, 2, 100),
            new CatalogItem(15, "Dapr Cap", 9.99M, "15.png", 2, 1, 100),
            new CatalogItem(16, "Dapr Zipper Hoodie", 14.99M, "16.png", 2, 5, 100),
            new CatalogItem(17, "Dapr Logo Sticker", 1.99M, "17.png", 2, 4, 100));
    }
}
