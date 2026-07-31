using System.Windows.Controls;
using POS.UI.ViewModels;

namespace POS.UI.Views
{
    public partial class CorteCajaView : UserControl
    {
        public CorteCajaView()
        {
            InitializeComponent();
            DataContextChanged += (s, e) =>
            {
                if (DataContext is CorteCajaViewModel vm)
                    vm.PasswordConfirmacion = PasswordBox.Password;
            };
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is CorteCajaViewModel vm)
                vm.PasswordConfirmacion = PasswordBox.Password;
        }
    }
}