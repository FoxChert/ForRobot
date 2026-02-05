using System;
using System.ComponentModel;

namespace ForRobot.Models.Detals
{
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
