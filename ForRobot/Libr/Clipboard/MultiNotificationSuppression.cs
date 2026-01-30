using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForRobot.Libr.Clipboard
{
    /// <summary>
    /// Менеджер для одновременного подавления уведомлений у нескольких объектов
    /// </summary>
    public class MultiNotificationSuppression : IDisposable
    {
        private readonly List<IDisposable> _suppressions = new List<IDisposable>();
        private readonly bool _ownsDisposables;

        public MultiNotificationSuppression(params IChangeNotificationControl[] controls)
        {
            foreach (var control in controls.Where(c => c != null))
            {
                _suppressions.Add(control.SuppressNotifications());
            }
            this._ownsDisposables = true;
        }

        public MultiNotificationSuppression(IEnumerable<IChangeNotificationControl> controls)
        {
            foreach (var control in controls.Where(c => c != null))
            {
                _suppressions.Add(control.SuppressNotifications());
            }
            this._ownsDisposables = true;
        }

        /// <summary>
        /// Конструктор для управления уже существующими IDisposable
        /// </summary>
        /// <param name="ownsDisposables"></param>
        public MultiNotificationSuppression(bool ownsDisposables = true)
        {
            this._ownsDisposables = ownsDisposables;
        }

        public void Add(IDisposable suppression)
        {
            this._suppressions.Add(suppression);
        }

        public void AddRange(IEnumerable<IDisposable> suppressions)
        {
            this._suppressions.AddRange(suppressions);
        }

        public void Dispose()
        {
            if (_ownsDisposables)
            {
                foreach (var suppression in _suppressions)
                {
                    suppression?.Dispose();
                }
            }
            this._suppressions.Clear();
        }
    }

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
