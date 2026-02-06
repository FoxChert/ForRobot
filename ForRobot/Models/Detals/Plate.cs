using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using ForRobot.Libr.Clipboard;
using ForRobot.Libr.Converters;
using ForRobot.Libr.Collections;
using System.Windows.Media.Media3D;

namespace ForRobot.Models.Detals
{
    public class Plate : Detal
    {
        #region Private variables
        
        private bool _diferentDistance = false;
        private bool _paralleleRibs = true;
        
        private decimal _ribsHeight;
        private decimal _ribsThickness;
        private int _ribCount = MIN_RIB_COUNT;
        private decimal _distanceToFirstRib;
        private decimal _distanceBetweenRibs;
        private decimal _ribsIdentToLeft;
        private decimal _ribsIdentToRight;
        private RibCollection _ribsCollection;

        #endregion

        #region Public variables

        public const int MIN_RIB_COUNT = 1;
        
        /// <inheritdoc cref="Detal.DetalType"/>
        public override string DetalType { get => DetalTypes.Plate; }

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
                this.OnPropertyChanged(nameof(this.DiferentDistance));
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
                this.OnPropertyChanged(nameof(this.ParalleleRibs));
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
                this.OnPropertyChanged(nameof(this.RibsHeight));
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
                this.OnPropertyChanged(nameof(this.RibsThickness));
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
                
                this.OnPropertyChanged(nameof(this.RibsCount));
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
                this.OnPropertyChanged();
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
                this.OnPropertyChanged();
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
                this.OnPropertyChanged();
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
                this.OnPropertyChanged();
            }
        }
       
        [JsonProperty("walls_list")]
        [JsonConverter(typeof(JsonCommentConverter), "Коллекция рёбер с параметрами:\n" +
                                                     "wall_height - высота ребра\n" +
                                                     "wall_thickness - толщина ребра\n" +
                                                     "wall_cross_dist_left - поперечное расстояние до ребра слева\n" +
                                                     "wall_cross_dist_right - поперечное расстояние до ребра справа\n" +
                                                     "wall_long_dist_left - продольное расстояние до ребра слева\n" +
                                                     "wall_long_dist_right - продольное расстояние до ребра справа")]
        /// <summary>
        /// Коллекция рёбер
        /// </summary>
        public RibCollection RibsCollection
        {
            get => this._ribsCollection;
            private set
            {
                if (this._ribsCollection != null)
                {
                    this._ribsCollection.RibPropertyChanged -= this.HandleChangeProperty_RibsCollection;
                    this._ribsCollection.CollectionChanged -= this.HandleCollectionChanged_RibsCollection;

                }

                this._ribsCollection = value;

                this._ribsCollection.RibPropertyChanged += this.HandleChangeProperty_RibsCollection;
                this._ribsCollection.CollectionChanged += this.HandleCollectionChanged_RibsCollection;
                this.OnPropertyChanged();
            }
        }

        #endregion

        #region Constructor

        public Plate() : base()
        {
            this.RibsCollection = new RibCollection(this.RibsCount);
            this.WeldingProperties.Welds = new WeldCollcetion(this.RibsCount);
            this.PropertyChanged += this.HandleChangeProperty;
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

                    using (SuppressNotifications())
                    {
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
                    }
                    break;

                case nameof(this.ParalleleRibs):
                    if (!this.ParalleleRibs || this.RibsCollection?.Count == 0)
                        break;

                    using (SuppressNotifications())
                    {
                        for (int i = 0; i < this.RibsCollection.Count; i++)
                        {
                            Rib rib = this.RibsCollection[i];

                            if (i == 0)
                            {
                                (rib.DistanceLeft, rib.DistanceRight) = (this.DistanceToFirstRib, this.DistanceToFirstRib);
                                continue;
                            }
                            (rib.DistanceLeft, rib.DistanceRight) = (this.DistanceBetweenRibs, this.DistanceBetweenRibs);
                        }
                    }
                    break;

                case nameof(this.RibsHeight):
                    using (SuppressNotifications())
                        this.RibsCollection.SetRibsHeight(this.RibsHeight);
                    break;

                case nameof(this.RibsThickness):
                    using (SuppressNotifications())
                        this.RibsCollection.SetRibsThickness(this.RibsThickness);
                    break;

                case nameof(this.DistanceToFirstRib):
                    if (this.RibsCollection?.Count == 0)
                        break;

                    using (SuppressNotifications())
                    {
                        this.RibsCollection[0].DistanceLeft = this.DistanceToFirstRib;
                        this.RibsCollection[0].DistanceRight = this.DistanceToFirstRib;
                    }
                    break;

                case nameof(this.DistanceBetweenRibs):
                    using (SuppressNotifications())
                    {
                        for (int i = 0; i < this.RibsCollection.Count; i++)
                        {
                            Rib rib = this.RibsCollection[i];

                            if (i == 0)
                                continue;

                            (rib.DistanceLeft, rib.DistanceRight) = (this.DistanceBetweenRibs, this.DistanceBetweenRibs);
                        }
                    }
                    break;

                case nameof(this.RibsIdentToLeft):
                    using (SuppressNotifications())
                        this.RibsCollection.SetRibsIdentToLeft(this.RibsIdentToLeft);
                    break;

                case nameof(this.RibsIdentToRight):
                    using (SuppressNotifications())
                        this.RibsCollection.SetRibsIdentToRight(this.RibsIdentToRight);
                    break;

                case nameof(this.RibsCount):
                    this.ComplexCascadingChange(() =>
                    {
                        this.RibsCollection.SetCount(this.RibsCount);
                        this.WeldingProperties.Welds?.SetCount(this.RibsCount);
                    });
                    break;
            }
        }

        /// <summary>
        /// Делегат изменения свойства ребра в <see cref="RibCollection"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleChangeProperty_RibsCollection(object sender, RibPropertyChangedEventArgs e)
        {
            if (e.Rib == null)
                return;

            switch (e.PropertyName)
            {
                case nameof(Rib.DistanceLeft):
                    using (SuppressNotifications())
                        if (this.ParalleleRibs)
                            e.Rib.DistanceRight = e.Rib.DistanceLeft;
                    break;
            }
            this.OnPropertyChanged(nameof(this.RibsCollection));
        }

        /// <summary>
        /// Делегат изменения <see cref="RibCollection"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleCollectionChanged_RibsCollection(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Remove:
                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }

        /// <summary>
        /// Выполнение комплексного каскадного изменения с подавлением уведомлений свойств <see cref="Detal.WeldingProperties"/> и <see cref="RibsCollection"/>
        /// </summary>
        /// <param name="changeAction">Делегат, выполняющий изменения</param>
        private void ComplexCascadingChange(Action changeAction)
        {
            var controlsToSuppress = new List<IChangeNotificationControl> { this };

            if (this.RibsCollection is IChangeNotificationControl ribsControl)
                controlsToSuppress.Add(ribsControl);

            if (this.WeldingProperties is IChangeNotificationControl weldingControl)
                controlsToSuppress.Add(weldingControl);

            using (NotificationSuppression.Suppress(controlsToSuppress))
            {
                changeAction?.Invoke();
            }
        }

        #endregion Handle

        #endregion Private functions

        #region Public functions

        #endregion Public functions

        #region Implementations of IDisposable

        private volatile int _disposed;

        ~Plate() => Dispose(false);

        public new void Dispose() => this.Dispose(true);

        public new void Dispose(bool disposing)
        {
            if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
            {
                if (disposing)
                {
                    this.PropertyChanged -= this.HandleChangeProperty;
                    GC.SuppressFinalize(this);
                }
            }
        }

        #endregion
    }
}
