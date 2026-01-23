using System;

namespace ForRobot.Models.Detals
{
    /// <summary>
    /// Перечень типов деталей
    /// </summary>
    public static class DetalTypes
    {
        /// <summary>
        /// Плита
        /// </summary>
        public const string Plate = "Настил с ребром";

        /// <summary>
        /// Плита со стрингером
        /// </summary>
        public const string Stringer = "Настил со стрингером";

        /// <summary>
        /// Плита треугольником
        /// </summary>
        public const string Treygolnik = "Настил треугольником";

        public static DetalType StringToEnum(string detalType)
        {
            switch (detalType)
            {
                case Plate:
                    return DetalType.Plate;

                case Stringer:
                    return DetalType.Stringer;

                case Treygolnik:
                    return DetalType.Treygolnik;

                default:
                    return DetalType.Plate;
            }
        }

        public static string EnumToString(DetalType detalType)
        {
            switch (detalType)
            {
                case DetalType.Plate:
                    return Plate;

                case DetalType.Stringer:
                    return Stringer;

                case DetalType.Treygolnik:
                    return Treygolnik;

                default:
                    return Plate;
            }
        }
    }
}
