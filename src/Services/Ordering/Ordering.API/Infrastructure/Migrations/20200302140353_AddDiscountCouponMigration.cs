using Microsoft.EntityFrameworkCore.Migrations;

namespace Ordering.API.Infrastructure.Migrations
{
    public partial class AddDiscountCouponMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Discount",
                schema: "ordering",
                table: "orders",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                schema: "ordering",
                table: "orders");
        }
    }
}
