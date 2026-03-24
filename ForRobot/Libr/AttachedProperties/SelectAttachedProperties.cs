using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;

namespace ForRobot.Libr.AttachedProperties
{
    public delegate void AttemptSelectedEventHandler(object sender, AttemptSelectedEventArgs e);

    public class AttemptSelectedEventArgs : RoutedEventArgs
    {
        public bool Cancel { get; set; } = false;

        public AttemptSelectedEventArgs() : base() { }

        public AttemptSelectedEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source) { }
    }

    /// <summary>
    /// Прикрепленные свойства для управления выбором элемента управления
    /// </summary>
    public static class SelectAttachedProperties
    {
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
                                                                                                   typeof(AttemptSelectedEventHandler),
                                                                                                   typeof(SelectAttachedProperties));

        /// <summary>
        /// Прикреплённое свойство для установки обработчика AttemptSelected
        /// </summary>
        public static readonly DependencyProperty AttemptSelectedCommandProperty = DependencyProperty.RegisterAttached("AttemptSelectedCommand",
                                                                                                                       typeof(ICommand),
                                                                                                                       typeof(SelectAttachedProperties),
                                                                                                                       new PropertyMetadata(null, OnAttemptSelectedCommandChanged));

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

                case ContentControl contentControl when contentControl is UIElement uiElement:
                    uiElement.PreviewMouseLeftButtonDown += ContentControl_PreviewMouseLeftButtonDown;
                    break;

                case UIElement uiElement:
                    uiElement.PreviewMouseLeftButtonDown += GenericElement_PreviewMouseLeftButtonDown;
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

                case ContentControl contentControl when contentControl is UIElement uiElement:
                    uiElement.PreviewMouseLeftButtonDown -= ContentControl_PreviewMouseLeftButtonDown;
                    break;

                case UIElement uiElement:
                    uiElement.PreviewMouseLeftButtonDown -= GenericElement_PreviewMouseLeftButtonDown;
                    break;
            }
        }

        #endregion PreventSelect Property

        #region AttemptSelected Event

        public static void AddAttemptSelectedHandler(UIElement element, AttemptSelectedEventHandler handler) => element.AddHandler(AttemptSelectedEvent, handler);

        public static void RemoveAttemptSelectedHandler(UIElement element, AttemptSelectedEventHandler handler) => element.RemoveHandler(AttemptSelectedEvent, handler);

        /// <summary>
        /// Вызов события AttemptSelected
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Возвращаем true, если действие отменяется</returns>
        public static bool RaiseAttemptSelectedEvent(DependencyObject source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (source is UIElement uiElement)
            {
                var args = new AttemptSelectedEventArgs(AttemptSelectedEvent, uiElement);
                uiElement.RaiseEvent(args);
                return args.Cancel;
            }
            return true;
        }

        #endregion AttemptSelected Event

        #region AttemptSelectedCommand Property

        public static ICommand GetAttemptSelectedCommand(DependencyObject obj) => (ICommand)obj.GetValue(AttemptSelectedCommandProperty);

        public static void SetAttemptSelectedCommand(DependencyObject obj, ICommand value) => obj.SetValue(AttemptSelectedCommandProperty, value);

        private static void OnAttemptSelectedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is UIElement element))
                return;

            if (e.OldValue is ICommand)
            {
                element.RemoveHandler(AttemptSelectedEvent, (AttemptSelectedEventHandler)OnAttemptSelectedExecuted);
            }

            if (e.NewValue is ICommand)
            {
                element.AddHandler(AttemptSelectedEvent, (AttemptSelectedEventHandler)OnAttemptSelectedExecuted);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>true - блокировать действие; false - не блокировать</returns>
        private static void OnAttemptSelectedExecuted(object sender, AttemptSelectedEventArgs e)
        {
            var element = sender as DependencyObject;
            var command = GetAttemptSelectedCommand(element);

            if (command?.CanExecute(e) == true)
            {
                command.Execute(e);
            }
        }

        #endregion AttemptSelectedCommand Property

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
            if (sender is DependencyObject element && GetPreventSelect(element) && e.Source == element)
            {
                bool cancel = RaiseAttemptSelectedEvent(element);
                e.Handled = cancel;
            }
        }

        private static void GenericElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DependencyObject element && GetPreventSelect(element) && e.Source == element)
            {
                bool cancel = RaiseAttemptSelectedEvent(element);
                e.Handled = cancel;
            }
        }

        #endregion Event Handlers
    }
}
