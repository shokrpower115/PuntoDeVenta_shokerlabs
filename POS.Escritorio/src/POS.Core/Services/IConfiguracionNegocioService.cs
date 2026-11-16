using System.Threading.Tasks;
using POS.Core.Models;

namespace POS.Core.Services
{
    public interface IConfiguracionNegocioService
    {
        // null si el negocio todavía no se configuró (primer arranque).
        Task<DatosNegocio?> ObtenerAsync();

        Task GuardarAsync(DatosNegocio datos);
    }
}
