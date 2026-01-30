using System;

namespace ForRobot.Libr.Clipboard
{
    /// <summary>
    /// Интерфейс для управления уведомлениями об изменениях
    /// </summary>
    public interface IChangeNotificationControl
    {
        /// <summary>
        /// Подавляет уведомления об изменениях
        /// </summary>
        IDisposable SuppressNotifications();

        /// <summary>
        /// Проверяет, подавлены ли уведомления
        /// </summary>
        bool IsNotificationsSuppressed { get; }
    }
}
