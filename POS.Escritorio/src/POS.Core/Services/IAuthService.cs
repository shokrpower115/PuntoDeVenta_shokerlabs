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
        Task<Usuario> CrearUsuarioAsync(Usuario usuario, string passwordTextoPlano);
    }
}