using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderItemOwnedMoneyTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_CustomerCategoryDiscount_AspNetUsers_ApplicationUserId",
            //     table: "CustomerCategoryDiscount");

            // migrationBuilder.DropIndex(
            //     name: "IX_CustomerCategoryDiscount_ApplicationUserId",
            //     table: "CustomerCategoryDiscount");

            // migrationBuilder.DropColumn(
            //     name: "ApplicationUserId",
            //     table: "CustomerCategoryDiscount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "CustomerCategoryDiscount",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCategoryDiscount_ApplicationUserId",
                table: "CustomerCategoryDiscount",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerCategoryDiscount_AspNetUsers_ApplicationUserId",
                table: "CustomerCategoryDiscount",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
