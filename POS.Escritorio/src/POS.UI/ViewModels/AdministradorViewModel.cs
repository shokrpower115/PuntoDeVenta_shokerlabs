using POS.Core.Models;
using POS.Core.Services;

namespace POS.UI.ViewModels
{
    public class AdministradorViewModel : ViewModelBase
    {
        private readonly IProductoService _productoService;
        private readonly IAuthService _authService;
        private readonly IConfiguracionNegocioService _configuracionService;
        private readonly DatosNegocio _datosNegocio;

        private ViewModelBase _pantallaActual = null!;
        public ViewModelBase PantallaActual
        {
            get => _pantallaActual;
            set => SetProperty(ref _pantallaActual, value);
        }

        public AdministradorViewModel(IProductoService productoService, IAuthService authService,
            IConfiguracionNegocioService configuracionService, DatosNegocio datosNegocio)
        {
            _productoService = productoService;
            _authService = authService;
            _configuracionService = configuracionService;
            _datosNegocio = datosNegocio;

            MostrarMenu();
        }

        private void MostrarProductos()
        {
            var vm = new ProductosAdminViewModel(_productoService);
            vm.VolverSolicitado += MostrarMenu;
            PantallaActual = vm;
        }

        private void MostrarUsuarios()
        {
            var vm = new UsuariosAdminViewModel(_authService);
            vm.VolverSolicitado += MostrarMenu;
            PantallaActual = vm;
        }

        private void MostrarConfiguracion()
        {
            var vm = new ConfiguracionNegocioViewModel(_configuracionService, _datosNegocio, esPrimerArranque: false);
            vm.VolverSolicitado += MostrarMenu;
            PantallaActual = vm;
        }

        private void MostrarMenu()
        {
            PantallaActual = new AdministradorMenuViewModel(
                new RelayCommand(MostrarProductos),
                new RelayCommand(MostrarUsuarios),
                new RelayCommand(MostrarConfiguracion));
        }
    }
}
