using System;
using System.Collections.Generic;

namespace ForRobot.Libr.Clipboard.UndoRedo
{
    public class PropertyChangeCommand : IUndoableCommand
    {
        private readonly object _target;
        private readonly string _propertyName;
        private readonly object _oldValue;
        private readonly object _newValue;

        public string Description { get; }

        public PropertyChangeCommand(object target, string propertyName, object oldValue, object newValue, string description = null)
        {
            _target = target;
            _propertyName = propertyName;
            _oldValue = oldValue;
            _newValue = newValue;
            Description = description;
        }

        public event EventHandler CanExecuteChanged;
        //{
        //    add { CommandManager.RequerySuggested += value; }
        //    remove { CommandManager.RequerySuggested -= value; }
        //}            

        public bool CanExecute(object parameter) => true;

        public void Execute() => this.SetValue(this._newValue);
        public void Execute(object parameter) => this.Execute();

        public void Unexecute() => this.SetValue(this._oldValue);

        private void SetValue(object value)
        {
            var property = _target.GetType().GetProperty(_propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(_target, value);
            }
        }
    }
}
