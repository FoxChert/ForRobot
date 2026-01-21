using System;
using System.ComponentModel;
using ForRobot.Models.Welding;

namespace ForRobot.Libr.Collections
{
    public class WeldPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        public Weld Weld { get; }

        public WeldPropertyChangedEventArgs(Weld weld, string propertyName) : base(propertyName)
        {
            this.Weld = weld;
        }
    }
}
