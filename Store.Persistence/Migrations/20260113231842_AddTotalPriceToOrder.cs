using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalPriceToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalPriceAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TotalPriceCurrency",
                table: "Orders",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPriceAmount",
                table: "OrderItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "OriginalPriceCurrency",
                table: "OrderItems",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPriceAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TotalPriceCurrency",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OriginalPriceAmount",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "OriginalPriceCurrency",
                table: "OrderItems");
        }
    }
}
