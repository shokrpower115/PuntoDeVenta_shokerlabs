using POS.Core;
using POS.Core.Models;
using POS.Core.Services;
using POS.Data;
using POS.Data.Impresion;
using POS.Data.Sqlite;
using POS.UI.ViewModels;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace POS.UI
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConfigurarLog();
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            try
            {
                // Crea la base de datos en una instalación nueva y aplica migraciones pendientes.
                InicializadorBaseDatos.Inicializar();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "No se pudo inicializar la base de datos en {Ruta}", RutaBaseDatos.Obtener());
                MessageBox.Show("No se pudo abrir la base de datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

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

                    var usuario = loginViewModel.UsuarioAutenticado;
                    Log.Information("Inicio de sesión: {Usuario} (sucursal {SucursalId})", usuario.NombreUsuario, usuario.SucursalId);

                    // 2) Contraseña por defecto o restablecida: se exige cambiarla antes de entrar.
                    if (usuario.DebeCambiarPassword)
                    {
                        var cambioWindow = new CambiarPasswordWindow(authService, usuario.Id, loginViewModel.Password);
                        if (cambioWindow.ShowDialog() != true)
                            continue; // Regresa al login
                    }

                    // 3) Hay una sola caja por sucursal: si otro cajero dejó su turno abierto,
                    //    nadie más puede vender en él hasta que se haga el corte. Un administrador
                    //    sí puede entrar (por ejemplo, para hacer ese corte).
                    var turnoAbierto = await corteCajaService.ObtenerTurnoAbiertoAsync(usuario.SucursalId);
                    if (turnoAbierto != null && turnoAbierto.UsuarioId != usuario.Id && !usuario.EsAdministrador)
                    {
                        MessageBox.Show(
                            $"La caja tiene un turno abierto por {turnoAbierto.Usuario?.NombreCompleto} " +
                            $"desde el {turnoAbierto.FechaApertura:dd/MM/yyyy hh:mm tt}.\n\n" +
                            "Esa persona (o un administrador) debe realizar el corte antes de que otro usuario use la caja.",
                            "Turno abierto por otro usuario", MessageBoxButton.OK, MessageBoxImage.Warning);
                        continue;
                    }

                    // 4) Login exitoso: mostramos Ventas de fondo (aunque el usuario aún no pueda operar).
                    var mainViewModel = new MainViewModel(
                        productoService, ventaService, corteCajaService,
                        impresoraTicketService, metodoPagoService, authService,
                        datosNegocio, usuario);

                    var mainWindow = new MainWindow(mainViewModel);
                    MainWindow = mainWindow;
                    mainWindow.Show();

                    // 5) Sin turno abierto hay que abrir uno — la ventana modal bloquea cualquier
                    //    clic en Ventas. Un administrador puede omitirlo para usar solo el Administrador.
                    if (turnoAbierto == null)
                    {
                        var fondoWindow = new FondoInicialWindow(
                            corteCajaService,
                            ventaService,
                            usuario.SucursalId,
                            usuario.Id)
                        {
                            Owner = mainWindow
                        };
                        bool? fondoOk = fondoWindow.ShowDialog();

                        if (fondoOk != true && !usuario.EsAdministrador)
                        {
                            MessageBox.Show("Debes abrir un turno para continuar.", "Turno requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                            mainWindow.Close();
                            continue; // Regresa al login, sin cerrar la app
                        }
                    }

                    // 6) Esperamos aquí hasta que el usuario cierre sesión (por ejemplo, tras un corte de caja).
                    var esperaCierre = new TaskCompletionSource<bool>();
                    mainWindow.Closed += (s, args) => esperaCierre.TrySetResult(true);
                    await esperaCierre.Task;

                    Log.Information("Cierre de sesión: {Usuario}", usuario.NombreUsuario);
                    // El bucle vuelve a empezar → nuevo LoginWindow
                }

                Shutdown();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error en el ciclo principal de la aplicación");
                MessageBox.Show("Error al iniciar la aplicación: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.CloseAndFlush();
            base.OnExit(e);
        }

        private void ConfigurarLog()
        {
            var carpetaLogs = Path.Combine(RutaBaseDatos.CarpetaDatos(), "logs");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(Path.Combine(carpetaLogs, "pos-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 60)
                .CreateLogger();

            // Los comandos async de los ViewModels terminan aquí si lanzan una excepción:
            // sin este manejador, cualquier error cerraría la aplicación a media venta.
            DispatcherUnhandledException += AlOcurrirErrorEnUI;

            TaskScheduler.UnobservedTaskException += (s, args) =>
            {
                Log.Error(args.Exception, "Excepción no observada en una tarea");
                args.SetObserved();
            };

            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
                Log.Fatal(args.ExceptionObject as Exception, "Excepción no controlada; la aplicación se cerrará");
        }

        private void AlOcurrirErrorEnUI(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            if (e.Exception is ReglaNegocioException regla)
            {
                Log.Warning("Regla de negocio: {Mensaje}", regla.Message);
                MessageBox.Show(regla.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Log.Error(e.Exception, "Error no controlado en la interfaz");
            MessageBox.Show(
                "Ocurrió un error inesperado y la operación no se completó.\n\n" + e.Exception.Message +
                "\n\nEl detalle quedó registrado en la bitácora de la aplicación.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
