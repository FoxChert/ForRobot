using System;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;

using ForRobot.Libr.Collections;

namespace ForRobot.Models
{
    /// <summary>
    /// Класс коллекции <see cref="SceneItem"/>
    /// </summary>
    public class SceneItemCollection : ObservableCollection<SceneItem>
    {
        protected override void InsertItem(int index, SceneItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, SceneItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            base.SetItem(index, item);
        }
    }

    public interface ISceneItem
    {
        string Name { get; }

        bool IsVisible { get; set; }
        bool IsSelected { get; set; }

        Model3DGroup VisualModel { get; }

        //Type ObjectType { get; }

        //Model3DGroup GetModel();

        //void UpdateTransform(Matrix3D transform);
    }

    public class TransformChangedEventArgs : EventArgs
    {
        public HomogeneousMatrix OldTransform { get; }
        public HomogeneousMatrix NewTransform { get; }

        public TransformChangedEventArgs(HomogeneousMatrix old, HomogeneousMatrix newTransform)
        {
            OldTransform = old;
            NewTransform = newTransform;
        }
    }

    /// <summary>
    /// Абстрактный класс элемента 3д сцены
    /// </summary>
    public abstract class SceneItem : DependencyObject, ISceneItem
    {
        #region Private variables

        //private string _name;
        private bool _isVisible = true;
        private bool _isSelected = false;
        private bool _transformDirty = true;
        //private double[] _XYZ = new double[3] { 0.0, 0.0, 0.0 };
        //private Transform3D _transform;
        private Material _originalMaterial; // Поле для запоминания оригенального материала.
        private HomogeneousMatrix _localTransform;
        private HomogeneousMatrix _cachedWorldTransform; // Кэшированная мировая матрица

        #endregion

        #region Public variables

        public static readonly Material TransparentMaterial = new DiffuseMaterial(System.Windows.Media.Brushes.Transparent);

        public virtual string Name { get; }

        public bool IsVisible
        {
            get => this._isVisible;
            set
            {
                this._isVisible = value;
                this.UpdateVisibility(this);
                this.OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => this._isSelected;
            set
            {
                this._isSelected = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Смещение от точки 0.0 по осям XYZ
        /// </summary>
        public virtual double[] XYZ { get; } = new double[3] { 0.0, 0.0, 0.0 };

        /// <summary>
        /// Смещение от точки 0.0 по оси X
        /// </summary>
        public virtual double LocalPositionX
        {
            get => this.XYZ[0];
            set
            {
                this.XYZ[0] = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.XYZ));
            }
        }
        /// <summary>
        /// Смещение от точки 0.0 по оси Y
        /// </summary>
        public virtual double LocalPositionY
        {
            get => this.XYZ[1];
            set
            {
                this.XYZ[1] = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.XYZ));
            }
        }
        /// <summary>
        /// Смещение от точки 0.0 по оси Z
        /// </summary>
        public virtual double LocalPositionZ
        {
            get => this.XYZ[2];
            set
            {
                this.XYZ[2] = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.XYZ));
            }
        }

        public virtual double[] ABC { get; } = new double[3] { 0.0, 0.0, 0.0 };
        
        /// <summary>
        /// Вращение по оси X
        /// </summary>
        public virtual double RotationX // A
        {
            get => this.ABC[0];
            set
            {
                this.ABC[0] = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.ABC));
            }
        }
        /// <summary>
        /// Вращение по оси Y
        /// </summary>
        public virtual double RotationY // B
        {
            get => this.ABC[1];
            set
            {
                this.ABC[1] = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.ABC));
            }
        }
        /// <summary>
        /// Вращение по оси Z
        /// </summary>
        public virtual double RotationZ // C
        {
            get => this.ABC[2];
            set
            {
                this.ABC[2] = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.ABC));
            }
        }

        //public Transform3D Transform
        //{
        //    get => this._transform;
        //    set
        //    {
        //        this._transform = value;
        //        this.OnPropertyChanged();
        //    }
        //}

        /// <summary>
        /// Визуальная модель элемента сцены
        /// </summary>
        public virtual Model3DGroup VisualModel { get; protected set; }

        ///// <summary>
        ///// Визуальный элемент
        ///// </summary>
        //public virtual ModelVisual3D VisualElement => null;

        /// <summary>
        /// Дочерние элементы сцены
        /// </summary>
        public SceneItemCollection Children { get; }

        /// <summary>
        /// Родительский элемент
        /// </summary>
        public SceneItem Parent { get; private set; }
        
        public HomogeneousMatrix LocalTransform
        {
            get => this._localTransform ?? (this._localTransform = HomogeneousMatrix.Identity4x4());
            set
            {
                var oldTransform = _localTransform?.Clone() ?? HomogeneousMatrix.Identity4x4();
                this._localTransform = value;
                this.OnTransformChanged(oldTransform, this._localTransform);
            }
        }
        
        public HomogeneousMatrix WorldTransform
        {
            get
            {
                if (_transformDirty)
                {
                    RecalculateWorldTransform();
                    _transformDirty = false;
                }
                return _cachedWorldTransform;
            }
        }

        //public IList<DependencyObject> Children { get; }

        public SceneItem this[int index] { get => this.Children[index]; set => this.Children[index] = value; }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<TransformChangedEventArgs> TransformChanged;

        #endregion Public variables

        public SceneItem()
        {
            this.Name = this.GetType().Name;
            this.Children = new SceneItemCollection();
            this.AddChildren(this);
        }

        #region Private functions
        
        private HomogeneousMatrix CalculateLocalTransform()
        {
            var transform = HomogeneousMatrix.Identity4x4();
            transform *= HomogeneousMatrix.Translation3D(LocalPositionX, LocalPositionY, LocalPositionZ);
            transform *= HomogeneousMatrix.Rotation3DAxisX(RotationX);
            transform *= HomogeneousMatrix.Rotation3DAxisY(RotationY);
            transform *= HomogeneousMatrix.Rotation3DAxisZ(RotationZ);
            return transform;
        }

        private void RecalculateWorldTransform()
        {
            _cachedWorldTransform = this.LocalTransform.Clone();

            var parent = this.Parent;
            while (parent != null)
            {
                _cachedWorldTransform = parent.LocalTransform * _cachedWorldTransform;
                parent = parent.Parent;
            }
        }

        private void InvalidateTransform()
        {
            _transformDirty = true;
            UpdateVisualModel();
        }

        private void AddChildren(object element)
        {
            switch (element)
            {
                //case Model3DGroup model3DGroup:
                //    //foreach (var item in model3DGroup.Children)
                //    //{
                //    //    this.Children.Add(item as SceneItem);
                //    //}
                //    break;

                case GeometryModel3D geometryModel3D:
                    this._originalMaterial = geometryModel3D.Material;
                    break;

                default:
                    foreach (DependencyObject item in LogicalTreeHelper.GetChildren(this))
                    {
                        this.Children.Add(item as SceneItem);
                    }
                    break;
            }
        }

        #endregion Private functions

        #region Public functions
        
        /// <summary>
        /// Вызов события изменения свойства
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected virtual void OnTransformChanged(HomogeneousMatrix oldTransform, HomogeneousMatrix newTransform)
        {
            _transformDirty = true;

            UpdateVisualModel();
            
            TransformChanged?.Invoke(this, new TransformChangedEventArgs(oldTransform, newTransform));

            // Уведомление дочерних элементов об изменении
            foreach (var child in this.Children)
            {
                child.InvalidateTransform();
            }
        }

        protected virtual void UpdateVisualModel()
        {
            if (this.VisualModel != null)
            {
                var matrix = ForRobot.Libr.Converters.MatrixConverter.MatrixToMatrix3D(this.WorldTransform);
                ForRobot.Libr.Modeling.Model3DExtensions.ApplyTransformToModel(this.VisualModel, matrix);
            }
        }

        /// <summary>
        /// Обновление видимости элемента и его потомков
        /// </summary>
        protected virtual void UpdateVisibility(object element)
        {
            switch (element)
            {
                case GeometryModel3D geometryModel3D:
                    geometryModel3D.Material = this.IsVisible ? this._originalMaterial : TransparentMaterial;
                    break;

                default:
                    return;
            }
        }
        
        public abstract void UpdateTransform(Matrix3D transform);

        /// <summary>
        /// Добавение дочернего элемента
        /// </summary>
        /// <param name="child"></param>
        public virtual void AddChild(SceneItem child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            if (child.Parent != null)
                child.Parent.RemoveChild(child);

            this.Children.Add(child);
            child.Parent = this;
            this.OnPropertyChanged(nameof(Children));
        }

        /// <summary>
        /// Удаление дочерего элемента
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public virtual bool RemoveChild(SceneItem child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            bool removed = this.Children.Remove(child);
            if (removed)
            {
                child.Parent = null;
                this.OnPropertyChanged(nameof(Children));
            }
            return removed;
        }

        /// <summary>
        /// Очищение всех дочерних элементов
        /// </summary>
        public virtual void ClearChildren()
        {
            foreach (var child in this.Children)
            {
                child.Parent = null;
            }
            this.Children.Clear();
            this.OnPropertyChanged(nameof(Children));
        }
       
        public override string ToString() => this.GetType().ToString();

        public static IEnumerable<MeshGeometry3D> ExtractMeshes(Model3DGroup group)
        {
            foreach (var model in group.Children)
            {
                if (model is Model3DGroup subGroup)
                {
                    foreach (var mesh in ExtractMeshes(subGroup))
                        yield return mesh;
                }
                else if (model is GeometryModel3D geomModel)
                {
                    if (geomModel.Geometry is MeshGeometry3D mesh)
                        yield return mesh;
                }
            }
        }

        #endregion Public functions
    }
}
