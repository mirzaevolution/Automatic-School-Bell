using System;
using System.Windows.Input;

namespace AutomaticSchoolBell.GUI
{
    public class RelayCommand : ICommand
    {
        private Action _action;
        private Func<bool> _canExecute;
        public RelayCommand(Action action)
        {
            _action = action;
        }
        public RelayCommand(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute();
            if (_action != null)
                return true;
            return false;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke();
        }
    }
    public class RelayCommand<T> : ICommand
    {
        private Action<T> _action;
        private Func<T, bool> _canExecute;
        public RelayCommand(Action<T> action)
        {
            _action = action;
        }
        public RelayCommand(Action<T> action, Func<T, bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute((T)parameter);
            if (_action != null)
                return true;
            return false;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke((T)parameter);
        }
    }
}
