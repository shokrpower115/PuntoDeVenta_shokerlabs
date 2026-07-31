using System;
using System.Windows.Input;

namespace POS.UI.ViewModels
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _ejecutar;
        private readonly Func<T, bool>? _puedeEjecutar;

        public RelayCommand(Action<T> ejecutar, Func<T, bool>? puedeEjecutar = null)
        {
            _ejecutar = ejecutar;
            _puedeEjecutar = puedeEjecutar;
        }

        public bool CanExecute(object? parameter) => _puedeEjecutar?.Invoke((T)parameter!) ?? true;
        public void Execute(object? parameter) => _ejecutar((T)parameter!);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}