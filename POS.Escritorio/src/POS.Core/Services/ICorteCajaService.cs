using System.Collections.Generic;
using System.Threading.Tasks;
using POS.Core.Models;

namespace POS.Core.Services
{
    public interface ICorteCajaService
    {
        Task<CorteDeCaja> AbrirTurnoAsync(int sucursalId, int usuarioId , decimal efectivoInicial);

        // Hay una sola caja por sucursal: devuelve el turno abierto (con su Usuario) o null.
        Task<CorteDeCaja?> ObtenerTurnoAbiertoAsync(int sucursalId);

        // Los totales de ventas se calculan aquí, a partir de las ventas ligadas al turno.
        Task<CorteDeCaja> CerrarTurnoAsync(int corteId, decimal efectivoFinal);

        // Nuevos:
        Task<List<CategoriaMovimientoCaja>> ObtenerCategoriasActivasAsync(TipoMovimientoCaja tipo);
        Task<MovimientoCaja> RegistrarMovimientoAsync(MovimientoCaja movimiento);
        Task<List<MovimientoCaja>> ObtenerMovimientosDelTurnoAsync(int corteId);
    }
}
