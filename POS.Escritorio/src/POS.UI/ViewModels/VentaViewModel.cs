using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using POS.Core.Models;
using POS.Core.Services;
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

        public VentaViewModel(IProductoService productoService, IVentaService ventaService,
            IImpresoraTicketService impresoraTicketService, DatosNegocio datosNegocio,
            int sucursalId, string nombreCajero, int usuarioId, IMetodoPagoService metodoPagoService)
        {
            _productoService = productoService;
            _ventaService = ventaService;
            _impresoraTicketService = impresoraTicketService;
            _datosNegocio = datosNegocio;
            _sucursalId = sucursalId;
            _nombreCajero = nombreCajero;
            _usuarioId = usuarioId;
            _metodoPagoService = metodoPagoService;

            AgregarAlCarritoCommand = new RelayCommand(async () => await AgregarAlCarritoAsync());
            ConsultarOtrasSucursalesCommand = new RelayCommand(async () => await ConsultarOtrasSucursalesAsync());
            CobrarCommand = new RelayCommand(async () => await CobrarAsync());

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
                MetodoPagoId = 1, // TODO: temporal, hardcodeado a "Efectivo" hasta que exista el selector de método de pago
                SucursalId = _sucursalId,
                UsuarioId = _usuarioId,
                Total = Total,
                Detalles = Carrito.ToList()
            };

            venta = await _ventaService.RegistrarVentaAsync(venta);

            foreach (var detalle in Carrito)
                await _productoService.DescontarStockAsync(detalle.ProductoId, detalle.Cantidad, _sucursalId);

            // Armamos el ticket a partir de la venta que YA quedó registrada
            // (así el folio es el Id real que le asignó el servicio, no un número inventado).
            var ticket = new TicketVenta
            {
                Folio = venta.Id,
                Fecha = venta.Fecha,
                NombreCajero = _nombreCajero,
                Detalles = venta.Detalles,
                Total = venta.Total,
                PagoCon = venta.Total // Por ahora asumimos pago exacto; esto cambia cuando agreguemos el campo de "efectivo recibido".
            };

            await _impresoraTicketService.ImprimirAsync(ticket, _datosNegocio);

            Carrito.Clear();
            Total = 0;
            Mensaje = "Venta registrada correctamente.";
            await CargarProductosAsync();
        }

        private async Task CargarMetodosPagoAsync()
        {
            var metodos = await _metodoPagoService.ObtenerActivosAsync();
            MetodosPago.Clear();
            foreach (var m in metodos)
                MetodosPago.Add(m);

            MetodoPagoSeleccionado = MetodosPago.FirstOrDefault(); // Efectivo por defecto, al ser el primero
        }
    }
}
