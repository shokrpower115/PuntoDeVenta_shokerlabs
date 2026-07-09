namespace POS.Core.Models
{
    public class Sucursal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Direccion { get; set; }

        public List<ProductoSucursal> StockDeProductos { get; set; } = new();
        public List<Usuario> Usuarios { get; set; } = new();
        public List<Venta> Ventas { get; set; } = new();
        public List<CorteDeCaja> CortesCaja { get; set; } = new();
    }
}