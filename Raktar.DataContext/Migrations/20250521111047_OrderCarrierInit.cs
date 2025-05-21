using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Raktar.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class OrderCarrierInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarrierId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CarrierId",
                table: "Orders",
                column: "CarrierId");

            migrationBuilder.AddForeignKey(
                name: "FK__Orders__Carrier",
                table: "Orders",
                column: "CarrierId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Orders__Carrier",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CarrierId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CarrierId",
                table: "Orders");
        }
    }
}
