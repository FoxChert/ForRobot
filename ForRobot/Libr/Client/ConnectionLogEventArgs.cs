using System;

using ForRobot.Libr.Logging;

namespace ForRobot.Libr.Client
{
    /// <summary>
    /// Класс логирования событий класса <see cref="ForRobot.Libr.Client.JsonRpcConnection"/>
    /// </summary>
    public class ConnectionLogEventArgs : LogEventArgs
    {
        #region Private variables

        private readonly string _host;
        private readonly int _port;

        #endregion

        #region Public variables

        public string Host { get => this._host; }
        public int Port { get => this._port; }

        #endregion

        #region Constructors

        public ConnectionLogEventArgs(string host, int port, string message) : base(message)
        {
            this._host = host;
            this._port = port;
        }

        #endregion
    }
}
