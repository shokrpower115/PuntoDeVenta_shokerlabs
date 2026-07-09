namespace POS.Core.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? CodigoBarras { get; set; }
        public decimal Precio { get; set; }

        public List<ProductoSucursal> StockPorSucursal { get; set; } = new();
    }

    public class DisponibilidadProducto
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int SucursalId { get; set; }
        public string NombreSucursal { get; set; } = string.Empty;
        public int Stock { get; set; }
    }
}