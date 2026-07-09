using System;
using System.Windows.Input;

namespace POS.UI.ViewModels
{
    // Esto es lo que conecta un botón del XAML (Command="{Binding GuardarCommand}")
    // con un método de C# en el ViewModel, sin usar code-behind.
    public class RelayCommand : ICommand
    {
        private readonly Action _ejecutar;
        private readonly Func<bool>? _puedeEjecutar;

        public RelayCommand(Action ejecutar, Func<bool>? puedeEjecutar = null)
        {
            _ejecutar = ejecutar;
            _puedeEjecutar = puedeEjecutar;
        }

        public bool CanExecute(object? parameter) => _puedeEjecutar?.Invoke() ?? true;
        public void Execute(object? parameter) => _ejecutar();

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
