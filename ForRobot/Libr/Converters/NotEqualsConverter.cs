using System;
using System.Windows.Data;

namespace ForRobot.Libr.Converters
{
    /// <summary>
    /// Конвертер для проверки неравенства двух значений.
    /// Возвращает true, если значение не равно параметру.
    /// </summary>
    public class NotEqualsConverter : IValueConverter
    {
        /// <summary>
        /// Преобразует значение в boolean путем сравнения с параметром на неравенство
        /// </summary>
        /// <param name="value">Текущее значение привязанного свойства</param>
        /// <param name="targetType">Тип целевого свойства (должен быть boolean)</param>
        /// <param name="parameter">Передаваемый параметр. Должен совпадать по типу с <paramref name="value"/></param>
        /// <param name="culture"></param>
        /// <returns>True если значение не совпадает с параметром, иначе False</returns>
        /// <exception cref="ArgumentNullException">Когда value или parameter равны null</exception>
        /// <exception cref="ArgumentException">Когда типы value и parameter несовместимы</exception>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //if (value == null)
            //    throw new ArgumentNullException("value");

            //if (parameter == null)
            //    throw new ArgumentNullException("parametr");

            if (value is bool boolValue && parameter is bool boolParameter)
            {
                return boolValue != boolParameter;
            }
            else if (value == null || parameter == null)
            {
                return !value?.Equals(parameter);
            }
            else if (value.GetType() == parameter.GetType())
            {
                return !value.Equals(parameter);
            }
            else if (targetType != typeof(object))
            {
                try
                {
                    var valueStr = value.ToString();
                    var paramStr = parameter.ToString();
                    return !(valueStr?.Equals(paramStr) ?? false);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Невозможно сравнить значения типа {value.GetType()} и {parameter.GetType()}", ex);
                }
            }
            else
                return parameter.ToString() != value.ToString();
        }

        /// <summary>
        /// Преобразует boolean значение обратно в параметр
        /// </summary>
        /// <param name="value">Boolean значение</param>
        /// <param name="targetType">Целевой тип значения</param>
        /// <param name="parameter">Параметр, представляющий значение для возврата</param>
        /// <param name="culture"></param>
        /// <returns>Параметр, если value равно true, иначе DependencyProperty.UnsetValue</returns>
        /// <exception cref="ArgumentNullException">Когда невозможно преобразовать параметр типа parameter в целевой тип targetType</exception>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isChecked && isChecked)
            {
                if (value == null || parameter == null)
                    return !value?.Equals(parameter);

                if (parameter != null && (targetType.IsAssignableFrom(parameter.GetType()) || targetType == typeof(object)))
                {
                    return parameter;
                }

                if (parameter != null && targetType != typeof(object))
                {
                    try
                    {
                        return System.Convert.ChangeType(parameter, targetType);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException($"Невозможно преобразовать параметр типа {parameter.GetType()} в целевой тип {targetType}", ex);
                    }
                }
            }
            return System.Windows.DependencyProperty.UnsetValue;
        }
    }
}
