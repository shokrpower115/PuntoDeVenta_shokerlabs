using System.Threading.Tasks;
using POS.Core.Models;

namespace POS.Core.Services
{
    public interface ICorteCajaService
    {
        Task<CorteDeCaja> AbrirTurnoAsync(int sucursalId, decimal efectivoInicial);
        Task<CorteDeCaja?> ObtenerTurnoAbiertoAsync(int sucursalId);
        Task<CorteDeCaja> CerrarTurnoAsync(int corteId, decimal efectivoFinal);
    }
}
