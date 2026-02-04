using System;
using System.Collections.Generic;
using System.Linq;

namespace ForRobot.Models.Welding
{
    /// <summary>
    /// Методы расширения для работы со схемами сварки
    /// </summary>
    public static class WeldingSchemaExtension
    {
        /// <summary>
        /// Заполнение схемы: сначала слева четные - нечетные, справа четные - нечетные
        /// </summary>
        /// <param name="weldingSchema">Схема сварки для заполнения</param>
        /// <returns>Заполненная схема сварки</returns>
        /// <exception cref="ArgumentNullException">Если weldingSchema равен null</exception>
        public static WeldingSchema BuildWeldingSchema_LeftEvenOddRightEvenOdd(this WeldingSchema weldingSchema)
        {
            if (weldingSchema == null)
                throw new ArgumentNullException(nameof(weldingSchema));

            var schemaList = weldingSchema.ToList();
            if (schemaList.Count == 0)
                return weldingSchema;

            int counter = 1;

            // Заполняем левую сторону: сначала четные индексы, затем нечетные
            counter = FillSideByParity(schemaList, counter, true, false);
            counter = FillSideByParity(schemaList, counter, true, true);

            // Заполняем правую сторону: сначала четные индексы, затем нечетные
            counter = FillSideByParity(schemaList, counter, false, false);
            FillSideByParity(schemaList, counter, false, true);

            return new WeldingSchema(schemaList);
        }

        /// <summary>
        /// Вспомогательный метод для заполнения стороны по четности индексов
        /// </summary>
        /// <param name="items">Список элементов схемы</param>
        /// <param name="startCounter">Начальное значение счетчика</param>
        /// <param name="isLeftSide">True для левой стороны, false для правой</param>
        /// <param name="isOddIndex">True для нечетных индексов, false для четных</param>
        /// <returns>Следующее значение счетчика</returns>
        private static int FillSideByParity(List<WeldingSchemaItem> items, int startCounter, bool isLeftSide, bool isOddIndex)
        {
            var filteredItems = items
                .Select((item, index) => new { Item = item, Index = index })
                .Where(x => (x.Index + 1) % 2 == (isOddIndex ? 1 : 0))
                .ToList();

            int counter = startCounter;
            foreach (var itemInfo in filteredItems)
            {
                if (isLeftSide)
                    itemInfo.Item.LeftSide = counter.ToString();
                else
                    itemInfo.Item.RightSide = counter.ToString();
                counter++;
            }

            return counter;
        }
    }
}
