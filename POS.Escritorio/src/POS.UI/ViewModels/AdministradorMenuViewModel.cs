namespace POS.UI.ViewModels
{
    public class AdministradorMenuViewModel : ViewModelBase
    {
        public RelayCommand IrAProductosCommand { get; }
        public RelayCommand IrAUsuariosCommand { get; }
        public RelayCommand IrAConfiguracionCommand { get; }

        public AdministradorMenuViewModel(RelayCommand irAProductos, RelayCommand irAUsuarios, RelayCommand irAConfiguracion)
        {
            IrAProductosCommand = irAProductos;
            IrAUsuariosCommand = irAUsuarios;
            IrAConfiguracionCommand = irAConfiguracion;
        }
    }
}
