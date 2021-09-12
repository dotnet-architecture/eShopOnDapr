using Microsoft.EntityFrameworkCore.Migrations;

namespace Microsoft.eShopOnDapr.Services.Catalog.API.Infrastructure.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "catalog_brand_hilo",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "catalog_hilo",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "catalog_type_hilo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "CatalogBrand",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogBrand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    PictureFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CatalogTypeId = table.Column<int>(type: "int", nullable: false),
                    CatalogBrandId = table.Column<int>(type: "int", nullable: false),
                    AvailableStock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogItem_CatalogBrand_CatalogBrandId",
                        column: x => x.CatalogBrandId,
                        principalTable: "CatalogBrand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogItem_CatalogType_CatalogTypeId",
                        column: x => x.CatalogTypeId,
                        principalTable: "CatalogType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CatalogBrand",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, ".NET" },
                    { 2, "Dapr" },
                    { 3, "Other" }
                });

            migrationBuilder.InsertData(
                table: "CatalogType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Cap" },
                    { 2, "Mug" },
                    { 3, "Pin" },
                    { 4, "Sticker" },
                    { 5, "T-Shirt" }
                });

            migrationBuilder.InsertData(
                table: "CatalogItem",
                columns: new[] { "Id", "AvailableStock", "CatalogBrandId", "CatalogTypeId", "Name", "PictureFileName", "Price" },
                values: new object[,]
                {
                    { 15, 100, 2, 1, "Dapr Cap", "15.png", 9.99m },
                    { 8, 100, 3, 5, "Kudu Purple Hoodie", "8.png", 8.5m },
                    { 7, 100, 3, 5, "Roslyn Red T-Shirt", "7.png", 12m },
                    { 6, 100, 1, 5, ".NET Blue Hoodie", "6.png", 12m },
                    { 4, 100, 1, 5, ".NET Foundation T-shirt", "4.png", 14.99m },
                    { 3, 100, 3, 5, "Prism White T-Shirt", "3.png", 12m },
                    { 1, 100, 1, 5, ".NET Bot Black Hoodie", "1.png", 19.5m },
                    { 12, 100, 3, 5, "Prism White TShirt", "12.png", 12m },
                    { 17, 100, 2, 4, "Dapr Logo Sticker", "17.png", 1.99m },
                    { 10, 100, 1, 3, ".NET Foundation Pin", "10.png", 9m },
                    { 5, 100, 3, 3, "Roslyn Red Pin", "5.png", 8.5m },
                    { 14, 100, 1, 2, "Modern Cup<T> White Mug", "14.png", 12m },
                    { 13, 100, 1, 2, "Modern .NET Black & White Mug", "13.png", 8.5m },
                    { 9, 100, 3, 2, "Cup<T> White Mug", "9.png", 12m },
                    { 2, 100, 1, 2, ".NET Black & White Mug", "2.png", 8.5m },
                    { 11, 100, 1, 3, "Cup<T> Pin", "11.png", 8.5m },
                    { 16, 100, 2, 5, "Dapr Zipper Hoodie", "16.png", 14.99m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItem_CatalogBrandId",
                table: "CatalogItem",
                column: "CatalogBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItem_CatalogTypeId",
                table: "CatalogItem",
                column: "CatalogTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogItem");

            migrationBuilder.DropTable(
                name: "CatalogBrand");

            migrationBuilder.DropTable(
                name: "CatalogType");

            migrationBuilder.DropSequence(
                name: "catalog_brand_hilo");

            migrationBuilder.DropSequence(
                name: "catalog_hilo");

            migrationBuilder.DropSequence(
                name: "catalog_type_hilo");
        }
    }
}
