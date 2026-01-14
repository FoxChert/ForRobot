using System;
using System.ComponentModel;
using ForRobot.Models.Detals;

namespace ForRobot.Libr.Collections
{
    public class RibPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        public Rib Rib { get; }

        public RibPropertyChangedEventArgs(Rib rib, string propertyName) : base(propertyName)
        {
            this.Rib = rib;
        }
    }
}
