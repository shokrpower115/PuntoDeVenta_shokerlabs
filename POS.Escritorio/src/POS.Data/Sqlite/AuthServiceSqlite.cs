using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using POS.Core;
using POS.Core.Models;
using POS.Core.Seguridad;
using POS.Core.Services;

namespace POS.Data.Sqlite
{
    public class AuthServiceSqlite : IAuthService
    {
        public async Task<Usuario?> LoginAsync(string nombreUsuario, string password)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            // La columna usa collation NOCASE: la comparación ya ignora mayúsculas.
            var usuario = await db.Usuarios
                .Include(u => u.Sucursal)
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario.Trim());

            if (usuario == null) return null;

            bool passwordCorrecta = HashPassword.Verificar(password, usuario.PasswordHash);
            return passwordCorrecta ? usuario : null;
        }

        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            return await db.Usuarios
                .Include(u => u.Sucursal)
                .ToListAsync();
        }

        public async Task<Usuario> CrearUsuarioAsync(Usuario usuario, string passwordTextoPlano)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            usuario.NombreUsuario = usuario.NombreUsuario.Trim();
            if (await db.Usuarios.AnyAsync(u => u.NombreUsuario == usuario.NombreUsuario))
                throw new ReglaNegocioException($"Ya existe un usuario llamado '{usuario.NombreUsuario}'.");

            usuario.PasswordHash = HashPassword.Generar(passwordTextoPlano);

            db.Usuarios.Add(usuario);
            await db.SaveChangesAsync();
            return usuario;
        }

        public async Task CambiarPasswordAsync(int usuarioId, string passwordNuevoTextoPlano)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            var usuario = await db.Usuarios.FirstAsync(u => u.Id == usuarioId);
            usuario.PasswordHash = HashPassword.Generar(passwordNuevoTextoPlano);
            usuario.DebeCambiarPassword = false;

            await db.SaveChangesAsync();
        }
    }
}
