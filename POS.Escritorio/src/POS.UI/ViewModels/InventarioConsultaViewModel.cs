using System.Collections.ObjectModel;
using System.Threading.Tasks;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.UI.ViewModels
{
    public class InventarioConsultaViewModel : ViewModelBase
    {
        private readonly IProductoService _productoService;
        private readonly int _sucursalId;

        public ObservableCollection<Producto> Productos { get; } = new();

        public RelayCommand RecargarCommand { get; }

        public InventarioConsultaViewModel(IProductoService productoService, int sucursalId)
        {
            _productoService = productoService;
            _sucursalId = sucursalId;

            RecargarCommand = new RelayCommand(async () => await CargarAsync());
            _ = CargarAsync();
        }

        private async Task CargarAsync()
        {
            var productos = await _productoService.ObtenerTodosAsync(_sucursalId);
            Productos.Clear();
            foreach (var p in productos)
                Productos.Add(p);
        }
    }
}