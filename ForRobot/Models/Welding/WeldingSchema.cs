using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using ForRobot.Libr;
using ForRobot.Libr.Collections;

namespace ForRobot.Models.Welding
{
    /// <summary>
    /// Схема очерёдности сварки швов
    /// </summary>
    public class WeldingSchema : FullyObservableCollection<WeldingSchemaItem>, INotifyPropertyChanged
    {
        #region Public variables

        /// <summary>
        /// Коллекция описаний схем сварки
        /// </summary>
        public static readonly IEnumerable<string> WeldingSchemasCollection = GetSchemasCollection();

        #endregion Public variables

        #region Constructors

        public WeldingSchema(int count) : base(new WeldingSchemaItem[count]) { }

        public WeldingSchema(List<WeldingSchemaItem> list) : base(list) { }

        public WeldingSchema(IEnumerable<WeldingSchemaItem> enumerable) : base(enumerable) { }

        #endregion Constructors

        /// <summary>
        /// Выборка описаний типов схем сварки
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<string> GetSchemasCollection()
        {
            var descriptions = Enum.GetValues(typeof(WeldingSchemaTypes))
                .Cast<WeldingSchemaTypes>()
                .Select(item => item.GetDescription());
            return descriptions.ToList<string>();
        }

        #region Public functions
        
        /// <summary>
        /// Вывод схемы для передачи
        /// </summary>
        /// <param name="schemaType"></param>
        /// <param name="schema">Схема сварки</param>
        /// <returns></returns>
        public static object[,] GetSchema(IEnumerable<WeldingSchemaItem> schema)
        {
            object[,] finishSchema = new object[schema.Count() * 2, 2];
            for (int i = 1; i <= (schema.Count() * 2); i++)
            {
                WeldingSchemaItem weld = schema.Where(item => item.LeftSide == i.ToString() || item.RightSide == i.ToString())?.Count() == 0 ? null
                                : schema.Where(item => item.LeftSide == i.ToString() || item.RightSide == i.ToString())?.First();
                if (weld == null)
                    throw new Exception(string.Format("При составлении схемы сварки не найдена очерёдность №{0}", i));
                finishSchema[i - 1, 0] = Array.IndexOf(schema.ToArray(), weld) + 1;
                finishSchema[i - 1, 1] = (weld.LeftSide == i.ToString()) ? "left_side" : "right_side";
            }
            return finishSchema;
        }

        /// <summary>
        /// Возврат атрибута <see cref="System.ComponentModel.DescriptionAttribute"/> перечисления <see cref="ShemasTypes"/>
        /// </summary>
        /// <param name="shemasTypes">Элемент перечисления <see cref="ShemasTypes"/></param>
        /// <returns></returns>
        public static string GetDescription(WeldingSchemaTypes shemaType)
        {
            var enums = typeof(WeldingSchemaTypes).GetFields();
            var descriptions = enums.Select(field => new { field.Name, Description = (field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false).SingleOrDefault() as System.ComponentModel.DescriptionAttribute)?.Description });
            return descriptions.Where(item => item.Name == shemaType.ToString()).First().Description;
        }

        /// <summary>
        /// Возвращение элемента <see cref="SchemasTypes"/>
        /// </summary>
        /// <param name="description">Атрибут описания элемента перечисления <see cref="SchemasTypes"/></param>
        /// <returns></returns>
        public static WeldingSchemaTypes GetSchemaType(string description)
        {            
            var enums = typeof(WeldingSchemaTypes).GetFields();
            var descriptions = enums.Select(field => new { Name = field.Name,  Description = (field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false).SingleOrDefault() as System.ComponentModel.DescriptionAttribute)?.Description });
            return (WeldingSchemaTypes)Enum.Parse(typeof(WeldingSchemaTypes), descriptions.Where(item => item.Description == description).First().Name);
        }

        #endregion Private functions
    }
}
