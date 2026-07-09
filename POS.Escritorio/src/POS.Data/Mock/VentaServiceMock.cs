using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.Data.Mock
{
    public class VentaServiceMock : IVentaService
    {
        private static readonly List<Venta> _ventas = new();
        private static int _siguienteId = 1;

        public Task<Venta> RegistrarVentaAsync(Venta venta)
        {
            venta.Id = _siguienteId++;
            venta.Fecha = DateTime.Now;
            _ventas.Add(venta);
            return Task.FromResult(venta);
        }

        public Task<List<Venta>> ObtenerVentasDelTurnoAsync(int sucursalId, DateTime desde)
        {
            var resultado = _ventas
                .Where(v => v.SucursalId == sucursalId && v.Fecha >= desde)
                .ToList();
            return Task.FromResult(resultado);
        }

        public Task<decimal> ObtenerTotalVentasAsync(int sucursalId, DateTime desde, DateTime hasta)
        {
            var total = _ventas
                .Where(v => v.SucursalId == sucursalId && v.Fecha >= desde && v.Fecha <= hasta)
                .Sum(v => v.Total);
            return Task.FromResult(total);
        }
    }
}
