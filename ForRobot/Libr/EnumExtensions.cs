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
            return Enum.GetValues(typeof(T)).Cast<T>().Where(item => item.GetDescription() == description).FirstOrDefault();
        }

        /// <summary>
        /// Вывод <see cref="DescriptionAttribute.Description"/> элементов <see cref="Enum"/>
        /// </summary>
        /// <param name="type">Тип перечисления</param>
        /// <returns></returns>
        public static IEnumerable<string> GetDescriptions(this Type type)
        {
            return type.GetFields().Select(item => ((DescriptionAttribute[])item.GetCustomAttributes(typeof(DescriptionAttribute), false)).FirstOrDefault().Description);
        }

        /// <summary>
        /// Возврат значения <see cref="DescriptionAttribute"/> элемента перечисления
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }
}
