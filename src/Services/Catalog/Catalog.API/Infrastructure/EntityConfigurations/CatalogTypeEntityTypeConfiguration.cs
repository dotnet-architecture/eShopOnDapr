using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopOnDapr.Services.Catalog.API.Model;

namespace Microsoft.eShopOnDapr.Services.Catalog.API.Infrastructure.EntityConfigurations
{
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
                new CatalogType { Id = 1, Name = "Cap" },
                new CatalogType { Id = 2, Name = "Mug" },
                new CatalogType { Id = 3, Name = "Pin" },
                new CatalogType { Id = 4, Name = "Sticker" },
                new CatalogType { Id = 5, Name = "T-Shirt" });
        }
    }
}
