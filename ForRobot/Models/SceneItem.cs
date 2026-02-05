using System;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Collections;

namespace ForRobot.Models
{
    /// <summary>
    /// Класс коллекции <see cref="SceneItem"/>
    /// </summary>
    public class SceneItemCollection : IList, ICollection, ICollection<SceneItem>
    {
        public object this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsReadOnly => throw new NotImplementedException();

        public bool IsFixedSize => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Add(SceneItem item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public bool Contains(SceneItem item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(SceneItem[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(SceneItem item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator<SceneItem> IEnumerable<SceneItem>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Абстрактный класс элемента 3д сцены
    /// </summary>
    public abstract class SceneItem : DependencyObject, ISceneItem
    {
        public string Name { get; }

        public ICollection<DependencyObject> Children { get; }

        public SceneItem()
        {
            this.Name = this.GetType().Name;
            this.AddChildren(this);
        }

        public abstract Model3DGroup GetModel();

        private void AddChildren(object element)
        {
            switch (element)
            {
                case Model3DGroup model3DGroup:
                    foreach (var item in model3DGroup.Children)
                    {
                        this.Children.Add(item);
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
    }
}
