using System;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using ForRobot.Libr.Json;
using ForRobot.Libr.Attributes;
using ForRobot.Libr.Converters;
using ForRobot.Libr.Collections;
using System.ComponentModel;
using System.Threading;

namespace ForRobot.Models.Detals
{
    public class Plita : Detal
    {
        #region Private variables

        private string _selectedWeldingSchema = WeldingSchemas.GetDescription(WeldingSchemas.SchemasTypes.LeftEvenOdd_RightEvenOdd);

        private bool _diferentDistance = false;
        private bool _paralleleRibs = true;
        private bool _diferentDissolutionLeft = false;
        private bool _diferentDissolutionRight = false;
        
        private decimal _ribsHeight;
        private decimal _ribsThickness;
        private int _ribCount;
        private decimal _distanceToFirstRib;
        private decimal _distanceBetweenRibs;
        private decimal _ribsIdentToLeft;
        private decimal _ribsIdentToRight;
        private decimal _weldsDissolutionLeft;
        private decimal _weldsDissolutionRight;
        //private FullyObservableCollection<Rib> _ribsCollection;
        private RibCollection _ribsCollection;

        #endregion

        #region Public variables

        public const int MIN_RIB_COUNT = 1;
        
        /// <inheritdoc cref="Detal.DetalType"/>
        public override string DetalType { get => DetalTypes.Plita; }

        [JsonConverter(typeof(JsonCommentConverter), "Разное ли рассояние между рёбрами")]
        /// <summary>
        /// Различно ли расстояние между рёбрами => отступы и т.д.
        /// </summary>
        public bool DiferentDistance
        {
            get => this._diferentDistance;
            set
            {
                this._diferentDistance = value;
                this.OnChangeProperty(nameof(this.DiferentDistance));
            }
        }

        [JsonConverter(typeof(JsonCommentConverter), "Параллельлны ли рёбра")]
        /// <summary>
        /// Паралельны ли рёбра друг к другу
        /// </summary>
        public bool ParalleleRibs
        {
            get => this.DiferentDistance ? this._paralleleRibs : true;
            set
            {
                this._paralleleRibs = value;
                this.OnChangeProperty(nameof(this.ParalleleRibs));
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
        
        [JsonConverter(typeof(JsonCommentConverter), "Высота рёбер (вертикальной стенки)")]
        /// <summary>
        /// Высота ребра
        /// </summary>
        public decimal RibsHeight
        {
            get => this._ribsHeight;
            set
            {
                this._ribsHeight = value;
                this.OnChangeProperty(nameof(this.RibsHeight));
            }
        }
        
        [JsonConverter(typeof(JsonCommentConverter), "Толщина рёбер")]
        /// <summary>
        /// Толщина ребра
        /// </summary>
        public decimal RibsThickness
        {
            get => this._ribsThickness;
            set
            {
                this._ribsThickness = value;
                this.OnChangeProperty(nameof(this.RibsThickness));
            }
        }

        [JsonProperty("wall_count")]
        [JsonConverter(typeof(JsonCommentConverter), "Кол-во рёбер")]
        /// <summary>
        /// Количество ребер
        /// </summary>
        public int RibsCount
        {
            get => this._ribCount;
            set
            {
                if (value < MIN_RIB_COUNT)
                    return;

                this._ribCount = value;

                //this.ChangeRibCollection();
                //this.ChangeWeldingSchema();
                this.OnChangeProperty(nameof(this.RibsCount));
            }
        }
        
        [JsonConverter(typeof(JsonCommentConverter), "Расстояние по ширине до осевой линии первого ребра")]
        /// <summary>
        /// Поперечное расстояние по ширине до осевой линии первого ребра
        /// </summary>
        public decimal DistanceToFirstRib
        {
            get => this._distanceToFirstRib;
            set
            {
                this._distanceToFirstRib = value;
                this.OnChangeProperty();
            }
        }
        
        [JsonConverter(typeof(JsonCommentConverter), "Расстояние между осевыми линиями рёбер")]
        /// <summary>
        /// Поперечное расстояние между осевыми линиями рёбер
        /// </summary>
        public decimal DistanceBetweenRibs
        {
            get => this._distanceBetweenRibs;
            set
            {
                this._distanceBetweenRibs = value;
                this.OnChangeProperty();
            }
        }
        
        [JsonConverter(typeof(JsonCommentConverter), "Продольное расстояние до ребра по левому краю")]
        /// <summary>
        /// Продольное расстояние до ребер по левому краю
        /// </summary>
        public decimal RibsIdentToLeft
        {
            get => this._ribsIdentToLeft;
            set
            {
                this._ribsIdentToLeft = value;
                this.OnChangeProperty();
            }
        }
        
        [JsonConverter(typeof(JsonCommentConverter), "Продольное расстояние до ребра по правому краю")]
        /// <summary>
        /// Продольное расстояние до ребер по правому краю
        /// </summary>
        public decimal RibsIdentToRight
        {
            get => this._ribsIdentToRight;
            set
            {
                this._ribsIdentToRight = value;
                this.OnChangeProperty();
            }
        }
        
        [JsonConverter(typeof(JsonCommentConverter), "Роспуск слева")]
        /// <summary>
        /// Отступ шва от левого края ребер (роспуск, выкружка)
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
        /// Отступ шва от правого края ребер (роспуск, выкружка)
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
                
        [JsonProperty("walls_list")]
        [JsonConverter(typeof(JsonCommentConverter), "Коллекция рёбер с параметрами:\n" +
                                                     "wall_height - высота ребра\n" +
                                                     "wall_thickness - толщина ребра\n" +
                                                     "wall_cross_dist_left - поперечное расстояние до ребра слева\n" +
                                                     "wall_cross_dist_right - поперечное расстояние до ребра справа\n" +
                                                     "wall_long_dist_left - продольное расстояние до ребра слева\n" +
                                                     "wall_long_dist_right - продольное расстояние до ребра справа\n" +
                                                     "weld_offset_left - отступ шва слева\n" +
                                                     "weld_offset_right - отступ шва справа")]
        /// <summary>
        /// Коллекция рёбер
        /// </summary>
        public RibCollection RibsCollection
        {
            get => this._ribsCollection;
            private set
            {
                if(this._ribsCollection != null)
                    this._ribsCollection.RibPropertyChanged -= (s, e) => this.OnChangeProperty(e.PropertyName);

                this._ribsCollection = value;
                this._ribsCollection.RibPropertyChanged += (s, e) => this.OnChangeProperty(e.PropertyName);
            }
        }

        //public FullyObservableCollection<Rib> RibsCollection
        //{
        //    get => this._ribsCollection;
        //    private set
        //    {
        //        if(this._ribsCollection != null)
        //            this._ribsCollection.ItemPropertyChanged -= (s, e) => this.OnChangeProperty();
                    
        //        this._ribsCollection = value;

        //        if (this._ribsCollection != null)
        //            this._ribsCollection.ItemPropertyChanged += (s, e) => this.OnChangeProperty();
        //    }
        //}

        #endregion

        #region Constructor

        public Plita() : base()
        {
            this.RibsCollection = this.FillRibsCollection();
            //this.WeldingSchema = this.FillWeldingSchema();

            this.ChangePropertyEvent += this.HandleChangeProperty;
        }

        #endregion

        #region Private functions

        #region Handle

        /// <summary>
        /// Делегат изменения свойства плиты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleChangeProperty(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.DiferentDistance):
                    if (this.DiferentDistance || this.RibsCollection?.Count == 0)
                        break;

                    for (int i = 0; i < this.RibsCollection.Count; i++)
                    {
                        this.RibsCollection[i].IdentToLeft = this.RibsIdentToLeft;
                        this.RibsCollection[i].IdentToRight = this.RibsIdentToRight;

                        if (i == 0)
                        {
                            this.RibsCollection[i].DistanceLeft = this.DistanceToFirstRib;
                            continue;
                        }
                        this.RibsCollection[i].DistanceLeft = this.DistanceBetweenRibs;
                    }
                    break;

                case nameof(this.ParalleleRibs):
                    if (!this.ParalleleRibs || this.RibsCollection?.Count == 0)
                        break;

                    for (int i = 0; i < this.RibsCollection.Count; i++)
                    {
                        Rib rib = this.RibsCollection[i];

                        if (i == 0)
                            (rib.DistanceLeft, rib.DistanceRight) = (this.DistanceToFirstRib, this.DistanceToFirstRib);

                        (rib.DistanceLeft, rib.DistanceRight) = (this.DistanceBetweenRibs, this.DistanceBetweenRibs);
                    }
                    break;

                case nameof(this.DiferentDissolutionLeft):
                    if (this.DiferentDissolutionLeft)
                        break;
                    this.RibsCollection.SetWeldsDissolutionLeft(this.WeldsDissolutionLeft);
                    break;

                case nameof(this.DiferentDissolutionRight):
                    if (this.DiferentDissolutionRight)
                        return;
                    this.RibsCollection.SetWeldsDissolutionRight(this.WeldsDissolutionRight);
                    break;

                case nameof(this.RibsHeight):
                    this.RibsCollection.SetRibsHeight(this.RibsHeight);
                    break;

                case nameof(this.RibsThickness):
                    this.RibsCollection.SetRibsThickness(this.RibsThickness);
                    break;

                case nameof(this.DistanceToFirstRib):
                    if (!this.ValidateRibsCollection())
                        break;
                    //if (this.RibsCollection?.Count == 0)
                    //    break;

                    this.RibsCollection[0].DistanceLeft = this.DistanceToFirstRib;
                    this.RibsCollection[0].DistanceRight = this.DistanceToFirstRib;
                    break;

                case nameof(this.DistanceBetweenRibs):
                    this.RibsCollection.SetRibsDistance(this.DistanceBetweenRibs);
                    break;

                case nameof(this.RibsIdentToLeft):
                    this.RibsCollection.SetRibsIdentToLeft(this.RibsIdentToLeft);
                    break;

                case nameof(this.RibsIdentToRight):
                    this.RibsCollection.SetRibsIdentToRight(this.RibsIdentToRight);
                    break;

                case nameof(this.WeldsDissolutionLeft):
                    this.RibsCollection.SetWeldsDissolutionLeft(this.WeldsDissolutionLeft);
                    break;

                case nameof(this.WeldsDissolutionRight):
                    this.RibsCollection.SetWeldsDissolutionRight(this.WeldsDissolutionRight);
                    break;

                case nameof(this.RibsCount):
                    this.RibsCollection.SetCount(this.RibsCount);

                    //if (this.RibsCollection?.Count == 0)
                    //    this.RibsCollection = this.FillRibsCollection();

                    this.WeldingProperties?.BuildingWeldingSchema(this.RibsCount);
                    break;

                case nameof(this.WeldingProperties.SelectedWeldingSchema):
                    this.WeldingProperties.BuildingWeldingSchema(this.RibsCount);
                    break;
            }
        }

        #endregion Handle

        /// <summary>
        /// Заполнение коллекции расстояний
        /// </summary>
        /// <returns></returns>
        private RibCollection FillRibsCollection()
        {
            Rib rib;
            List<Rib> ribsList = new List<Rib>();

            for (int i = 0; i < this.RibsCount; i++)
            {
                rib = new Rib()
                {
                    Height = this.RibsHeight,
                    Thickness = this.RibsThickness,
                    IdentToLeft = this.RibsIdentToLeft,
                    IdentToRight = this.RibsIdentToRight,
                    DissolutionLeft = this.WeldsDissolutionLeft,
                    DistanceRight = this.WeldsDissolutionRight
                };

                if (i == 0)
                {
                    rib.DistanceLeft = this.DistanceToFirstRib;
                    rib.DistanceRight = this.DistanceToFirstRib;
                }
                else
                {
                    rib.DistanceLeft = this.DistanceBetweenRibs;
                    rib.DistanceRight = this.DistanceBetweenRibs;
                }

                ribsList.Add(rib);
            }
            return new RibCollection(ribsList);
        }

        private bool ValidateRibsCollection()
        {
            if(this.RibsCollection == null || this.RibsCollection.Count == 0)
            {
                this.FillRibsCollection();
                return false;
            }
            return true;
        }

        //private FullyObservableCollection<WeldingSchemas.SchemaItem> FillWeldingSchema()
        //{
        //    if (string.IsNullOrEmpty(this.SelectedWeldingSchema))
        //        return null;

        //    FullyObservableCollection<WeldingSchemas.SchemaItem> schema = ForRobot.Models.Detals.WeldingSchemas.BuildingSchema(ForRobot.Models.Detals.WeldingSchemas.GetSchemaType(this.SelectedWeldingSchema), base.RibsCount);
        //    schema.ItemPropertyChanged += (s, e) =>
        //    {
        //        if (this.SelectedWeldingSchema != WeldingSchemas.GetDescription(WeldingSchemas.SchemasTypes.Edit))
        //            this.SelectedWeldingSchema = ForRobot.Models.Detals.WeldingSchemas.GetDescription(WeldingSchemas.SchemasTypes.Edit);

        //        this.OnChangeProperty(nameof(this.WeldingSchema));
        //    };
        //    return schema;
        //}

        //private void ChangeWeldingSchema()
        //{
        //    if (this.WeldingSchema == null || this.WeldingSchema?.Count == 0)
        //        return;

        //    if (this.RibsCount > this.WeldingSchema.Count)
        //        for (int i = this.WeldingSchema.Count; i < this.RibsCount; i++)
        //            this.WeldingSchema.Add(new WeldingSchemas.SchemaItem());
        //    else
        //    {
        //        for (int i = this.WeldingSchema.Count - 1; i >= this.RibsCount; i--)
        //            this.WeldingSchema.RemoveAt(i);
        //    }

        //    this.SelectedWeldingSchema = this.SelectedWeldingSchema;
        //}

        #endregion Private functions

        #region Public functions

        //public FullyObservableCollection<Rib> SetRibsCollection(FullyObservableCollection<Rib> collection) => this.RibsCollection = collection;

        #endregion Public functions

        #region Implementations of IDisposable

        private volatile int _disposed;

        ~Plita() => Dispose(false);

        public new void Dispose() => this.Dispose(true);

        public new void Dispose(bool disposing)
        {
            if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
            {
                if (disposing)
                {
                    this.ChangePropertyEvent -= this.HandleChangeProperty;
                    GC.SuppressFinalize(this);
                }
            }
        }

        #endregion
    }
}
