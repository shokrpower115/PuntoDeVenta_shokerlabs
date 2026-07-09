using System.Collections.Generic;
using System.Threading.Tasks;
using POS.Core.Models;

namespace POS.Core.Services
{
    // Mañana esta misma interfaz la implementa POS.Data/ProductoServiceApi.cs
    // haciendo llamadas HTTP en vez de leer una lista en memoria.
    public interface IProductoService
    {
        Task<List<Producto>> ObtenerTodosAsync(int sucursalId);
        Task<Producto?> BuscarPorCodigoAsync(string codigoBarras, int sucursalId);
        Task DescontarStockAsync(int productoId, int cantidad, int sucursalId);
        Task<List<DisponibilidadProducto>> ConsultarEnOtrasSucursalesAsync(int productoId);
    }
}
