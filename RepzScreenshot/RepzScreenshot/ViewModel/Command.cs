using System;
using System.Windows.Input;

namespace RepzScreenshot.ViewModel
{
    class Command : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool> _condition;

        public Command(Action action, Func<bool> condition)
        {
            
            _action = action;
            _condition = condition;
        }

        public Command(Action action):this(action, delegate { return true; }){}

        public void Execute(object parameter)
        {
            
            _action();
        }

        public bool CanExecute(object parameter)
        {
            
            return _condition();
        }

        public event EventHandler CanExecuteChanged;

        public void NotifyCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
        
    }
}
