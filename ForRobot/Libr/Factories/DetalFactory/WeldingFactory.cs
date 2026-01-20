using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ForRobot.Models.Detals;
using ForRobot.Models.Welding;

namespace ForRobot.Libr.Factories.DetalFactory
{
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
                case Type plate when plate == typeof(Plita):
                    return DetalType.Plita;

                default:
                    throw new NotSupportedException($"Тип {targetType.Name} не поддерживается фабрикой");
            }
        }

        /// <summary>
        /// Заполнение схемы: сначала слева четные - нечетные, справа четные - нечетные
        /// </summary>
        /// <param name="iSumRib">Кол-во рёбер</param>
        /// <returns></returns>
        private static IEnumerable<WeldingSchemaItem> BuildLeftEvenOddRightEvenOdd(int weldsCount, IEnumerable<WeldingSchemaItem> weldingSchema)
        {
            int i = 1;
            var schemaList = weldingSchema.ToList<WeldingSchemaItem>();
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

            return schemaList;
        }

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
                    return BuildLeftEvenOddRightEvenOdd(ribsCount * 2, CreateEnumerableWithCount(ribsCount));

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
                    case DetalType.Plita:
                        return CreatePlitaSchema(typeSchema, weldCount == 0 ? Plita.MIN_RIB_COUNT : weldCount);

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
