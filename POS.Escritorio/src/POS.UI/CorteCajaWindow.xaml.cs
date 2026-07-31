using System.Windows;
using POS.UI.ViewModels;

namespace POS.UI
{
    public partial class CorteCajaWindow : Window
    {
        public event Action? SesionDebeCerrarse;
        public CorteCajaWindow(CorteCajaViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.CorteFinalizado += () =>
            {
                SesionDebeCerrarse?.Invoke();
                Close();
            };
        }
    }
}