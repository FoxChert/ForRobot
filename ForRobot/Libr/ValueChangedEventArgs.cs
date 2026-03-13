using System;

namespace ForRobot.Libr
{
    /// <summary>
    /// Данные события изменения свойства объекта
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Старое значение
        /// </summary>
        public object OldValue { get; }
        /// <summary>
        /// Новое значение
        /// </summary>
        public object NewValue { get; }
        /// <summary>
        /// Наименование изменённого свойства
        /// </summary>
        public string PropertyName { get; }
        /// <summary>
        /// Тип свойства
        /// </summary>
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
