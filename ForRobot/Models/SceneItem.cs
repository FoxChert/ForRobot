using System;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;

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

    /// <summary>
    /// Абстрактный класс элемента 3д сцены
    /// </summary>
    public abstract class SceneItem : DependencyObject, ISceneItem
    {
        private string _name;
        private bool _isVisible = true;
        private bool _isSelected = false;
        private Transform3D _transform;

        public string Name { get; }

        public bool ISVisible
        {
            get => this._isVisible;
            set
            {
                this._isVisible = value;
                this.UpdateVisibility();
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

        public Transform3D Transform
        {
            get => this._transform;
            set
            {
                this._transform = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Визуальная модель элемента сцены
        /// </summary>
        public virtual Model3D VisualModel => null;

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

        //public IList<DependencyObject> Children { get; }

        public SceneItem this[int index] { get => this.Children[index]; set => this.Children[index] = value; }

        public event PropertyChangedEventHandler PropertyChanged;

        public SceneItem()
        {
            this.Name = this.GetType().Name;
            this.Children = new SceneItemCollection();
            this.AddChildren(this);
        }

        private void AddChildren(object element)
        {
            switch (element)
            {
                case Model3DGroup model3DGroup:
                    foreach (var item in model3DGroup.Children)
                    {
                        this.Children.Add(item as SceneItem);
                    }
                    break;

                case GeometryModel3D geometryModel3D:
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

        /// <summary>
        /// Обновление видимости элемента и его потомков
        /// </summary>
        protected virtual void UpdateVisibility()
        {

        }
        
        public override string ToString() => this.GetType().ToString();
    }
}
