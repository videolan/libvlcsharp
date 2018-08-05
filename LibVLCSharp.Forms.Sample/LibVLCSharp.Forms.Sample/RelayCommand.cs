using System;
using System.Windows.Input;

namespace LibVLCSharp.Forms.Sample
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            ExecuteAction = execute;
            CanExecuteFunction = canExecute;
        }

        Func<bool> CanExecuteFunction { get; }
        private Action ExecuteAction { get; }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunction?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            ExecuteAction();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
