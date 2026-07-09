using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.Data.Mock
{
    public class ProductoServiceMock : IProductoService
    {
        private static readonly List<Producto> _productos = new()
        {
            new Producto { Id = 1, Nombre = "Refresco 600ml", CodigoBarras = "7501000111", Precio = 18 },
            new Producto { Id = 2, Nombre = "Sabritas 45g",   CodigoBarras = "7501000222", Precio = 15 },
            new Producto { Id = 3, Nombre = "Agua 1L",        CodigoBarras = "7501000333", Precio = 12 },
        };

        private static readonly List<ProductoSucursal> _stock = new()
        {
            new ProductoSucursal { ProductoId = 1, SucursalId = 1, Stock = 12 },
            new ProductoSucursal { ProductoId = 1, SucursalId = 2, Stock = 0 },
            new ProductoSucursal { ProductoId = 1, SucursalId = 3, Stock = 5 },

            new ProductoSucursal { ProductoId = 2, SucursalId = 1, Stock = 20 },
            new ProductoSucursal { ProductoId = 2, SucursalId = 2, Stock = 8 },
            new ProductoSucursal { ProductoId = 2, SucursalId = 3, Stock = 0 },

            new ProductoSucursal { ProductoId = 3, SucursalId = 1, Stock = 30 },
        };

        private static readonly Dictionary<int, string> _nombresSucursal = new()
        {
            { 1, "Sucursal Centro" },
            { 2, "Sucursal Norte" },
            { 3, "Sucursal Sur" },
        };

        public Task<List<Producto>> ObtenerTodosAsync(int sucursalId)
        {
            var idsConStockEnSucursal = _stock
                .Where(s => s.SucursalId == sucursalId)
                .Select(s => s.ProductoId);

            var resultado = _productos
                .Where(p => idsConStockEnSucursal.Contains(p.Id))
                .Select(p => ConStockDeSucursal(p, sucursalId))
                .ToList();

            return Task.FromResult(resultado);
        }

        public Task<Producto?> BuscarPorCodigoAsync(string codigoBarras, int sucursalId)
        {
            var producto = _productos.FirstOrDefault(p => p.CodigoBarras == codigoBarras);
            if (producto == null) return Task.FromResult<Producto?>(null);

            return Task.FromResult<Producto?>(ConStockDeSucursal(producto, sucursalId));
        }

        public Task DescontarStockAsync(int productoId, int cantidad, int sucursalId)
        {
            var stock = _stock.FirstOrDefault(s => s.ProductoId == productoId && s.SucursalId == sucursalId);
            if (stock != null)
                stock.Stock -= cantidad;

            return Task.CompletedTask;
        }

        public Task<List<DisponibilidadProducto>> ConsultarEnOtrasSucursalesAsync(int productoId)
        {
            var producto = _productos.FirstOrDefault(p => p.Id == productoId);
            if (producto == null) return Task.FromResult(new List<DisponibilidadProducto>());

            var resultado = _stock
                .Where(s => s.ProductoId == productoId)
                .Select(s => new DisponibilidadProducto
                {
                    ProductoId = productoId,
                    NombreProducto = producto.Nombre,
                    SucursalId = s.SucursalId,
                    NombreSucursal = _nombresSucursal.GetValueOrDefault(s.SucursalId, "Desconocida"),
                    Stock = s.Stock
                })
                .ToList();

            return Task.FromResult(resultado);
        }

        private Producto ConStockDeSucursal(Producto producto, int sucursalId)
        {
            var stock = _stock.FirstOrDefault(s => s.ProductoId == producto.Id && s.SucursalId == sucursalId);
            return new Producto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                CodigoBarras = producto.CodigoBarras,
                Precio = producto.Precio,
                StockPorSucursal = stock != null ? new List<ProductoSucursal> { stock } : new()
            };
        }
    }
}