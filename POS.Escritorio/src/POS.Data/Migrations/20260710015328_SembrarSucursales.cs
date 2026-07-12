using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace POS.Data.Migrations
{
    /// <inheritdoc />
    public partial class SembrarSucursales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Sucursales",
                columns: new[] { "Id", "Direccion", "Nombre" },
                values: new object[,]
                {
                    { 1, null, "Sucursal Centro" },
                    { 2, null, "Sucursal Norte" },
                    { 3, null, "Sucursal Sur" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
