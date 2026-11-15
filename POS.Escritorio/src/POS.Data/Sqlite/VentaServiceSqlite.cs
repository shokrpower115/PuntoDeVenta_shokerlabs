using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using POS.Core;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.Data.Sqlite
{
    public class VentaServiceSqlite : IVentaService
    {
        public async Task<Venta> RegistrarVentaAsync(Venta venta)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            var turno = await db.CortesCaja
                .FirstOrDefaultAsync(c => c.SucursalId == venta.SucursalId && !c.Cerrado);
            if (turno == null)
                throw new ReglaNegocioException("No hay un turno abierto en esta sucursal. Abre un turno antes de vender.");
            venta.CorteCajaId = turno.Id;

            // Se valida y descuenta el stock en el mismo DbContext que la venta:
            // un solo SaveChangesAsync = una sola transacción. Si algo falla,
            // no se guarda ni la venta ni el descuento de inventario.
            var cantidadesPorProducto = venta.Detalles
                .GroupBy(d => d.ProductoId)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Cantidad));

            var stocks = await db.ProductoSucursales
                .Include(s => s.Producto)
                .Where(s => s.SucursalId == venta.SucursalId && cantidadesPorProducto.Keys.Contains(s.ProductoId))
                .ToDictionaryAsync(s => s.ProductoId);

            foreach (var (productoId, cantidad) in cantidadesPorProducto)
            {
                if (!stocks.TryGetValue(productoId, out var stock) || stock.Stock < cantidad)
                {
                    var nombre = venta.Detalles.First(d => d.ProductoId == productoId).NombreProducto;
                    var disponible = stock?.Stock ?? 0;
                    throw new ReglaNegocioException(
                        $"Stock insuficiente de '{nombre}': se piden {cantidad} y hay {disponible}. No se registró la venta.");
                }
                stock.Stock -= cantidad;
            }

            db.Ventas.Add(venta);
            await db.SaveChangesAsync();

            // Después de SaveChangesAsync, EF Core ya llenó venta.Id
            // (y el Id de cada VentaDetalle) con los valores reales generados por SQLite.
            return venta;
        }

        public async Task<List<Venta>> ObtenerVentasDelTurnoAsync(int corteId)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            return await db.Ventas
                .Include(v => v.Detalles)
                .Where(v => v.CorteCajaId == corteId)
                .ToListAsync();
        }

        public async Task<decimal> ObtenerTotalPorTipoPagoAsync(int corteId, TipoMetodoPago tipo)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            // SQLite no suma decimales del lado de la base, por eso se suma en memoria.
            var totales = await db.Ventas
                .Where(v => v.CorteCajaId == corteId && v.MetodoPago!.Tipo == tipo)
                .Select(v => v.Total)
                .ToListAsync();

            return totales.Sum();
        }

        public async Task<List<(string MetodoPago, decimal Total)>> ObtenerDesglosePorMetodoPagoAsync(int corteId)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            var ventas = await db.Ventas
                .Include(v => v.MetodoPago)
                .Where(v => v.CorteCajaId == corteId)
                .ToListAsync();

            return ventas
                .GroupBy(v => v.MetodoPago!.Nombre)
                .Select(g => (g.Key, g.Sum(v => v.Total)))
                .ToList();
        }
    }
}
