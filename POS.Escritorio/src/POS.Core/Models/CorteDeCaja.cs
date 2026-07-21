using System;
using System.Collections.Generic;
using System.Linq;

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

        public decimal TotalVentasEfectivo { get; set; }
        public decimal TotalVentasTarjeta { get; set; }

        public List<MovimientoCaja> Movimientos { get; set; } = new();

        public decimal TotalEntradas => Movimientos
            .Where(m => m.Categoria != null && m.Categoria.Tipo == TipoMovimientoCaja.Entrada)
            .Sum(m => m.Monto);

        public decimal TotalRetiros => Movimientos
            .Where(m => m.Categoria != null && m.Categoria.Tipo == TipoMovimientoCaja.Retiro)
            .Sum(m => m.Monto);

        public decimal EfectivoEsperado => EfectivoInicial + TotalVentasEfectivo + TotalEntradas - TotalRetiros;
        public decimal Diferencia => EfectivoFinal - EfectivoEsperado;

        public bool Cerrado { get; set; }
    }
}