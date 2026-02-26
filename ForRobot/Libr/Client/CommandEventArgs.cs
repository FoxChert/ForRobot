using System;

namespace ForRobot.Libr.Client
{
    public class CommandEventArgs : EventArgs
    {
        public Command Command { get; }

        public CommandEventArgs(Command command)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
        }
    }
}
