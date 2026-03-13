using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Interactivity;

namespace ForRobot.Libr.AttachedProperties
{
    /// <summary>
    /// Прикрепленные свойства для управления фокусом элементов управления
    /// </summary>
    public static class FocusAttachedProperties
    {
        /// <summary>
        /// Получен ли фокус элементом управления
        /// </summary>
        public static readonly DependencyProperty IsFocusedProperty =  DependencyProperty.RegisterAttached("IsFocused", 
                                                                                                           typeof(bool), typeof(FocusAttachedProperties),
                                                                                                           new UIPropertyMetadata(false, OnIsFocusedChanged));

        public static bool GetIsFocused(DependencyObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return (bool)obj.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            obj.SetValue(IsFocusedProperty, value);
        }

        public static readonly DependencyProperty PreventFocusProperty = DependencyProperty.RegisterAttached("PreventFocus",
                                                                                                             typeof(bool),
                                                                                                             typeof(FocusAttachedProperties),
                                                                                                             new UIPropertyMetadata(false, OnPreventFocusChanged));

        public static bool GetPreventFocus(DependencyObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return (bool)obj.GetValue(PreventFocusProperty);
        }

        public static void SetPreventFocus(DependencyObject obj, bool value)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            obj.SetValue(PreventFocusProperty, value);
        }

        /// <summary>
        /// Обработка изменения прикрепленного свойства "IsFocused"
        /// </summary>
        /// <param name="d">Элемент управления, свойство которого изменилось</param>
        /// <param name="e"></param>
        private static void OnIsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element && (bool)e.NewValue)
            {
                if (CanReceiveFocus(element))
                    element.Dispatcher.BeginInvoke(new Action(() => element.Focus()), DispatcherPriority.Input);
            }
        }

        /// <summary>
        /// Обработка изменения прикрепленного свойства "PreventFocus"
        /// </summary>
        /// <param name="d">Элемент управления, свойство которого изменилось</param>
        /// <param name="e"></param>
        private static void OnPreventFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                bool preventFocus = (bool)e.NewValue;
                element.Focusable = !preventFocus;
            }
        }

        /// <summary>
        /// Проверка, может ли элемент получить фокус
        /// </summary>
        /// <param name="element">Элемент для проверки</param>
        /// <returns></returns>
        private static bool CanReceiveFocus(this UIElement element)
        {
            if (GetPreventFocus(element))
                return false;

            if (element is FrameworkElement frameworkElement)
            {
                return frameworkElement.Focusable;
            }

            return true;
        }
    }
}
