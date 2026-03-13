using System.Windows.Data;
using System;
using System.Globalization;

namespace ForRobot.Libr.Converters
{
    /// <summary>
    /// Класс-преобразователь для форматирования строки. Реализует <see cref="IValueConverter"/>
    /// </summary>
    /// <example>
    /// <code>Converter={StaticResource ResourceKey=StringFormat}, ConverterParameter='Hello {0}'</code>
    /// </example>
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format(parameter as string, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Класс-преобразователь для форматирования строки. Реализует <see cref="IMultiValueConverter"/>
    /// </summary>
    /// <example>
    /// <code>
    /// <MultiBinding Converter=\"{StaticResource StringFormatMultiConvert}\" ConverterParameter=\"Hello {0}\">
    ///     <Binding Path = \"Version" Mode="OneWay\"/>
    /// </MultiBinding>
    /// </code>
    /// </example> 
    public class StringFormatMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format(parameter as string, values);
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
