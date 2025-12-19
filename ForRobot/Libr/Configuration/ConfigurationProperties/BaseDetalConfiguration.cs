using System;
using System.Configuration;

namespace ForRobot.Libr.Configuration.ConfigurationProperties
{
    /// <summary>
    /// Базовый класс выборки значений свойств конфигурации
    /// </summary>
    public abstract class BaseConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Получение значения свойств конфигурации по имени с возможностью указания значения по умолчанию
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого значения</typeparam>
        /// <param name="propertyName">Имя свойства конфигурации</param>
        /// <param name="defaultValue">Значение по умолчанию, возвращаемое в случае ошибки или отсутствия значения</param>
        /// <returns>
        /// Значение свойства конфигурации, приведенное к типу T, 
        /// или значение по умолчанию, если свойство отсутствует, равно null или возникла ошибка при получении
        /// </returns>
        /// <exception cref="ArgumentNullException">Если propertyName равен null</exception>
        /// <remarks>
        /// Метод использует блок try-catch для перехвата любых исключений, возникающих 
        /// при доступе к свойствам конфигурации, и возвращает значение по умолчанию в таких случаях
        /// </remarks>
        protected T GetValue<T>(string propertyName, T defaultValue = default(T))
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName), "Имя свойства конфигурации не может быть равно null");

            if (string.IsNullOrEmpty(propertyName))
                return defaultValue;

            try
            {
                var value = this[propertyName];

                if (value == null)
                    return defaultValue;

                if (typeof(T).IsValueType && value is T typedValue)
                    return typedValue;

                if (!typeof(T).IsValueType && value is T)
                    return (T)value;

                return defaultValue;
            }
            catch (Exception ex) when (ex is ConfigurationErrorsException || ex is InvalidCastException)
            {
                return defaultValue;
            }
        }
    }
}
