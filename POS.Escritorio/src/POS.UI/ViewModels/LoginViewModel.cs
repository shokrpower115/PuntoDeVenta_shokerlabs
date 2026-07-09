using System;
using System.Threading.Tasks;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.UI.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;

        // Esto es lo que MainWindow espera recibir cuando el login sea exitoso.
        public Usuario? UsuarioAutenticado { get; private set; }

        // Evento para avisarle a App.xaml.cs "ya terminé, con éxito o sin él".
        public event Action? LoginExitoso;

        private string _nombreUsuario = string.Empty;
        public string NombreUsuario
        {
            get => _nombreUsuario;
            set => SetProperty(ref _nombreUsuario, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _mensajeError = string.Empty;
        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        public RelayCommand IniciarSesionCommand { get; }

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            IniciarSesionCommand = new RelayCommand(async () => await IniciarSesionAsync());
        }

        private async Task IniciarSesionAsync()
        {
            MensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(NombreUsuario) || string.IsNullOrWhiteSpace(Password))
            {
                MensajeError = "Escribe usuario y contraseña.";
                return;
            }

            var usuario = await _authService.LoginAsync(NombreUsuario, Password);

            if (usuario == null)
            {
                MensajeError = "Usuario o contraseña incorrectos.";
                return;
            }

            UsuarioAutenticado = usuario;
            LoginExitoso?.Invoke();
        }
    }
}
