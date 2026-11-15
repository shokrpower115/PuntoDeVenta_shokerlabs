using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POS.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fase1Estabilizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CortesCaja_Sucursales_SucursalId",
                table: "CortesCaja");

            migrationBuilder.DropForeignKey(
                name: "FK_CortesCaja_Usuarios_UsuarioId",
                table: "CortesCaja");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosCaja_CategoriasMovimientoCaja_CategoriaId",
                table: "MovimientosCaja");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosCaja_Usuarios_UsuarioId",
                table: "MovimientosCaja");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductoSucursales_Sucursales_SucursalId",
                table: "ProductoSucursales");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Sucursales_SucursalId",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_MetodosPago_MetodoPagoId",
                table: "Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Sucursales_SucursalId",
                table: "Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Usuarios_UsuarioId",
                table: "Ventas");

            migrationBuilder.AddColumn<int>(
                name: "CorteCajaId",
                table: "Ventas",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NombreUsuario",
                table: "Usuarios",
                type: "TEXT",
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<bool>(
                name: "DebeCambiarPassword",
                table: "Usuarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Productos",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "MetodosPago",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "RequiereDescripcion",
                table: "CategoriasMovimientoCaja",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "CategoriasMovimientoCaja",
                keyColumn: "Id",
                keyValue: 1,
                column: "RequiereDescripcion",
                value: false);

            migrationBuilder.UpdateData(
                table: "CategoriasMovimientoCaja",
                keyColumn: "Id",
                keyValue: 2,
                column: "RequiereDescripcion",
                value: true);

            migrationBuilder.UpdateData(
                table: "CategoriasMovimientoCaja",
                keyColumn: "Id",
                keyValue: 3,
                column: "RequiereDescripcion",
                value: false);

            migrationBuilder.UpdateData(
                table: "CategoriasMovimientoCaja",
                keyColumn: "Id",
                keyValue: 4,
                column: "RequiereDescripcion",
                value: false);

            migrationBuilder.UpdateData(
                table: "CategoriasMovimientoCaja",
                keyColumn: "Id",
                keyValue: 5,
                column: "RequiereDescripcion",
                value: true);

            migrationBuilder.UpdateData(
                table: "MetodosPago",
                keyColumn: "Id",
                keyValue: 1,
                column: "Tipo",
                value: 0);

            migrationBuilder.UpdateData(
                table: "MetodosPago",
                keyColumn: "Id",
                keyValue: 2,
                column: "Tipo",
                value: 1);

            migrationBuilder.UpdateData(
                table: "MetodosPago",
                keyColumn: "Id",
                keyValue: 3,
                column: "Tipo",
                value: 1);

            migrationBuilder.UpdateData(
                table: "MetodosPago",
                keyColumn: "Id",
                keyValue: 4,
                column: "Tipo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "DebeCambiarPassword",
                value: false);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "DebeCambiarPassword",
                value: false);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                column: "DebeCambiarPassword",
                value: false);

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_CorteCajaId",
                table: "Ventas",
                column: "CorteCajaId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NombreUsuario",
                table: "Usuarios",
                column: "NombreUsuario",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CortesCaja_Sucursales_SucursalId",
                table: "CortesCaja",
                column: "SucursalId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CortesCaja_Usuarios_UsuarioId",
                table: "CortesCaja",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosCaja_CategoriasMovimientoCaja_CategoriaId",
                table: "MovimientosCaja",
                column: "CategoriaId",
                principalTable: "CategoriasMovimientoCaja",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosCaja_Usuarios_UsuarioId",
                table: "MovimientosCaja",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoSucursales_Sucursales_SucursalId",
                table: "ProductoSucursales",
                column: "SucursalId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Sucursales_SucursalId",
                table: "Usuarios",
                column: "SucursalId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_CortesCaja_CorteCajaId",
                table: "Ventas",
                column: "CorteCajaId",
                principalTable: "CortesCaja",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_MetodosPago_MetodoPagoId",
                table: "Ventas",
                column: "MetodoPagoId",
                principalTable: "MetodosPago",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Sucursales_SucursalId",
                table: "Ventas",
                column: "SucursalId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Usuarios_UsuarioId",
                table: "Ventas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Liga las ventas existentes con el turno en el que se cobraron
            // (antes solo se relacionaban por sucursal y fecha). Las ventas hechas
            // fuera de cualquier turno se quedan sin turno (CorteCajaId = NULL).
            migrationBuilder.Sql(@"
                UPDATE Ventas
                SET CorteCajaId = (
                    SELECT c.Id FROM CortesCaja c
                    WHERE c.SucursalId = Ventas.SucursalId
                      AND Ventas.Fecha >= c.FechaApertura
                      AND (c.FechaCierre IS NULL OR Ventas.Fecha <= c.FechaCierre)
                    ORDER BY c.Id
                    LIMIT 1)
                WHERE CorteCajaId IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CortesCaja_Sucursales_SucursalId",
                table: "CortesCaja");

            migrationBuilder.DropForeignKey(
                name: "FK_CortesCaja_Usuarios_UsuarioId",
                table: "CortesCaja");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosCaja_CategoriasMovimientoCaja_CategoriaId",
                table: "MovimientosCaja");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosCaja_Usuarios_UsuarioId",
                table: "MovimientosCaja");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductoSucursales_Sucursales_SucursalId",
                table: "ProductoSucursales");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Sucursales_SucursalId",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_CortesCaja_CorteCajaId",
                table: "Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_MetodosPago_MetodoPagoId",
                table: "Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Sucursales_SucursalId",
                table: "Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Usuarios_UsuarioId",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_CorteCajaId",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_NombreUsuario",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CorteCajaId",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "DebeCambiarPassword",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "MetodosPago");

            migrationBuilder.DropColumn(
                name: "RequiereDescripcion",
                table: "CategoriasMovimientoCaja");

            migrationBuilder.AlterColumn<string>(
                name: "NombreUsuario",
                table: "Usuarios",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldCollation: "NOCASE");

            migrationBuilder.AddForeignKey(
                name: "FK_CortesCaja_Sucursales_SucursalId",
                table: "CortesCaja",
                column: "SucursalId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CortesCaja_Usuarios_UsuarioId",
                table: "CortesCaja",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosCaja_CategoriasMovimientoCaja_CategoriaId",
                table: "MovimientosCaja",
                column: "CategoriaId",
                principalTable: "CategoriasMovimientoCaja",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosCaja_Usuarios_UsuarioId",
                table: "MovimientosCaja",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoSucursales_Sucursales_SucursalId",
                table: "ProductoSucursales",
                column: "SucursalId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Sucursales_SucursalId",
                table: "Usuarios",
                column: "SucursalId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_MetodosPago_MetodoPagoId",
                table: "Ventas",
                column: "MetodoPagoId",
                principalTable: "MetodosPago",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Sucursales_SucursalId",
                table: "Ventas",
                column: "SucursalId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Usuarios_UsuarioId",
                table: "Ventas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
