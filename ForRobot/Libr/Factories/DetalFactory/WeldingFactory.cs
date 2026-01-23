using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ForRobot.Models.Detals;
using ForRobot.Models.Welding;

namespace ForRobot.Libr.Factories.DetalFactory
{
    public static class WeldingSchemaExtention
    {
        /// <summary>
        /// Заполнение схемы: сначала слева четные - нечетные, справа четные - нечетные
        /// </summary>
        /// <param name="weldingSchema"></param>
        /// <returns></returns>
        public static WeldingSchema BuildWeldingSchema_LeftEvenOddRightEvenOdd(this WeldingSchema weldingSchema)
        {
            int i = 1;
            var schemaList = weldingSchema.ToList();
            for (int index = 0; index < schemaList.Count; index++)
            {
                if ((index + 1) % 2 == 0)
                {
                    schemaList[index].LeftSide = i.ToString();
                    i++;
                }
            }
            for (int index = 0; index < schemaList.Count; index++)
            {
                if ((index + 1) % 2 != 0)
                {
                    schemaList[index].LeftSide = i.ToString();
                    i++;
                }
            }
            for (int index = 0; index < schemaList.Count; index++)
            {
                if ((index + 1) % 2 == 0)
                {
                    schemaList[index].RightSide = i.ToString();
                    i++;
                }
            }
            for (int index = 0; index < schemaList.Count; index++)
            {
                if ((index + 1) % 2 != 0)
                {
                    schemaList[index].RightSide = i.ToString();
                    i++;
                }
            }
            return new WeldingSchema(schemaList);
        }
    }

    /// <summary>
    /// Фабрика для создания схемы сварки деталей
    /// </summary>
    public static class WeldingFactory
    {
        #region Private functions

        /// <summary>
        /// Определение DetalType на основе generic типа
        /// </summary>
        /// <typeparam name="T">Тип детали</typeparam>
        /// <returns>Соответствующий DetalType</returns>
        /// <exception cref="NotSupportedException">Если тип не поддерживается</exception>
        private static DetalType GetDetalTypeFromGenericType<T>() where T : Detal
        {
            Type targetType = typeof(T);

            switch (targetType)
            {
                case Type plate when plate == typeof(Plate):
                    return DetalType.Plate;

                default:
                    throw new NotSupportedException($"Тип {targetType.Name} не поддерживается фабрикой");
            }
        }

        /// <summary>
        /// Создание заполненной коллекции <see cref="WeldingSchemaItem"/>
        /// </summary>
        /// <param name="count">Кол-во элементов коллекции</param>
        /// <returns></returns>
        private static IEnumerable<WeldingSchemaItem> CreateEnumerableWithCount(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new WeldingSchemaItem();
            }
        }

        /// <summary>
        /// Сборка схемы варки для плиты с рёбрами
        /// </summary>
        /// <param name="typeSchema">Тип схемы</param>
        private static IEnumerable<WeldingSchemaItem> CreatePlitaSchema(WeldingSchemaTypes typeSchema, int ribsCount)
        {
            switch (typeSchema)
            {
                case WeldingSchemaTypes.LeftEvenOdd_RightEvenOdd:
                    return (CreateEnumerableWithCount(ribsCount * 2) as WeldingSchema).BuildWeldingSchema_LeftEvenOddRightEvenOdd();

                default:
                    return CreateEnumerableWithCount(ribsCount);
            }
        }

        #endregion Private functions

        #region Public functions

        public static IEnumerable<WeldingSchemaItem> CreateSchema<T>(WeldingSchemaTypes typeSchema, int weldCount)  where T : Detal
        {
            DetalType detalType = GetDetalTypeFromGenericType<T>();
            try
            {
                switch (detalType)
                {
                    case DetalType.Plate:
                        return CreatePlitaSchema(typeSchema, weldCount == 0 ? Plate.MIN_RIB_COUNT : weldCount);

                    default:
                        throw new ArgumentException($"Тип детали {DetalTypes.EnumToString(detalType)} не поддерживается", nameof(detalType));
                }
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                throw new InvalidOperationException($"Ошибка при создании схемы сварки детали типа {detalType}", ex);
            }
        }

        #endregion Public functions
    }
}
