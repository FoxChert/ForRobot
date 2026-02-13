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
        //private string _name;
        private bool _isVisible = true;
        private bool _isSelected = false;
        private bool _worldTransformDirty = true;
        //private Transform3D _transform;
        private Material _originalMaterial; // Поле для запоминания оригенального материала.
        private HomogeneousMatrix _localTransform;
        private HomogeneousMatrix _cachedWorldTransform; // Кэшированная мировая матрица

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

        public double LocalPositionX { get; set; }
        public double LocalPositionY { get; set; }
        public double LocalPositionZ { get; set; }

        public double RotationX { get; set; }
        public double RotationY { get; set; }
        public double RotationZ { get; set; }

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
        public virtual Model3DGroup VisualModel { get; private set; }

        /// <summary>
        /// Визуальный элемент
        /// </summary>
        public virtual ModelVisual3D VisualElement => null;

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
            get => this.CalculateLocalTransform();
            set => _localTransform = value;
        }

        public HomogeneousMatrix WorldTransform
        {
            get
            {
                if (_worldTransformDirty)
                {
                    RecalculateWorldTransform();
                    _worldTransformDirty = false;
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

        private HomogeneousMatrix CalculateLocalTransform()
        {
            var transform = HomogeneousMatrix.Identity4x4();
            transform = transform * HomogeneousMatrix.Translation3D(LocalPositionX, LocalPositionY, LocalPositionZ);
            transform = transform * HomogeneousMatrix.Rotation3DAxisX(RotationX);
            transform = transform * HomogeneousMatrix.Rotation3DAxisY(RotationY);
            transform = transform * HomogeneousMatrix.Rotation3DAxisZ(RotationZ);
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

        //private HomogeneousMatrix CalculateWorldTransform()
        //{
        //    var world = this.LocalTransform.Clone();
        //    var currentParent = this.Parent;

        //    while (currentParent != null)
        //    {
        //        world = currentParent.LocalTransform * world;
        //        currentParent = currentParent.Parent;
        //    }
        //    return world as HomogeneousMatrix;
        //}

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

        /// <summary>
        /// Вызов события изменения свойства
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected virtual void OnTransformChanged(HomogeneousMatrix oldTransform, HomogeneousMatrix newTransform)
        {
            TransformChanged?.Invoke(this, new TransformChangedEventArgs(oldTransform, newTransform));

            // Уведомление дочерних элементов об изменении
            foreach (var child in this.Children)
            {
                child.InvalidateWorldTransform();
            }
        }

        //public abstract Model3DGroup GetModel();

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

        //public virtual void InvalidateWorldTransform() => this._worldTransform = null;

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
        
        public override string ToString() => this.GetType().ToString();
    }
}
