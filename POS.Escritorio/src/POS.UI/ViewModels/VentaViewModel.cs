using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.UI.ViewModels
{
    public class VentaViewModel : ViewModelBase
    {
        private readonly IProductoService _productoService;
        private readonly IVentaService _ventaService;
        private readonly IImpresoraTicketService _impresoraTicketService;
        private readonly IMetodoPagoService _metodoPagoService;
        private readonly ICorteCajaService _corteCajaService;

        private readonly DatosNegocio _datosNegocio;
        private readonly int _sucursalId;
        private readonly string _nombreCajero;
        private readonly int _usuarioId;

        public ObservableCollection<Producto> ProductosDisponibles { get; } = new();
        public ObservableCollection<VentaDetalle> Carrito { get; } = new();
        public ObservableCollection<DisponibilidadProducto> DisponibilidadOtrasSucursales { get; } = new();
        public ObservableCollection<MetodoPago> MetodosPago { get; } = new();

        private Producto? _productoSeleccionado;
        public Producto? ProductoSeleccionado
        {
            get => _productoSeleccionado;
            set => SetProperty(ref _productoSeleccionado, value);
        }

        private decimal _total;
        public decimal Total
        {
            get => _total;
            private set => SetProperty(ref _total, value);
        }

        private string _mensaje = string.Empty;
        public string Mensaje
        {
            get => _mensaje;
            set => SetProperty(ref _mensaje, value);
        }

        private MetodoPago? _metodoPagoSeleccionado;
        public MetodoPago? MetodoPagoSeleccionado
        {
            get => _metodoPagoSeleccionado;
            set => SetProperty(ref _metodoPagoSeleccionado, value);
        }

        public RelayCommand AgregarAlCarritoCommand { get; }
        public RelayCommand ConsultarOtrasSucursalesCommand { get; }
        public RelayCommand CobrarCommand { get; }
        public RelayCommand AbrirCorteCajaCommand { get; }
        public RelayCommand AbrirRetiroEfectivoCommand { get; }

        // Definir umbral
        private const decimal UMBRAL_ACUMULACION_EFECTIVO = 4000m;

        public VentaViewModel(IProductoService productoService, IVentaService ventaService,
            IImpresoraTicketService impresoraTicketService, DatosNegocio datosNegocio,
            int sucursalId, string nombreCajero, int usuarioId, IMetodoPagoService metodoPagoService,
            ICorteCajaService corteCajaService)
        {
            _productoService = productoService;
            _ventaService = ventaService;
            _impresoraTicketService = impresoraTicketService;
            _datosNegocio = datosNegocio;
            _sucursalId = sucursalId;
            _nombreCajero = nombreCajero;
            _usuarioId = usuarioId;
            _metodoPagoService = metodoPagoService;
            _corteCajaService = corteCajaService;

            AgregarAlCarritoCommand = new RelayCommand(async () => await AgregarAlCarritoAsync());
            ConsultarOtrasSucursalesCommand = new RelayCommand(async () => await ConsultarOtrasSucursalesAsync());
            CobrarCommand = new RelayCommand(async () => await CobrarAsync());
            AbrirCorteCajaCommand = new RelayCommand(() => AbrirCorteCaja());
            AbrirRetiroEfectivoCommand = new RelayCommand(() => AbrirRetiroEfectivo());

            _ = CargarProductosAsync();
            _ = CargarMetodosPagoAsync();
        }

        private async Task CargarProductosAsync()
        {
            var productos = await _productoService.ObtenerTodosAsync(_sucursalId);
            ProductosDisponibles.Clear();
            foreach (var p in productos)
                ProductosDisponibles.Add(p);
        }

        private async Task AgregarAlCarritoAsync()
        {
            if (ProductoSeleccionado == null) return;

            if ((ProductoSeleccionado.StockPorSucursal.FirstOrDefault()?.Stock ?? 0) <= 0)
            {
                Mensaje = $"Sin stock local de '{ProductoSeleccionado.Nombre}'. Puedes revisar otras sucursales.";
                await ConsultarOtrasSucursalesAsync();
                return;
            }

            Carrito.Add(new VentaDetalle
            {
                ProductoId = ProductoSeleccionado.Id,
                NombreProducto = ProductoSeleccionado.Nombre,
                Cantidad = 1,
                PrecioUnitario = ProductoSeleccionado.Precio
            });

            Total = Carrito.Sum(d => d.Subtotal);
            Mensaje = string.Empty;
        }

        private async Task ConsultarOtrasSucursalesAsync()
        {
            if (ProductoSeleccionado == null) return;

            var disponibilidad = await _productoService.ConsultarEnOtrasSucursalesAsync(ProductoSeleccionado.Id);
            DisponibilidadOtrasSucursales.Clear();
            foreach (var d in disponibilidad.Where(d => d.SucursalId != _sucursalId))
                DisponibilidadOtrasSucursales.Add(d);
        }

        private async Task CobrarAsync()
        {
            if (!Carrito.Any()) return;

            if (MetodoPagoSeleccionado == null)
            {
                Mensaje = "Selecciona un método de pago.";
                return;
            }

            var venta = new Venta
            {
                MetodoPagoId = MetodoPagoSeleccionado?.Id ?? 1,
                SucursalId = _sucursalId,
                UsuarioId = _usuarioId,
                Total = Total,
                Detalles = Carrito.ToList()
            };

            venta = await _ventaService.RegistrarVentaAsync(venta);
            foreach (var detalle in Carrito)
                await _productoService.DescontarStockAsync(detalle.ProductoId, detalle.Cantidad, _sucursalId);

            var ticket = new TicketVenta
            {
                Folio = venta.Id,
                Fecha = venta.Fecha,
                NombreCajero = _nombreCajero,
                Detalles = venta.Detalles,
                Total = venta.Total,
                PagoCon = venta.Total
            };

            await _impresoraTicketService.ImprimirAsync(ticket, _datosNegocio);

            Carrito.Clear();
            Total = 0;
            Mensaje = "Venta registrada correctamente.";
            await CargarProductosAsync();

            // -------------- CHECK ACUMULACION EFECTIVO --------------
            var turno = await _corteCajaService.ObtenerTurnoAbiertoAsync(_sucursalId);
            if (turno != null)
            {
                var totalEfectivo = await _ventaService.ObtenerTotalPorMetodoPagoAsync(_sucursalId, turno.FechaApertura, DateTime.Now, "Efectivo");
                if (totalEfectivo > UMBRAL_ACUMULACION_EFECTIVO)
                {
                    // Notificar y ofrecer abrir modal RETIRO
                    var result = System.Windows.MessageBox.Show(
                        $"Acumulaste {totalEfectivo:C} en efectivo desde el inicio del turno. ¿Deseas registrar un retiro ahora?",
                        "Alerta: Acumulación de efectivo",
                        System.Windows.MessageBoxButton.YesNo,
                        System.Windows.MessageBoxImage.Warning);

                    if (result == System.Windows.MessageBoxResult.Yes)
                    {
                        var corteVm = new CorteCajaViewModel(_corteCajaService, _ventaService, _sucursalId, _usuarioId);
                        var corteWindow = new CorteCajaWindow(corteVm)
                        {
                            Owner = System.Windows.Application.Current.MainWindow
                        };
                        corteWindow.ShowDialog();
                        // El usuario puede usar RegistrarRetiro dentro del corteVm
                    }
                }
            }
        }

        private async Task CargarMetodosPagoAsync()
        {
            var metodos = await _metodoPagoService.ObtenerActivosAsync();
            MetodosPago.Clear();
            foreach (var m in metodos)
                MetodosPago.Add(m);

            MetodoPagoSeleccionado = MetodosPago.FirstOrDefault();
        }

        // ===== NUEVOS MÉTODOS PARA ABRIR VENTANAS MODALES =====

        private void AbrirCorteCaja()
        {
            var corteCajaViewModel = new CorteCajaViewModel(_corteCajaService, _ventaService, _sucursalId, _usuarioId);
            var corteCajaWindow = new CorteCajaWindow(corteCajaViewModel)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            corteCajaWindow.ShowDialog();
        }

        private void AbrirRetiroEfectivo()
        {
            // TODO: Implementar ventana modal para Retiro de Efectivo
            Mensaje = "Funcionalidad de Retiro de Efectivo pendiente de implementar.";
        }
    }
}
