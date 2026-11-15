using System.Collections.Generic;
using System.Threading.Tasks;
using POS.Core.Models;

namespace POS.Core.Services
{
    public interface IAuthService
    {
        Task<Usuario?> LoginAsync(string nombreUsuario, string password);

        // Nuevos, para la gestión de usuarios desde el Administrador:
        Task<List<Usuario>> ObtenerTodosAsync();

        // Lanza ReglaNegocioException si el nombre de usuario ya existe.
        Task<Usuario> CrearUsuarioAsync(Usuario usuario, string passwordTextoPlano);

        Task CambiarPasswordAsync(int usuarioId, string passwordNuevoTextoPlano);
    }
}
