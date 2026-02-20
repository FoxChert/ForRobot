using System;

namespace ForRobot.Libr.Logging
{
    public class LogEventArgs
    {
        #region Private variables

        private readonly string _message;

        #endregion

        #region Public variables

        public string Message { get => this._message; }

        #endregion

        #region Constructors

        public LogEventArgs(string message) : base()
        {
            this._message = message;
        }

        #endregion
    }
}
