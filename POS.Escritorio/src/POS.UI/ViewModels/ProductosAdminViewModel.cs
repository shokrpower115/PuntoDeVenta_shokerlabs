using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.UI.ViewModels
{
    public class ProductosAdminViewModel : ViewModelBase
    {
        private readonly IProductoService _productoService;

        public event Action? VolverSolicitado;

        public RelayCommand VolverCommand { get; }

        public ObservableCollection<Producto> Productos { get; } = new();

        // Sucursales fijas (mismo criterio que usamos en el login mock).
        // Si en el futuro se agregan más sucursales, esto se reemplaza
        // por una consulta real a la tabla Sucursales.
        public Dictionary<int, string> Sucursales { get; } = new()
        {
            { 1, "Sucursal Centro" },
            { 2, "Sucursal Norte" },
            { 3, "Sucursal Sur" },
        };

        private string _nuevoNombre = string.Empty;
        public string NuevoNombre
        {
            get => _nuevoNombre;
            set => SetProperty(ref _nuevoNombre, value);
        }

        private string _nuevoCodigoBarras = string.Empty;
        public string NuevoCodigoBarras
        {
            get => _nuevoCodigoBarras;
            set => SetProperty(ref _nuevoCodigoBarras, value);
        }

        private decimal _nuevoPrecio;
        public decimal NuevoPrecio
        {
            get => _nuevoPrecio;
            set => SetProperty(ref _nuevoPrecio, value);
        }

        private int _nuevoStockCentro;
        public int NuevoStockCentro
        {
            get => _nuevoStockCentro;
            set => SetProperty(ref _nuevoStockCentro, value);
        }

        private int _nuevoStockNorte;
        public int NuevoStockNorte
        {
            get => _nuevoStockNorte;
            set => SetProperty(ref _nuevoStockNorte, value);
        }

        private int _nuevoStockSur;
        public int NuevoStockSur
        {
            get => _nuevoStockSur;
            set => SetProperty(ref _nuevoStockSur, value);
        }

        private Producto? _productoSeleccionado;
        public Producto? ProductoSeleccionado
        {
            get => _productoSeleccionado;
            set => SetProperty(ref _productoSeleccionado, value);
        }

        private string _mensaje = string.Empty;
        public string Mensaje
        {
            get => _mensaje;
            set => SetProperty(ref _mensaje, value);
        }



        public RelayCommand CrearProductoCommand { get; }
        public RelayCommand EliminarProductoCommand { get; }
        public RelayCommand RecargarCommand { get; }

        public ProductosAdminViewModel(IProductoService productoService)
        {
            _productoService = productoService;

            CrearProductoCommand = new RelayCommand(async () => await CrearProductoAsync());
            EliminarProductoCommand = new RelayCommand(async () => await EliminarProductoAsync());
            RecargarCommand = new RelayCommand(async () => await CargarAsync());
            VolverCommand = new RelayCommand(() => VolverSolicitado?.Invoke());

            _ = CargarAsync();
        }

        private async Task CargarAsync()
        {
            var productos = await _productoService.ObtenerCatalogoCompletoAsync();
            Productos.Clear();
            foreach (var p in productos)
                Productos.Add(p);
        }

        private async Task CrearProductoAsync()
        {
            if (string.IsNullOrWhiteSpace(NuevoNombre) || NuevoPrecio <= 0)
            {
                Mensaje = "Escribe al menos el nombre y un precio válido.";
                return;
            }

            var producto = new Producto
            {
                Nombre = NuevoNombre,
                CodigoBarras = string.IsNullOrWhiteSpace(NuevoCodigoBarras) ? null : NuevoCodigoBarras,
                Precio = NuevoPrecio
            };

            var stockPorSucursal = new Dictionary<int, int>
            {
                { 1, NuevoStockCentro },
                { 2, NuevoStockNorte },
                { 3, NuevoStockSur },
            };

            await _productoService.CrearAsync(producto, stockPorSucursal);

            NuevoNombre = string.Empty;
            NuevoCodigoBarras = string.Empty;
            NuevoPrecio = 0;
            NuevoStockCentro = 0;
            NuevoStockNorte = 0;
            NuevoStockSur = 0;
            Mensaje = "Producto creado correctamente.";

            await CargarAsync();
        }

        private async Task EliminarProductoAsync()
        {
            if (ProductoSeleccionado == null) return;

            await _productoService.EliminarAsync(ProductoSeleccionado.Id);
            Mensaje = "Producto eliminado.";
            await CargarAsync();
        }
    }
}