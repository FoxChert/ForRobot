using System;
using System.Globalization;
using System.Windows.Data;

namespace ForRobot.Libr.Converters
{
    /// <summary>
    /// Класс-преобразователь для инвертации значения типа <see cref="bool"/>
    /// </summary>
    public class InvertBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool boolValue)
                return !boolValue;

            throw new FormatException("to use this converter, value and parameter shall inherit from Boolean");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;

            throw new FormatException("to use this converter, value and parameter shall inherit from Boolean");
        }
    }
}
