using System;
using System.Linq;

using NLog;

namespace ForRobot.Models
{
    public class Message
    {
        #region Private variables

        #endregion

        #region Public variables

        public DateTime Time { get; set; }

        public NLog.LogLevel LogLevel { get; set; }

        public string Content { get; set; }

        public string Ditails { get; set; } = string.Empty;

        #endregion

        #region Constructor

        public Message(string content, Exception exception = null)
        {
            this.Content = content;
        }

        public Message(string[] values)
        {
            this.Time = Convert.ToDateTime(values[0]);
            this.LogLevel = NLog.LogLevel.AllLoggingLevels.Where(item => string.Equals(item.Name, values[1], StringComparison.InvariantCultureIgnoreCase)).First();
            this.Content = values[2];
            this.Ditails = values[3];
        }

        #endregion
    }
}
