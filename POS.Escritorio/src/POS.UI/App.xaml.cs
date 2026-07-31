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
                    SitioWeb = "[www.ColegioMontesori.com](https://www.ColegioMontesori.com)"
                };

                this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                bool continuarSesiones = true;
                while (continuarSesiones)
                {
                    // 1) Mostramos el login PRIMERO, sin abrir todavía la ventana principal.
                    var loginViewModel = new LoginViewModel(authService);
                    var loginWindow = new LoginWindow(loginViewModel);
                    bool? loginOk = loginWindow.ShowDialog();

                    if (loginOk != true || loginViewModel.UsuarioAutenticado == null)
                    {
                        continuarSesiones = false;
                        break;
                    }

                    // 2) Login exitoso: mostramos Ventas de fondo (aunque el usuario aún no pueda operar).
                    var mainViewModel = new MainViewModel(
                        productoService, ventaService, corteCajaService,
                        impresoraTicketService, metodoPagoService, authService,
                        datosNegocio, loginViewModel.UsuarioAutenticado);

                    var mainWindow = new MainWindow(mainViewModel);
                    mainWindow.Show();

                    // 3) Verificamos turno DESPUÉS de mostrar la ventana, pero antes de que el
                    //    usuario pueda interactuar — la ventana modal bloquea cualquier clic en Ventas.
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

                        if (fondoOk != true)
                        {
                            MessageBox.Show("Debes abrir un turno para continuar.", "Turno requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                            mainWindow.Close();
                            continue; // Regresa al login, sin cerrar la app
                        }
                    }

                    // 4) Esperamos aquí hasta que el usuario cierre sesión (por ejemplo, tras un corte de caja).
                    var esperaCierre = new TaskCompletionSource<bool>();
                    mainWindow.Closed += (s, args) => esperaCierre.TrySetResult(true);
                    await esperaCierre.Task;

                    // El bucle vuelve a empezar → nuevo LoginWindow
                }

                Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar la aplicación: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
