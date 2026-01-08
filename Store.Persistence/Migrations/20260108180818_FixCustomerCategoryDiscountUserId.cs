using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Persistence.Migrations
{
    public partial class FixCustomerCategoryDiscountUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Удаляем лишний FK
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerCategoryDiscount_AspNetUsers_ApplicationUserId",
                table: "CustomerCategoryDiscount");

            // Удаляем индекс на ApplicationUserId
            migrationBuilder.DropIndex(
                name: "IX_CustomerCategoryDiscount_ApplicationUserId",
                table: "CustomerCategoryDiscount");

            // Удаляем столбец ApplicationUserId
            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "CustomerCategoryDiscount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Восстанавливаем столбец
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "CustomerCategoryDiscount",
                type: "nvarchar(450)",
                nullable: true);

            // Восстанавливаем индекс
            migrationBuilder.CreateIndex(
                name: "IX_CustomerCategoryDiscount_ApplicationUserId",
                table: "CustomerCategoryDiscount",
                column: "ApplicationUserId");

            // Восстанавливаем FK
            migrationBuilder.AddForeignKey(
                name: "FK_CustomerCategoryDiscount_AspNetUsers_ApplicationUserId",
                table: "CustomerCategoryDiscount",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
