using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace ForRobot.Libr.Converters
{
    public class ThicknessConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var left = (double)values[0];
            var top = (double)values[1];
            var right = (double)values[2];
            var bottom = (double)values[3];
            return new Thickness(left, top, right, bottom);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var th = (Thickness)value;
            return new object[4] { th.Left, th.Top, th.Right, th.Bottom };
        }
    }
}
