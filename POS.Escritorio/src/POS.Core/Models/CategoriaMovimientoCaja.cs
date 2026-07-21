namespace POS.Core.Models
{
    public enum TipoMovimientoCaja
    {
        Entrada,
        Retiro
    }

    public class CategoriaMovimientoCaja
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public TipoMovimientoCaja Tipo { get; set; }
        public bool Activo { get; set; } = true;
    }
}