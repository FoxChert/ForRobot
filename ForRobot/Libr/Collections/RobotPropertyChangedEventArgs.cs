using System;
using System.ComponentModel;
using ForRobot.Models.RoboticComplex;

namespace ForRobot.Libr.Collections
{
    public class RobotPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        public Robot Robot { get; }

        public RobotPropertyChangedEventArgs(Robot robot, string propertyName) : base(propertyName)
        {
            this.Robot = robot;
        }
    }
}
