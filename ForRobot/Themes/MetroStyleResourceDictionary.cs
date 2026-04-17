using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ForRobot.Themes
{
    public partial class MetroStyleResourceDictionary
    {
        /// <summary>
        /// Метод выделения содержимого TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
                textBox.SelectAll();

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new Libr.Messages.ProperteisNameMessage(textBox.Tag as string));
        }

        /// <summary>
        /// Метод потери фокуса <see cref="TextBox"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new Libr.Messages.ProperteisNameMessage(string.Empty)); // Отправка сообщения для изменения аннотации в HelixAnnotationsBehavior.
        }

        /// <summary>
        /// Метод выделения содержимого <see cref="TextBox"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null && !textBox.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                textBox.Focus();
            }
        }

        /// <summary>
        /// Метод изменения размера <see cref="DataGrid"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                dataGrid.UpdateLayout();
                foreach (var column in dataGrid.Columns)
                {
                    if (column.Width.IsStar)
                    {
                        column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик события закрытия/открытия содержимого <see cref="Expander"/>, т.е. сведений строки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Expander_ExpandedOrCollapsed(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
        }
    }
}
