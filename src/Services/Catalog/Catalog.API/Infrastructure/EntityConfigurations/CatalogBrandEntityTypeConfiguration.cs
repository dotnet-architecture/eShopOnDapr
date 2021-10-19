using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopOnDapr.Services.Catalog.API.Model;

namespace Microsoft.eShopOnDapr.Services.Catalog.API.Infrastructure.EntityConfigurations
{
    public class CatalogBrandEntityTypeConfiguration : IEntityTypeConfiguration<CatalogBrand>
    {
        public void Configure(EntityTypeBuilder<CatalogBrand> builder)
        {
            builder.ToTable("CatalogBrand");

            builder.HasKey(brand => brand.Id);

            builder.Property(brand => brand.Id)
               .UseHiLo("catalog_brand_hilo")
               .IsRequired();

            builder.Property(brand => brand.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasData(
                new CatalogBrand { Id = 1, Name = ".NET" },
                new CatalogBrand { Id = 2, Name = "Dapr" },
                new CatalogBrand { Id = 3, Name = "Other" });
        }
    }
}
