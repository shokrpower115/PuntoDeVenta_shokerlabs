using System.Windows;
using POS.UI.ViewModels;

namespace POS.UI
{
    public partial class MovimientoCajaWindow : Window
    {
        private readonly MovimientoCajaViewModel _viewModel;

        public MovimientoCajaWindow(MovimientoCajaViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.ConfirmadoExitoso += () => { DialogResult = true; Close(); };
            _viewModel.CancelarSolicitado += () => { DialogResult = false; Close(); };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.PasswordConfirmacion = PasswordBox.Password;
        }
    }
}