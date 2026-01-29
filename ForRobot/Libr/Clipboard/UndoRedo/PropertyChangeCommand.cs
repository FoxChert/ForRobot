using System;
using System.Collections.Generic;

namespace ForRobot.Libr.Clipboard.UndoRedo
{
    public class PropertyChangeCommand : IUndoableCommand
    {
        private readonly object _target;
        private readonly string _propertyPath;
        private readonly string _propertyName;
        private readonly object _oldValue;
        private readonly object _newValue;
        private readonly string[] _propertyChain;

        public string Description { get; }

        public PropertyChangeCommand(object target, string propertyName, object oldValue, object newValue, string description = null)
        {
            this._target = target;
            this._propertyName = propertyName;
            this._propertyPath = PropertyPathHelper.GetFullPropertyPath(target, propertyName);
            this._oldValue = oldValue;
            this._newValue = newValue;
            this.Description = description;
            this._propertyChain = this._propertyPath.Split('.');
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute() => this.SetValue(this._newValue);
        public void Execute(object parameter) => this.Execute();

        public void Unexecute() => this.SetValue(this._oldValue);

        private void SetValue(object value)
        {
            if (_propertyChain.Length == 0) return;
            
            object targetObject = GetTargetObject();
            if (targetObject == null) return;
            
            string propertyName = _propertyChain[_propertyChain.Length - 1];

            var property = targetObject.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(targetObject, value);
            }

            //var property = _target.GetType().GetProperty(_propertyName);
            //if (property != null && property.CanWrite)
            //{
            //    property.SetValue(_target, value);
            //}
        }

        private object GetTargetObject()
        {
            object current = _target;
            
            for (int i = 0; i < _propertyChain.Length - 1; i++)
            {
                if (current == null) return null;

                var property = current.GetType().GetProperty(_propertyChain[i]);
                if (property == null || !property.CanRead) return null;

                current = property.GetValue(current);
            }

            return current;
        }
    }
}
