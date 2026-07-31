using System.Windows;
using POS.UI.ViewModels;

namespace POS.UI
{
    public partial class InventarioConsultaWindow : Window
    {
        public InventarioConsultaWindow(InventarioConsultaViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}