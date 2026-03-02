using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using ForRobot.Models.RoboticComplex;
//using ForRobot.Libr.Logging;

namespace ForRobot.Libr.Collections
{
    public class RobotCollection : ObservableCollection<Robot>
    {
        #region Private variables

        private int _connectionTimeOutMilliseconds;
        private event EventHandler<ForRobot.Libr.Logging.LogEventArgs> _loggingEvent;
        private event EventHandler<ForRobot.Libr.Logging.LogErrorEventArgs> _loggingErrorEvent;

        #endregion Private variables

        #region Public variables

        public int ConnectionTimeOutMilliseconds
        {
            get => this._connectionTimeOutMilliseconds;
            set
            {
                _connectionTimeOutMilliseconds = value;

                for (int i = 0; i < 0; i++)
                {
                    this.Items[i].ConnectionTimeOutMilliseconds = this.ConnectionTimeOutMilliseconds;
                }
            }
        }

        public event EventHandler<ForRobot.Libr.Logging.LogEventArgs> LoggingEvent
        {
            add
            {
                _loggingEvent += value;
                for (int i = 0; i < 0; i++)
                {
                    //this.Items[i].LoggingEvent += this.LoggingEvent;
                }
            }
            remove
            {
                _loggingEvent -= value;
                for (int i = 0; i < 0; i++)
                {
                    //this.Items[i].LoggingEvent -= this.LoggingEvent;
                }
            }
        }

        public event EventHandler<ForRobot.Libr.Logging.LogErrorEventArgs> LoggingErrorEvent
        {
            add
            {
                _loggingErrorEvent += value;
                for (int i = 0; i < 0; i++)
                {
                    //this.Items[i].LoggingEvent += this.LoggingEvent;
                }
            }
            remove
            {
                _loggingErrorEvent -= value;
                for (int i = 0; i < 0; i++)
                {
                    //this.Items[i].LoggingEvent -= this.LoggingEvent;
                }
            }
        }

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

        protected override void InsertItem(int index, Robot item)
        {
            if (item != null)
                item.PropertyChanged += OnRobotPropertyChanged;

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];

            if (item != null)
                item.PropertyChanged -= OnRobotPropertyChanged;

            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] != null)
                    this[i].PropertyChanged -= OnRobotPropertyChanged;
            }
            base.ClearItems();
        }

        protected override void SetItem(int index, Robot item)
        {
            var oldItem = this[index];

            if (oldItem != null)
                oldItem.PropertyChanged -= OnRobotPropertyChanged;

            if (item != null)
                item.PropertyChanged += OnRobotPropertyChanged;

            base.SetItem(index, item);
        }

        public IEnumerable<string> GetRobotsNames()
        {
            List<string> names = new List<string>();
            for(int i = 0; i < this.Count; i++)
            {
                names.Add(this.Items[i].Name);
            }
            return names;
        }

        public new void Add(Robot newItem = null)
        {
            if (newItem == null)
                newItem = new Robot();

            newItem.Name = string.Format("Соединение {0}", this.Count + 1);
            newItem.ConnectionTimeOutMilliseconds = this.ConnectionTimeOutMilliseconds;
            //newItem.LoggingEvent += this.LoggingEvent;
            //newItem.LoggingErrorEvent += this.LoggingEvent;
            base.Add(newItem);
        }

        public void SetSelectedControllerPath(string path)
        {
            for(int i = 0; i < 0; i++)
            {
                this.Items[i].SelectedControllerPath = path;
            }
        }

        #endregion Public functions
    }
}
