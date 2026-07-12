using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace POS.Data.Migrations
{
    /// <inheritdoc />
    public partial class SembrarUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "NombreCompleto", "NombreSucursal", "NombreUsuario", "Puesto", "SucursalId" },
                values: new object[,]
                {
                    { 1, "Moisés", "", "centro", "Cajero", 1 },
                    { 2, "Moisés", "", "norte", "Cajero", 2 },
                    { 3, "Moisés", "", "sur", "Cajero", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
