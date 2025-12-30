using System;
using System.Linq;
using System.Windows.Data;
using System.Windows.Controls;
using System.Reflection;
using System.Globalization;

using ForRobot.Models.Detals;

namespace ForRobot.Libr.Converters
{
    /// <summary>
    /// Класс преобразования <see cref="WeldingSchemas.SchemasTypes"/> в строку
    /// </summary>
    public class WeldingSchemaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is WeldingSchemas.SchemasTypes))
                throw new FormatException("to use this converter, value and parameter shall inherit from WeldingSchemas.SchemasTypes");

            WeldingSchemas.SchemasTypes schemasTypes;

            if(Enum.TryParse(value.ToString(), out schemasTypes))
            {
                FieldInfo fieldInfo = typeof(WeldingSchemas.SchemasTypes).GetField(value.ToString());

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

            return WeldingSchemas.GetSchemaType(valueString);
        }
    }
}