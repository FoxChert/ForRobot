using System;

namespace ForRobot.Libr.Logging
{
    /// <summary>
    /// Класс логирования ошибок
    /// </summary>
    public class LogErrorEventArgs : LogEventArgs
    {
        #region Private variables

        private readonly Exception _exception;

        #endregion

        #region Public variables

        public Exception Exception { get => this._exception; }

        #endregion

        #region Constructors

        public LogErrorEventArgs(string message) : base(message) { }

        public LogErrorEventArgs(string message, Exception exception) : base(message)
        {
            this._exception = exception;
        }

        #endregion
    }
}
