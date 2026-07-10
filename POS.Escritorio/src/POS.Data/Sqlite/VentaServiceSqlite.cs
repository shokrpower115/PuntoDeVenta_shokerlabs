using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.Data.Sqlite
{
    public class VentaServiceSqlite : IVentaService
    {
        public async Task<Venta> RegistrarVentaAsync(Venta venta)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            db.Ventas.Add(venta);
            await db.SaveChangesAsync();

            // Después de SaveChangesAsync, EF Core ya llenó venta.Id
            // (y el Id de cada VentaDetalle) con los valores reales generados por SQLite.
            return venta;
        }

        public async Task<List<Venta>> ObtenerVentasDelTurnoAsync(int sucursalId, DateTime desde)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            return await db.Ventas
                .Include(v => v.Detalles)
                .Where(v => v.SucursalId == sucursalId && v.Fecha >= desde)
                .ToListAsync();
        }

        public async Task<decimal> ObtenerTotalVentasAsync(int sucursalId, DateTime desde, DateTime hasta)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            return await db.Ventas
                .Where(v => v.SucursalId == sucursalId && v.Fecha >= desde && v.Fecha <= hasta)
                .SumAsync(v => v.Total);
        }

        public async Task<decimal> ObtenerTotalPorMetodoPagoAsync(int sucursalId, DateTime desde, DateTime hasta, string nombreMetodoPago)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            return await db.Ventas
                .Include(v => v.MetodoPago)
                .Where(v => v.SucursalId == sucursalId
                         && v.Fecha >= desde && v.Fecha <= hasta
                         && v.MetodoPago!.Nombre == nombreMetodoPago)
                .SumAsync(v => v.Total);
        }
    }
}