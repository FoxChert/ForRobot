using System;
using System.Xml;
using System.Collections.Generic;
using System.Configuration;

namespace ForRobot.Libr.Configuration.ConfigurationProperties
{
    /// <summary>
    /// Базовый класс выборки значений свойств конфигурации
    /// </summary>
    public abstract class BaseConfigurationSection : ConfigurationSection
    {
        private Dictionary<string, string> _propertyValues = new Dictionary<string, string>();

        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            var xml = reader.ReadOuterXml();

            if (!string.IsNullOrEmpty(xml))
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        _propertyValues[node.Name] = node.InnerText;
                    }
                }
            }

            //using (var stringReader = new System.IO.StringReader($"<{this.GetType().Name}>{xml}</{this.GetType().Name}>"))
            //using (var stringReader = new System.IO.StringReader(xml))
            //using (var xmlReader = XmlReader.Create(stringReader))
            //{
            //    xmlReader.Read();
            //    xmlReader.Read();
            //    base.DeserializeElement(xmlReader, serializeCollectionKey);
            //}
        }

        /// <summary>
        /// Получение значения свойств конфигурации по имени с возможностью указания значения по умолчанию
        /// </summary>
        protected T GetValue<T>(string propertyName, T defaultValue = default(T))
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName), "Имя свойства конфигурации не может быть равно null");

            if (string.IsNullOrEmpty(propertyName))
                return defaultValue;

            try
            {
                if (_propertyValues.ContainsKey(propertyName))
                {
                    var value = _propertyValues[propertyName];
                    if (!string.IsNullOrEmpty(value))
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                }

                return defaultValue;
            }
            catch (Exception ex) when (ex is ConfigurationErrorsException || ex is InvalidCastException || ex is FormatException)
            {
                return defaultValue;
            }
        }
    }
}
