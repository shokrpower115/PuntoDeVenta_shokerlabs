using System;

namespace POS.Core.Models
{
    public class CorteDeCaja
    {
        public int Id { get; set; }

        public int SucursalId { get; set; }
        public Sucursal? Sucursal { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal EfectivoInicial { get; set; }
        public decimal EfectivoFinal { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal Diferencia => EfectivoFinal - (EfectivoInicial + TotalVentas);
        public bool Cerrado { get; set; }
    }
}