using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.UI.ViewModels
{
    public class MovimientoCajaViewModel : ViewModelBase
    {
        private readonly ICorteCajaService _corteCajaService;
        private readonly IAuthService _authService;
        private readonly int _sucursalId;
        private readonly int _usuarioId;
        private readonly string _nombreUsuarioEsperado;

        public ObservableCollection<CategoriaMovimientoCaja> RazonesDisponibles { get; } = new();

        private TipoMovimientoCaja _tipoSeleccionado = TipoMovimientoCaja.Retiro;
        public TipoMovimientoCaja TipoSeleccionado
        {
            get => _tipoSeleccionado;
            set
            {
                if (SetProperty(ref _tipoSeleccionado, value))
                    _ = CargarRazonesAsync();
            }
        }

        private CategoriaMovimientoCaja? _razonSeleccionada;
        public CategoriaMovimientoCaja? RazonSeleccionada
        {
            get => _razonSeleccionada;
            set
            {
                if (SetProperty(ref _razonSeleccionada, value))
                    OnPropertyChanged(nameof(RequiereDescripcionOtro));
            }
        }

        public bool RequiereDescripcionOtro => RazonSeleccionada?.Nombre == "Otro";

        private string _descripcionOtro = string.Empty;
        public string DescripcionOtro
        {
            get => _descripcionOtro;
            set => SetProperty(ref _descripcionOtro, value);
        }

        private decimal _monto;
        public decimal Monto
        {
            get => _monto;
            set => SetProperty(ref _monto, value);
        }

        private string _nombreUsuarioConfirmacion = string.Empty;
        public string NombreUsuarioConfirmacion
        {
            get => _nombreUsuarioConfirmacion;
            set => SetProperty(ref _nombreUsuarioConfirmacion, value);
        }

        public string PasswordConfirmacion { get; set; } = string.Empty;

        private string _mensajeError = string.Empty;
        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        public event Action? ConfirmadoExitoso;
        public event Action? CancelarSolicitado;

        public RelayCommand ConfirmarCommand { get; }
        public RelayCommand CancelarCommand { get; }

        public MovimientoCajaViewModel(ICorteCajaService corteCajaService, IAuthService authService,
            int sucursalId, int usuarioId, string nombreUsuarioEsperado)
        {
            _corteCajaService = corteCajaService;
            _authService = authService;
            _sucursalId = sucursalId;
            _usuarioId = usuarioId;
            _nombreUsuarioEsperado = nombreUsuarioEsperado;

            ConfirmarCommand = new RelayCommand(async () => await ConfirmarAsync());
            CancelarCommand = new RelayCommand(() => CancelarSolicitado?.Invoke());

            _ = CargarRazonesAsync();
        }

        private async Task CargarRazonesAsync()
        {
            var razones = await _corteCajaService.ObtenerCategoriasActivasAsync(TipoSeleccionado);
            RazonesDisponibles.Clear();
            foreach (var r in razones)
                RazonesDisponibles.Add(r);

            RazonSeleccionada = null; // Forzamos que elija de nuevo al cambiar de Tipo
        }

        private async Task ConfirmarAsync()
        {
            MensajeError = string.Empty;

            if (RazonSeleccionada == null)
            {
                MensajeError = "Selecciona una razón.";
                return;
            }
            if (Monto <= 0)
            {
                MensajeError = "Escribe un monto mayor a 0.";
                return;
            }
            if (string.IsNullOrWhiteSpace(NombreUsuarioConfirmacion) || string.IsNullOrWhiteSpace(PasswordConfirmacion))
            {
                MensajeError = "Confirma tu usuario y contraseña.";
                return;
            }
            if (!NombreUsuarioConfirmacion.Trim().Equals(_nombreUsuarioEsperado, StringComparison.OrdinalIgnoreCase))
            {
                MensajeError = "Debes confirmar con tu propio usuario.";
                return;
            }

            var usuarioValidado = await _authService.LoginAsync(NombreUsuarioConfirmacion, PasswordConfirmacion);
            if (usuarioValidado == null)
            {
                MensajeError = "Usuario o contraseña incorrectos.";
                return;
            }

            var turno = await _corteCajaService.ObtenerTurnoAbiertoAsync(_sucursalId);
            if (turno == null)
            {
                MensajeError = "No hay un turno abierto en esta sucursal.";
                return;
            }

            var movimiento = new MovimientoCaja
            {
                CorteCajaId = turno.Id,
                CategoriaId = RazonSeleccionada.Id,
                DescripcionOtro = RequiereDescripcionOtro ? DescripcionOtro : null,
                Monto = Monto,
                UsuarioId = _usuarioId
            };

            await _corteCajaService.RegistrarMovimientoAsync(movimiento);

            ConfirmadoExitoso?.Invoke();
        }
    }
}