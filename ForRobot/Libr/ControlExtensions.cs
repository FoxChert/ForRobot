using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace ForRobot.Libr
{
    /// <summary>
    /// Класс, расширяющий тип <see cref="Control"/>
    /// </summary>
    public static class ControlExtensions
    {
        public static void ReversValue(this Control control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            switch (control)
            {
                //case CheckBox checkBox when checkBox.ToString() == control.ToString():
                //    break;

                case CheckBox checkBox:
                    checkBox.IsChecked = !checkBox.IsChecked;
                    break;
            }
        }

        /// <summary>
        /// Сброс фокуса
        /// </summary>
        /// <param name="control"></param>
        public static void FocusCancel(this Control control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            switch (control)
            {
                case TreeView tree when tree.ToString() == control.ToString():
                    break;

                case TreeViewItem treeViewItem when treeViewItem.ToString() == control.ToString():
                    treeViewItem = control as TreeViewItem;
                    treeViewItem.IsExpanded = false;
                    treeViewItem.IsSelected = false;
                    break;

                case CheckBox checkBox:
                //case CheckBox checkBox when checkBox.ToString() == control.ToString():
                    checkBox.IsChecked = !checkBox.IsChecked;
                    break;

                case TextBox textBox when textBox.ToString() == control.ToString():
                    textBox = control as TextBox;
                    if (textBox == null) return;

                    var parent = textBox.Parent as UIElement;
                    if (parent != null && parent.Focusable)
                    {
                        parent.Focus();
                    }
                    else
                    {
                        var page = textBox.FindParent<Window>();
                        if (page != null)
                        {
                            page.Focus();
                        }
                    }
                    Keyboard.ClearFocus();
                    break;

                default:
                    if (control == null) return;

                    // Сброс фокуса для всех областей фокуса
                    var focusScope = FocusManager.GetFocusScope(control);
                    FocusManager.SetFocusedElement(focusScope, null);

                    // Дополнительно: поиск и сброс фокуса во всех дочерних элементах
                    var children = control.FindVisualChildren<UIElement>();
                    foreach (var child in children)
                    {
                        if (child.IsKeyboardFocused)
                        {
                            Keyboard.ClearFocus();
                            break;
                        }
                    }
                    break;
            }
        }
    }
}
