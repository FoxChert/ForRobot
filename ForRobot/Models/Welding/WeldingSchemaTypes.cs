using System;
using System.ComponentModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ForRobot.Models.Welding
{
    [JsonConverter(typeof(StringEnumConverter))]
    /// <summary>
    /// Перечисление схем сварки
    /// </summary>
    public enum WeldingSchemaTypes
    {
        [Description("Левые четные-нечётные, правые чётные-нечетные")]
        /// <summary>
        /// Схема сварки сначала слева чётных-нечётных, потом справа чётных-неяётных рёбер
        /// </summary>
        LeftEvenOdd_RightEvenOdd = 0,

        [Description("Редактировать")]
        /// <summary>
        /// Пользовательская схема
        /// </summary>
        Edit = 1
    }
}
