using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.ComponentModel;

using Newtonsoft.Json;

using ForRobot.Libr.Converters;
using ForRobot.Libr.Collections;

namespace ForRobot.Models.Welding
{
    public class WeldingProperties : INotifyPropertyChanged, IDisposable
    {
        #region Private variables

        private decimal _searchOffsetStart;
        private decimal _searchOffsetEnd;
        private decimal _weldsDissolutionLeft;
        private decimal _weldsDissolutionRight;
        private decimal _techOffsetSeamStart;
        private decimal _techOffsetSeamEnd;
        private decimal _seamsOverlap;
        private int _programNom;
        private int _weldingSpead;
        private decimal _distanceForSearch;
        private decimal _distanceForWelding;
        private bool _diferentDissolutionLeft = false;
        private bool _diferentDissolutionRight = false;
        private WeldCollcetion _welds;
        private WeldingSchemaTypes _selectedWeldingSchema;
        private WeldingSchema _weldingSchema;

        #endregion Private variables

        #region Public variables

        [JsonProperty("search_offset_start")]
        [JsonConverter(typeof(JsonCommentConverter), "Отступ поиска в начале шва")]
        /// <summary>
        /// Отступ поиска в начале шва
        /// </summary>
        public decimal SearchOffsetStart
        {
            get => this._searchOffsetStart;
            set
            {
                this._searchOffsetStart = value;
                this.OnChangeProperty(nameof(this.SearchOffsetStart));
            }
        }

        [JsonProperty("search_offset_end")]
        [JsonConverter(typeof(JsonCommentConverter), "Отступ поиска в конце шва")]
        /// <summary>
        /// Отступ поиска в конце шва
        /// </summary>
        public decimal SearchOffsetEnd
        {
            get => this._searchOffsetEnd;
            set
            {
                this._searchOffsetEnd = value;
                this.OnChangeProperty(nameof(this.SearchOffsetEnd));
            }
        }

        [JsonConverter(typeof(JsonCommentConverter), "Роспуск слева")]
        /// <summary>
        /// Отступ швов от левого края ребер (роспуск, выкружка)
        /// </summary>
        public decimal WeldsDissolutionLeft
        {
            get => this._weldsDissolutionLeft;
            set
            {
                this._weldsDissolutionLeft = value;
                this.OnChangeProperty();
            }
        }

        [JsonConverter(typeof(JsonCommentConverter), "Роспуск справа")]
        /// <summary>
        /// Отступ швов от правого края ребер (роспуск, выкружка)
        /// </summary>
        public decimal WeldsDissolutionRight
        {
            get => this._weldsDissolutionRight;
            set
            {
                this._weldsDissolutionRight = value;
                this.OnChangeProperty();
            }
        }

        [JsonProperty("weld_tech_offset_start")]
        [JsonConverter(typeof(JsonCommentConverter), "Технологический отступ начала шва")]
        /// <summary>
        /// Технологический отступ начала шва
        /// </summary>
        public decimal TechOffsetSeamStart
        {
            get => this._techOffsetSeamStart;
            set
            {
                this._techOffsetSeamStart = value;
                this.OnChangeProperty(nameof(this.TechOffsetSeamStart));
            }
        }

        [JsonProperty("weld_tech_offset_end")]
        [JsonConverter(typeof(JsonCommentConverter), "Технологический отступ конца шва")]
        /// <summary>
        /// Технологический отступ конца шва
        /// </summary>
        public decimal TechOffsetSeamEnd
        {
            get => this._techOffsetSeamEnd;
            set
            {
                this._techOffsetSeamEnd = value;
                this.OnChangeProperty(nameof(this.TechOffsetSeamEnd));
            }
        }

        [JsonProperty("weld_overlap")]
        [JsonConverter(typeof(JsonCommentConverter), "Перекрытие швов в месте соединения")]
        /// <summary>
        /// Перекрытие швов в месте соединения
        /// </summary>
        public decimal SeamsOverlap
        {
            get => this._seamsOverlap;
            set
            {
                this._seamsOverlap = value;
                this.OnChangeProperty(nameof(this.SeamsOverlap));
            }
        }

        [JsonProperty("weld_job")]
        [JsonConverter(typeof(JsonCommentConverter), "Номер используемого джоба (ячейки) на источнике")]
        /// <summary>
        /// Номер сварочной программы
        /// </summary>
        public int ProgramNom
        {
            get => this._programNom;
            set
            {
                this._programNom = value;
                this.OnChangeProperty(nameof(this.ProgramNom));
            }
        }

        [JsonProperty("weld_velocity")]
        [JsonConverter(typeof(JsonCommentConverter), "Скорость сварки (обратно пропорциональна получаемому катету)")]
        /// <summary>
        /// Скорость сварки
        /// </summary>
        public int WeldingSpead
        {
            get => this._weldingSpead;
            set
            {
                this._weldingSpead = value;
                this.OnChangeProperty(nameof(this.WeldingSpead));
            }
        }

        [JsonProperty("gantry_radius_weld")]
        [JsonConverter(typeof(JsonCommentConverter), "Расстояние между фланцем робота и позиционера на сварке для расчёта положения позиционера")]
        /// <summary>
        /// Дистанция до позиционера для сварки
        /// </summary>
        public decimal DistanceForWelding
        {
            get => this._distanceForWelding;
            set
            {
                this._distanceForWelding = value;
                this.OnChangeProperty(nameof(this.DistanceForWelding));
            }
        }

        [JsonProperty("gantry_radius_search")]
        [JsonConverter(typeof(JsonCommentConverter), "Расстояние между фланцем робота и позиционера на поисках")]
        /// <summary>
        /// Дистанция до позиционера для поиска
        /// </summary>
        public decimal DistanceForSearch
        {
            get => this._distanceForSearch;
            set
            {
                this._distanceForSearch = value;
                this.OnChangeProperty(nameof(this.DistanceForSearch));
            }
        }

        [JsonConverter(typeof(JsonCommentConverter), "Разный ли роспуск слева")]
        /// <summary>
        /// Разный ли роспуск слева
        /// </summary>
        public bool DiferentDissolutionLeft
        {
            get => this._diferentDissolutionLeft;
            set
            {
                this._diferentDissolutionLeft = value;
                this.OnChangeProperty(nameof(this.DiferentDissolutionLeft));
            }
        }

        [JsonConverter(typeof(JsonCommentConverter), "Разный ли роспуск справа")]
        /// <summary>
        /// Разный ли роспуск справа
        /// </summary>
        public bool DiferentDissolutionRight
        {
            get => this._diferentDissolutionRight;
            set
            {
                this._diferentDissolutionRight = value;
                this.OnChangeProperty(nameof(this.DiferentDissolutionRight));
            }
        }

        [JsonProperty("welds_list")]
        [JsonConverter(typeof(JsonCommentConverter), "Коллекция швов")]
        /// <summary>
        /// Коллекция швов
        /// </summary>
        public WeldCollcetion Welds
        {
            get => this._welds;
            set
            {
                if (this._welds != null)
                    this._welds.WeldPropertyChanged -= (s, e) => this.OnChangeProperty(e.PropertyName);

                this._welds = value;

                if (this._welds != null)
                    this._welds.WeldPropertyChanged += (s, e) => this.OnChangeProperty(e.PropertyName);

                this.OnChangeProperty(nameof(this.Welds));
            }
        }

        [JsonConverter(typeof(JsonCommentConverter), "Выбранная схема сварки рёбер")]
        /// <summary>
        /// Выбранная схема сварки рёбер
        /// </summary>
        public WeldingSchemaTypes SelectedWeldingSchema
        {
            get => this._selectedWeldingSchema;
            set
            {
                this._selectedWeldingSchema = value;
                this.OnChangeProperty(nameof(this.SelectedWeldingSchema));
            }
        }
        
        [JsonProperty("welding_schema")]
        [JsonConverter(typeof(JsonCommentConverter), "Схема сварки")]
        /// <summary>
        /// Схема сварки (в какой очерёдности будут накладываться сварные швы)
        /// </summary>
        public WeldingSchema WeldingSchema
        {
            get => this._weldingSchema;
            set
            {
                if (this._weldingSchema != null)
                    this._weldingSchema.ItemPropertyChanged -= this.HandlerPropertyChanged_WeldingSchemaItem;

                this._weldingSchema = value;

                if (this._weldingSchema != null)
                    this._weldingSchema.ItemPropertyChanged += this.HandlerPropertyChanged_WeldingSchemaItem;

                this.OnChangeProperty(nameof(this.WeldingSchema));
            }
        }

        #region Events

        /// <summary>
        /// Событие изменения параметра детали
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #endregion Public variables

        public WeldingProperties()
        {
            this.PropertyChanged += this.HandlerPropertyChanged;

            //this.SelectedWeldingSchema = WeldingSchemaTypes.Edit;
            //this.WeldingSchema = new FullyObservableCollection<WeldingSchemaItem>();
        }

        private void HandlerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.DiferentDissolutionLeft):
                case nameof(this.WeldsDissolutionLeft):

                    if (this.DiferentDissolutionLeft)
                        break;

                    this.Welds?.SetWeldsDissolutionLeft(this.WeldsDissolutionLeft);
                    break;

                case nameof(this.DiferentDissolutionRight):
                case nameof(this.WeldsDissolutionRight):

                    if (this.DiferentDissolutionRight)
                        break;

                    this.Welds?.SetWeldsDissolutionRight(this.WeldsDissolutionRight);
                    break;
            }
        }

        private void HandlerPropertyChanged_WeldingSchemaItem(object sender, ItemPropertyChangedEventArgs e)
        {
            this.SelectedWeldingSchema = WeldingSchemaTypes.Edit;
            this.OnChangeProperty(e.PropertyName);
        }

        /// <summary>
        /// Вызов события изменения свойства
        /// </summary>
        /// <param name="propertyName">Наименование свойства</param>
        private void OnChangeProperty([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void FillWeldsCollection(int count)
        {
            Weld weld;
            List<Weld> weldsList = new List<Weld>();
            for (int i = 0; i < count; i++)
            {
                weld = new Weld()
                {
                    DissolutionLeft = this.WeldsDissolutionLeft,
                    DissolutionRight = this.WeldsDissolutionRight
                };
                weldsList.Add(weld);
            }
            this.Welds = new WeldCollcetion(weldsList);
        }

        //public void BuildingWeldingSchema(int ribsCount = Plate.MIN_RIB_COUNT)
        //{
        //    if (this.WeldingSchema != null)
        //        this.WeldingSchema.ItemPropertyChanged -= HandlerPropertyChanged_WeldingSchemaItem;

        //    this.WeldingSchema = ForRobot.Models.Detals.WeldingSchemas.BuildingSchema(this.SelectedWeldingSchema, ribsCount) as FullyObservableCollection<WeldingSchemas.SchemaItem>;

        //    if (this.WeldingSchema != null)
        //        this.WeldingSchema.ItemPropertyChanged += HandlerPropertyChanged_WeldingSchemaItem;
        //}

        //public static FullyObservableCollection<WeldingSchemas.SchemaItem> FillWeldingSchema(WeldingSchemas.SchemasTypes schemasType, int weldsCount)
        //{
        //    FullyObservableCollection<WeldingSchemas.SchemaItem> schema = ForRobot.Models.Detals.WeldingSchemas.BuildingSchema(schemasType, weldsCount);
        //    schema.ItemPropertyChanged += (s, e) =>
        //    {
        //        if (this.SelectedWeldingSchema != WeldingSchemas.GetDescription(WeldingSchemas.SchemasTypes.Edit))
        //            this.SelectedWeldingSchema = ForRobot.Models.Detals.WeldingSchemas.GetDescription(WeldingSchemas.SchemasTypes.Edit);

        //        this.OnChangeProperty(nameof(this.WeldingSchema));
        //    };
        //    return schema;
        //}

        #region Implementations of IDisposable

        ~WeldingProperties() => this.Dispose();

        public void Dispose()
        {
            this.PropertyChanged -= HandlerPropertyChanged;
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
