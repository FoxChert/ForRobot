using System;
using System.Windows.Media;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ForRobot.Libr.Json
{    
    public class UniversalImageSourceContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            
            if (typeof(ImageSource).IsAssignableFrom(property.PropertyType))
            {
                property.Converter = new ForRobot.Libr.Converters.ImageSourceConverter();
                property.ShouldSerialize = instance =>
                {
                    var value = member is PropertyInfo pi
                        ? pi.GetValue(instance)
                        : ((FieldInfo)member).GetValue(instance);
                    return value != null;
                };
            }

            return property;
        }
    }
}
