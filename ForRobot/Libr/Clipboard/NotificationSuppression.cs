using System;
using System.Collections.Generic;

namespace ForRobot.Libr.Clipboard
{
    /// <summary>
    /// Фабрика для создания контекста подавления уведомлений
    /// </summary>
    public static class NotificationSuppression
    {
        public static IDisposable Suppress(params IChangeNotificationControl[] controls) => new MultiNotificationSuppression(controls);

        public static IDisposable Suppress(IEnumerable<IChangeNotificationControl> controls) => new MultiNotificationSuppression(controls);

        public static MultiNotificationSuppression CreateContext() => new MultiNotificationSuppression(false);
    }
}
