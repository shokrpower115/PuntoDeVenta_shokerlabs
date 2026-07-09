using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models
{
    public class TicketVenta
    {
        public int Folio { get; set; }
        public DateTime Fecha { get; set; }
        public string NombreCajero { get; set; } = string.Empty;
        public List<VentaDetalle> Detalles { get; set; } = new();
        public decimal Total { get; set; }
        public decimal PagoCon { get; set; }
        public decimal Cambio => PagoCon - Total;
    }
}
