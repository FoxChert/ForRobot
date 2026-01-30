using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using ForRobot.Models.Welding;

namespace ForRobot.Libr.Collections
{
    public class WeldCollcetion : ObservableCollection<Weld>
    {        
        #region Public variables

        /// <summary>
        /// Событие изменения свойства шва в коллекции
        /// </summary>
        public event EventHandler<WeldPropertyChangedEventArgs> WeldPropertyChanged;

        #endregion Public variables

        #region Constructors

        public WeldCollcetion() : base()
        { }

        public WeldCollcetion(List<Weld> list) : base(list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            foreach (var item in Items)
            {
                if (item != null)
                    item.PropertyChanged += OnWeldPropertyChanged;
            }
        }

        public WeldCollcetion(IEnumerable<Weld> enumerable) : base(enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            foreach (var item in Items)
            {
                if (item != null)
                    item.PropertyChanged += OnWeldPropertyChanged;
            }
        }

        public WeldCollcetion(int count) : this(GetCollection(count)) { }

        #endregion Constructors

        #region Private functions

        private static IEnumerable<Weld> GetCollection(int count)
        {
            List<Weld> welds = new List<Weld>();
            for(int i=0; i<count; i++)
            {
                welds.Add(new Weld());
            }
            return welds;
        }

        private void OnWeldPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var weld = (Weld)sender;
            this.WeldPropertyChanged?.Invoke(this, new WeldPropertyChangedEventArgs(weld, e.PropertyName));
        }

        #endregion Private functions

        #region Public functions

        protected override void InsertItem(int index, Weld item)
        {
            if (item != null)
                item.PropertyChanged += OnWeldPropertyChanged;

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];

            if (item != null)
                item.PropertyChanged -= OnWeldPropertyChanged;

            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] != null)
                    this[i].PropertyChanged -= OnWeldPropertyChanged;
            }
            base.ClearItems();
        }

        protected override void SetItem(int index, Weld item)
        {
            var oldItem = this[index];

            if (oldItem != null)
                oldItem.PropertyChanged -= OnWeldPropertyChanged;

            if (item != null)
                item.PropertyChanged += OnWeldPropertyChanged;

            base.SetItem(index, item);
        }

        public void SetCount(int count)
        {
            if (this == null || this.Count == 0)
                return;

            if (this.Count < count)
            {
                while (this.Count < count)
                {
                    this.Add(this.Last<Weld>().Clone() as Weld);
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

        #endregion Public functions
    }
}
