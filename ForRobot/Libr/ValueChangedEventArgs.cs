using System;

namespace ForRobot.Libr
{
    /// <summary>
    /// Событие изменения
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValueChangedEventArgs : EventArgs
    {
        public object OldValue { get; }
        public object NewValue { get; }
        public string PropertyName { get; }
        public Type ValueType { get; }

        public ValueChangedEventArgs(object oldValue, object newValue, string propertyName = null)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.PropertyName = propertyName;
            this.ValueType = newValue?.GetType();
        }
    }
}
