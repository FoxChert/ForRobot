using System;
using System.ComponentModel;
//using System.Text.Json.Serialization;l
using Newtonsoft.Json;

using ForRobot.Libr.Json;

namespace ForRobot.Models.Detals
{
    /// <summary>
    /// Модель ребра настила
    /// </summary>
    public class Rib : ICloneable
    {
        private decimal _height;
        private decimal _thickness;
        private decimal _distanceLeft;
        private decimal _distanceRight;
        private decimal _identToLeft;
        private decimal _identToRight;
        private decimal _dissolutionLeft;
        private decimal _dissolutionRight;
        //private decimal _hightLeft;
        //private decimal _hightRight;

        [JsonProperty("wall_height")]
        /// <summary>
        /// Высота ребра
        /// </summary>
        public decimal Height
        {
            get => this._height;
            set
            {
                this._height = value;
                this.OnChangeProperty();
            }
        }

        [JsonProperty("wall_thickness")]
        /// <summary>
        /// Толщина ребра
        /// </summary>
        public decimal Thickness
        {
            get => this._thickness;
            set
            {
                this._thickness = value;
                this.OnChangeProperty();
            }
        }

        [JsonProperty("wall_cross_dist_left")]
        /// <summary>
        /// Поперечное расстояние до следующего ребра по левому краю
        /// </summary>
        public decimal DistanceLeft
        {
            get => this._distanceLeft;
            set
            {
                this._distanceLeft = value;
                this.OnChangeProperty();
            }
        }

        [JsonProperty("wall_cross_dist_right")]
        /// <summary>
        /// Поперечное расстояние до ребра по правому краю
        /// </summary>
        public decimal DistanceRight
        {
            get => this._distanceRight;
            set
            {
                this._distanceRight = value;
                this.OnChangeProperty();
            }
        }

        [JsonProperty("wall_long_dist_left")]
        /// <summary>
        /// Продольное расстояние до ребра по левому краю
        /// </summary>
        public decimal IdentToLeft
        {
            get => this._identToLeft;
            set
            {
                this._identToLeft = value;
                this.OnChangeProperty();
            }
        }

        [JsonProperty("wall_long_dist_right")]
        /// <summary>
        /// Продольное расстояние до ребра по правому краю
        /// </summary>
        public decimal IdentToRight
        {
            get => this._identToRight;
            set
            {
                this._identToRight = value;
                this.OnChangeProperty();
            }
        }

        [JsonProperty("weld_offset_left")]
        /// <summary>
        /// Отступ шва от левого края ребра
        /// </summary>
        public decimal DissolutionLeft
        {
            get => this._dissolutionLeft;
            set
            {
                this._dissolutionLeft = value;
                this.OnChangeProperty();
            }
        }

        [JsonProperty("weld_offset_right")]
        /// <summary>
        /// Отступ шва от правого края ребра
        /// </summary>
        public decimal DissolutionRight
        {
            get => this._dissolutionRight;
            set
            {
                this._dissolutionRight = value;
                this.OnChangeProperty();
            }
        }

        //[JsonProperty("h1")]
        ///// <summary>
        ///// высота ребра (общая или слева)
        ///// </summary>
        //public decimal HightLeft
        //{
        //    get => this._hightLeft;
        //    set
        //    {
        //        Set(ref this._hightLeft, value);
        //        this.ChangeHight?.Invoke(this, null);
        //    }
        //}

        //[JsonProperty("h2")]
        ///// <summary>
        ///// высота ребра справа
        ///// </summary>
        //public decimal HightRight { get => this._hightRight; set => Set(ref this._hightRight, value); }

        /// <summary>
        /// Событие изменения параметра детали
        /// </summary>
        public event PropertyChangedEventHandler ChangePropertyEvent;

        public Rib() { }

        /// <summary>
        /// Вызов события изменения свойства
        /// </summary>
        /// <param name="propertyName">Наименование свойства</param>
        public virtual void OnChangeProperty([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null) => this.ChangePropertyEvent?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public object Clone() => (Rib)this.MemberwiseClone();
    }
}
