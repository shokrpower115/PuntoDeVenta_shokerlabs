namespace POS.UI.ViewModels
{
    public class AdministradorMenuViewModel : ViewModelBase
    {
        public RelayCommand IrAProductosCommand { get; }
        public RelayCommand IrAUsuariosCommand { get; }

        public AdministradorMenuViewModel(RelayCommand irAProductos, RelayCommand irAUsuarios)
        {
            IrAProductosCommand = irAProductos;
            IrAUsuariosCommand = irAUsuarios;
        }
    }
}