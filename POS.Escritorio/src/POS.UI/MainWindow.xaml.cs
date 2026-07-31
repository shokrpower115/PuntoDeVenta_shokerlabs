using System.Windows;
using POS.UI.ViewModels;

namespace POS.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.SesionCerrada += () => Close();
        }
    }
}