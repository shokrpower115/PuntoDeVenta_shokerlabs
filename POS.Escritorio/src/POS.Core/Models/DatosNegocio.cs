namespace POS.Core.Models
{
    // Identidad del negocio que usa el sistema: aparece en la pantalla principal,
    // en el login y en los tickets. Se captura en el asistente de primer arranque
    // y la edita el administrador; nunca debe estar escrita en el código.
    // Solo existe un registro (Id = 1).
    public class DatosNegocio
    {
        public const int IdUnico = 1;
        public const string ColorPrimarioPorDefecto = "#2563EB";
        public const string MensajeTicketPorDefecto = "GRACIAS POR SU COMPRA";

        public int Id { get; set; } = IdUnico;

        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Rfc { get; set; } = string.Empty;
        public string SitioWeb { get; set; } = string.Empty;

        // Pie del ticket de venta.
        public string MensajeTicket { get; set; } = MensajeTicketPorDefecto;

        // Color de marca en formato #RRGGBB. La interfaz deriva de él los tonos
        // de botones, menú y selección.
        public string ColorPrimario { get; set; } = ColorPrimarioPorDefecto;

        // Imagen del logo (PNG/JPG). Se guarda en la base para que un respaldo
        // de pos.db incluya toda la configuración del negocio.
        public byte[]? Logo { get; set; }

        public DatosNegocio Clonar() => (DatosNegocio)MemberwiseClone();

        public void CopiarDe(DatosNegocio otro)
        {
            Nombre = otro.Nombre;
            Direccion = otro.Direccion;
            Telefono = otro.Telefono;
            Rfc = otro.Rfc;
            SitioWeb = otro.SitioWeb;
            MensajeTicket = otro.MensajeTicket;
            ColorPrimario = otro.ColorPrimario;
            Logo = otro.Logo;
        }
    }
}
