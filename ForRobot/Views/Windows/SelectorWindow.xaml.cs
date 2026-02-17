using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace ForRobot.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для SelectorWindow.xaml
    /// </summary>
    public partial class SelectorWindow : Window, INotifyPropertyChanged, IDisposable
    {
        #region Public variables

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource),
                                                                                                    typeof(IEnumerable),
                                                                                                    typeof(SelectorWindow),
                                                                                                    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnItemsSourceChanged));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(nameof(SelectedItems),
                                                                                                      typeof(IEnumerable),
                                                                                                      typeof(SelectorWindow),
                                                                                                      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsChanged));

        public IEnumerable SelectedItems
        {
            get => this.SelectedListBox.SelectedItems;
            set => this.SelectedListBox.SelectedItem = value;
        }

        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(nameof(SelectionMode),
                                                                                                      typeof(SelectionMode),
                                                                                                      typeof(SelectorWindow),
                                                                                                      new FrameworkPropertyMetadata(SelectionMode.Single, OnSelectionModeChanged));

        public SelectionMode SelectionMode
        {
            get => this.SelectedListBox.SelectionMode;
            set => this.SelectedListBox.SelectionMode = value;
        }

        public static readonly DependencyProperty CanDeleteItemsProperty = DependencyProperty.Register(nameof(CanDeleteItems),
                                                                                                       typeof(bool),
                                                                                                       typeof(SelectorWindow),
                                                                                                       new FrameworkPropertyMetadata(false));

        public bool CanDeleteItems
        {
            get { return (bool)GetValue(CanDeleteItemsProperty); }
            set { SetValue(CanDeleteItemsProperty, value); }
        }

        public static readonly DependencyProperty CanAddItemsProperty = DependencyProperty.Register(nameof(CanAddItems),
                                                                                                    typeof(bool),
                                                                                                    typeof(SelectorWindow),
                                                                                                    new FrameworkPropertyMetadata(false));

        public bool CanAddItems
        {
            get { return (bool)GetValue(CanAddItemsProperty); }
            set { SetValue(CanAddItemsProperty, value); }
        }

        public EventHandler ItemsSourceChangedEvent;

        #endregion Public variables

        #region Constructs

        public SelectorWindow()
        {
            InitializeComponent();
            this.Closed += (a, b) => this.Dispose();
        }

        public SelectorWindow(IEnumerable itemsSource, IEnumerable selectedItems = null, ResourceDictionary resource = null) : this()
        {
            this.ItemsSource = itemsSource;
            this.SelectedItems = selectedItems;
        }

        #endregion Construct

        #region Private functions

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SelectorWindow)d;
            control.SelectedListBox.ItemsSource = e.NewValue as IEnumerable;
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SelectorWindow)d;
            control.UpdateSelectedItems();
        }

        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SelectorWindow)d;
            control.SelectionMode = (SelectionMode)e.NewValue;
            control.SelectedListBox.SelectionMode = control.SelectionMode;
        }

        private void UpdateSelectedItems()
        {
            if (SelectedItems == null) return;

            SelectedListBox.SelectedItems.Clear();
            foreach (var item in SelectedItems)
            {
                SelectedListBox.SelectedItems.Add(item);
            }
        }

        private void BtnDialogOk_Click(object sender, RoutedEventArgs e) => this.DialogResult = true;

        private void BtnDeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            //if (!this.CanDeleteItems)
            //    return;

            //(this.ItemsSource as List<object>).Remove(((e.OriginalSource as System.Windows.Controls.Button).TemplatedParent as ContentPresenter).Content);

            if (!CanDeleteItems || ItemsSource == null)
                return;
            
            var itemsList = ItemsSource as IList;
            if (itemsList == null)
                return; // Удаление невозможно: источник данных не поддерживает удаление.

            if (!(sender is FrameworkElement element))
                return;

            object itemToRemove = element.DataContext;

            if (itemToRemove != null && itemsList.Contains(itemToRemove))
            {
                itemsList.Remove(itemToRemove);
            }
        }

        #endregion Private functions

        #region IDisposable Support

        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing) { }
                _disposedValue = true;
            }
        }

        ~SelectorWindow() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
