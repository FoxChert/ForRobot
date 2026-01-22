using System;
using System.Linq;
using System.Windows.Data;
using System.Reflection;
using System.Globalization;

using ForRobot.Models.Welding;

namespace ForRobot.Libr.Converters
{
    /// <summary>
    /// Класс преобразования <see cref="WeldingSchemas.SchemasTypes"/> в строку
    /// </summary>
    public class WeldingSchemaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is WeldingSchemaTypes schemasTypes))
                throw new FormatException("to use this converter, value and parameter shall inherit from WeldingSchemas.SchemasTypes");
            
            if(Enum.TryParse(value.ToString(), out schemasTypes))
            {
                FieldInfo fieldInfo = typeof(WeldingSchemaTypes).GetField(value.ToString());

                if (fieldInfo != null && fieldInfo.IsLiteral)
                {
                    return fieldInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false).SingleOrDefault() as System.ComponentModel.DescriptionAttribute;
                }
                else
                    return null;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string valueString) || string.IsNullOrEmpty(valueString))
                throw new FormatException("to use this converter, value and parameter shall inherit from String");

            return WeldingSchema.GetSchemaType(valueString);
        }
    }
}