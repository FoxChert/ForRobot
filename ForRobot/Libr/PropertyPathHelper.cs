using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

namespace ForRobot.Libr
{
    /// <summary>
    /// Вспомогательный статичный класс для работы со свойствами объектов и их путями
    /// </summary>
    public static class PropertyPathHelper
    {
        /// <summary>
        /// Вывод полного пути выбранного свойства объекта
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetPropertyPath<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var parts = new List<string>();
            Expression currentExpression = expression.Body;

            while (currentExpression is MemberExpression memberExpression)
            {
                parts.Add(memberExpression.Member.Name);
                currentExpression = memberExpression.Expression;
            }
            parts.Reverse();
            return string.Join(".", parts);
        }

        /// <summary>
        /// Вывод значение свойства объекта по его пути
        /// </summary>
        /// <param name="obj">Объект содержащий свойство необходимого значения</param>
        /// <param name="propertyPath">Полный путь свойства</param>
        /// <returns></returns>
        public static object GetValueFromPath(object obj, string propertyPath)
        {
            foreach (string part in propertyPath.Split('.'))
            {
                if (obj == null) return null;

                Type type = obj.GetType();
                PropertyInfo prop = type.GetProperty(part);

                if (prop == null) return null;

                obj = prop.GetValue(obj, null);
            }
            return obj;
        }

        /// <summary>
        /// Вывод полного пути свойства по его имени
        /// </summary>
        /// <param name="rootObject">Корневой объект поиска</param>
        /// <param name="propertyName">Наименование свойства в объекте</param>
        /// <returns></returns>
        public static string GetFullPropertyPath(object rootObject, string propertyName)
        {
            if (rootObject == null || string.IsNullOrEmpty(propertyName))
                return null;

            // Свойство есть в корневом объекте
            if (rootObject.GetType().GetProperty(propertyName) != null)
                return propertyName;

            // Поиск во вложенных объектах
            var queue = new Queue<(object obj, string path)>();
            var visited = new HashSet<object>(new ObjectReferenceEqualityComparer());

            queue.Enqueue((rootObject, ""));
            visited.Add(rootObject);

            while (queue.Count > 0)
            {
                var (currentObj, currentPath) = queue.Dequeue();

                if (currentObj == null) continue;

                Type currentType = currentObj.GetType();

                foreach (var property in currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return string.IsNullOrEmpty(currentPath) ? property.Name : $"{currentPath}.{property.Name}";
                    }
                    if (property.PropertyType.IsClass &&
                        property.PropertyType != typeof(string) &&
                        property.CanRead)
                    {
                        try
                        {
                            object propertyValue = property.GetValue(currentObj);
                            if (propertyValue != null && !visited.Contains(propertyValue))
                            {
                                string newPath = string.IsNullOrEmpty(currentPath) ? property.Name : $"{currentPath}.{property.Name}";
                                queue.Enqueue((propertyValue, newPath));
                                visited.Add(propertyValue);
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Присвоение значение свойству по пути
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyPath"></param>
        /// <param name="value"></param>
        public static void SetPropertyValueWithPath(object target, string propertyPath, object value)
        {
            if (string.IsNullOrEmpty(propertyPath)) return;

            string[] parts = propertyPath.Split('.');
            object current = target;

            // Навигация к последнему объекту в цепочке
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (current == null) return;
                var prop = current.GetType().GetProperty(parts[i]);
                if (prop == null || !prop.CanRead) return;
                current = prop.GetValue(current);
            }

            // Установка значения последнего свойства
            if (current != null)
            {
                var finalProp = current.GetType().GetProperty(parts[parts.Length - 1]);
                if (finalProp != null && finalProp.CanWrite)
                {
                    finalProp.SetValue(current, value);
                }
            }
        }
    }
}
