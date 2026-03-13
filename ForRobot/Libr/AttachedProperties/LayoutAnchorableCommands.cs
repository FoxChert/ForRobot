using System;
using System.Windows;
using System.Windows.Input;

using AvalonDock.Layout;

namespace ForRobot.Libr.AttachedProperties
{
    /// <summary>
    /// Прикрепленные свойства для управления выбором элемента управления
    /// </summary>
    public class SelectAttachedProperties
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// Прикреплённое свойство "PreventSelect" для запреса выбора элемента управления
        /// </summary>
        public static readonly DependencyProperty PreventFocusProperty = DependencyProperty.RegisterAttached("PreventSelect",
                                                                                                             typeof(bool),
                                                                                                             typeof(SelectAttachedProperties),
                                                                                                             new UIPropertyMetadata(false, OnPreventSelectChanged));

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

        public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected",
                                                                                            RoutingStrategy.Bubble,
                                                                                            typeof(RoutedEventHandler),
                                                                                            typeof(SelectAttachedProperties));

        public static void AddSelectedHandler(UIElement element, RoutedEventHandler handler)
        {
            element.AddHandler(SelectedEvent, handler);
        }

        public static void RemoveSelectedHandler(UIElement element, RoutedEventHandler handler)
        {
            element.RemoveHandler(SelectedEvent, handler);
        }

        public static void RaiseSelectedEvent(UIElement source) => source.RaiseEvent(new RoutedEventArgs(SelectedEvent));

        //public static readonly DependencyProperty PreventFocusProperty = DependencyProperty.RegisterAttached("PreventSelect");

        //public static readonly DependencyProperty SelectedEventProperty = DependencyProperty.RegisterAttached("SelectedEvent",
        //                                                                                                      typeof(EventHandler),
        //                                                                                                      typeof(SelectAttachedProperties),
        //                                                                                                      new UIPropertyMetadata(null));

        //public static EventHandler GetSelectedEvent(DependencyObject obj)
        //{
        //    if (obj == null)
        //        throw new ArgumentNullException(nameof(obj));

        //    return (EventHandler)obj.GetValue(SelectedEventProperty);
        //}

        //public static void SetSelectedEvent(DependencyObject obj, bool value)
        //{
        //    if (obj == null)
        //        throw new ArgumentNullException(nameof(obj));

        //    obj.SetValue(SelectedEventProperty, value);
        //}

        /// <summary>
        /// Обработка изменения прикрепленного свойства "PreventSelect"
        /// </summary>
        /// <param name="d">Элемент управления, свойство которого изменилось</param>
        /// <param name="e"></param>
        private static void OnPreventSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            switch (d)
            {
                case LayoutContent layoutContent:
                    //layoutContent.IsSelected = false;
                    //layoutContent.IsActive = false;
                    if ((bool)e.NewValue)
                        layoutContent.IsSelectedChanged += LayoutContent_IsSelectedChanged;
                    else
                        layoutContent.IsSelectedChanged -= LayoutContent_IsSelectedChanged;
                    break;
            }
        }

        #region Handlers IsSelectedChanged

        private static void LayoutContent_IsSelectedChanged(object sender, EventArgs e)
        {
            //SelectedEvent?.Invoke(this, null);
            //LayoutContent layoutContent = sender as LayoutContent;
            //layoutContent.IsSelected = false;
            //layoutContent.IsActive = false;
        }

        #endregion
    }

    //public static class LayoutAnchorableCommands
    //{
    //    public static readonly DependencyProperty IsClosedProperty = DependencyProperty.RegisterAttached("IsClosed",
    //                                                                                                     typeof(bool),
    //                                                                                                     typeof(LayoutAnchorableCommands),
    //                                                                                                     new PropertyMetadata(false, OnIsClosedChanged));

    //    public static bool GetIsClosed(DependencyObject obj)
    //    {
    //        return (bool)obj.GetValue(IsClosedProperty);
    //    }

    //    public static void SetIsClosed(DependencyObject obj, bool value)
    //    {
    //        obj.SetValue(IsClosedProperty, value);
    //    }

    //    //public static readonly DependencyProperty OpenedCommandProperty = DependencyProperty.RegisterAttached("OpenedCommand",
    //    //                                                                                                      typeof(ICommand),
    //    //                                                                                                      typeof(LayoutAnchorableCommands),
    //    //                                                                                                      new PropertyMetadata(null, OnOpenedCommandChanged));

    //    private static void OnIsClosedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is LayoutAnchorable layoutAnchorable)
    //        {
    //            if (e.NewValue == null)
    //                return;                

    //            if((bool)e.NewValue)
    //                layoutAnchorable.IsSelectedChanged += LayoutAnchorable_IsSelectedChanged;
    //            else
    //                layoutAnchorable.IsSelectedChanged -= LayoutAnchorable_IsSelectedChanged;
    //        }
    //    }

    //    //public static ICommand GetOpenedCommand(DependencyObject obj)
    //    //{
    //    //    return (ICommand)obj.GetValue(OpenedCommandProperty);
    //    //}

    //    public static void SetOpenedCommand(DependencyObject obj, ICommand value)
    //    {
    //        //obj.SetValue(OpenedCommandProperty, value);
    //    }

    //    private static void OnOpenedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is LayoutAnchorable layoutAnchorable)
    //        {
    //            if (e.OldValue is ICommand oldCommand)
    //            {
    //                layoutAnchorable.IsVisibleChanged -= LayoutAnchorable_IsSelectedChanged;
    //            }
    //            if (e.NewValue is ICommand newCommand)
    //            {
    //                layoutAnchorable.IsVisibleChanged += LayoutAnchorable_IsSelectedChanged;
    //            }
    //        }
    //    }

    //    private static void LayoutAnchorable_IsSelectedChanged(object sender, EventArgs e)
    //    {
    //        if (sender is LayoutAnchorable layoutAnchorable)
    //        {
    //            //var command = GetOpenedCommand(layoutAnchorable);
    //            //if (command != null && command.CanExecute(layoutAnchorable.Content))
    //            //{
    //            //    command.Execute(layoutAnchorable.Content);
    //            //}
    //        }
    //    }
    //}
}
