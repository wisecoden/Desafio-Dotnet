using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvanadeAwesomeShop.Service.Orders.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedCustomerAndOrderData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 8, 20, 10, 0, 0, 0, DateTimeKind.Utc), "teste@email.com", "Cliente Teste" });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CreatedAt", "CustomerId", "Status", "TotalAmount", "UpdatedAt" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 8, 20, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 1, 0m, new DateTime(2025, 8, 20, 10, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));
        }
    }
}
