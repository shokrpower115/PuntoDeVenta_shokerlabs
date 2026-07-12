using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using POS.Core.Models;
using POS.Core.Services;
using System.Collections.Generic;

namespace POS.UI.ViewModels
{
    public class UsuariosAdminViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;

        public ObservableCollection<Usuario> Usuarios { get; } = new();

        public Dictionary<int, string> Sucursales { get; } = new()
        {
            { 1, "Sucursal Centro" },
            { 2, "Sucursal Norte" },
            { 3, "Sucursal Sur" },
        };

        public List<RolUsuario> RolesDisponibles { get; } = new()
        {
            RolUsuario.Cajero,
            RolUsuario.Administrador
        };

        private string _nuevoNombreUsuario = string.Empty;
        public string NuevoNombreUsuario
        {
            get => _nuevoNombreUsuario;
            set => SetProperty(ref _nuevoNombreUsuario, value);
        }

        private string _nuevoPassword = string.Empty;
        public string NuevoPassword
        {
            get => _nuevoPassword;
            set => SetProperty(ref _nuevoPassword, value);
        }

        private string _nuevoNombreCompleto = string.Empty;
        public string NuevoNombreCompleto
        {
            get => _nuevoNombreCompleto;
            set => SetProperty(ref _nuevoNombreCompleto, value);
        }

        private RolUsuario _nuevoRol = RolUsuario.Cajero;
        public RolUsuario NuevoRol
        {
            get => _nuevoRol;
            set => SetProperty(ref _nuevoRol, value);
        }

        private int _nuevaSucursalId = 1;
        public int NuevaSucursalId
        {
            get => _nuevaSucursalId;
            set => SetProperty(ref _nuevaSucursalId, value);
        }

        private string _mensaje = string.Empty;
        public string Mensaje
        {
            get => _mensaje;
            set => SetProperty(ref _mensaje, value);
        }

        public event Action? VolverSolicitado;
        public RelayCommand VolverCommand { get; }
        public RelayCommand CrearUsuarioCommand { get; }

        public UsuariosAdminViewModel(IAuthService authService)
        {
            _authService = authService;

            VolverCommand = new RelayCommand(() => VolverSolicitado?.Invoke());
            CrearUsuarioCommand = new RelayCommand(async () => await CrearUsuarioAsync());

            _ = CargarAsync();
        }

        private async Task CargarAsync()
        {
            var usuarios = await _authService.ObtenerTodosAsync();
            Usuarios.Clear();
            foreach (var u in usuarios)
                Usuarios.Add(u);
        }

        private async Task CrearUsuarioAsync()
        {
            if (string.IsNullOrWhiteSpace(NuevoNombreUsuario) || string.IsNullOrWhiteSpace(NuevoPassword))
            {
                Mensaje = "Escribe usuario y contraseña.";
                return;
            }

            var usuario = new Usuario
            {
                NombreUsuario = NuevoNombreUsuario,
                NombreCompleto = NuevoNombreCompleto,
                Puesto = NuevoRol == RolUsuario.Administrador ? "Administrador" : "Cajero",
                Rol = NuevoRol,
                SucursalId = NuevaSucursalId
            };

            await _authService.CrearUsuarioAsync(usuario, NuevoPassword);

            NuevoNombreUsuario = string.Empty;
            NuevoPassword = string.Empty;
            NuevoNombreCompleto = string.Empty;
            Mensaje = "Usuario creado correctamente.";

            await CargarAsync();
        }
    }
}