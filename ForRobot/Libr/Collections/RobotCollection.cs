using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using ForRobot.Models.RoboticComplex;

namespace ForRobot.Libr.Collections
{
    public class RobotCollection : ObservableCollection<Robot>
    {
        #region Public variables

        /// <summary>
        /// Событие изменения свойства любого робота в коллекции
        /// </summary>
        public event EventHandler<RobotPropertyChangedEventArgs> RobotPropertyChanged;

        #endregion Public variables

        #region Constructors

        public RobotCollection() : base() { }

        public RobotCollection(List<Robot> list) : base(list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            foreach (var item in Items)
            {
                if (item != null)
                    item.PropertyChanged += OnRobotPropertyChanged;
            }
        }

        public RobotCollection(IEnumerable<Robot> enumerable) : base(enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            foreach (var item in Items)
            {
                if (item != null)
                    item.PropertyChanged += OnRobotPropertyChanged;
            }
        }

        #endregion

        #region Private functions

        private void OnRobotPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var robot = (Robot)sender;
            RobotPropertyChanged?.Invoke(this, new RobotPropertyChangedEventArgs(robot, e.PropertyName));
        }

        #endregion Private functions

        #region Public functions

        #endregion Public functions
    }
}
