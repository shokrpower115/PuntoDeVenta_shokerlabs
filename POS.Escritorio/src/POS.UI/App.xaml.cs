using POS.Core.Models;
using POS.Core.Services;
using POS.Data.Impresion;
using POS.Data.Mock;
using POS.UI.ViewModels;
using System.Windows;

namespace POS.UI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // === ÚNICO LUGAR QUE CAMBIARÁ cuando conectes la API real ===
            IProductoService productoService = new ProductoServiceMock();
            IVentaService ventaService = new VentaServiceMock();
            ICorteCajaService corteCajaService = new CorteCajaServiceMock();
            IAuthService authService = new AuthServiceMock();
            IImpresoraTicketService impresoraTicketService = new ImpresoraTicketPdfService();

            var datosNegocio = new DatosNegocio
            {
                Nombre = "MILYLAA",
                Direccion = "DIRECCION 123 COL. COLONIA",
                Telefono = "(555) 123 4567",
                Rfc = "RFC0031282AB1",
                SitioWeb = "www.abarrotespuntodeventa.com"
            };

            // 1) Mostramos el login PRIMERO, sin abrir todavía la ventana principal.
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var loginViewModel = new LoginViewModel(authService);
            var loginWindow = new LoginWindow(loginViewModel);

            // ShowDialog() bloquea aquí hasta que LoginWindow se cierre
            // (recuerda: se cierra sola cuando LoginExitoso se dispara).
            bool? loginOk = loginWindow.ShowDialog();

            // 2) Si el usuario cerró la ventana sin loguearse (la "X"), cerramos la app.
            if (loginOk != true || loginViewModel.UsuarioAutenticado == null)
            {
                Shutdown();
                return;
            }

            // 3) Login exitoso: ahora sí armamos la ventana principal,
            //    pasándole el usuario que acaba de autenticarse.
            var mainViewModel = new MainViewModel(
                 productoService, ventaService, corteCajaService,
                 impresoraTicketService, datosNegocio, loginViewModel.UsuarioAutenticado);

            var mainWindow = new MainWindow(mainViewModel);
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            mainWindow.Show();


            var negocioPrueba = new DatosNegocio
            {
                Nombre = "MILYLAA",
                Direccion = "DIRECCION 123 COL. COLONIA",
                Telefono = "(555) 123 4567",
                Rfc = "RFC0031282AB1",
                SitioWeb = "www.abarrotespuntodeventa.com"
            };

            var ticketPrueba = new TicketVenta
            {
                Folio = 1,
                Fecha = DateTime.Now,
                NombreCajero = "MOISES BARRAA",
                Total = 10,
                PagoCon = 10,
                Detalles = new()
                {
                    new VentaDetalle { NombreProducto = "ROSA", Cantidad = 1, PrecioUnitario = 10 }
                }
            };

            IImpresoraTicketService impresora = new ImpresoraTicketPdfService();
            _ = impresora.ImprimirAsync(ticketPrueba, negocioPrueba);
        }
    }
}
