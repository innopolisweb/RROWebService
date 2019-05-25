using System;
using System.Windows.Input;

namespace DataBaseImporter.Helpers
{
    public class Command : ICommand
    {
        protected readonly Action<object> ExecutingAction;
        protected readonly Func<object, bool> CanExecuteFunc;

        public Command(Action<object> action, Func<object, bool> canExecute = null)
        {
            ExecutingAction = action;
            CanExecuteFunc = canExecute ?? (obj => true);
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc(parameter);
        }

        public virtual void Execute(object parameter)
        {
            ExecutingAction(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}