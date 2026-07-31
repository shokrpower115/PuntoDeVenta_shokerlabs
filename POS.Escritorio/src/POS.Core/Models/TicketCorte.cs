using System;
using System.Collections.Generic;

namespace POS.Core.Models
{
    public class TicketCorte
    {
        public int CorteCajaId { get; set; }
        public string NombreCajero { get; set; } = string.Empty;
        public DateTime FechaApertura { get; set; }
        public DateTime FechaCierre { get; set; }

        public decimal EfectivoInicial { get; set; }
        public List<(string MetodoPago, decimal Total)> VentasPorMetodoPago { get; set; } = new();

        public List<MovimientoCaja> Movimientos { get; set; } = new();

        public decimal TotalEntradas { get; set; }
        public decimal TotalRetiros { get; set; }
        public decimal EfectivoEsperado { get; set; }
        public decimal EfectivoContado { get; set; }
        public decimal Diferencia { get; set; }
    }
}