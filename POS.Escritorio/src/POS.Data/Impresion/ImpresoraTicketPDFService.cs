using POS.Core.Models;
using POS.Core.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace POS.Data.Impresion
{
    public class ImpresoraTicketPdfService : IImpresoraTicketService
    {
        // Ancho típico de rollo térmico de 80mm ≈ 226 puntos (72pt = 1 pulgada)
        private const float AnchoTicketPuntos = 226f;

        static ImpresoraTicketPdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public Task ImprimirAsync(TicketVenta ticket, DatosNegocio negocio)
        {
            var carpeta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TicketsPOS");
            Directory.CreateDirectory(carpeta);
            var ruta = Path.Combine(carpeta, $"ticket_{ticket.Folio}.pdf");

            Document.Create(contenedor =>
            {
                contenedor.Page(pagina =>
                {
                    pagina.Size(AnchoTicketPuntos, 800, Unit.Point);
                    pagina.Margin(10);
                    pagina.DefaultTextStyle(e => e.FontSize(9).FontFamily("Consolas"));

                    pagina.Content().Column(col =>
                    {
                        col.Item().AlignCenter().Text(negocio.Nombre).Bold().FontSize(13);
                        col.Item().AlignCenter().Text(negocio.Direccion);
                        col.Item().AlignCenter().Text(negocio.Telefono);
                        col.Item().AlignCenter().Text(negocio.Rfc);

                        col.Item().PaddingTop(5).Text(ticket.Fecha.ToString("dd/MM/yyyy hh:mm tt"));
                        col.Item().Text($"CAJERO: {ticket.NombreCajero}");
                        col.Item().Text($"FOLIO: {ticket.Folio}");

                        col.Item().PaddingTop(5).Row(fila =>
                        {
                            fila.RelativeItem(1).Text("CANT.").Bold();
                            fila.RelativeItem(3).Text("DESCRIPCION").Bold();
                            fila.RelativeItem(2).AlignRight().Text("IMPORTE").Bold();
                        });
                        col.Item().LineHorizontal(1);

                        foreach (var detalle in ticket.Detalles)
                        {
                            col.Item().Row(fila =>
                            {
                                fila.RelativeItem(1).Text(detalle.Cantidad.ToString());
                                fila.RelativeItem(3).Text(detalle.NombreProducto);
                                fila.RelativeItem(2).AlignRight().Text(detalle.Subtotal.ToString("C"));
                            });
                        }

                        col.Item().PaddingTop(3).LineHorizontal(1);

                        int totalArticulos = 0;
                        foreach (var d in ticket.Detalles) totalArticulos += d.Cantidad;

                        col.Item().PaddingTop(3).AlignRight().Text($"NO. DE ARTICULOS: {totalArticulos}");
                        col.Item().AlignRight().Text($"TOTAL: {ticket.Total:C}").Bold();
                        col.Item().AlignRight().Text($"PAGO CON: {ticket.PagoCon:C}");
                        col.Item().AlignRight().Text($"SU CAMBIO: {ticket.Cambio:C}");

                        col.Item().PaddingTop(10).AlignCenter().Text("GRACIAS POR SU COMPRA");
                        if (!string.IsNullOrWhiteSpace(negocio.SitioWeb))
                            col.Item().AlignCenter().Text(negocio.SitioWeb.ToUpper());
                    });
                });
            })
            .GeneratePdf(ruta);

            // Abre el PDF automáticamente con el lector predeterminado de Windows.
            Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });

            return Task.CompletedTask;
        }
    }
}