using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Raktar.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class ProductBlockChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "StorageId",
                table: "Blocks",
                newName: "BlockId");

            migrationBuilder.AlterColumn<int>(
                name: "MaxQuantityPerBlock",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BlockId",
                table: "Blocks",
                newName: "StorageId");

            migrationBuilder.AlterColumn<int>(
                name: "MaxQuantityPerBlock",
                table: "Product",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
