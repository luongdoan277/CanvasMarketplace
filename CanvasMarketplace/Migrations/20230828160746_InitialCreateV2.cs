using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanvasMarketplace.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AppUsers_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppUsers_UserId",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AppUsers_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AppUsers_UserId",
                table: "Products",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AppUsers_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppUsers_UserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AppUsers_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AppUsers_UserId",
                table: "Products",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
