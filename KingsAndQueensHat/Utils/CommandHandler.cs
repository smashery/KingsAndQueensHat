using System;
using System.Windows.Input;

namespace KingsAndQueensHat.Utils
{
    public class CommandHandler : ICommand
    {
        private Action _action;
        private Func<bool> _canExecute;
        public CommandHandler(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            var result = _canExecute();
            return result;
        }

        public void RaiseCanExecuteChanged()
        {
            var @event = CanExecuteChanged;
            if (@event != null)
            {
                @event(this, new EventArgs());
            }
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
}