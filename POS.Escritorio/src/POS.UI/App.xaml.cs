using POS.Core.Models;
using POS.Core.Services;
using POS.Data.Impresion;
using POS.Data.Sqlite;
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
            IProductoService productoService = new ProductoServiceSqlite();
            IVentaService ventaService = new VentaServiceSqlite();
            ICorteCajaService corteCajaService = new CorteCajaServiceSqlite();
            IAuthService authService = new AuthServiceSqlite();
            IImpresoraTicketService impresoraTicketService = new ImpresoraTicketPdfService();
            IMetodoPagoService metodoPagoService = new MetodoPagoServiceSqlite();

            var datosNegocio = new DatosNegocio
            {
                Nombre = "MONTESORIO",
                Direccion = "DIRECCION 123 COL. COLONIA",
                Telefono = "(555) 123 4567",
                Rfc = "RFC0031282AB1",
                SitioWeb = "www.ColegioMontesori.com"
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
            impresoraTicketService, metodoPagoService, authService,
            datosNegocio, loginViewModel.UsuarioAutenticado);

            var mainWindow = new MainWindow(mainViewModel);
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            mainWindow.Show();

        }
    }
}
