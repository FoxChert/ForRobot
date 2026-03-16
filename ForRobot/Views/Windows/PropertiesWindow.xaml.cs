using System;
using System.Windows;
using System.ComponentModel;

using ForRobot.Models.Settings;

namespace ForRobot.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для PropertiesWindow.xaml
    /// </summary>
    public partial class PropertiesWindow : Window, IDisposable
    {
        public PropertiesWindow()
        {
            InitializeComponent();
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
