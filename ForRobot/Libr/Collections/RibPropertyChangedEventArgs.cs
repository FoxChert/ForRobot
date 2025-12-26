using System;

using ForRobot.Models.Detals;

namespace ForRobot.Libr.Collections
{
    public class RibPropertyChangedEventArgs : EventArgs
    {
        public Rib Rib { get; }
        public string PropertyName { get; }

        public RibPropertyChangedEventArgs(Rib rib, string propertyName)
        {
            this.Rib = rib;
            this.PropertyName = propertyName;
        }
    }
}
