using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;

namespace ForRobot.Libr
{
    /// <summary>
    /// Класс, расширяющий тип <see cref="Enum"/>
    /// </summary>
    public static class EnumExtensions
    {

        /// <summary>
        /// Вывод элемента перечисления по его
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T GetByDescription<T>(this T enumValue, string description) where T : Enum 
        {
            if (string.IsNullOrEmpty(description))
                return default(T);

            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (string.Equals(value.GetDescription(), description, StringComparison.Ordinal))
                {
                    return value;
                }
            }
            return default(T);
            //return Enum.GetValues(typeof(T)).Cast<T>().Where(item => item.GetDescription() == description).FirstOrDefault();
        }

        /// <summary>
        /// Вывод <see cref="DescriptionAttribute.Description"/> элементов <see cref="Enum"/>
        /// </summary>
        /// <param name="type">Тип перечисления</param>
        /// <returns></returns>
        public static IEnumerable<string> GetDescriptions<T>(this T enumValue) where T : Enum
        {
            return enumValue.GetType().GetDescriptions();
        }

        /// <summary>
        /// Вывод <see cref="DescriptionAttribute.Description"/> элементов перечисления
        /// </summary>
        /// <param name="type">Тип перечисления</param>
        /// <returns></returns>
        public static IEnumerable<string> GetDescriptions(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!type.IsEnum)
                throw new ArgumentException("Тип должен представлять перечисление.", nameof(type));

            List<string> descriptions = new List<string>();

            foreach (Enum value in Enum.GetValues(type))
            {
                string description = value.GetDescription();
                if (!string.IsNullOrEmpty(description))
                    descriptions.Add(description);
            }
            return descriptions;
            //return Enum.GetValues(type).Cast<Enum>().Select(value => value.GetDescription()).Where(desc => !string.IsNullOrEmpty(desc));
        }

        /// <summary>
        /// Возврат значения <see cref="DescriptionAttribute"/> элемента перечисления
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription<T>(this T value) where T : Enum
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo == null)
                return value.ToString();

            var attribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }

        /// <summary>
        /// Возврат поля статического класса-перечисления
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static object FieldByDescription(this Type type, string description)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (string.IsNullOrEmpty(description))
                return default(type);

            var fields = type.GetFields().Where(field => (field.GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() as DescriptionAttribute).Description == description);
            return fields.First().GetValue(null);
        }
    }
}
