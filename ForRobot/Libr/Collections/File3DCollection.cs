using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

using ForRobot.Models.File3D;

namespace ForRobot.Libr.Collections
{
    public class File3DCollection : ObservableCollection<IFile3D>
    {
        public event EventHandler<FilePropertyChangedEventArgs> FilePropertyChanged;

        protected override void InsertItem(int index, IFile3D item)
        {
            base.InsertItem(index, item);
            if (item != null)
                item.PropertyChanged += OnFilePropertyChanged;
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            if (item != null)
                item.PropertyChanged -= OnFilePropertyChanged;
            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            foreach (var item in this)
            {
                if (item != null)
                    item.PropertyChanged -= OnFilePropertyChanged;
            }
            base.ClearItems();
        }

        protected override void SetItem(int index, IFile3D item)
        {
            var oldItem = this[index];
            if (oldItem != null)
                oldItem.PropertyChanged -= OnFilePropertyChanged;

            base.SetItem(index, item);

            if (item != null)
                item.PropertyChanged += OnFilePropertyChanged;
        }

        private void OnFilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var file = (IFile3D)sender;
            FilePropertyChanged?.Invoke(this, new FilePropertyChangedEventArgs(file, e.PropertyName));
        }

        public void Add(string path)
        {
            var file = File3D.Load(path);

            if (file != null)
                file.PropertyChanged += OnFilePropertyChanged;

            base.Add(file);
        }

        public void SaveAll()
        {
            foreach(var item in this)
            {
                item.Save();
            }
        }
    }
}
