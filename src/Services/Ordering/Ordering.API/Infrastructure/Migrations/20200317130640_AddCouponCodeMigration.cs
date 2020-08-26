using Microsoft.EntityFrameworkCore.Migrations;

namespace Ordering.API.Infrastructure.Migrations
{
    public partial class AddCouponCodeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Discount",
                schema: "ordering",
                table: "orders",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DiscountCode",
                schema: "ordering",
                table: "orders",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountCode",
                schema: "ordering",
                table: "orders");

            migrationBuilder.AlterColumn<int>(
                name: "Discount",
                schema: "ordering",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
