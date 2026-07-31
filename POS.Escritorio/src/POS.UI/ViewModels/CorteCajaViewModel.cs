using POS.Core.Models;
using POS.Core.Services;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace POS.UI.ViewModels
{
    public class CorteCajaViewModel : ViewModelBase
    {
        private readonly ICorteCajaService _corteCajaService;
        private readonly IVentaService _ventaService;
        private readonly int _sucursalId;
        private readonly int _usuarioId;

        private readonly IImpresoraTicketService _impresoraTicketService;
        private readonly DatosNegocio _datosNegocio;

        private readonly IAuthService _authService;
        private readonly string _nombreUsuarioEsperado;

        private string _mensaje = string.Empty;
        public string Mensaje
        {
            get => _mensaje;
            set => SetProperty(ref _mensaje, value);
        }

        public ObservableCollection<CategoriaMovimientoCaja> CategoriasEntrada { get; } = new();
        public ObservableCollection<CategoriaMovimientoCaja> CategoriasRetiro { get; } = new();


        private CorteDeCaja? _turnoActual;
        public CorteDeCaja? TurnoActual
        {
            get => _turnoActual;
            set => SetProperty(ref _turnoActual, value);
        }

        private decimal _efectivoInicial;
        public decimal EfectivoInicial
        {
            get => _efectivoInicial;
            set => SetProperty(ref _efectivoInicial, value);
        }

        private decimal _efectivoFinal;
        public decimal EfectivoFinal
        {
            get => _efectivoFinal;
            set => SetProperty(ref _efectivoFinal, value);
        }

        private decimal _totalVentasTurno;
        public decimal TotalVentasTurno
        {
            get => _totalVentasTurno;
            set => SetProperty(ref _totalVentasTurno, value);
        }

        private decimal _totalEfectivo;
        public decimal TotalEfectivo
        {
            get => _totalEfectivo;
            private set => SetProperty(ref _totalEfectivo, value);
        }

        private decimal _totalTarjeta;
        public decimal TotalTarjeta
        {
            get => _totalTarjeta;
            private set => SetProperty(ref _totalTarjeta, value);
        }

        public decimal EfectivoEsperado => TurnoActual?.EfectivoEsperado ?? 0;

        public RelayCommand AbrirTurnoCommand { get; }
        public RelayCommand CerrarTurnoCommand { get; }

        public CorteCajaViewModel(ICorteCajaService corteCajaService, IVentaService ventaService,
            IAuthService authService, IImpresoraTicketService impresoraTicketService, DatosNegocio datosNegocio,
            int sucursalId, int usuarioId, string nombreUsuarioEsperado)
        {
            _corteCajaService = corteCajaService;
            _ventaService = ventaService;
            _authService = authService;
            _sucursalId = sucursalId;
            _usuarioId = usuarioId;
            _nombreUsuarioEsperado = nombreUsuarioEsperado;
            _impresoraTicketService = impresoraTicketService;
            _datosNegocio = datosNegocio;

            AbrirTurnoCommand = new RelayCommand(async () => await AbrirTurnoAsync());
            CerrarTurnoCommand = new RelayCommand(async () => await CerrarTurnoAsync());
            RegistrarEntradaCommand = new RelayCommand(async () => await RegistrarEntradaAsync());
            RegistrarRetiroCommand = new RelayCommand(async () => await RegistrarRetiroAsync());
            RealizarCorteCommand = new RelayCommand(async () => await RealizarCorteAsync());


            _ = CargarTurnoAsync();
            _ = CargarCategoriasAsync();
        }

        private CategoriaMovimientoCaja? _categoriaEntradaSeleccionada;
        public CategoriaMovimientoCaja? CategoriaEntradaSeleccionada
        {
            get => _categoriaEntradaSeleccionada;
            set => SetProperty(ref _categoriaEntradaSeleccionada, value);
        }

        private CategoriaMovimientoCaja? _categoriaRetiroSeleccionada;
        public CategoriaMovimientoCaja? CategoriaRetiroSeleccionada
        {
            get => _categoriaRetiroSeleccionada;
            set => SetProperty(ref _categoriaRetiroSeleccionada, value);
        }

        private string _descripcionOtroEntrada = string.Empty;
        public string DescripcionOtroEntrada
        {
            get => _descripcionOtroEntrada;
            set => SetProperty(ref _descripcionOtroEntrada, value);
        }

        private decimal _montoEntrada;
        public decimal MontoEntrada
        {
            get => _montoEntrada;
            set => SetProperty(ref _montoEntrada, value);
        }

        private string _descripcionOtroRetiro = string.Empty;
        public string DescripcionOtroRetiro
        {
            get => _descripcionOtroRetiro;
            set => SetProperty(ref _descripcionOtroRetiro, value);
        }

        private decimal _montoRetiro;
        public decimal MontoRetiro
        {
            get => _montoRetiro;
            set => SetProperty(ref _montoRetiro, value);
        }

        public RelayCommand RegistrarEntradaCommand { get; }
        public RelayCommand RegistrarRetiroCommand { get; }

        private string _nombreUsuarioConfirmacion = string.Empty;
        public string NombreUsuarioConfirmacion
        {
            get => _nombreUsuarioConfirmacion;
            set => SetProperty(ref _nombreUsuarioConfirmacion, value);
        }

        public string PasswordConfirmacion { get; set; } = string.Empty; // No necesita SetProperty, no se bindea visualmente por seguridad

        private string _mensajeError = string.Empty;
        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        private async Task CargarTurnoAsync()
        {
            TurnoActual = await _corteCajaService.ObtenerTurnoAbiertoAsync(_sucursalId);
            if (TurnoActual != null)
                await ActualizarTotalVentasAsync();
        }

        private async Task AbrirTurnoAsync()
        {
            TurnoActual = await _corteCajaService.AbrirTurnoAsync(_sucursalId, _usuarioId, EfectivoInicial);
            await ActualizarTotalVentasAsync();

            Mensaje = "Ingreso de efectivo con exito.";
        }

        private async Task ActualizarTotalVentasAsync()
        {
            if (TurnoActual == null) return;

            TotalEfectivo = await _ventaService.ObtenerTotalPorMetodoPagoAsync(
                _sucursalId, TurnoActual.FechaApertura, DateTime.Now, "Efectivo");

            TotalTarjeta = await _ventaService.ObtenerTotalPorMetodoPagoAsync(
                _sucursalId, TurnoActual.FechaApertura, DateTime.Now, "Tarjeta de crédito")
                + await _ventaService.ObtenerTotalPorMetodoPagoAsync(
                _sucursalId, TurnoActual.FechaApertura, DateTime.Now, "Tarjeta de débito");

            TotalVentasTurno = TotalEfectivo + TotalTarjeta; // Solo informativo, ya no se usa para la diferencia
        }

        private async Task CerrarTurnoAsync()
        {
            if (TurnoActual == null) return;
            await ActualizarTotalVentasAsync();
            TurnoActual = await _corteCajaService.CerrarTurnoAsync(TurnoActual.Id, EfectivoFinal, TotalEfectivo, TotalTarjeta);
        }

        private async Task CargarCategoriasAsync()
        {
            var entradas = await _corteCajaService.ObtenerCategoriasActivasAsync(TipoMovimientoCaja.Entrada);
            CategoriasEntrada.Clear();
            foreach (var c in entradas) CategoriasEntrada.Add(c);

            var retiros = await _corteCajaService.ObtenerCategoriasActivasAsync(TipoMovimientoCaja.Retiro);
            CategoriasRetiro.Clear();
            foreach (var c in retiros) CategoriasRetiro.Add(c);
        }

        private async Task RegistrarEntradaAsync()
        {
            if (TurnoActual == null)
            {
                Mensaje = "Primero debes abrir un turno.";
                return;
            }
            if (CategoriaEntradaSeleccionada == null)
            {
                Mensaje = "Selecciona una categoría.";
                return;
            }
            if (MontoEntrada <= 0)
            {
                Mensaje = "Escribe un monto mayor a 0.";
                return;
            }

            var movimiento = new MovimientoCaja
            {
                CorteCajaId = TurnoActual.Id,
                CategoriaId = CategoriaEntradaSeleccionada.Id,
                DescripcionOtro = CategoriaEntradaSeleccionada.Nombre == "Otro" ? DescripcionOtroEntrada : null,
                Monto = MontoEntrada,
                UsuarioId = _usuarioId
            };

            await _corteCajaService.RegistrarMovimientoAsync(movimiento);

            MontoEntrada = 0;
            DescripcionOtroEntrada = string.Empty;
            await CargarTurnoAsync(); // Recarga el turno para reflejar el movimiento nuevo en los totales
        }

        private async Task RegistrarRetiroAsync()
        {
            if (TurnoActual == null || CategoriaRetiroSeleccionada == null || MontoRetiro <= 0) return;

            var movimiento = new MovimientoCaja
            {
                CorteCajaId = TurnoActual.Id,
                CategoriaId = CategoriaRetiroSeleccionada.Id,
                DescripcionOtro = CategoriaRetiroSeleccionada.Nombre == "Otro" ? DescripcionOtroRetiro : null,
                Monto = MontoRetiro,
                UsuarioId = _usuarioId
            };

            await _corteCajaService.RegistrarMovimientoAsync(movimiento);

            MontoRetiro = 0;
            DescripcionOtroRetiro = string.Empty;
            await CargarTurnoAsync();
        }

        public RelayCommand RealizarCorteCommand { get; }

        public event Action? CorteFinalizado;
        private async Task RealizarCorteAsync()
        {
            MensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(NombreUsuarioConfirmacion) || string.IsNullOrWhiteSpace(PasswordConfirmacion))
            {
                MensajeError = "Confirma tu usuario y contraseña.";
                return;
            }

            // Paso 1: ¿el nombre de usuario escrito es el mismo que inició sesión?
            if (!NombreUsuarioConfirmacion.Trim().Equals(_nombreUsuarioEsperado, StringComparison.OrdinalIgnoreCase))
            {
                MensajeError = "Debes confirmar con tu propio usuario, no el de otra persona.";
                return;
            }

            // Paso 2: ¿la contraseña es correcta para ESE usuario?
            var usuarioValidado = await _authService.LoginAsync(NombreUsuarioConfirmacion, PasswordConfirmacion);
            if (usuarioValidado == null)
            {
                MensajeError = "Usuario o contraseña incorrectos.";
                return;
            }

            if (TurnoActual == null)
            {
                MensajeError = "No hay un turno abierto para cerrar.";
                return;
            }

            // Validación ya pasó: aquí sigue la fórmula del corte (siguiente paso).
            await ActualizarTotalVentasAsync();
            var corteCerrado = await _corteCajaService.CerrarTurnoAsync(TurnoActual.Id, EfectivoFinal, TotalEfectivo, TotalTarjeta);
            TurnoActual = corteCerrado;

            var desglose = await _ventaService.ObtenerDesglosePorMetodoPagoAsync(_sucursalId, corteCerrado.FechaApertura, corteCerrado.FechaCierre ?? DateTime.Now);
            var movimientos = await _corteCajaService.ObtenerMovimientosDelTurnoAsync(corteCerrado.Id);

            var ticket = new TicketCorte
            {
                CorteCajaId = corteCerrado.Id,
                NombreCajero = _nombreUsuarioEsperado,
                FechaApertura = corteCerrado.FechaApertura,
                FechaCierre = corteCerrado.FechaCierre ?? DateTime.Now,
                EfectivoInicial = corteCerrado.EfectivoInicial,
                VentasPorMetodoPago = desglose,
                Movimientos = movimientos,
                TotalEntradas = movimientos.Where(m => m.Categoria?.Tipo == TipoMovimientoCaja.Entrada).Sum(m => m.Monto),
                TotalRetiros = movimientos.Where(m => m.Categoria?.Tipo == TipoMovimientoCaja.Retiro).Sum(m => m.Monto),
                EfectivoEsperado = corteCerrado.EfectivoEsperado,
                EfectivoContado = EfectivoFinal,
                Diferencia = corteCerrado.Diferencia
            };

            await _impresoraTicketService.ImprimirCorteAsync(ticket, _datosNegocio);

            MensajeError = "Corte realizado correctamente.";

            CorteFinalizado?.Invoke();
        }
    }
}
