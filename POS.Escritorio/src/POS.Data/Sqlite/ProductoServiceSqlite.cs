using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.Data.Sqlite
{
    public class ProductoServiceSqlite : IProductoService
    {
        public async Task<List<Producto>> ObtenerTodosAsync(int sucursalId)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            return await db.Productos
                .Include(p => p.StockPorSucursal.Where(s => s.SucursalId == sucursalId))
                .Where(p => p.StockPorSucursal.Any(s => s.SucursalId == sucursalId))
                .ToListAsync();
        }

        public async Task<Producto?> BuscarPorCodigoAsync(string codigoBarras, int sucursalId)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            return await db.Productos
                .Include(p => p.StockPorSucursal.Where(s => s.SucursalId == sucursalId))
                .FirstOrDefaultAsync(p => p.CodigoBarras == codigoBarras);
        }

        public async Task DescontarStockAsync(int productoId, int cantidad, int sucursalId)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            var stock = await db.ProductoSucursales
                .FirstOrDefaultAsync(s => s.ProductoId == productoId && s.SucursalId == sucursalId);

            if (stock != null)
            {
                stock.Stock -= cantidad;
                await db.SaveChangesAsync();
            }
        }

        public async Task EliminarAsync(int productoId)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            var producto = await db.Productos
                .Include(p => p.StockPorSucursal)
                .FirstAsync(p => p.Id == productoId);

            db.Productos.Remove(producto);
            await db.SaveChangesAsync();
        }

        public async Task<List<Producto>> ObtenerCatalogoCompletoAsync()
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            return await db.Productos
                .Include(p => p.StockPorSucursal)
                    .ThenInclude(s => s.Sucursal)
                .ToListAsync();
        }

        public async Task<List<DisponibilidadProducto>> ConsultarEnOtrasSucursalesAsync(int productoId)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            var producto = await db.Productos.FirstOrDefaultAsync(p => p.Id == productoId);
            if (producto == null) return new List<DisponibilidadProducto>();

            return await db.ProductoSucursales
                .Include(s => s.Sucursal)
                .Where(s => s.ProductoId == productoId)
                .Select(s => new DisponibilidadProducto
                {
                    ProductoId = productoId,
                    NombreProducto = producto.Nombre,
                    SucursalId = s.SucursalId,
                    NombreSucursal = s.Sucursal!.Nombre,
                    Stock = s.Stock
                })
                .ToListAsync();
        }

        public async Task ActualizarStockAsync(int productoId, int sucursalId, int nuevoStock)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            var stock = await db.ProductoSucursales
                .FirstOrDefaultAsync(s => s.ProductoId == productoId && s.SucursalId == sucursalId);

            if (stock != null)
            {
                stock.Stock = nuevoStock;
                await db.SaveChangesAsync();
            }
        }

        public async Task<Producto> CrearAsync(Producto producto, Dictionary<int, int> stockInicialPorSucursal)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            foreach (var (sucursalId, stock) in stockInicialPorSucursal)
            {
                producto.StockPorSucursal.Add(new ProductoSucursal
                {
                    SucursalId = sucursalId,
                    Stock = stock
                });
            }

            db.Productos.Add(producto);
            await db.SaveChangesAsync();
            return producto;
        }

        public async Task ActualizarAsync(Producto producto)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            var existente = await db.Productos.FirstAsync(p => p.Id == producto.Id);
            existente.Nombre = producto.Nombre;
            existente.CodigoBarras = producto.CodigoBarras;
            existente.Precio = producto.Precio;

            await db.SaveChangesAsync();
        }
    }
}