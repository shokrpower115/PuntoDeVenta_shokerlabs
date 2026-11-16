using System.Windows;
using POS.UI.ViewModels;

namespace POS.UI
{
    public partial class AsistenteConfiguracionWindow : Window
    {
        public AsistenteConfiguracionWindow(ConfiguracionNegocioViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.Guardado += () => { DialogResult = true; Close(); };
        }
    }
}
