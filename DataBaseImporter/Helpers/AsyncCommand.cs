using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using DataBaseImporter.Annotations;

namespace DataBaseImporter.Helpers
{
    public class AsyncCommand : Command, INotifyPropertyChanged
    {
        private bool _isExecuting;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Dispatcher _currentDispatcher;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AsyncCommand(Action<object> action, Func<object, bool> canExecute = null) : base(action, canExecute)
        {
            _currentDispatcher = Dispatcher.CurrentDispatcher;
        }

        public void ReportProgress(Action action)
        {
            if (_currentDispatcher.CheckAccess())
                action();
            else
                _currentDispatcher.Invoke(action);
        }

        public override void Execute(object parameter)
        {
            if (IsExecuting) return;

            ExecutingAction.BeginInvoke(parameter, ExecutionCompleted, null);
            IsExecuting = true;
        }

        private void ExecutionCompleted(IAsyncResult ar)
        {
            IsExecuting = false;
        }

        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                _isExecuting = value;
                OnPropertyChanged();
            }
        }
    }
}
