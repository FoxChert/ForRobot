using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using HelixToolkit.Wpf;
using GalaSoft.MvvmLight.Messaging;

using ForRobot.Models.Welding;
using ForRobot.Models.Detals;

namespace ForRobot.Libr.Behavior
{
    public class WeldVisual : ScreenSpaceVisual3D
    {
        #region Private variables

        private readonly LinesVisual3D _line1;
        private readonly LinesVisual3D _line2;
        private Point3D _startPoint;
        private Point3D _endPoint;
        private Point3D _centerPoint;
        private Color _color;
        private Color _leftLineColor;
        private Color _rightLineColor;
        private double _thickness = 2.0;
        private bool _isDivided = false;
        
        #endregion Private variables

        #region Public variables

        public string Name { get; set; }

        /// <summary>
        /// Общий цвет шва
        /// </summary>
        public new Color Color
        {
            get => this._color;
            set
            {
                this._color = value;
                this.UpdateColor();
            }
        }
        /// <summary>
        /// Цвет левой полавины шва
        /// </summary>
        public Color LeftLineColor
        {
            get => this._leftLineColor;
            set
            {
                this._leftLineColor = value;
                this.UpdateColor();
            }
        }
        /// <summary>
        /// Цвет правой полавины шва
        /// </summary>
        public Color RightLineColor
        {
            get => this._rightLineColor;
            set
            {
                this._rightLineColor = value;
                this.UpdateColor();
            }
        }
        
        /// <summary>
        /// Точка начала шва
        /// </summary>
        public Point3D StartPoint
        {
            get => this._startPoint;
            set
            {
                this._startPoint = value;
                this.UpdateCenterPoint();
                this.UpdateGeometry();
            }
        }

        /// <summary>
        /// Точка конца шва
        /// </summary>
        public Point3D EndPoint
        {
            get => this._endPoint;
            set
            {
                this._endPoint = value;
                this.UpdateCenterPoint();
                this.UpdateGeometry();
            }
        }

        /// <summary>
        /// Центральная точка, где сходятся два сегмента
        /// </summary>
        public Point3D CenterPoint
        {
            get => this._centerPoint;
            set
            {
                this._centerPoint = value;
                this.UpdateGeometry();
            }
        }

        /// <summary>
        /// Толщина линии шва
        /// </summary>
        public double Thickness
        {
            get => this._thickness;
            set
            {
                this._thickness = value;
                if (this._line1 != null) this._line1.Thickness = this._thickness;
                if (this._line2 != null) this._line2.Thickness = this._thickness;
            }
        }

        /// <summary>
        /// Разделён ли шов по цвету попалам
        /// </summary>
        public bool IsDivided
        {
            get => this._isDivided;
            set
            {
                this._isDivided = value;
                this.UpdateColor();
            }
        }

        #endregion Public variables

        #region Constructors

        public WeldVisual()
        {
            this._line1 = new LinesVisual3D()
            {
                Thickness = this.Thickness
            };

            this._line2 = new LinesVisual3D()
            {
                Thickness = this.Thickness
            };

            Children.Add(this._line1);
            Children.Add(this._line2);
        }

        public WeldVisual(Color color, Color? leftLineColor, Color? rightLineColor) : this()
        {
            this.Color = color;
            this.LeftLineColor = leftLineColor ?? this.Color;
            this.RightLineColor = rightLineColor ?? this.Color;
        }

        #endregion Constructors

        protected override bool UpdateTransforms() => true;

        protected override void UpdateGeometry()
        {
            if (this._line1 == null || this._line2 == null) return;

            this._line1.Points = new Point3DCollection { this.StartPoint, this.CenterPoint };

            this._line2.Points = new Point3DCollection { this.CenterPoint, this.EndPoint };
        }

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

        /// <summary>
        /// Обновление цвета двух сегментов шва
        /// </summary>
        private void UpdateColor()
        {
            if (this.IsDivided)
            {
                this._line1.Color = this.LeftLineColor;
                this._line2.Color = this.RightLineColor;
            }
            else
            {
                this._line1.Color = this.Color;
                this._line2.Color = this.Color;
            }
        }
    }

    public class HelixWeldsBehavior<T> : HelixAddCollectionBehavior<WeldVisual>
    {
        //public Detal Detal
        //{
        //    get => (Detal)GetValue(DetalProperty);
        //    set => SetValue(DetalProperty, value);
        //}

        //public static readonly DependencyProperty DetalProperty = DependencyProperty.Register(nameof(Detal),
        //                                                                                      typeof(Detal),
        //                                                                                      typeof(HelixWeldsBehavior),
        //                                                                                      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDetalChanged));

        public IEnumerable<T> ItemsSource { get; set; }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource),
                                                                                                    typeof(IEnumerable<T>),
                                                                                                    typeof(HelixWeldsBehavior<T>),
                                                                                                    new PropertyMetadata(null, OnItemsSourceChanged));

        public double Thickness
        {
            get => (double)GetValue(ThicknessProperty);
            set => SetValue(ThicknessProperty, value);
        }

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(nameof(Thickness),
                                                                                                  typeof(double),
                                                                                                  typeof(HelixWeldsBehavior<T>),
                                                                                                  new PropertyMetadata(Services.WeldService.DEFAULT_WELD_THICKNESS, OnThicknessChanged));

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color),
                                                                                              typeof(Color),
                                                                                              typeof(HelixWeldsBehavior<T>),
                                                                                              new PropertyMetadata(Services.WeldService.DEFAULT_WELD_THICKNESS, OnThicknessChanged));

        public bool IsDivided
        {
            get => (bool)GetValue(IsDividedProperty);
            set => SetValue(IsDividedProperty, value);
        }

        public static readonly DependencyProperty IsDividedProperty = DependencyProperty.Register(nameof(IsDivided),
                                                                                                  typeof(bool),
                                                                                                  typeof(HelixWeldsBehavior<T>),
                                                                                                  new PropertyMetadata(false, OnIsDividedChanged));

        public HelixWeldsBehavior()
        {
            //Messenger.Default.Register<ForRobot.Libr.Messages.UpdateCurrentDetalMessage>(this, message =>
            //{
            //    this.Detal = message.Detal;
            //});
        }

        #region Static functions

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HelixWeldsBehavior<T> helixWeldsBehavior = (HelixWeldsBehavior<T>)d;

            if (helixWeldsBehavior.Items != null)
                foreach (var item in helixWeldsBehavior.Items) item.Children.Clear();
                       
            helixWeldsBehavior.ItemsSource = (IEnumerable<T>)e.NewValue;
        }

        //private static void OnDetalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    HelixWeldsBehavior helixWeldsBehavior = (HelixWeldsBehavior)d;

        //    if (e.OldValue != null && e.OldValue is Detal oldDetal)
        //        oldDetal.PropertyChanged -= helixWeldsBehavior.PropertyChangeHandle;

        //    if (helixWeldsBehavior.Items != null && helixWeldsBehavior.Items is ObservableCollection<Weld> currentCollection)
        //        foreach (var item in currentCollection) item.Children.Clear();

        //    helixWeldsBehavior.Detal = (Detal)e.NewValue;

        //    if (helixWeldsBehavior.Detal == null)
        //    {
        //        if (helixWeldsBehavior.Items is ObservableCollection<Weld> weldsCollection)
        //            weldsCollection.Clear();
        //    }
        //    else
        //    {
        //        helixWeldsBehavior.Detal.PropertyChanged += helixWeldsBehavior.PropertyChangeHandle;
        //        helixWeldsBehavior.UpdateWelds();
        //    }
        //}

        private static void OnThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HelixWeldsBehavior<T> helixWeldsBehavior = (HelixWeldsBehavior<T>)d;
            helixWeldsBehavior.Thickness = (double)e.NewValue;
            helixWeldsBehavior.UpdateWeldsThickness();
        }

        private static void OnIsDividedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HelixWeldsBehavior<T> helixWeldsBehavior = (HelixWeldsBehavior<T>)d;
            helixWeldsBehavior.IsDivided = (bool)e.NewValue;
            helixWeldsBehavior.UpdateWeldsIsDivided();
        }

        #endregion Static functions

        private void PropertyChangeHandle(object sender, PropertyChangedEventArgs e) => this.UpdateWelds();

        private void UpdateWelds()
        {
            //ForRobot.Libr.Services.IWeldService weldService = new ForRobot.Libr.Services.WeldService(ForRobot.Models.Settings.Settings.ScaleFactor);

            //if (this.Detal == null)
            //    return;

            //var welds = weldService.GetWelds(this.Detal);

            //if (this.Items is ObservableCollection<Weld> currentCollection)
            //{
            //    currentCollection.Clear();
            //    foreach (var weld in welds)
            //    {
            //        this.UpdateWeldProperties(weld);
            //        currentCollection.Add(weld);
            //    }
            //}
            //else
            //{
            //    foreach (var item in welds)
            //        this.UpdateWeldProperties(item);

            //    this.Items = welds;
            //}
        }

        private void UpdateWeldsThickness()
        {
            if (!(this.Items is IEnumerable<WeldVisual> currentCollection) || currentCollection == null)
                return;

            foreach (var item in currentCollection)
                item.Thickness = this.Thickness;
        }

        private void UpdateWeldsIsDivided()
        {
            if (!(this.Items is IEnumerable<WeldVisual> currentCollection) || currentCollection == null)
                return;

            foreach (var item in currentCollection)
                item.IsDivided = this.IsDivided;
        }

        /// <summary>
        /// Обновление свойств объекта класса <see cref="Weld"/>
        /// </summary>
        /// <param name="weld"></param>
        private void UpdateWeldProperties(WeldVisual weld)
        {
            weld.Thickness = this.Thickness;
            weld.IsDivided = this.IsDivided;
        }

        //~HelixWeldsBehavior()
        //{
        //    Messenger.Default.Unregister<ForRobot.Libr.Messages.UpdateCurrentDetalMessage>(this);
        //}
    }
}
