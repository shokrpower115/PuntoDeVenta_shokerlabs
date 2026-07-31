using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using POS.Core.Models;
using POS.Core.Services;
using System;

namespace POS.UI.ViewModels
{
    public class VentaViewModel : ViewModelBase
    {
        private readonly IProductoService _productoService;
        private readonly IVentaService _ventaService;
        private readonly IImpresoraTicketService _impresoraTicketService;
        private readonly IMetodoPagoService _metodoPagoService;
        private readonly ICorteCajaService _corteCajaService;
        private readonly IAuthService _authService;
        private readonly string _nombreUsuario;

        private readonly DatosNegocio _datosNegocio;
        private readonly int _sucursalId;
        private readonly string _nombreCajero;
        private readonly int _usuarioId;

        public ObservableCollection<Producto> ProductosDisponibles { get; } = new();
        public ObservableCollection<VentaDetalle> Carrito { get; } = new();
        public ObservableCollection<DisponibilidadProducto> DisponibilidadOtrasSucursales { get; } = new();
        public ObservableCollection<MetodoPago> MetodosPago { get; } = new();
        public RelayCommand AbrirInventarioCommand { get; }


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
    ICorteCajaService corteCajaService, IAuthService authService, string nombreUsuario)
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
            _authService = authService;
            _nombreUsuario = nombreUsuario;

            AgregarAlCarritoCommand = new RelayCommand(async () => await AgregarAlCarritoAsync());
            ConsultarOtrasSucursalesCommand = new RelayCommand(async () => await ConsultarOtrasSucursalesAsync());
            CobrarCommand = new RelayCommand(async () => await CobrarAsync());
            AbrirCorteCajaCommand = new RelayCommand(() => AbrirCorteCaja());
            AbrirRetiroEfectivoCommand = new RelayCommand(() => AbrirRetiroEfectivo());
            AbrirInventarioCommand = new RelayCommand(() => AbrirInventario());
            AumentarCantidadCommand = new RelayCommand<VentaDetalle>(AumentarCantidad);
            DisminuirCantidadCommand = new RelayCommand<VentaDetalle>(DisminuirCantidad);
            EliminarDelCarritoCommand = new RelayCommand<VentaDetalle>(EliminarDelCarrito);

            _ = CargarProductosAsync();
            _ = CargarMetodosPagoAsync();
        }

        private async Task CargarProductosAsync()
        {
            _productosCompletos = await _productoService.ObtenerTodosAsync(_sucursalId);
            FiltrarProductos();
        }

        private async Task AgregarAlCarritoAsync()
        {
            if (ProductoSeleccionado == null) return;

            int stockDisponible = ProductoSeleccionado.StockPorSucursal.FirstOrDefault()?.Stock ?? 0;

            // Cuánto de ESTE producto ya lleva el cajero en el carrito
            int cantidadEnCarrito = Carrito
                .Where(d => d.ProductoId == ProductoSeleccionado.Id)
                .Sum(d => d.Cantidad);

            if (cantidadEnCarrito >= stockDisponible)
            {
                Mensaje = $"Sin stock disponible de '{ProductoSeleccionado.Nombre}' (ya tienes {cantidadEnCarrito} en el carrito, stock: {stockDisponible}). Puedes revisar otras sucursales.";
                await ConsultarOtrasSucursalesAsync();
                return;
            }

            // Si el producto ya está en el carrito, solo incrementa la cantidad
            // en vez de crear una línea duplicada.
            var lineaExistente = Carrito.FirstOrDefault(d => d.ProductoId == ProductoSeleccionado.Id);
            if (lineaExistente != null)
            {
                lineaExistente.Cantidad++;
                // Forzamos actualización visual del DataGrid, ya que VentaDetalle
                // no implementa notificación de cambios por sí solo.
                var index = Carrito.IndexOf(lineaExistente);
                Carrito[index] = lineaExistente;
            }
            else
            {
                Carrito.Add(new VentaDetalle
                {
                    ProductoId = ProductoSeleccionado.Id,
                    NombreProducto = ProductoSeleccionado.Nombre,
                    Cantidad = 1,
                    PrecioUnitario = ProductoSeleccionado.Precio
                });
            }

            Total = Carrito.Sum(d => d.Subtotal);
            Mensaje = string.Empty;
        }

        private async Task ConsultarOtrasSucursalesAsync()
        {
            if (ProductoSeleccionado == null) return;

            NombreProductoConsultado = ProductoSeleccionado.Nombre;

            var disponibilidad = await _productoService.ConsultarEnOtrasSucursalesAsync(ProductoSeleccionado.Id);
            DisponibilidadOtrasSucursales.Clear();
            foreach (var d in disponibilidad.Where(d => d.SucursalId != _sucursalId))
                DisponibilidadOtrasSucursales.Add(d);

            var ventana = new DisponibilidadWindow(this)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };
            ventana.ShowDialog();
        }

        private async Task CobrarAsync()
        {
            if (!Carrito.Any())
            {
                Mensaje = "Agrega al menos un producto al carrito.";
                return;
            }

            var cobroViewModel = new CobroViewModel(Total, MetodosPago.ToList());
            var cobroWindow = new CobroWindow(cobroViewModel)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            bool? resultado = cobroWindow.ShowDialog();
            if (resultado != true) return; // Usuario canceló, no se registra nada

            var venta = new Venta
            {
                MetodoPagoId = cobroViewModel.MetodoPagoSeleccionado!.Id,
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
                PagoCon = cobroViewModel.MontoPagado // ← ahora sí es el monto real, no el total
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
                        var corteVm = new CorteCajaViewModel(_corteCajaService, _ventaService, _authService,
                                                                _impresoraTicketService, _datosNegocio, _sucursalId, _usuarioId, _nombreUsuario);
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
            var corteCajaViewModel = new CorteCajaViewModel(
                _corteCajaService, _ventaService, _authService, _impresoraTicketService, _datosNegocio,
                _sucursalId, _usuarioId, _nombreUsuario);

            var corteCajaWindow = new CorteCajaWindow(corteCajaViewModel)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            corteCajaWindow.SesionDebeCerrarse += () =>
            {
                // Cierra la ventana principal → esto dispara el flujo de vuelta a Login
                System.Windows.Application.Current.MainWindow?.Close();
            };

            corteCajaWindow.ShowDialog();
        }

        private void AbrirRetiroEfectivo()
        {
            var movimientoVm = new MovimientoCajaViewModel(_corteCajaService, _authService, _sucursalId, _usuarioId, _nombreUsuario);
            var movimientoWindow = new MovimientoCajaWindow(movimientoVm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            bool? resultado = movimientoWindow.ShowDialog();
            if (resultado == true)
                Mensaje = "Movimiento de caja registrado correctamente.";
        }

        private void AbrirInventario()
        {
            var inventarioViewModel = new InventarioConsultaViewModel(_productoService, _sucursalId);
            var inventarioWindow = new InventarioConsultaWindow(inventarioViewModel)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };
            inventarioWindow.ShowDialog();
        }

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (SetProperty(ref _textoBusqueda, value))
                    FiltrarProductos();
            }
        }

        public RelayCommand<VentaDetalle> AumentarCantidadCommand { get; }
        public RelayCommand<VentaDetalle> DisminuirCantidadCommand { get; }
        public RelayCommand<VentaDetalle> EliminarDelCarritoCommand { get; }

        private List<Producto> _productosCompletos = new();

        public string NombreProductoConsultado { get; private set; } = string.Empty;

        private void FiltrarProductos()
        {
            var filtrados = string.IsNullOrWhiteSpace(TextoBusqueda)
                ? _productosCompletos
                : _productosCompletos.Where(p =>
                    p.Nombre.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                    (p.CodigoBarras != null && p.CodigoBarras.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase))
                  ).ToList();

            ProductosDisponibles.Clear();
            foreach (var p in filtrados)
                ProductosDisponibles.Add(p);
        }

        private void AumentarCantidad(VentaDetalle detalle)
        {
            int stockDisponible = _productosCompletos
                .FirstOrDefault(p => p.Id == detalle.ProductoId)?
                .StockPorSucursal.FirstOrDefault()?.Stock ?? 0;

            if (detalle.Cantidad >= stockDisponible)
            {
                Mensaje = $"Sin más stock disponible de '{detalle.NombreProducto}'.";
                return;
            }

            detalle.Cantidad++;
            RefrescarLinea(detalle);
        }

        private void DisminuirCantidad(VentaDetalle detalle)
        {
            if (detalle.Cantidad <= 1)
            {
                EliminarDelCarrito(detalle);
                return;
            }

            detalle.Cantidad--;
            RefrescarLinea(detalle);
        }

        private void EliminarDelCarrito(VentaDetalle detalle)
        {
            Carrito.Remove(detalle);
            Total = Carrito.Sum(d => d.Subtotal);
        }

        private void RefrescarLinea(VentaDetalle detalle)
        {
            var index = Carrito.IndexOf(detalle);
            Carrito[index] = detalle; // fuerza actualización visual, mismo truco que ya usamos
            Total = Carrito.Sum(d => d.Subtotal);
        }
    }
}
