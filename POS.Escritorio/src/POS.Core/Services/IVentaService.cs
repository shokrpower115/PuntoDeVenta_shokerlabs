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
        Task<decimal> ObtenerTotalVentasAsync(int sucursalId, DateTime desde, DateTime hasta);
    }
}
