using System;

namespace ForRobot.Libr.Attributes
{
    /// <summary>
    /// Тип ввода числа
    /// </summary>
    public enum NumberWritteTypes
    {
        Writte,

        Click
    }

    public class PropertyNameAttribute : Attribute
    {
        public string PropertyName { get; private set; }
        public string Category { get; private set; }
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Шаг прибавления/убавления числа
        /// </summary>
        public double Step { get; private set; }

        /// <summary>
        /// Способ ввода числа
        /// </summary>
        public NumberWritteTypes NumberWritteTypes { get; private set; }

        public PropertyNameAttribute(string propertyName, string category = "", bool isReadOnly = false)
        {
            this.PropertyName = propertyName;
            Category = category;
            IsReadOnly = isReadOnly;
        }


        public PropertyNameAttribute(string propertyName, string category, bool isReadOnly, NumberWritteTypes types = NumberWritteTypes.Writte, double step = 1)
            : this(propertyName, category, isReadOnly)
        {
            NumberWritteTypes = types;
            Step = step;
        }

    }
}
