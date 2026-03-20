using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Interactivity;
using System.Reflection;

namespace ForRobot.Libr.Behavior
{
    public class DependecyPropertyBehavior : Behavior<DependencyObject>
    {
        private Delegate _handler;
        private EventInfo _eventInfo;
        private PropertyInfo _propertyInfo;
        private DependencyObject element;

        public string Property { get; set; }
        public string UpdateEvent { get; set; }

        public static readonly DependencyProperty BindingProperty = DependencyProperty.RegisterAttached(nameof(Binding),
                                                                                                        typeof(object),
                                                                                                        typeof(DependecyPropertyBehavior),
                                                                                                        new FrameworkPropertyMetadata { BindsTwoWayByDefault = true });

        public object Binding
        {
            get { return GetValue(BindingProperty); }
            set { SetValue(BindingProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            element = AssociatedObject;
            Type elementType = element.GetType();
            _propertyInfo = elementType.GetProperty(Property, BindingFlags.Instance | BindingFlags.Public);
            _eventInfo = elementType.GetEvent(UpdateEvent);
            _handler = CreateDelegateForEvent(_eventInfo, EventFired);
            _eventInfo.AddEventHandler(element, _handler);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            _eventInfo.RemoveEventHandler(element, _handler);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name != "Binding") return;

            object oldValue = _propertyInfo.GetValue(element, null);
            if (oldValue.Equals(e.NewValue)) return;

            if (_propertyInfo.CanWrite)
                _propertyInfo.SetValue(element, e.NewValue, null);

            base.OnPropertyChanged(e);
        }

        private static Delegate CreateDelegateForEvent(EventInfo eventInfo, Action action)
        {
            ParameterExpression[] parameters = eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters().Select(parameter =>  System.Linq.Expressions.Expression.Parameter(parameter.ParameterType)).ToArray();

            return System.Linq.Expressions.Expression.Lambda(eventInfo.EventHandlerType,
                                                             System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Constant(action), "Invoke", Type.EmptyTypes),
                                                             parameters).Compile();
        }

        private void EventFired() => Binding = _propertyInfo.GetValue(element, null);
    }
}
