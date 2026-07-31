using System.Windows;
using POS.UI.ViewModels;

namespace POS.UI
{
    public partial class CobroWindow : Window
    {
        public CobroWindow(CobroViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.ConfirmadoExitoso += () => { DialogResult = true; Close(); };
            viewModel.CancelarSolicitado += () => { DialogResult = false; Close(); };
        }
    }
}