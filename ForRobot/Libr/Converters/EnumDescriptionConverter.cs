using System;
using System.Linq;
using System.Windows.Data;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ForRobot.Models.Detals;

namespace ForRobot.Libr.Converters
{
    /// <summary>
    /// Класс преобразования элемента перечисления (<see cref="Enum"/> или статический класс) в его <see cref="DescriptionAttribute.Description"/>
    /// </summary>
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if(value is Enum enumValue)
            {
                return ForRobot.Libr.EnumExtensions.GetDescription(enumValue);
            }
            
            if(parameter is Type staticClassType)
            {
				var fieldInfos = staticClassType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Where(f => f.IsLiteral);
										  
				foreach (var fieldInfo in fieldInfos)
				{
                    try
                    {
                        var constantValue = fieldInfo.GetRawConstantValue();
                        if (constantValue?.Equals(fieldInfo.GetValue(staticClassType)) == true)
                        {
                            var descriptionAttr = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
                            return descriptionAttr?.Description ?? fieldInfo.Name;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                return value.ToString() ?? string.Empty;
            }
            throw new InvalidOperationException("Unsupported value type for conversion.");
        }

        /// <summary>
        /// Преобразование из <see cref="DescriptionAttribute.Description"/> в <see cref="Enum"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string description = value as string;

            if (string.IsNullOrEmpty(description))
                return null;

            if (targetType.IsEnum)
            {
                foreach (var fieldInfo in targetType.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var descriptionAttr = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
                    if (descriptionAttr != null && descriptionAttr.Description == description)
                    {
                        return Enum.Parse(targetType, fieldInfo.Name);
                    }
                }

                try
                {
                    return Enum.Parse(targetType, description);
                }
                catch
                {
                    throw new InvalidOperationException($"Cannot find enum value for description: {description}");
                }
            }

            if (parameter is Type staticClassType)
            {
                var fieldInfos = staticClassType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Where(f => f.IsLiteral);

                foreach (var fieldInfo in fieldInfos)
                {
                    var descriptionAttr = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
                    if (descriptionAttr != null && descriptionAttr.Description == description)
                    {
                        return fieldInfo.GetRawConstantValue();
                    }
                }

                var fieldByName = staticClassType.GetField(description, BindingFlags.Public | BindingFlags.Static);
                if (fieldByName != null)
                {
                    return fieldByName.GetRawConstantValue();
                }

                return targetType.FieldByDescription(description) ?? null;
            }
            throw new InvalidOperationException($"Cannot convert back to {targetType.Name}");
        }
    }
}
