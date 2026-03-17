using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ForRobot.Libr.AttachedProperties
{
    /// <summary>
    /// Прикрепленные свойства для управления выбором элемента управления
    /// </summary>
    public class SelectAttachedProperties
    {
        private static readonly object _lock = new object();

        ///// <summary>
        ///// Прикреплённое свойство "IsSelected" для управления состоянием выбора элементом
        ///// </summary>
        //public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.RegisterAttached("IsSelected",
        //                                                                                                   typeof(bool),
        //                                                                                                   typeof(SelectAttachedProperties),
        //                                                                                                   new UIPropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// Прикреплённое свойство "PreventSelect" для запрета выбора элемента управления
        /// </summary>
        public static readonly DependencyProperty PreventSelectProperty = DependencyProperty.RegisterAttached("PreventSelect",
                                                                                                              typeof(bool),
                                                                                                              typeof(SelectAttachedProperties),
                                                                                                              new UIPropertyMetadata(false, OnPreventSelectChanged));

        /// <summary>
        /// Прикреплённое свойство "AttemptSelected" события попотки получения выбора элементом
        /// </summary>
        public static readonly RoutedEvent AttemptSelectedEvent = EventManager.RegisterRoutedEvent("AttemptSelected",
                                                                                                   RoutingStrategy.Bubble,
                                                                                                   typeof(RoutedEventHandler),
                                                                                                   typeof(SelectAttachedProperties));

        //#region IsSelcted Property

        //public static bool GetIsSelected(DependencyObject obj)
        //{
        //    if (obj == null)
        //        throw new ArgumentNullException(nameof(obj));

        //    return (bool)obj.GetValue(IsSelectedProperty);
        //}

        //public static void SetIsSelected(DependencyObject obj, bool value)
        //{
        //    if (obj == null)
        //        throw new ArgumentNullException(nameof(obj));

        //    obj.SetValue(IsSelectedProperty, value);
        //}

        //private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{

        //}

        //#endregion IsSelcted Property

        #region PreventSelect Property

        public static bool GetPreventSelect(DependencyObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return (bool)obj.GetValue(PreventSelectProperty);
        }

        public static void SetPreventSelect(DependencyObject obj, bool value)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            obj.SetValue(PreventSelectProperty, value);
        }

        /// <summary>
        /// Обработка изменения прикрепленного свойства "PreventSelect"
        /// </summary>
        /// <param name="d">Элемент управления, свойство которого изменилось</param>
        /// <param name="e"></param>
        private static void OnPreventSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null || e.NewValue == null)
                return;

            if ((bool)e.NewValue)
            {
                SubscribeToSelectionEvents(d);
            }
            else
            {
                UnsubscribeFromSelectionEvents(d);
            }
        }

        /// <summary>
        /// Подписка на события выбора в зависимости от типа элемента
        /// </summary>
        private static void SubscribeToSelectionEvents(DependencyObject element)
        {
            switch (element)
            {
                case Selector selector:
                    selector.SelectionChanged += Selector_SelectionChanged;
                    break;

                case ContentControl contentControl:
                    if (contentControl is UIElement uiElement)
                    {
                        uiElement.PreviewMouseLeftButtonDown += ContentControl_PreviewMouseLeftButtonDown;
                    }
                    break;
                    
                default:
                    if (element is UIElement genericElement)
                    {
                        genericElement.PreviewMouseLeftButtonDown += GenericElement_PreviewMouseLeftButtonDown;
                    }
                    break;
            }
        }

        /// <summary>
        /// Отписка от событий выбора
        /// </summary>
        private static void UnsubscribeFromSelectionEvents(DependencyObject element)
        {
            switch (element)
            {
                case Selector selector:
                    selector.SelectionChanged -= Selector_SelectionChanged;
                    break;

                case ContentControl contentControl:
                    if (contentControl is UIElement uiElement)
                    {
                        uiElement.PreviewMouseLeftButtonDown -= ContentControl_PreviewMouseLeftButtonDown;
                    }
                    break;

                default:
                    if (element is UIElement genericElement)
                    {
                        genericElement.PreviewMouseLeftButtonDown -= GenericElement_PreviewMouseLeftButtonDown;
                    }
                    break;
            }
        }

        #endregion PreventSelect Property

        #region AttemptSelected Event

        public static void AddAttemptSelectedHandler(UIElement element, RoutedEventHandler handler) => element.AddHandler(AttemptSelectedEvent, handler);

        public static void RemoveAttemptSelectedHandler(UIElement element, RoutedEventHandler handler) => element.RemoveHandler(AttemptSelectedEvent, handler);

        /// <summary>
        /// Вызов события AttemptSelected
        /// </summary>
        /// <param name="source"></param>
        public static void RaiseAttemptSelectedEvent(DependencyObject source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (source is UIElement uiElement)
            {
                uiElement.RaiseEvent(new RoutedEventArgs(AttemptSelectedEvent, uiElement));
            }
        }

        #endregion AttemptSelected Event

        #region Event Handlers

        private static void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is Selector selector && selector.SelectedItem != null)
            {
                DependencyObject container = selector.ItemContainerGenerator.ContainerFromItem(selector.SelectedItem) as DependencyObject;

                if (container != null && GetPreventSelect(container))
                {
                    if (e.AddedItems.Count > 0)
                    {
                        e.RemovedItems.Add(e.AddedItems[0]);
                        e.AddedItems.Clear();
                    }

                    RaiseAttemptSelectedEvent(container);
                }
            }
        }

        private static void ContentControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DependencyObject element && GetPreventSelect(element))
            {
                e.Handled = true;
                RaiseAttemptSelectedEvent(element);
            }
        }

        private static void GenericElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DependencyObject element && GetPreventSelect(element))
            {
                e.Handled = true;
                RaiseAttemptSelectedEvent(element);
            }
        }

        #endregion Event Handlers
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
