using System.Collections.Generic;
using System.Threading.Tasks;
using POS.Core.Models;

namespace POS.Core.Services
{
    public interface IVentaService
    {
        // Registra la venta en el turno abierto de la sucursal y descuenta el stock,
        // todo en una sola transacción. Lanza ReglaNegocioException si no hay turno
        // abierto o si algún producto no tiene stock suficiente (y entonces no se guarda nada).
        Task<Venta> RegistrarVentaAsync(Venta venta);

        Task<List<Venta>> ObtenerVentasDelTurnoAsync(int corteId);
        Task<decimal> ObtenerTotalPorTipoPagoAsync(int corteId, TipoMetodoPago tipo);
        Task<List<(string MetodoPago, decimal Total)>> ObtenerDesglosePorMetodoPagoAsync(int corteId);
    }
}
