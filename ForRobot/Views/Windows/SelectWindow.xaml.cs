using System;
using System.Windows;
using System.Collections;
using System.ComponentModel;

namespace ForRobot.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для SelectWindow.xaml
    /// </summary>
    public partial class SelectWindow : Window, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource),
                                                                                                    typeof(IEnumerable),
                                                                                                    typeof(SelectWindow),
                                                                                                    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnItemsSourceChanged));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(nameof(SelectedItem),
                                                                                              typeof(IEnumerable),
                                                                                              typeof(SelectorWindow),
                                                                                              new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsChanged));

        public object SelectedItem
        {
            get => GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public SelectWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Вызов события изменения свойства
        /// </summary>
        /// <param name="propertyName">Наименование свойства</param>
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SelectWindow)d;
            //control.SelectedComboBox.ItemsSource = e.NewValue as IEnumerable;
            control.OnPropertyChanged(nameof(control.ItemsSource));
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SelectWindow)d;
            control.OnPropertyChanged(nameof(control.SelectedItem));
        }
    }
}
