using System.Windows;
using POS.UI.ViewModels;

namespace POS.UI
{
    public partial class CorteCajaWindow : Window
    {
        public CorteCajaWindow(CorteCajaViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}