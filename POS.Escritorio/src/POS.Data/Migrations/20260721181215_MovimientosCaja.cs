using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace POS.Data.Migrations
{
    /// <inheritdoc />
    public partial class MovimientosCaja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalVentas",
                table: "CortesCaja",
                newName: "TotalVentasTarjeta");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalVentasEfectivo",
                table: "CortesCaja",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "CategoriasMovimientoCaja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasMovimientoCaja", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosCaja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CorteCajaId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoriaId = table.Column<int>(type: "INTEGER", nullable: false),
                    DescripcionOtro = table.Column<string>(type: "TEXT", nullable: true),
                    Monto = table.Column<decimal>(type: "TEXT", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosCaja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosCaja_CategoriasMovimientoCaja_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "CategoriasMovimientoCaja",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovimientosCaja_CortesCaja_CorteCajaId",
                        column: x => x.CorteCajaId,
                        principalTable: "CortesCaja",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovimientosCaja_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CategoriasMovimientoCaja",
                columns: new[] { "Id", "Activo", "Nombre", "Tipo" },
                values: new object[,]
                {
                    { 1, true, "Fondo adicional / Cambio solicitado", 0 },
                    { 2, true, "Otro", 0 },
                    { 3, true, "Retiro por seguridad", 1 },
                    { 4, true, "Depósito bancario", 1 },
                    { 5, true, "Otro", 1 }
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "2ysu65SpAZMDqaBczOfJ0w==.FbzrKO0gw1x9k438UvVyLJPXafHqZx9P052GaD9aHKM=");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "J7JS3UEacFTKclq1dCVWvA==.dO8j6wgs7hFmnKJ3DO/IK/p7n+k5F17Z5tGv8xnnd5Q=");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "EzzYO4MDYuynJni/1CiPpQ==.f/ieFBawAvVHfSj+p6pQkKGn118/9IAJtwdm6sRqQe8=");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCaja_CategoriaId",
                table: "MovimientosCaja",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCaja_CorteCajaId",
                table: "MovimientosCaja",
                column: "CorteCajaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCaja_UsuarioId",
                table: "MovimientosCaja",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimientosCaja");

            migrationBuilder.DropTable(
                name: "CategoriasMovimientoCaja");

            migrationBuilder.DropColumn(
                name: "TotalVentasEfectivo",
                table: "CortesCaja");

            migrationBuilder.RenameColumn(
                name: "TotalVentasTarjeta",
                table: "CortesCaja",
                newName: "TotalVentas");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "Y3lzhbqcgxasyFhImX0QMg==.qWue49ZJOCA+vahhdYktFJZ/kUcGEtpmx9+qN49G+wk=");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "MywnH8vWDoByu1Xa+mk0KA==.nqDOnj9H5a0dSSrTKGZh+Rlqo3QhCPruw6WrKxsAwSc=");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "v2DbgCZ+Enxem90DjDPA1A==.eotfZxOy4H5PASUNxaBwMGPZ/Pnu5SJERtvW222GMIY=");
        }
    }
}
