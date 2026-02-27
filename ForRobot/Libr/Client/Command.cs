using System;
using System.Threading;

namespace ForRobot.Libr.Client
{
    /// <summary>
    /// Класс-модель представления JsonRPC команды с метаданными
    /// </summary>
    public class Command
    {
        public string Id { get; private set; }

        /// <summary>
        /// Наименование JsonRPC метода
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Параметры метода
        /// </summary>
        public object[] Parameters { get; }

        /// <summary>
        /// Время ожидания выполнения команды
        /// </summary>
        public TimeSpan Timeout { get; private set; }

        /// <summary>
        /// Токен отмены для управления временем жизни команды
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; set; }

        /// <summary>
        /// Время создания команды
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Время начала выполнения команды
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// Время завершения команды, успешного или с ошибкой
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Флаг, указывающий, произошел ли таймаут
        /// </summary>
        public bool HasTimedOut { get; private set; } = false;

        /// <summary>
        /// Флаг, указывающий активна ли команда
        /// </summary>
        public bool IsActive => !CancellationTokenSource.IsCancellationRequested && !HasTimedOut;

        /// <summary>
        /// Событие таймаута команды
        /// </summary>
        public event EventHandler TimeoutElapsed;

        /// <summary>
        /// Инициализация команды с одним параметром
        /// </summary>
        /// <param name="methodName">Наименование метода</param>
        /// <param name="parameter">Передаваемый параметр</param>
        /// <param name="timeout">Время ожидания выполнения команды (по-умолчанию используется <see cref="JsonRpcConnection.DEFAULT_TIMEOUT_MILLISECONDS"/>)</param>
        public Command(string methodName, object parameter, TimeSpan? timeout = null)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName));

            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.UtcNow;
            MethodName = methodName;
            Timeout = timeout ?? new TimeSpan(JsonRpcConnection.DEFAULT_TIMEOUT_MILLISECONDS);
            this.StartTimeoutTimer();
        }

        /// <summary>
        /// Инициализация команды
        /// </summary>
        /// <param name="methodName">Наименование метода</param>
        /// <param name="parameters">Массив передаваемых параметров</param>
        /// <param name="timeout">Время ожидания выполнения команды (по-умолчанию используется <see cref="JsonRpcConnection.DEFAULT_TIMEOUT_MILLISECONDS"/>)</param>
        public Command(string methodName, object[] parameters, TimeSpan? timeout = null)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName));

            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.UtcNow;
            MethodName = methodName;
            Parameters = parameters ?? Array.Empty<object>();
            Timeout = timeout ?? new TimeSpan(JsonRpcConnection.DEFAULT_TIMEOUT_MILLISECONDS);
            this.StartTimeoutTimer();
        }

        /// <summary>
        /// Запуск таймера для отслеживания таймаута
        /// </summary>
        private void StartTimeoutTimer()
        {
            CancellationTokenSource = new CancellationTokenSource(Timeout);
            
            CancellationTokenSource.Token.Register(() =>
            {
                if (!HasTimedOut) // Защита от повторного срабатывания
                {
                    HasTimedOut = true;
                    TimeoutElapsed?.Invoke(this, EventArgs.Empty);
                }
            });
        }

        /// <summary>
        /// Отмена выполнения команды
        /// <para>Возможна утечка CancellationTokenSource, лучше использовать using</para>
        /// </summary>
        public void Cancel() => CancellationTokenSource?.Cancel();

        public void Dispose()
        {
            CancellationTokenSource?.Dispose();
        }
    }
}
