using System;
using System.Windows.Input;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Simple <see cref="ICommand"/> implementation
    /// </summary>
    internal class ActionCommand<TParameter> : ICommand
    {
        event EventHandler? ICommand.CanExecuteChanged
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ActionCommand{TParameter}"/> class
        /// </summary>
        /// <param name="action">action to execute</param>
        public ActionCommand(Action<TParameter> action)
        {
            Action = action;
        }

        private Action<TParameter> Action { get; }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked
        /// </summary>
        /// <param name="parameter">data used by the command</param>
        public void Execute(object parameter)
        {
            Action((TParameter)parameter);
        }
    }
}
