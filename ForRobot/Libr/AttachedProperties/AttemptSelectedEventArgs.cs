using System;
using System.Windows;

namespace ForRobot.Libr.AttachedProperties
{
    public class AttemptSelectedEventArgs : RoutedEventArgs
    {
        public bool Cancel { get; set; } = false;

        public AttemptSelectedEventArgs() : base() { }

        public AttemptSelectedEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source) { }
    }
}
