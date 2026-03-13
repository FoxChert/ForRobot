using System;
using System.Windows;
using System.ComponentModel;

using ForRobot.Models.Settings;

namespace ForRobot.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для PropertiesWindow.xaml
    /// </summary>
    public partial class PropertiesWindow : Window, INotifyPropertyChanged, IDisposable
    {
        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register(nameof(Settings),
                                                                                                 typeof(Settings),
                                                                                                 typeof(PropertiesWindow),
                                                                                                 new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSettingsChanged));

        public Settings Settings { get; set; }

        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public PropertiesWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Вызов события изменения свойства
        /// </summary>
        /// <param name="propertyName">Наименование свойства</param>
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private static void OnSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PropertiesWindow)d;
            control.Settings = e.NewValue as Settings;
            control.OnPropertyChanged(nameof(control.Settings));
        }

        private void BtnDefaultSettings_Click(object sender, RoutedEventArgs e)
        {
            ForRobot.Themes.Colors.DefaultColors();
            this.Settings = new Settings();
        }

        private void BtnDialogOk_Click(object sender, RoutedEventArgs e) => this.DialogResult = true;

        #region IDisposable Support

        ~PropertiesWindow() => Dispose();

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
