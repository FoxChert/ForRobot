using System;
using System.Collections.Generic;
using System.Linq;

using ForRobot.Models.Welding;

namespace ForRobot.Libr.Factories
{
    /// <summary>
    /// Фабрика для создания схемы сварки деталей
    /// </summary>
    public static class WeldingFactory
    {
        #region Private functions

        /// <summary>
        /// Создание заполненной коллекции <see cref="WeldingSchemaItem"/>
        /// </summary>
        /// <param name="count">Кол-во элементов коллекции</param>
        /// <returns>Коллекция WeldingSchemaItem заданного размера</returns>
        private static IEnumerable<WeldingSchemaItem> CreateEnumerableWithCount(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Количество элементов не может быть отрицательным");

            for (int i = 0; i < count; i++)
            {
                yield return new WeldingSchemaItem();
            }
        }

        /// <summary>
        /// Создает схему типа LeftEvenOdd_RightEvenOdd
        /// </summary>
        /// <param name="weldCount">Количество точек сварки</param>
        /// <returns>Заполненная схема сварки</returns>
        private static IEnumerable<WeldingSchemaItem> CreateLeftEvenOddRightEvenOddSchema(int weldCount)
        {
            var items = CreateEnumerableWithCount(weldCount).ToList();
            var schema = new WeldingSchema(items);
            return schema.BuildWeldingSchema_LeftEvenOddRightEvenOdd();
        }

        #endregion Private functions

        #region Public functions

        /// <summary>
        /// Создает схему сварки указанного типа
        /// </summary>
        /// <param name="typeSchema">Тип схемы сварки</param>
        /// <param name="weldCount">Количество точек сварки</param>
        /// <returns>Коллекция элементов схемы сварки</returns>
        public static IEnumerable<WeldingSchemaItem> CreateSchema(WeldingSchemaTypes typeSchema, int weldCount)
        {
            if (weldCount < 0)
                throw new ArgumentException("Количество сварок не может быть отрицательным", nameof(weldCount));

            try
            {
                switch (typeSchema)
                {
                    case WeldingSchemaTypes.LeftEvenOdd_RightEvenOdd:
                        return CreateLeftEvenOddRightEvenOddSchema(weldCount);

                    default:
                        return CreateEnumerableWithCount(weldCount);
                }
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                throw new InvalidOperationException($"Ошибка при создании схемы сварки", ex);
            }
        }

        #endregion Public functions
    }
}
