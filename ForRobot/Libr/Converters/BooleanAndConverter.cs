using System;
using System.Globalization;
using System.Windows.Data;

namespace ForRobot.Libr.Converters
{
    /// <summary>
    /// Класс преобразования, выполняющий логическую операцию AND над булевыми значениями
    /// </summary>
    public class BooleanAndConverter : IMultiValueConverter
    {
        /// <summary>
        /// Выполняет логическую операцию AND над всеми входными булевыми значениями.
        /// </summary>
        /// <param name="values">Массив объектов для конвертации</param>
        /// <param name="targetType">Тип целевого свойства (должен быть bool)</param>
        /// <param name="parameter">Параметр конвертера (не используется)</param>
        /// <param name="culture">Информация о культуре (не используется)</param>
        /// <returns>True, если все значения true; False, если хотя бы одно значение false или не является bool</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var value in values)
            {
                if (value is bool b && !b) return false;
            }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
