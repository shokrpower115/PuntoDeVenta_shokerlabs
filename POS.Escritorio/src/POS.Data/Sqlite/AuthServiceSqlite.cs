using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

            var usuario = await db.Usuarios
                .Include(u => u.Sucursal)
                .FirstOrDefaultAsync(u => u.NombreUsuario.ToLower() == nombreUsuario.ToLower());

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

            usuario.PasswordHash = HashPassword.Generar(passwordTextoPlano);

            db.Usuarios.Add(usuario);
            await db.SaveChangesAsync();
            return usuario;
        }
    }
}