using System.Linq;
using Microsoft.EntityFrameworkCore;
using POS.Core.Models;
using POS.Core.Seguridad;

namespace POS.Data
{
    public static class InicializadorBaseDatos
    {
        public const string UsuarioAdminInicial = "admin";
        public const string PasswordAdminInicial = "admin";

        // Crea la base de datos si no existe, aplica las migraciones pendientes y
        // garantiza que haya al menos un administrador. Se llama una vez al arrancar.
        public static void Inicializar()
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());
            db.Database.Migrate();

            // No se siembra con HasData porque una base existente puede tener ya
            // usuarios con ese Id o ese nombre (creados desde el Administrador).
            bool hayAdministrador = db.Usuarios.Any(u =>
                u.Rol == RolUsuario.Administrador || u.Rol == RolUsuario.SuperAdministrador);

            if (!hayAdministrador && !db.Usuarios.Any(u => u.NombreUsuario == UsuarioAdminInicial))
            {
                db.Usuarios.Add(new Usuario
                {
                    NombreUsuario = UsuarioAdminInicial,
                    PasswordHash = HashPassword.Generar(PasswordAdminInicial),
                    NombreCompleto = "Administrador",
                    Puesto = "Administrador",
                    Rol = RolUsuario.Administrador,
                    SucursalId = db.Sucursales.OrderBy(s => s.Id).Select(s => s.Id).First(),
                    DebeCambiarPassword = true
                });
                db.SaveChanges();
            }
        }
    }
}
