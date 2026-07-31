using POS.Core.Models;
using POS.Core.Services;

namespace POS.UI.ViewModels
{
    // Este ViewModel es el "cascarón": decide qué pantalla se muestra
    // en el área central de la ventana principal.
    public class MainViewModel : ViewModelBase
    {
        private readonly IProductoService _productoService;
        private readonly IVentaService _ventaService;
        private readonly ICorteCajaService _corteCajaService;
        private readonly IImpresoraTicketService _impresoraTicketService;
        private readonly DatosNegocio _datosNegocio;
        private readonly IAuthService _authService;
        private readonly IMetodoPagoService _metodoPagoService;

        // Ahora viene del login, ya no está fijo.
        public int SucursalActualId { get; }
        public string NombreUsuarioActual { get; }
        public string PuestoActual { get; }
        public int UsuarioIdActual { get; }
        public string NombreUsuarioLogin { get; }

        private ViewModelBase _pantallaActual = null!;
        public ViewModelBase PantallaActual
        {
            get => _pantallaActual;
            set => SetProperty(ref _pantallaActual, value);
        }

        public event Action? SesionCerrada;
        public RelayCommand CerrarSesionCommand { get; }

        public RelayCommand IrAVentaCommand { get; }
        public RelayCommand IrAAdministradorCommand { get; }

        public MainViewModel(IProductoService productoService, IVentaService ventaService,
              ICorteCajaService corteCajaService, IImpresoraTicketService impresoraTicketService,
              IMetodoPagoService metodoPagoService, IAuthService authService,
              DatosNegocio datosNegocio, Usuario usuarioActual)
        {
            _productoService = productoService;
            _ventaService = ventaService;
            _corteCajaService = corteCajaService;
            _impresoraTicketService = impresoraTicketService;
            _metodoPagoService = metodoPagoService;
            _datosNegocio = datosNegocio;
            _authService = authService;

            SucursalActualId = usuarioActual.SucursalId;
            NombreUsuarioActual = usuarioActual.NombreCompleto;
            UsuarioIdActual = usuarioActual.Id;
            NombreUsuarioLogin = usuarioActual.NombreUsuario;

            CerrarSesionCommand = new RelayCommand(async () => await CerrarSesionAsync());

            IrAVentaCommand = new RelayCommand(() =>
            PantallaActual = new VentaViewModel(_productoService, _ventaService, _impresoraTicketService,
                _datosNegocio, SucursalActualId, NombreUsuarioActual, UsuarioIdActual, _metodoPagoService,
                _corteCajaService, _authService, NombreUsuarioLogin));

            
            IrAAdministradorCommand = new RelayCommand(() =>
                PantallaActual = new AdministradorViewModel(_productoService, _authService));

            // Pantalla inicial
            PantallaActual = new VentaViewModel(_productoService, _ventaService, _impresoraTicketService,
                _datosNegocio, SucursalActualId, NombreUsuarioActual, UsuarioIdActual, _metodoPagoService,
                _corteCajaService, _authService, NombreUsuarioLogin);
        }

        private async Task CerrarSesionAsync()
        {
            var turnoAbierto = await _corteCajaService.ObtenerTurnoAbiertoAsync(SucursalActualId);

            if (turnoAbierto != null)
            {
                var resultado = System.Windows.MessageBox.Show(
                    "No ha realizado el corte del día. ¿Está seguro que desea cerrar sesión?",
                    "Corte pendiente",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning);

                if (resultado != System.Windows.MessageBoxResult.Yes)
                    return; // "Regresar" — no hace nada, se queda en la pantalla actual
            }

            SesionCerrada?.Invoke();
        }
    }
}
