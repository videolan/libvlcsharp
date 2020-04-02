using System;
using System.Windows.Input;

namespace LibVLCSharp.UWP.Sample
{
    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other objects by invoking delegates
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Creates a new command
        /// </summary>
        /// <param name="executeAction">the execution logic</param>
        public RelayCommand(Action<T> executeAction)
        {
            ExecuteAction = executeAction;
        }

        private Action<T> ExecuteAction { get; }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Determines whether the command can execute in its current state
        /// </summary>
        /// <param name="parameter">data used by the command.
        /// If the command does not require data to be passed, this object can be set to null</param>
        /// <returns>true</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the <see cref="RelayCommand"/>
        /// </summary>
        /// <param name="parameter">data used by the command</param>
        public void Execute(object parameter)
        {
            ExecuteAction?.Invoke((T)parameter);
        }
    }
}
