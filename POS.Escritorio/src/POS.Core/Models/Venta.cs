using System;
using System.Collections.Generic;

namespace POS.Core.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;

        public int SucursalId { get; set; }
        public Sucursal? Sucursal { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int MetodoPagoId { get; set; }
        public MetodoPago? MetodoPago { get; set; }

        public decimal Total { get; set; }
        public decimal PagoCon { get; set; }

        public List<VentaDetalle> Detalles { get; set; } = new();
    }

    public class VentaDetalle
    {
        public int Id { get; set; }

        public int VentaId { get; set; }
        public Venta? Venta { get; set; }

        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }

}
