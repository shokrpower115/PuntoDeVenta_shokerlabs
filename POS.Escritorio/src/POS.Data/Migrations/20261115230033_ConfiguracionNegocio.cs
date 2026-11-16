using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POS.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConfiguracionNegocio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DatosNegocio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", nullable: false),
                    Rfc = table.Column<string>(type: "TEXT", nullable: false),
                    SitioWeb = table.Column<string>(type: "TEXT", nullable: false),
                    MensajeTicket = table.Column<string>(type: "TEXT", nullable: false),
                    ColorPrimario = table.Column<string>(type: "TEXT", nullable: false),
                    Logo = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatosNegocio", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatosNegocio");
        }
    }
}
