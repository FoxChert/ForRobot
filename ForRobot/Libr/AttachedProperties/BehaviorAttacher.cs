using System;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace ForRobot.Libr.AttachedProperties
{
    /// <summary>
    /// Прикрепление коллекции поведений к элементам управления через XAML.
    /// Позволяет задавать поведения как вложенные свойства в разметке.
    /// </summary>
    /// <example>
    /// В ресурсах:
    /// <behaviors:BehaviorCollection x:Key="MyBehaviors">
    ///     <behaviors:MouseDragBehavior />
    ///     <behaviors:KeyboardFocusBehavior />
    /// </behaviors:BehaviorCollection>
    /// 
    /// В XAML:
    /// <Grid behaviors:BehaviorAttacher.Behaviors="{StaticResource MyBehaviors}"...
    /// </example>
    public static class BehaviorAttacher
    {
        /// <summary>
        /// Прикреплённое свойство "Behaviors" для хранения коллекции поведений
        /// </summary>
        public static readonly DependencyProperty BehaviorsProperty = DependencyProperty.RegisterAttached("Behaviors", 
                                                                                                          typeof(BehaviorCollection),
                                                                                                          typeof(BehaviorAttacher),
                                                                                                          new PropertyMetadata(null, OnBehaviorsChanged));

        public static BehaviorCollection GetBehaviors(DependencyObject obj)
        {
            return (BehaviorCollection)obj.GetValue(BehaviorsProperty);
        }

        public static void SetBehaviors(DependencyObject obj, BehaviorCollection value)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            obj.SetValue(BehaviorsProperty, value);
        }

        private static void OnBehaviorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element && e.NewValue is BehaviorCollection newBehaviors)
            {
                Interaction.GetBehaviors(element).Clear();
                foreach (var behavior in newBehaviors)
                {
                    Interaction.GetBehaviors(element).Add(behavior);
                }
            }
        }
    }
}
