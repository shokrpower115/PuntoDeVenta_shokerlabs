using POS.Core.Models;
using POS.Core.Services;
using POS.Data.Impresion;
using POS.Data.Sqlite;
using POS.UI.ViewModels;
using System;
using System.Windows;

namespace POS.UI
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
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

                bool? loginOk = loginWindow.ShowDialog();

                if (loginOk != true || loginViewModel.UsuarioAutenticado == null)
                {
                    Shutdown();
                    return;
                }

                // 2) Login exitoso: mostramos la ventana principal (Ventas) PRIMERO.
                var mainViewModel = new MainViewModel(
                    productoService, ventaService, corteCajaService,
                    impresoraTicketService, metodoPagoService, authService,
                    datosNegocio, loginViewModel.UsuarioAutenticado);

                var mainWindow = new MainWindow(mainViewModel);
                this.ShutdownMode = ShutdownMode.OnMainWindowClose;
                mainWindow.Show();

                // 3) Después de mostrar la ventana principal, si no hay turno abierto,
                //    solicitamos el fondo inicial en una ventana independiente (owned by mainWindow).
                var turnoAbierto = await corteCajaService.ObtenerTurnoAbiertoAsync(loginViewModel.UsuarioAutenticado.SucursalId);
                if (turnoAbierto == null)
                {
                    var fondoWindow = new FondoInicialWindow(
                        corteCajaService,
                        ventaService,
                        loginViewModel.UsuarioAutenticado.SucursalId,
                        loginViewModel.UsuarioAutenticado.Id)
                    {
                        Owner = mainWindow
                    };

                    bool? fondoOk = fondoWindow.ShowDialog();

                    // Si usuario cancela o no abrió turno, impedir continuar
                    turnoAbierto = await corteCajaService.ObtenerTurnoAbiertoAsync(loginViewModel.UsuarioAutenticado.SucursalId);
                    if (turnoAbierto == null)
                    {
                        MessageBox.Show("Debes abrir un turno para continuar.", "Turno requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                        Shutdown();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar la aplicación: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
