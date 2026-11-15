namespace POS.Core.Models
{
    // La lógica del corte (efectivo esperado, total con tarjeta) se basa en el Tipo,
    // nunca en el Nombre, para que el administrador pueda renombrar métodos sin romper nada.
    public enum TipoMetodoPago
    {
        Efectivo,
        Tarjeta,
        Transferencia,
        Otro
    }

    public class MetodoPago
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public TipoMetodoPago Tipo { get; set; } = TipoMetodoPago.Otro;
        public bool Activo { get; set; } = true;
    }
}
