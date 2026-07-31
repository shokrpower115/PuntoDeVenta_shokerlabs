using System.Windows;
using POS.UI.ViewModels;

namespace POS.UI
{
    public partial class DisponibilidadWindow : Window
    {
        public DisponibilidadWindow(VentaViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}