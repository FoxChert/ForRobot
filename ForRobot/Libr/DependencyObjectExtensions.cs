using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

namespace ForRobot.Libr
{
    /// <summary>
    /// Класс, расширяющий тип <see cref="DependencyObject"/>
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Поиск родительского элемента определённого типа
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <returns>родительский элемент к исходному</returns>
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as T;
        }

        /// <summary>
        /// Поиск дочерниго элемента определенного типа
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns>дочерний элементк исходному</returns>
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        /// <summary>
        /// Сброс фокуса на заданный элемент
        /// </summary>
        /// <param name="dependencyObject"></param>
        public static void LostFocus(this DependencyObject dependencyObject)
        {
            System.Windows.Input.Keyboard.ClearFocus();
            System.Windows.Input.FocusManager.SetFocusedElement(System.Windows.Input.FocusManager.GetFocusScope(dependencyObject), null);
        }
    }
}
