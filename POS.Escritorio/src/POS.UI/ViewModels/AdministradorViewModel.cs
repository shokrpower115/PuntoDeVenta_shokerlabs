using POS.Core.Services;

namespace POS.UI.ViewModels
{
    public class AdministradorViewModel : ViewModelBase
    {
        private readonly IProductoService _productoService;
        private readonly IAuthService _authService;

        private ViewModelBase _pantallaActual = null!;
        public ViewModelBase PantallaActual
        {
            get => _pantallaActual;
            set => SetProperty(ref _pantallaActual, value);
        }

        public AdministradorViewModel(IProductoService productoService, IAuthService authService)
        {
            _productoService = productoService;
            _authService = authService;

            var irAProductos = new RelayCommand(MostrarProductos);
            var irAUsuarios = new RelayCommand(MostrarUsuarios);

            PantallaActual = new AdministradorMenuViewModel(irAProductos, irAUsuarios);
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

        private void MostrarMenu()
        {
            var irAProductos = new RelayCommand(MostrarProductos);
            var irAUsuarios = new RelayCommand(MostrarUsuarios);
            PantallaActual = new AdministradorMenuViewModel(irAProductos, irAUsuarios);
        }
    }
}