using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace ForRobot.Libr.Json.Schemas
{
    /// <summary>
    /// Менеджер для работы с json-схемами, встроенными в сборку как Embedded Resources
    /// </summary>
    /// <remarks>
    /// Предоставляет функциональность для поиска, загрузки и анализа json-схем,
    /// которые хранятся в сборке как встроенные ресурсы.
    /// </remarks>
    public static class JsonManager
    {
        /// <summary>
        /// Свойства json-schema, которые могут содержать информацию о целевом типе
        /// </summary>
        private static string[] _titleProperties = new string[] { "meta:targetClass", "meta:namespace", "className", "fullName", "x-target-type", "x-class-name", "x-full-name" };

        /// <summary>
        /// Проверка, является ли строка валидной JSON-схемой
        /// </summary>
        /// <param name="content">Проверяемое содержимое</param>
        /// <returns>True, если содержимое является валидной JSON-схемой</returns>
        private static bool IsValidJsonSchema(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            try
            {
                JSchema.Parse(content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Возвращение json-схемы для указанного типа из Embedded Resources сборки
        /// </summary>
        /// <typeparam name="T">Тип, для которого требуется получить схему</typeparam>
        /// <returns>Строка json-схемы</returns>
        /// <exception cref="InvalidOperationException">Если схема для типа не найдена</exception>
        public static string GetSchema<T>()
        {
            Type targetType = typeof(T);
            return GetSchema(targetType);
        }

        /// <summary>
        /// Возвращает json-схему для указанного типа из Embedded Resources сборки
        /// </summary>
        /// <param name="type">Тип, для которого требуется получить схему</param>
        /// <returns>Строка JSON-схемы</returns>
        /// <exception cref="ArgumentNullException">Если type равен null</exception>
        /// <exception cref="InvalidOperationException">Если схема для типа не найдена</exception>
        public static string GetSchema(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // Получение текущей сборки и списка всех встроенных ресурсов
            var assembly = Assembly.GetExecutingAssembly();
            var resourcesNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            foreach (var resourceName in resourcesNames)
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                        continue;

                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        string content = streamReader.ReadToEnd();

                        if (!IsValidJsonSchema(content))
                            continue;

                        JSchema jSchema = JSchema.Parse(content);
                        string schemaTarget = GetSchemaTargetType(jSchema);

                        if (string.Equals(type.FullName, schemaTarget, StringComparison.Ordinal) || string.Equals(type.Name, schemaTarget, StringComparison.Ordinal))
                            return content;
                    }
                }
            }
            throw new Exception(string.Format("В сборке не найдена json-схема для типа {0}.", type.FullName));
        }

        /// <summary>
        /// Получение целевого типа из JSON-схемы
        /// </summary>
        /// <param name="schema">JSON-схема</param>
        /// <returns>Имя целевого типа</returns>
        /// <exception cref="ArgumentNullException">Если schema равен null</exception>
        /// <exception cref="InvalidOperationException">Если в схеме не найдено свойство, обозначающее класс объекта</exception>
        public static string GetSchemaTargetType(JSchema schema)
        {
            if (schema == null)
                throw new ArgumentNullException(nameof(schema));

            if (schema.ExtensionData != null)
            {
                foreach (var property in _titleProperties)
                {
                    if (schema.ExtensionData.TryGetValue(property, out var value) && value != null)
                    {
                        if (value.Type == JTokenType.String)
                        {
                            return value.ToString();
                        }

                        return value.ToString();
                    }
                }
            }

            if (schema.Properties != null)
            {
                foreach (var property in _titleProperties)
                {
                    if (schema.Properties.ContainsKey(property))
                    {
                        var propValue = schema.Properties[property];
                        if (propValue?.Default != null)
                        {
                            return propValue.Default.ToString();
                        }
                    }
                }
            }

            throw new InvalidOperationException("В json-схеме не найдено свойство, обозначающее класс объекта.");
        }

    }
}
