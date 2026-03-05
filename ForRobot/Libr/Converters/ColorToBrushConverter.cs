using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace ForRobot.Libr.Converters
{
    /// <summary>
    /// Класс преобразователь из <see cref="System.Windows.Media.Color"/> в <see cref="System.Windows.Media.Brush"/>
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Преобразование <see cref="Color"/> в <see cref="SolidColorBrush"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color v = (Color)value;

            if (v == null)
                throw new FormatException("to use this converter, value and parameter shall inherit from Color");

            return new SolidColorBrush(v);
        }

        /// <summary>
        /// Обратное преобразование <see cref="SolidColorBrush"/> в <see cref="Color"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush v = (Brush)value;

            if (v == null)
                throw new FormatException("to use this converter, value and parameter shall inherit from Color");

            return ((SolidColorBrush)v).Color;
        }
    }
}
