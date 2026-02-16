using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;

using Newtonsoft.Json;

using ForRobot.Libr.Clipboard;
using ForRobot.Libr.Converters;
using ForRobot.Models.Welding;

namespace ForRobot.Models.Detals
{
    public abstract class Detal : SceneItem, IDisposable, IChangeNotificationControl
    {
        #region Private variables

        private string _scoseType = ScoseTypes.Rect;
        private decimal _reverseDeflection;
        private decimal _plateWidth;
        private decimal _plateWidthSave;
        private decimal _plateLength;
        private decimal _plateThickness;
        private decimal _plateBevelToLeft;
        private decimal _plateBevelToRight;
        private decimal _plateBevelToLeftSave;
        private decimal _plateBevelToRightSave;

        //private double _localPositionX = 0.0;
        //private double _localPositionY = 0.0;
        //private double _localPositionZ = 0.0;
        //private decimal[] _XYZOffset = new decimal[3] { 0, 0, 0 };

        private WeldingProperties _weldingProperties = new WeldingProperties();

        #endregion

        #region Public variables

        /// <summary>
        /// Тип детали. Использует <see cref="ForRobot.Models.Detals.DetalType"/>
        /// </summary>
        public virtual DetalType DetalType { get; }

        [JsonProperty("d_type")]
        [JsonConverter(typeof(JsonCommentConverter), "Тип скоса")]
        /// <summary>
        /// Тип скоса
        /// </summary>
        public string ScoseType
        {
            get => this._scoseType;
            set
            {
                this._scoseType = value;
                this.OnPropertyChanged(nameof(this.ScoseType));
            }
        }

        [JsonProperty("reverse_deflection")]
        [JsonConverter(typeof(JsonCommentConverter), "Обратный прогиб детали")]
        /// <summary>
        /// Обратный прогиб детали
        /// </summary>
        public decimal ReverseDeflection
        {
            get => this._reverseDeflection;
            set
            {
                this._reverseDeflection = value;
                this.OnPropertyChanged(nameof(this.ReverseDeflection));
            }
        }

        [JsonProperty("base_width")]
        [JsonConverter(typeof(JsonCommentConverter), "Ширина настила")]
        /// <summary>
        /// Ширина настила
        /// </summary>
        public decimal PlateWidth
        {
            get => this._plateWidth;
            set
            {
                this._plateWidth = value;
                this.OnPropertyChanged(nameof(this.PlateWidth));
            }
        }

        [JsonProperty("base_length")]
        [JsonConverter(typeof(JsonCommentConverter), "Длина настила")]
        /// <summary>
        /// Длина настила
        /// </summary>
        public decimal PlateLength
        {
            get => this._plateLength;
            set
            {
                this._plateLength = value;
                this.OnPropertyChanged(nameof(this.PlateLength));
            }
        }

        [JsonProperty("base_thickness")]
        [JsonConverter(typeof(JsonCommentConverter), "Толщина настила")]
        /// <summary>
        /// Толщина настила
        /// </summary>
        public decimal PlateThickness
        {
            get => this._plateThickness;
            set
            {
                this._plateThickness = value;
                this.OnPropertyChanged(nameof(this.PlateThickness));
            }
        }

        [JsonProperty("base_bevel_left")]
        [JsonConverter(typeof(JsonCommentConverter), "Скос настила слева")]
        /// <summary>
        /// Скос настила слева
        /// </summary>
        public decimal PlateBevelToLeft
        {
            get => this._plateBevelToLeft;
            set
            {
                this._plateBevelToLeft = value;
                this.OnPropertyChanged(nameof(this.PlateBevelToLeft));
            }
        }

        [JsonProperty("base_bevel_right")]
        [JsonConverter(typeof(JsonCommentConverter), "Скос настила справа")]
        /// <summary>
        /// Скос настила справа
        /// </summary>
        public decimal PlateBevelToRight
        {
            get => this._plateBevelToRight;
            set
            {
                this._plateBevelToRight = value;
                this.OnPropertyChanged(nameof(this.PlateBevelToRight));
            }
        }

        [JsonProperty("base_displace")]
        [JsonConverter(typeof(JsonCommentConverter), "Смещение настила от нулевой точки стола")]
        /// <summary>
        /// Смещение детали от 0 точки по осям XYZ
        /// </summary>
        public override double[] XYZ { get => base.XYZ; }        

        //[JsonIgnore]
        ///// <summary>
        ///// Смещение детали от 0 точки по оси X
        ///// </summary>
        //public override double LocalPositionX
        //{
        //    get => base.LocalPositionX;
        //    set
        //    {
        //        base.LocalPositionX = value;
        //        this.OnPropertyChanged(nameof(this.LocalPositionX));
        //    }
        //}
        //[JsonIgnore]
        ///// <summary>
        ///// Смещение детали от 0 точки по оси Y
        ///// </summary>
        //public override double LocalPositionY
        //{
        //    get => this._localPositionY;
        //    set
        //    {
        //        this._localPositionY = value;
        //        this.OnPropertyChanged(nameof(this.LocalPositionY));
        //    }
        //}
        //[JsonIgnore]
        ///// <summary>
        ///// Смещение детали от 0 точки по оси Z
        ///// </summary>
        //public override double LocalPositionZ
        //{
        //    get => this._localPositionZ;
        //    set
        //    {
        //        this._localPositionZ = value;
        //        this.OnPropertyChanged(nameof(this.LocalPositionZ));
        //    }
        //}
        
        [JsonProperty("welding_properties")]
        [JsonConverter(typeof(JsonCommentConverter), "Параметры сварки")]
        /// <summary>
        /// Свойства сварки детали
        /// </summary>
        public WeldingProperties WeldingProperties
        {
            get => this._weldingProperties;
            set
            {
                if (this._weldingProperties != null)
                    this._weldingProperties.PropertyChanged -= this.HandleChangeProperty_WeldingProperties;

                this._weldingProperties = value;

                if (this._weldingProperties != null)
                    this._weldingProperties.PropertyChanged += this.HandleChangeProperty_WeldingProperties;

                this.OnPropertyChanged(nameof(WeldingProperties));
            }
        }

        /// <summary>
        /// Визуальная модель элемента сцены
        /// </summary>
        public override System.Windows.Media.Media3D.Model3DGroup VisualModel { get; protected set; }
        //{ get => ForRobot.Libr.Modeling.ModelingService.GetDetalModel(this); }

        #endregion

        #region Event

        #endregion

        #region Constructors

        public Detal()
        {
            this.WeldingProperties = new WeldingProperties();
            this.PropertyChanged += this.HandleChangeProperty;
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Делегат изменения свойства детали
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>,
        private void HandleChangeProperty(object sender, PropertyChangedEventArgs e)
        {
            if (_suppressNotifications)
                return;

            if (e.PropertyName == nameof(this.ScoseType))
            {
                ComplexCascadingChange(() =>
                {
                    if (this.ScoseType == ScoseTypes.Rect)
                    {
                        (this._plateWidthSave, this.PlateWidth) = (this.PlateWidth, 0);
                        (this._plateBevelToLeftSave, this._plateBevelToRightSave) = (this.PlateBevelToLeft, this.PlateBevelToRight);
                        (this.PlateBevelToLeft, this.PlateBevelToRight) = (0, 0);
                    }
                    else
                    {
                        this.PlateWidth = this._plateWidthSave;
                        (this.PlateBevelToLeft, this.PlateBevelToRight) = (this._plateBevelToLeftSave, this._plateBevelToRightSave);
                    }
                });
            }

            //this.VisualModel = ForRobot.Libr.Modeling.ModelingService.GetDetalModel(this);
        }

        /// <summary>
        /// Делегат изменения свойства параметров сворки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>,
        private void HandleChangeProperty_WeldingProperties(object sender, PropertyChangedEventArgs e) => this.OnPropertyChanged(e.PropertyName);

        /// <summary>
        /// Выполнение комплексного каскадного изменения с подавлением уведомлений свойства <see cref="WeldingProperties"/>
        /// </summary>
        /// <param name="changeAction">Делегат, выполняющий изменения</param>
        private void ComplexCascadingChange(Action changeAction)
        {
            var controlsToSuppress = new List<IChangeNotificationControl> { this };
            
            if (this.WeldingProperties is IChangeNotificationControl weldingControl)
                controlsToSuppress.Add(weldingControl);

            using (NotificationSuppression.Suppress(controlsToSuppress))
            {
                changeAction?.Invoke();
            }
        }

        #endregion

        #region Public functions

        //public override System.Windows.Media.Media3D.Model3DGroup GetModel()

        public override void UpdateTransform(System.Windows.Media.Media3D.Matrix3D transform)
        {
            throw new NotImplementedException();
        }

        public virtual object Clone()
        {
            var json = JsonConvert.SerializeObject(this);
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };
            return JsonConvert.DeserializeObject(json, this.GetType(), settings);
        }

        public bool Equals(Detal detal)
        {
            var detals = new System.Collections.Generic.HashSet<Detal>();
            detals.Add(this);
            return detals.Contains(detal);
        }

        /// <summary>
        /// Вызов события изменения свойства
        /// </summary>
        /// <param name="propertyName">Наименование свойства</param>
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this._suppressNotifications)
                return;

            base.OnPropertyChanged(propertyName);
        }

        #endregion

        #region Implementations of IChangeNotificationControl

        private bool _suppressNotifications = false;

        public bool IsNotificationsSuppressed => this._suppressNotifications;

        public IDisposable SuppressNotifications() => new NotificationSuppressionScope(this);

        private class NotificationSuppressionScope : IDisposable
        {
            private readonly Detal _owner;
            private readonly bool _wasSuppressed;

            public NotificationSuppressionScope(Detal owner)
            {
                _owner = owner;
                _wasSuppressed = _owner._suppressNotifications;
                _owner._suppressNotifications = true;
            }

            public void Dispose()
            {
                if (_owner != null)
                    _owner._suppressNotifications = _wasSuppressed;
            }
        }

        #endregion

        #region Implementations of IDisposable

        private volatile int _disposed;

        ~Detal() => Dispose(false);

        public void Dispose() => this.Dispose(true);

        public void Dispose(bool disposing)
        {
            if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
            {
                if (disposing)
                {
                    this.PropertyChanged -= this.HandleChangeProperty;
                    if (this.WeldingProperties != null)
                        this.WeldingProperties.PropertyChanged -= HandleChangeProperty_WeldingProperties;
                    GC.SuppressFinalize(this);
                }
            }
        }

        #endregion
    }
}
