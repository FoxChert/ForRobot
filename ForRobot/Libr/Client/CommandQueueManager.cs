using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace ForRobot.Libr.Client
{
    /// <summary>
    /// Менеджер очереди JsonRPC команд. Отвечает за отслеживание активных команд и управление их жизненным циклом
    /// </summary>
    public class CommandQueueManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, Command> _activeCommands;
        private readonly JsonRpcConnection _connection;
        private readonly object _lockObject = new object();
        private volatile int _disposed;

        /// <summary>
        /// Событие добавления команды
        /// </summary>
        public event EventHandler<CommandEventArgs> CommandAdded;
        /// <summary>
        /// Событие выполнения команды
        /// </summary>
        public event EventHandler<CommandEventArgs> CommandCompleted;
        /// <summary>
        /// Событие отмены команды
        /// </summary>
        public event EventHandler<CommandEventArgs> CommandCancelled;
        /// <summary>
        /// Событие окончания времени ожидания команды
        /// </summary>
        public event EventHandler<CommandEventArgs> CommandTimedOut;

        public CommandQueueManager(JsonRpcConnection connection)
        {
            _activeCommands = new ConcurrentDictionary<string, Command>();
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        #region Private functions

        private async Task CancelCommandWithTimeout(Command command, CancellationToken cancellationToken)
        {
            try
            {
                command.Cancel();
                this.OnCommandCancelled(command);
                await Task.CompletedTask;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
        }

        #endregion Private functions

        #region Public functions

        /// <summary>
        /// Вызов события добавления комманды
        /// </summary>
        /// <param name="command"></param>
        protected virtual void OnCommandAdded(Command command) => CommandAdded?.Invoke(this, new CommandEventArgs(command));
        /// <summary>
        /// Событие выполнения команды (успешно или с ошибкой)
        /// </summary>
        /// <param name="command"></param>
        protected virtual void OnCommandCompleted(Command command) => CommandCompleted?.Invoke(this, new CommandEventArgs(command));
        /// <summary>
        /// Вызов события отмены команды
        /// </summary>
        /// <param name="command"></param>
        protected virtual void OnCommandCancelled(Command command) => CommandCancelled?.Invoke(this, new CommandEventArgs(command));
        /// <summary>
        /// Событие истечения времени ожидания выполнени команды
        /// </summary>
        /// <param name="command"></param>
        protected virtual void OnCommandTimedOut(Command command) => CommandTimedOut?.Invoke(this, new CommandEventArgs(command));

        public string EnqueueCommand(string methodName, object[] parameters, TimeSpan timeout)
        {
            if (_disposed == 1)
                throw new ObjectDisposedException(nameof(CommandQueueManager));

            var command = new Command(methodName, parameters, timeout);
            command.TimeoutElapsed += (s, e) =>
            {
                OnCommandTimedOut((Command)s);
                _activeCommands.TryRemove(((Command)s).Id, out _);
            };
            if (!_activeCommands.TryAdd(command.Id, command))
            {
                throw new InvalidOperationException("Failed to enqueue command");
            }
            this.OnCommandAdded(command);
            return command.Id;
        }

        public IReadOnlyList<Command> GetActiveCommands() => _activeCommands.Values.ToList();

        /// <summary>
        /// Содержится ли команда в активных
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns></returns>
        public bool ContainsCommand(string commandId) => _activeCommands.ContainsKey(commandId);

        /// <summary>
        /// Отмена команды
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns></returns>
        public bool CancelCommand(string commandId)
        {
            if (_activeCommands.TryRemove(commandId, out var command))
            {
                command.Cancel();
                this.OnCommandCancelled(command);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Отмена всех команд
        /// </summary>
        /// <param name="timeout">Интервал для отмены команд</param>
        public void CancelAllCommands(TimeSpan? timeout = null)
        {
            var commandsToCancel = _activeCommands.Values.ToArray();

            if (timeout.HasValue)
            {
                var cts = new CancellationTokenSource(timeout.Value);
                var tasks = commandsToCancel.Select(cmd => CancelCommandWithTimeout(cmd, cts.Token)).ToArray();
                
                try
                {
                    Task.WaitAll(tasks, timeout.Value);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                foreach (var command in commandsToCancel)
                {
                    command.Cancel();
                    OnCommandCancelled(command);
                }
            }

            foreach (var command in commandsToCancel)
            {
                _activeCommands.TryRemove(command.Id, out _);
            }
        }

        /// <summary>
        /// Удаление выполненной команды
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns></returns>
        public bool RemoveCompletedCommand(string commandId)
        {
            if (_activeCommands.TryRemove(commandId, out var command))
            {
                this.OnCommandCompleted(command);
                return true;
            }
            return false;
        }

        #endregion Public functions

        #region Implementations of IDisposable

        ~CommandQueueManager() => Dispose();

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
            {
                foreach (var cmd in _activeCommands)
                {
                    if (_activeCommands.TryRemove(cmd.Key, out var command))
                    {
                        command?.Dispose();
                    }
                }
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}
