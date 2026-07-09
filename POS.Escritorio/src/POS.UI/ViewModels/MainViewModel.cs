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

        // Ahora viene del login, ya no está fijo.
        public int SucursalActualId { get; }
        public string NombreUsuarioActual { get; }
        public string PuestoActual { get; }

        private ViewModelBase _pantallaActual = null!;
        public ViewModelBase PantallaActual
        {
            get => _pantallaActual;
            set => SetProperty(ref _pantallaActual, value);
        }

        public RelayCommand IrAVentaCommand { get; }
        public RelayCommand IrAInventarioCommand { get; }
        public RelayCommand IrACorteCajaCommand { get; }

        public MainViewModel(IProductoService productoService, IVentaService ventaService,
            ICorteCajaService corteCajaService, IImpresoraTicketService impresoraTicketService,
            DatosNegocio datosNegocio, Usuario usuarioActual)
        {
            _productoService = productoService;
            _ventaService = ventaService;
            _corteCajaService = corteCajaService;
            _impresoraTicketService = impresoraTicketService;
            _datosNegocio = datosNegocio;

            SucursalActualId = usuarioActual.SucursalId;
            NombreUsuarioActual = usuarioActual.NombreCompleto;
            PuestoActual = usuarioActual.Puesto;

            IrAVentaCommand = new RelayCommand(() =>
                PantallaActual = new VentaViewModel(_productoService, _ventaService, _impresoraTicketService,
                    _datosNegocio, SucursalActualId, NombreUsuarioActual));

            IrAInventarioCommand = new RelayCommand(() =>
                PantallaActual = new InventarioViewModel(_productoService, SucursalActualId));

            IrACorteCajaCommand = new RelayCommand(() =>
                PantallaActual = new CorteCajaViewModel(_corteCajaService, _ventaService, SucursalActualId));

            // Pantalla inicial
            PantallaActual = new VentaViewModel(_productoService, _ventaService, _impresoraTicketService,
                _datosNegocio, SucursalActualId, NombreUsuarioActual);
        }
    }
}
