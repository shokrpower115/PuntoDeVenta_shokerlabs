using System.Collections.Generic;
using System.Threading.Tasks;
using POS.Core.Models;

namespace POS.Core.Services
{
    public interface ICorteCajaService
    {
        Task<CorteDeCaja> AbrirTurnoAsync(int sucursalId, int usuarioId , decimal efectivoInicial);
        Task<CorteDeCaja?> ObtenerTurnoAbiertoAsync(int sucursalId);
        Task<CorteDeCaja> CerrarTurnoAsync(int corteId, decimal efectivoFinal, decimal totalVentasEfectivo, decimal totalVentasTarjeta);

        // Nuevos:
        Task<List<CategoriaMovimientoCaja>> ObtenerCategoriasActivasAsync(TipoMovimientoCaja tipo);
        Task<MovimientoCaja> RegistrarMovimientoAsync(MovimientoCaja movimiento);
        Task<List<MovimientoCaja>> ObtenerMovimientosDelTurnoAsync(int corteId);
    }
}