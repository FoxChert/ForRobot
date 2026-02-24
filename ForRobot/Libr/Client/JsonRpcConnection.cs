using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Sockets;

using StreamJsonRpc;

namespace ForRobot.Libr.Client
{
    /// <summary>
    /// Класс-модель комманды
    /// </summary>
    public class Command
    {
        public string Id { get; private set; }
        public string MethodName { get; }
        public object[] Parameters { get; }
        public TimeSpan Timeout { get; private set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public Task<object> Task { get; set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; } // Нужен для логирования завершения команды

        public Command(string methodName, object parameter)
        {
            this.Initialisation();
            MethodName = methodName;
        }

        public Command(string methodName, params object[] parameters)
        {
            this.Initialisation();
            MethodName = methodName;
            Parameters = parameters ?? Array.Empty<object>();
        }

        /// <summary>
        /// Метод-инициализатор для повторяющихся свойств
        /// </summary>
        private void Initialisation()
        {
            Id = Guid.NewGuid().ToString();
            Timeout = new TimeSpan(JsonRpcConnection.DEFAULT_TIMEOUT_MILLISECONDS);
            CancellationTokenSource = new CancellationTokenSource();
            CreatedAt = DateTime.UtcNow;
        }

        public bool IsTimeout => this.CancellationTokenSource.IsCancellationRequested && !CancellationTokenSource.Token.IsCancellationRequested;

        public void Cancel()
        {
            CancellationTokenSource.Cancel();
        }
    }

    public class CommandEventArgs : EventArgs
    {
        public Command Command { get; }

        public CommandEventArgs(Command command)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
        }
    }

    //public class CommandException : Exception
    //{
    //    public Command Command { get; }

    //    public CommandException(Command command)
    //    {
    //        Command = command ?? throw new ArgumentNullException(nameof(command));
    //    }
    //}

    public static class TaskExtensions
    {
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            }
            return await task;
        }
    }

    public class CommandQueueManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, Command> _activeCommands;
        private readonly JsonRpcConnection _connection;
        private readonly object _lockObject = new object();
        private volatile int _disposed;

        public event EventHandler<CommandEventArgs> CommandAdded;
        public event EventHandler<CommandEventArgs> CommandCompleted;
        public event EventHandler<CommandEventArgs> CommandCancelled;
        public event EventHandler<CommandEventArgs> CommandTimedOut;

        public CommandQueueManager(JsonRpcConnection connection)
        {
            _activeCommands = new ConcurrentDictionary<string, Command>();
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        #region Private functions

        //private async Task<T> ExecuteCommandWithTrackingAsync<T>(Command command)
        //{
        //    if (!_activeCommands.TryAdd(command.Id, command))
        //    {
        //        throw new InvalidOperationException("Ошибка добавления команды в очередь.");
        //    }

        //    command.StartedAt = DateTime.UtcNow;
        //    this.OnCommandAdded(command);

        //    try
        //    {
        //        var result = await ExecuteCommandAsync<T>(command);
        //        command.CompletedAt = DateTime.UtcNow;
        //        this.OnCommandCompleted(command);
        //        return result;
        //    }
        //    catch (OperationCanceledException) when (command.CancellationTokenSource.IsCancellationRequested)
        //    {
        //        command.CompletedAt = DateTime.UtcNow;
        //        if (command.CancellationTokenSource.Token.IsCancellationRequested)
        //        {
        //            this.OnCommandTimedOut(command);
        //            throw new TimeoutException($"Command '{command.MethodName}' timed out after {command.Timeout.TotalSeconds} seconds");
        //        }
        //        else
        //        {
        //            this.OnCommandCancelled(command);
        //            throw;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        command.CompletedAt = DateTime.UtcNow;
        //        throw;
        //    }
        //    finally
        //    {
        //        _activeCommands.TryRemove(command.Id, out _);
        //    }
        //}

        //private async Task<T> ExecuteCommandAsync<T>(Command command)
        //{
        //    var cts = new CancellationTokenSource(command.Timeout);

        //    try
        //    {
        //        return await command.Task.WithCancellation<T>(cts.Token);
        //    }
        //    catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
        //    {
        //        throw new TimeoutException($"Команда \"{command.MethodName}\" была остановлена после {command.Timeout.TotalSeconds} секунд.");
        //    }
        //}

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

        protected virtual void OnCommandAdded(Command command) => CommandAdded?.Invoke(this, new CommandEventArgs(command));
        protected virtual void OnCommandCompleted(Command command) => CommandCompleted?.Invoke(this, new CommandEventArgs(command));
        protected virtual void OnCommandCancelled(Command command) => CommandCancelled?.Invoke(this, new CommandEventArgs(command));
        protected virtual void OnCommandTimedOut(Command command) => CommandTimedOut?.Invoke(this, new CommandEventArgs(command));

        public string EnqueueCommand(string methodName, object[] parameters, TimeSpan timeout)
        {
            if (_disposed == 1)
                throw new ObjectDisposedException(nameof(CommandQueueManager));

            var command = new Command(methodName, parameters, timeout);

            if (!_activeCommands.TryAdd(command.Id, command))
            {
                throw new InvalidOperationException("Failed to enqueue command");
            }

            this.OnCommandAdded(command);
            return command.Id;
        }

        //public async Task<T> EnqueueCommandAsync<T>(Command command)
        //{
        //    if (_disposed)
        //        throw new ObjectDisposedException(nameof(CommandQueueManager));

        //    return await ExecuteCommandWithTrackingAsync<T>(command);
        //}

        //public async Task<T> EnqueueCommandAsync<T>(string methodName, TimeSpan timeout, params object[] parameters)
        //{
        //    if (_disposed)
        //        throw new ObjectDisposedException(nameof(CommandQueueManager));

        //    var command = new Command(methodName, parameters, timeout);
        //    return await ExecuteCommandWithTrackingAsync<T>(command);
        //}

        //public async Task<T> EnqueueCommandAsync<T>(string methodName, params object[] parameters) => await EnqueueCommandAsync<T>(methodName, _defaultTimeout, parameters);

        //public async Task<T> EnqueueCommandAsync<T>(string methodName, CancellationToken cancellationToken, params object[] parameters)
        //{
        //    if (_disposed)
        //        throw new ObjectDisposedException(nameof(CommandQueueManager));

        //    var command = new Command(methodName, parameters, _defaultTimeout);

        //    var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(command.CancellationTokenSource.Token, cancellationToken);

        //    command.CancellationTokenSource = combinedCts;

        //    return await ExecuteCommandWithTrackingAsync<T>(command);
        //}

        public IReadOnlyList<Command> GetActiveCommands() => _activeCommands.Values.ToList();

        public bool ContainsCommand(string commandId) => _activeCommands.ContainsKey(commandId);

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

        public void CancelAllCommands(TimeSpan? timeout = null)
        {
            var commandsToCancel = _activeCommands.Values.ToList();

            if (timeout.HasValue)
            {
                var cts = new CancellationTokenSource(timeout.Value);
                var tasks = commandsToCancel.Select(cmd => CancelCommandWithTimeout(cmd, cts.Token)).ToArray();

                try
                {
                    Task.WaitAll(tasks, timeout.Value);
                }
                catch (Exception)
                {
                    // Логирование или обработка частичной отмены
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

            _activeCommands.Clear();
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
                foreach (var command in _activeCommands.Values)
                {
                    command.CancellationTokenSource?.Dispose();
                }
                _activeCommands.Clear();
                _disposed = 1;
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }

    /// <summary>
    /// Класс <c>JsonRpcConnection</c>
    /// </summary>
    public class JsonRpcConnection : IDisposable
    {
        #region Constants

        public const int DEFAULT_PORT = 0000;
        public const int DEFAULT_TIMEOUT_MILLISECONDS = 3000;
        public const string DEFAULT_HOST = "0.0.0.0";
        public const string OK_RESPONSE = "Ok";
        public const string DEFAULT_ROOT = "KRC:\\";

        #endregion

        #region Private variables

        /// <summary>
        /// Обработчик исключений
        /// </summary>
        private static readonly Action<CommandQueueManager, Command, Exception> _exceptionCallback = new Action<CommandQueueManager, Command, Exception>((sender, command, e) =>
        {
            try
            {
                throw e;
            }
            catch (OperationCanceledException ex) when (!(sender as CommandQueueManager).ContainsCommand(command.Id))
            {
                (sender as CommandQueueManager).CancelCommand(command.Id);
                //exceptionCallback?.Invoke(new OperationCanceledException($"Command {methodName} was cancelled"));
                throw new OperationCanceledException($"Команда {command.MethodName} была отменена");
            }
            catch (TimeoutException)
            {
                (sender as CommandQueueManager).RemoveCompletedCommand(command.Id);
                //exceptionCallback?.Invoke(new TimeoutException($"Command {methodName} timed out"));
                throw new TimeoutException($"Время ожидания команды {command.MethodName} превысело установленный лимит {command.Timeout.Seconds} сек.");
            }
            catch (Exception ex)
            {
                (sender as CommandQueueManager).RemoveCompletedCommand(command.Id);
                //exceptionCallback?.Invoke(ex);
                throw ex;
            }
            //catch (CommandException commandEx)
            //{
            //    //throw new ConnectionLogErrorEventArgs(Host)
            //}
        ////catch (ConnectionLogEventArgs connectionException)
        ////{
        ////    throw()
        ////}
        //catch (ConnectionLogErrorEventArgs connectionErrorException)
            //{
            //    throw ()
            //}
        });

        private readonly CommandQueueManager _commandQueueManager;

        /// <summary>
        /// Управляет подключением JSON-RPC к серверу через Stream
        /// </summary>
        private JsonRpc JsonRpc;

        #endregion

        #region Public variables

        /// <summary>
        /// Используется классом для указания, что соединение установлено
        /// </summary>
        protected bool IsConnected
        {
            get
            {
                try
                {
                    if (this.Client == null || !this.Client.Connected)
                        return false;
                }
                catch (Exception ex)
                {
                    throw new Exception("Не удалось определить состояние TCP-сокета", ex);
                }

                return this.CheckKey();
            }
        }

        public string Host { get; set; }

        public int Port { get; set; }

        /// <summary>
        /// Время ожидания ответа, тик
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        public TcpClient Client { get; set; }

        #region Events

        /// <summary>
        /// Событие открытия соединения
        /// </summary>
        public event EventHandler Connected;
        /// <summary>
        /// Событие прерывания соединения
        /// </summary>
        public event EventHandler Aborted;
        /// <summary>
        /// Событие закрытия соединения
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        /// Событие записи лога
        /// </summary>
        public event EventHandler<ConnectionLogEventArgs> LoggingEvent;
        /// <summary>
        /// Событие логирования ошибки
        /// </summary>
        public event EventHandler<ConnectionLogErrorEventArgs> LoggingErrorEvent;

        #endregion

        #endregion

        #region Constructors

        public JsonRpcConnection() : this(null, 0, 0)
        {
            this._commandQueueManager = new CommandQueueManager(this);
        }

        public JsonRpcConnection(string hostname = DEFAULT_HOST, int port = DEFAULT_PORT, int timeout_milliseconds = DEFAULT_TIMEOUT_MILLISECONDS)
        {
            this.Host = hostname;
            this.Port = port;
            this.Timeout = timeout_milliseconds;
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Запрос ключа с сервера
        /// </summary>
        /// <returns></returns>
        private bool CheckKey() => Task.Run(async () => await this.JsonRpc.InvokeAsync<string>("auth", "My_example_KEY")).Result == OK_RESPONSE;

        public async Task<T> ExecuteCommandAsync<T>(string methodName, TimeSpan? timeout = null, Action<Exception> exceptionCallback = null, params object[] parameters)
        {
            var effectiveTimeout = timeout ?? TimeSpan.FromTicks(this.Timeout);
            var commandId = _commandQueueManager.EnqueueCommand(methodName, parameters, effectiveTimeout);
            Command activeCommand = null;
            try
            {
                activeCommand = _commandQueueManager.GetActiveCommands().FirstOrDefault(c => c.Id == commandId);

                if (activeCommand != null)
                    activeCommand.StartedAt = DateTime.UtcNow;

                T result = await InvokeMethodAsync<T>(methodName, parameters, effectiveTimeout);
                _commandQueueManager.RemoveCompletedCommand(commandId);
                return result;
            }
            catch (Exception ex)
            {
                //await _exceptionCallback?.Invoke(_commandQueueManager, activeCommand, ex);
                //await new System.Windows.Threading.Dispatcher()?.BeginInvoke(_exceptionCallback, _commandQueueManager, activeCommand, ex);
                throw new ConnectionLogErrorEventArgs(this.Host, this.Port, ex.Message, ex);
            }
        }

        public async Task<T> ExecuteCommandAsync<T>(string methodName, object parameters = null, TimeSpan? timeout = null, Action<Exception> exceptionCallback = null) => await this.ExecuteCommandAsync<T>(methodName, timeout, exceptionCallback, parameters);
        //{
            //var effectiveTimeout = timeout ?? TimeSpan.FromTicks(this.Timeout);
            //var commandId = _commandQueueManager.EnqueueCommand(methodName, parameters, effectiveTimeout);
            //Command activeCommand = null;
            //try
            //{
            //    activeCommand = _commandQueueManager.GetActiveCommands().FirstOrDefault(c => c.Id == commandId);

            //    if (activeCommand != null)
            //        activeCommand.StartedAt = DateTime.UtcNow;

            //    T result = await InvokeMethodAsync<T>(methodName, parameters, effectiveTimeout);
            //    _commandQueueManager.RemoveCompletedCommand(commandId);
            //    return result;
            //}
            //catch (Exception ex)
            //{
            //    //await _exceptionCallback?.Invoke(_commandQueueManager, activeCommand, ex);
            //    //await new System.Windows.Threading.Dispatcher()?.BeginInvoke(_exceptionCallback, _commandQueueManager, activeCommand, ex);
            //    throw new ConnectionLogErrorEventArgs(this.Host, this.Port, ex.Message, ex);
            //}
        //}

        private async Task<T> InvokeMethodAsync<T>(string methodName, object[] parameters, TimeSpan timeout)
        {
            using (var cts = new CancellationTokenSource(timeout))
            {
                return await this.JsonRpc.InvokeAsync<T>(methodName, parameters).WithCancellation(cts.Token);
            }
        }

        /// <summary>
        /// Проверка отмены команды
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns></returns>
        private bool IsCommandCancelled(string commandId) => !_commandQueueManager.ContainsCommand(commandId);

        //private async Task<T> CreateCommand<T>(string method, object parametr)
        //{
        //    var task = await this.JsonRpc.InvokeAsync<T>(method, parametr);

        //    return await this._commandQueueManager.EnqueueCommandAsync<T>(new Command(method, parametr) { Task = task });
        //}

        //private async Task<T> CreateCommand<T>(string method, params object[] parametrs)
        //{
        //    var task = await this.JsonRpc.InvokeAsync<T>(method, parametrs);
        //    return await this._commandQueueManager.EnqueueCommandAsync<T>(new Command(method, parametrs) { Task = task });
        //}

        //private async Task<T> CreateCommand<T>(string method, params object[] parametrs) => await this.JsonRpc.InvokeAsync<T>(method, parametrs);

        /// <summary>
        /// Срабатывание события подключения
        /// </summary>
        private void OnConnected()
        {
            if (this.Connected == null)
                return;

            this.Connected(this, null);
        }

        /// <summary>
        /// Срабатывание события прерывания подключения
        /// </summary>
        private void OnAborted()
        {
            if (this.Aborted == null)
                return;

            this.Aborted(this, null);
        }

        /// <summary>
        /// Срабатывание события отключения соединения
        /// </summary>
        private void OnDisconnected()
        {
            if (this.Disconnected == null)
                return;

            this.Disconnected(this, null);
        }

        /// <summary>
        /// Вызов события записи в журнал логгирования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoggingEvent(string message) => this.LoggingEvent?.Invoke(this, new ConnectionLogEventArgs(this.Host, this.Port, message));

        /// <summary>
        /// Вызов события записи ошибки в журнал логгирования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoggingErrorEvent(string message, Exception exception = null) => this.LoggingErrorEvent?.Invoke(this, new ConnectionLogErrorEventArgs(this.Host, this.Port, message, exception));

        #endregion Private functions

        #region Public function

        /// <summary>
        /// Открытие соединения
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            bool opened = false;
            try
            {
                this.Client = new TcpClient(this.Host, this.Port)
                {
                    SendTimeout = (int)this.Timeout,
                    ReceiveTimeout = (int)this.Timeout + 500
                };
                this.JsonRpc = new JsonRpc(new NewLineDelimitedMessageHandler(Client.GetStream(), Client.GetStream(), new JsonMessageFormatter()));
                this.JsonRpc.StartListening();
                this.JsonRpc.Disconnected += (sender, e) =>
                {
                    if (this._disposed == 0)
                        this.OnAborted();
                    else
                        this.OnDisconnected();
                };

                if (!this.IsConnected)
                {
                    this.Close();
                    return opened;
                }

                this.OnConnected();
                opened = true;
            }
            catch (SocketException ex)
            {
                throw new Exception($"Не удалось открыть соединение с сервером", ex);
            }
            return opened;
        }

        /// <summary>
        /// Закрытие соединения
        /// </summary>
        public bool Close()
        {
            bool closed = false;
            if (this.Client.Connected)
            {
                this.OnLoggingEvent($"Закрытие соединения . . .");
                try
                {
                    this.Client.Client.Disconnect(false);
                    closed = true;
                    this.OnLoggingEvent($"Соединение закрыто");
                }
                catch (Exception ex)
                {
                    throw new Exception("Не удалось отключиться от TCP-сокета", ex);
                }
            }
            return closed;
        }

        #region Asyn

        /// <summary>
        /// Запрос статуса процесса
        /// </summary>
        /// <returns></returns>
        public async Task<string> Process_StateAsync() => await this.ExecuteCommandAsync<string>("Var_ShowVar", "$PRO_STATE");
        //public async Task<string> Process_StateAsync() => await this.JsonRpc.InvokeAsync<string>("Var_ShowVar", "$PRO_STATE");

        /// <summary>
        /// Название программы
        /// </summary>
        /// <returns></returns>
        public async Task<string> Pro_NameAsync() => await this.ExecuteCommandAsync<string>("Var_ShowVar", "$PRO_NAME[]");
        //public async Task<string> Pro_NameAsync() => await this.JsonRpc.InvokeAsync<string>("Var_ShowVar", "$PRO_NAME[]");

        /// <summary>
        /// Запрос состояния входов
        /// </summary>
        /// <returns></returns>
        public async Task<string> InAsync() => await this.ExecuteCommandAsync<string>("Var_ShowVar", "$IN[]");
        //public async Task<string> InAsync() => await this.JsonRpc.InvokeAsync<string>("Var_ShowVar", "$IN[]");

        /// <summary>
        /// Вывод содержимого файла
        /// </summary>
        /// <param name="sFilePath">Путь файла</param>
        /// <returns></returns>
        public async Task<string> CopyFile2MemAsync(string sFilePath) => await this.ExecuteCommandAsync<string>("File_CopyFile2Mem", sFilePath);
        //public async Task<string> CopyFile2MemAsync(string sFilePath) => await this.JsonRpc.InvokeAsync<string>("File_CopyFile2Mem", sFilePath);

        /// <summary>
        /// Копирование файла в дерективу робота
        /// </summary>
        /// <param name="sFilePath">Директория файла</param>
        /// <param name="sNewPath">Новый путь для файла</param>
        /// <returns></returns>
        public async Task<bool> CopyAsync(string sFilePath, string sNewPath)
        {
            this.OnLoggingEvent($"Копирование файла программы в директорию робота {sNewPath} . . .");
            object[] args = { sFilePath, sNewPath, 64 };
            string result = await this.ExecuteCommandAsync<string>("File_Copy", args);
            //string result = await this.JsonRpc.InvokeAsync<string>("File_Copy", args);
            if (result == OK_RESPONSE)
                return true;
            return false;
        }

        /// <summary>
        /// Копирование содержание файла
        /// </summary>
        /// <param name="sFilePath">Директория файла</param>
        /// <param name="sFinalPath">Конечный путь для</param>
        /// <returns></returns>
        public async Task<bool> CopyMem2FileAsync(string sFilePath, string sFinalPath)
        {
            this.OnLoggingEvent($"Копирование содержание файла {sFilePath} в {sFinalPath} . . .");

            if (!File.Exists(Path.Combine(sFilePath)))
                this.OnLoggingErrorEvent($"Не существует файла {sFilePath}");

            string content;
            using (StreamReader reader = new StreamReader(sFilePath))
            {
                content = await reader.ReadToEndAsync();
            }

            object[] args = { content, sFinalPath, 64 };
            string result = await this.ExecuteCommandAsync<string>("File_CopyMem2File", args);
            //string result = await this.JsonRpc.InvokeAsync<string>("File_CopyMem2File", args);

            if (result == OK_RESPONSE)
                return true;
            return false;
        }

        /// <summary>
        /// Выбор файла программы
        /// </summary>
        /// <param name="sFilePath">Директория файла программы</param>
        /// <returns></returns>
        public async Task<bool> SelectAsync(string sFilePath)
        {
            this.OnLoggingEvent($"Выбор программы {sFilePath} . . .");

            string result = await this.ExecuteCommandAsync<string>("Select_Select", sFilePath);
            //string result = await this.JsonRpc.InvokeAsync<string>("Select_Select", sFilePath);
            return result == OK_RESPONSE;
        }

        /// <summary>
        /// Отмена выбора программы
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SelectCancelAsync()
        {
            this.OnLoggingEvent($"Отмена выбора текущего файла . . .");

            string result = await this.ExecuteCommandAsync<string>("Select_Cancel");
            //string result = await this.JsonRpc.InvokeAsync<string>("Select_Cancel");

            return result == OK_RESPONSE;
        }

        /// <summary>
        /// Удаление файла
        /// </summary>
        /// <param name="sFilePath">Директория файла</param>
        /// <returns></returns>
        public async Task<bool> FileDeleteAsync(string sFilePath)
        {
            this.OnLoggingEvent($"Удаление файла {sFilePath} . . .");

            string result = await this.ExecuteCommandAsync<string>("File_Delete", sFilePath);
            //string result = await this.JsonRpc.InvokeAsync<string>("File_Delete", sFilePath);

            return result == OK_RESPONSE;
        }

        /// <summary>
        /// Запуск уже выбранной программы
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StartAsync()
        {
            this.OnLoggingEvent($"Запуск текущей программы . . .");

            string result = await this.ExecuteCommandAsync<string>("Select_Start");
            //string result = await this.JsonRpc.InvokeAsync<string>("Select_Start");

            return result == OK_RESPONSE;
        }

        /// <summary>
        /// Выбор и запуск программы
        /// </summary>
        /// <param name="sFilePath">Директория файла</param>
        /// <returns></returns>
        public async Task<bool> RunAsync(string sFilePath)
        {
            this.OnLoggingEvent($"Запуск программы {sFilePath} . . .");

            string result = await this.ExecuteCommandAsync<string>("Select_Run", sFilePath);
            //string result = await this.JsonRpc.InvokeAsync<string>("Select_Run", sFilePath);

            return result == OK_RESPONSE;
        }

        /// <summary>
        /// Остановка программы
        /// </summary>
        /// <returns></returns>
        public async Task<bool> PauseAsync()
        {
            this.OnLoggingEvent($"Остановка текущей программы . . .");

            object[] args = { 1 };

            string result = await this.ExecuteCommandAsync<string>("Select_Stop", args);
            //string result = await this.JsonRpc.InvokeAsync<string>("Select_Stop", args);

            return result == OK_RESPONSE;
        }

        /// <summary>
        /// Вывод списка файлов в папке
        /// </summary>
        /// <param name="sFolderPath">Директория папки для запроса</param>
        /// <returns></returns>
        public async Task<Dictionary<String, String>> File_NameListAsync(string sFolderPath = DEFAULT_ROOT)
        {
            Dictionary<String, String> result = new Dictionary<string, string>();
            try
            {
                object[] args = { sFolderPath, 511, 127 };
                result = await this.ExecuteCommandAsync<Dictionary<String, String>>("File_NameList", args);
                //result = await this.JsonRpc.InvokeAsync<Dictionary<String, String>>("File_NameList", args);
            }
            catch (Exception e)
            {
                App.Current.Logger.Error(e, e.Message);
            }
            return result;
        }

        #endregion

        #endregion Public functions

        #region Implementations of IDisposable

        private volatile int _disposed;

        ~JsonRpcConnection() => Dispose(false);

        public void Dispose() => this.Dispose(true);

        public void Dispose(bool disposing)
        {
            if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
            {
                if (disposing)
                {
                    try
                    {
                        lock (this.Client.Client)
                        {
                            this.Client.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Не удалось закрыть TCP-клиент", ex);
                    }
                    finally
                    {
                        this._disposed = 1;
                    }
                    GC.SuppressFinalize(this);
                }
            }
        }

        #endregion
    }
}
