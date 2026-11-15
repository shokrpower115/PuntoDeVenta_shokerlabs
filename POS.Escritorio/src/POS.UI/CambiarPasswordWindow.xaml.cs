using System.Windows;
using POS.Core.Services;

namespace POS.UI
{
    public partial class CambiarPasswordWindow : Window
    {
        private const int LongitudMinima = 6;

        private readonly IAuthService _authService;
        private readonly int _usuarioId;
        private readonly string _passwordActual;

        public CambiarPasswordWindow(IAuthService authService, int usuarioId, string passwordActual)
        {
            InitializeComponent();
            _authService = authService;
            _usuarioId = usuarioId;
            _passwordActual = passwordActual;
        }

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            var nueva = PasswordNueva.Password;

            if (nueva.Length < LongitudMinima)
            {
                TxtError.Text = $"La contraseña debe tener al menos {LongitudMinima} caracteres.";
                return;
            }
            if (nueva != PasswordConfirmacion.Password)
            {
                TxtError.Text = "Las contraseñas no coinciden.";
                return;
            }
            if (nueva == _passwordActual)
            {
                TxtError.Text = "La contraseña nueva debe ser distinta a la actual.";
                return;
            }

            await _authService.CambiarPasswordAsync(_usuarioId, nueva);
            DialogResult = true;
            Close();
        }
    }
}
