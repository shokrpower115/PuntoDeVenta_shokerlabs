using System.Threading.Tasks;
using POS.Core.Models;

namespace POS.Core.Services
{
    // Mañana esto lo implementa AuthServiceApi haciendo POST a /api/auth/login
    // contra tu backend, validando contraseña con hash, JWT, etc.
    // Hoy, el mock solo compara contra una lista fija en memoria.
    public interface IAuthService
    {
        // Devuelve el Usuario si las credenciales son correctas, o null si no.
        Task<Usuario?> LoginAsync(string nombreUsuario, string password);
    }
}
