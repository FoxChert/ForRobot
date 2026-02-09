using System;
using System.Linq;
using System.ComponentModel;

namespace ForRobot.Models.Detals
{
    public static class DetalTypeExtensions
    {
        public static DetalType StringToEnum(this string detalTypeDescription) => DetalTypeCollection().Where(item => ForRobot.Libr.EnumExtensions.GetDescription(item) == detalTypeDescription).FirstOrDefault();

        public static string EnumToString(this DetalType detalType) => ForRobot.Libr.EnumExtensions.GetDescription(detalType);

        public static System.Collections.Generic.IList<DetalType> DetalTypeCollection() => Enum.GetValues(typeof(Detals.DetalType)).Cast<DetalType>().Where(t => t != Detals.DetalType.All).ToList();
    }

    [Flags]
    /// <summary>
    /// Перечень типов деталей
    /// </summary>
    public enum DetalType
    {
        [Description("Настил с рёбрами")]
        /// <summary>
        /// Плита
        /// </summary>
        Plate = 1,

        [Description("Настил со стрингером")]
        /// <summary>
        /// Плита со стрингером
        /// </summary>
        Stringer = 2,

        [Description("Насил треугольником")]
        /// <summary>
        /// Плита треугольником
        /// </summary>
        Treygolnik = 3,

        All = Plate & Stringer & Treygolnik
    }
}
