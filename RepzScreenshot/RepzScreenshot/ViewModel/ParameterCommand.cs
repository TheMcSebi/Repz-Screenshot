﻿using System;
using System.Windows.Input;

namespace RepzScreenshot.ViewModel
{
    class ParameterCommand<T> : Command
    {
        private readonly Action<T> _action;
        private readonly Func<T, bool> _condition;

        public ParameterCommand(Action<T> action, Func<T, bool> condition)
        {
            
            _action = action;
            _condition = condition;
        }

        public ParameterCommand(Action<T> action) : this(action, delegate { return true; }) { }

        public override bool CanExecute(object parameter)
        {
            if (typeof(T) != parameter.GetType())
            {
                throw new ArgumentException("Parameter must be of type " + typeof(T));
            }
            return _condition((T)parameter);
        }

        public override void Execute(object parameter)
        {
            if (typeof(T) != parameter.GetType())
            {
                throw new ArgumentException("Parameter must be of type " + typeof(T));
            }
            _action((T)parameter);
        }

        
    }
}
