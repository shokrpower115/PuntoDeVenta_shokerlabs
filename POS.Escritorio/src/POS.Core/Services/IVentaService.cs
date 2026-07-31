using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using POS.Core.Models;

namespace POS.Core.Services
{
    public interface IVentaService
    {
        Task<Venta> RegistrarVentaAsync(Venta venta);
        Task<List<Venta>> ObtenerVentasDelTurnoAsync(int sucursalId, DateTime desde);
        Task<decimal> ObtenerTotalPorMetodoPagoAsync(int sucursalId, DateTime desde, DateTime hasta, string nombreMetodoPago);
        Task<List<(string MetodoPago, decimal Total)>> ObtenerDesglosePorMetodoPagoAsync(int sucursalId, DateTime desde, DateTime hasta);
    }
}
