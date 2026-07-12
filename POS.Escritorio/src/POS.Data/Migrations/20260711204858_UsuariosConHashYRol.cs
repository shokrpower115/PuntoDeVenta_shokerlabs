using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UsuariosConHashYRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NombreSucursal",
                table: "Usuarios",
                newName: "PasswordHash");

            migrationBuilder.AddColumn<int>(
                name: "Rol",
                table: "Usuarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "Rol" },
                values: new object[] { "Y3lzhbqcgxasyFhImX0QMg==.qWue49ZJOCA+vahhdYktFJZ/kUcGEtpmx9+qN49G+wk=", 0 });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PasswordHash", "Rol" },
                values: new object[] { "MywnH8vWDoByu1Xa+mk0KA==.nqDOnj9H5a0dSSrTKGZh+Rlqo3QhCPruw6WrKxsAwSc=", 0 });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PasswordHash", "Rol" },
                values: new object[] { "v2DbgCZ+Enxem90DjDPA1A==.eotfZxOy4H5PASUNxaBwMGPZ/Pnu5SJERtvW222GMIY=", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rol",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Usuarios",
                newName: "NombreSucursal");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "NombreSucursal",
                value: "");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "NombreSucursal",
                value: "");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                column: "NombreSucursal",
                value: "");
        }
    }
}
