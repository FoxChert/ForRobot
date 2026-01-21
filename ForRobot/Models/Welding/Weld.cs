using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.ComponentModel;

using Newtonsoft.Json;

using ForRobot.Models.File3D;

namespace ForRobot.Models.Welding
{
    /// <summary>
    /// Модель представления шва
    /// </summary>
    public class Weld : INotifyPropertyChanged
    {
        #region Private variables

        private decimal _dissolutionLeft;
        private decimal _dissolutionRight;

        #endregion Private variables

        #region Public variables

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

        ///// <summary>
        ///// Точка начала шва
        ///// </summary>
        //public Point3D StartPoint
        //{
        //    get => this._startPoint;
        //    set
        //    {
        //        this._startPoint = value;
        //        this.UpdateCenterPoint();
        //        this.UpdateGeometry();
        //    }
        //}

        ///// <summary>
        ///// Точка конца шва
        ///// </summary>
        //public Point3D EndPoint
        //{
        //    get => this._endPoint;
        //    set
        //    {
        //        this._endPoint = value;
        //        this.UpdateCenterPoint();
        //        this.UpdateGeometry();
        //    }
        //}

        ///// <summary>
        ///// Центральная точка, где сходятся два сегмента
        ///// </summary>
        //public Point3D CenterPoint
        //{
        //    get => this._centerPoint;
        //    set
        //    {
        //        this._centerPoint = value;
        //        this.UpdateGeometry();
        //    }
        //}

        [JsonIgnore]
        /// <summary>
        /// Точка начала шва
        /// </summary>
        public Point3D StartPoint { get; set; }

        [JsonIgnore]
        /// <summary>
        /// Точка конца шва
        /// </summary>
        public Point3D EndPoint { get; set; }

        [JsonIgnore]
        /// <summary>
        /// Центральная точка, где сходятся два сегмента
        /// </summary>
        public Point3D CenterPoint { get; set; }

        ///// <summary>
        ///// Толщина линии шва
        ///// </summary>
        //public double Thickness
        //{
        //    get => this._thickness;
        //    set
        //    {
        //        this._thickness = value;
        //        if (this._line1 != null) this._line1.Thickness = this._thickness;
        //        if (this._line2 != null) this._line2.Thickness = this._thickness;
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public variables

        #region Constructor

        public Weld() { }

        #endregion

        /// <summary>
        /// Вызов события изменения свойства
        /// </summary>
        /// <param name="propertyName">Наименование свойства</param>
        private void OnChangeProperty([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Обновение центральной точки как середины между StartPoint и EndPoint
        /// </summary>
        private void UpdateCenterPoint()
        {
            if (this.StartPoint != null && this.EndPoint != null)
            {
                this.CenterPoint = new Point3D(
                    (this.StartPoint.X + this.EndPoint.X) / 2,
                    (this.StartPoint.Y + this.EndPoint.Y) / 2,
                    (this.StartPoint.Z + this.EndPoint.Z) / 2
                );
            }
        }
    }
}
