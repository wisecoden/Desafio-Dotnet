using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvanadeAwesomeShop.Service.Stock.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Rename_StockQuantity_To_Quantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StockQuantity",
                table: "Products",
                newName: "Quantity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Products",
                newName: "StockQuantity");
        }
    }
}
