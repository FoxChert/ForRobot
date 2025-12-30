using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using ForRobot.Models.Detals;

namespace ForRobot.Libr.Collections
{
    /// <summary>
    /// Коллекция ребер с поддержкой уведомлений об изменениях свойств отдельных элементов
    /// </summary>
    public class RibCollection : ObservableCollection<Rib>
    {

        #region Public variables

        /// <summary>
        /// Событие изменения свойства любого ребра в коллекции
        /// </summary>
        public event EventHandler<RibPropertyChangedEventArgs> RibPropertyChanged;

        #endregion Public variables

        #region Constructors

        public RibCollection() : base()
        { }

        public RibCollection(List<Rib> list) : base(list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            foreach (var item in Items)
            {
                if (item != null)
                    item.PropertyChanged += OnRibPropertyChanged;
            }
        }

        public RibCollection(IEnumerable<Rib> enumerable) : base(enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            foreach (var item in Items)
            {
                if (item != null)
                    item.PropertyChanged += OnRibPropertyChanged;
            }
        }

        #endregion

        #region Private functions

        private void OnRibPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var rib = (Rib)sender;
            RibPropertyChanged?.Invoke(this, new RibPropertyChangedEventArgs(rib, e.PropertyName));
        }

        protected override void InsertItem(int index, Rib item)
        {
            if (item != null)
                item.PropertyChanged += OnRibPropertyChanged;

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];

            if (item != null)
                item.PropertyChanged -= OnRibPropertyChanged;

            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] != null)
                    this[i].PropertyChanged -= OnRibPropertyChanged;
            }
            base.ClearItems();
        }

        protected override void SetItem(int index, Rib item)
        {
            var oldItem = this[index];

            if (oldItem != null)
                oldItem.PropertyChanged -= OnRibPropertyChanged;

            if (item != null)
                item.PropertyChanged += OnRibPropertyChanged;

            base.SetItem(index, item);
        }

        #endregion

        #region Public functions

        public void SetCount(int count)
        {
            if (this == null || this.Count == 0)
                return;

            if (this.Count < count)
            {
                while (this.Count < count)
                {
                    this.Add(this.Last<Rib>().Clone() as Rib);
                }
            }
            else
            {
                while (this.Count > count)
                {
                    this.RemoveItem(this.Count - 1);
                }
            }
        }

        public void SetWeldsDissolutionLeft(decimal dissolutionLeft)
        {
            for (int i = 0; i < this?.Count; i++)
            {
                this[i].DissolutionLeft = dissolutionLeft;
            }
        }

        public void SetWeldsDissolutionRight(decimal dissolutionRight)
        {
            for (int i = 0; i < this?.Count; i++)
            {
                this[i].DissolutionRight = dissolutionRight;
            }
        }

        public void SetRibsDistance(decimal distanceBetweenRibs)
        {
            for (int i = 0; i < this?.Count; i++)
            {
                this[i].DistanceLeft = distanceBetweenRibs;
                this[i].DistanceRight = distanceBetweenRibs;
            }
        }

        public void SetRibsHeight(decimal ribsHeight)
        {
            for (int i = 0; i < this?.Count; i++)
            {
                this[i].Height = ribsHeight;
            }
        }

        public void SetRibsThickness(decimal ribsThickness)
        {
            for (int i = 0; i < this?.Count; i++)
            {
                this[i].Thickness = ribsThickness;
            }
        }

        public void SetRibsIdentToLeft(decimal ribsIdentToLeft)
        {
            for (int i = 0; i < this?.Count; i++)
            {
                this[i].IdentToLeft = ribsIdentToLeft;
            }
        }

        public void SetRibsIdentToRight(decimal ribsIdentToRight)
        {
            for (int i = 0; i < this?.Count; i++)
            {
                this[i].IdentToRight = ribsIdentToRight;
            }
        }

        #endregion Public functions
    }
}
