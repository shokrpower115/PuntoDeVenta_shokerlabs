using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.Data.Mock
{
    public class AuthServiceMock : IAuthService
    {
        // Usuario y contraseña de prueba, uno por sucursal.
        // ⚠️ Esto es SOLO para el prototipo. Cuando exista la API real,
        // las contraseñas nunca se comparan en texto plano así.
        private static readonly List<(Usuario Usuario, string Password)> _usuarios = new()
        {
            (new Usuario { Id = 1, NombreUsuario = "centro", NombreCompleto = "Moisés", Puesto = "Cajero", SucursalId = 1, NombreSucursal = "Sucursal Centro" }, "1234"),
            (new Usuario { Id = 2, NombreUsuario = "norte",  NombreCompleto = "Moisés", Puesto = "Cajero", SucursalId = 2, NombreSucursal = "Sucursal Norte"  }, "1234"),
            (new Usuario { Id = 3, NombreUsuario = "sur",    NombreCompleto = "Moisés", Puesto = "Cajero", SucursalId = 3, NombreSucursal = "Sucursal Sur"    }, "1234"),        };

        public Task<Usuario?> LoginAsync(string nombreUsuario, string password)
        {
            var encontrado = _usuarios.FirstOrDefault(u =>
                u.Usuario.NombreUsuario.Equals(nombreUsuario, System.StringComparison.OrdinalIgnoreCase)
                && u.Password == password);

            return Task.FromResult(encontrado.Usuario);
        }
    }
}
