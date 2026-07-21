using System;

namespace POS.Core.Models
{
    public class MovimientoCaja
    {
        public int Id { get; set; }

        public int CorteCajaId { get; set; }
        public CorteDeCaja? CorteCaja { get; set; }

        public int CategoriaId { get; set; }
        public CategoriaMovimientoCaja? Categoria { get; set; }

        public string? DescripcionOtro { get; set; } // Solo se llena si Categoria.Nombre == "Otro"

        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}