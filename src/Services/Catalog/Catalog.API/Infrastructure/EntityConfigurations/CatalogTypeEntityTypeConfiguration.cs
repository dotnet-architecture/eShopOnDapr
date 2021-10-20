namespace Microsoft.eShopOnDapr.Services.Catalog.API.Infrastructure.EntityConfigurations;

public class CatalogTypeEntityTypeConfiguration : IEntityTypeConfiguration<CatalogType>
{
    public void Configure(EntityTypeBuilder<CatalogType> builder)
    {
        builder.ToTable("CatalogType");

        builder.HasKey(type => type.Id);

        builder.Property(type => type.Id)
            .UseHiLo("catalog_type_hilo")
            .IsRequired();

        builder.Property(type => type.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasData(
            new CatalogType(1, "Cap"),
            new CatalogType(2, "Mug"),
            new CatalogType(3, "Pin"),
            new CatalogType(4, "Sticker"),
            new CatalogType(5, "T-Shirt"));
    }
}
