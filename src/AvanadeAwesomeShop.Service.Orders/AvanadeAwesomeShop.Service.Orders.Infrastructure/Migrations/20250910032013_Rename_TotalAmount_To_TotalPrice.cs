using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvanadeAwesomeShop.Service.Orders.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Rename_TotalAmount_To_TotalPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Orders",
                newName: "TotalPrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Orders",
                newName: "TotalAmount");
        }
    }
}
