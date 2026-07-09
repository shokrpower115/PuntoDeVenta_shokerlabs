using System.Windows;
using POS.UI.ViewModels;

namespace POS.UI
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            // Cuando el login es exitoso, cerramos esta ventana con "resultado positivo".
            // App.xaml.cs está esperando esto para abrir la ventana principal.
            _viewModel.LoginExitoso += () =>
            {
                DialogResult = true;
                Close();
            };
        }

        // PasswordBox no permite Binding directo por seguridad (no se puede
        // referenciar su contenido desde XAML), así que lo pasamos a mano
        // cada vez que el usuario teclea.
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
        }
    }
}
