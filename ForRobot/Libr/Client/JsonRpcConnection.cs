using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Sockets;

using StreamJsonRpc;

namespace ForRobot.Libr.Client
{
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
        private static readonly Action<JsonRpcConnection, Command, Exception> _exceptionCallback = new Action<JsonRpcConnection, Command, Exception>((sender, command, e) =>
        {
            CommandQueueManager manager = (sender as JsonRpcConnection)._commandQueueManager;
            try
            {
                throw e;
            }
            catch (OperationCanceledException) when (!manager.ContainsCommand(command.Id))
            {
                manager.CancelCommand(command.Id);
                throw new OperationCanceledException($"Команда '{command.MethodName}' была отменена");
            }
            catch (OperationCanceledException) when (command.CancellationTokenSource.IsCancellationRequested)
            {
                command.CompletedAt = DateTime.UtcNow;

                if (command.CancellationTokenSource.Token.IsCancellationRequested)
                {
                    //manager.OnCommandTimedOut(command);
                    throw new TimeoutException();
                }
                else
                {
                    manager.CancelCommand(command.Id);
                    return;
                }
            }
            catch (TimeoutException)
            {
                throw new TimeoutException($"Время ожидания команды {command.MethodName} превысело установленный лимит {command.Timeout.Seconds} сек.");
            }
            catch (Exception ex)
            {
                command.CompletedAt = DateTime.UtcNow;
                //throw ex;
                sender.OnLoggingErrorEvent(ex.Message, ex);
            }
            finally
            {
                manager.RemoveCompletedCommand(command.Id);
            }
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

                return this.CheckKey().Result;
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

        public JsonRpcConnection(string hostname = DEFAULT_HOST, int port = DEFAULT_PORT, int timeout_milliseconds = DEFAULT_TIMEOUT_MILLISECONDS)
        {
            this.Host = hostname;
            this.Port = port;
            this.Timeout = timeout_milliseconds;
            this._commandQueueManager = new CommandQueueManager(this);
        }

        #endregion

        #region Private functions

        public async Task<T> ExecuteCommandAsync<T>(string methodName, Action<Exception> exceptionCallback = null) => await ExecuteCommandAsync<T>(methodName, (object[])null, exceptionCallback);

        public async Task<T> ExecuteCommandAsync<T>(string methodName, object parameter, Action<Exception> exceptionCallback = null)
        {
            object[] parameters = parameter != null ? new object[] { parameter } : null;
            return await ExecuteCommandAsync<T>(methodName, parameters, exceptionCallback);
        }

        public async Task<T> ExecuteCommandAsync<T>(string methodName, object[] parameters, Action<Exception> exceptionCallback = null)
        {
            if (_disposed == 1)
                throw new ObjectDisposedException(nameof(CommandQueueManager));

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException("Method name cannot be null or empty", nameof(methodName));

            TimeSpan effectiveTimeout = TimeSpan.FromMilliseconds(this.Timeout);
            var commandId = _commandQueueManager.EnqueueCommand(methodName, parameters, effectiveTimeout);
            Command activeCommand = _commandQueueManager.GetActiveCommands().FirstOrDefault(c => c.Id == commandId);

            if (activeCommand != null)
                activeCommand.StartedAt = DateTime.UtcNow;
            try
            {
                T result = await InvokeMethodAsync<T>(methodName, parameters, effectiveTimeout);

                //if (activeCommand != null)
                //    activeCommand.CompletedAt = DateTime.UtcNow;

                _commandQueueManager.RemoveCompletedCommand(commandId);
                return result;
            }
            catch (Exception ex)
            {
                _exceptionCallback?.Invoke(this, activeCommand, ex);
                throw;
                //await _exceptionCallback?.Invoke(_commandQueueManager, activeCommand, ex);
                //await new System.Windows.Threading.Dispatcher()?.BeginInvoke(_exceptionCallback, _commandQueueManager, activeCommand, ex);
                //throw new ConnectionLogErrorEventArgs(this.Host, this.Port, ex.Message, ex);
            }
        }

        private async Task<T> InvokeMethodAsync<T>(string methodName, object[] parameters, TimeSpan timeout)
        {
            using (var cts = new CancellationTokenSource(timeout))
            {
                return await this.JsonRpc.InvokeAsync<T>(methodName, parameters).WithCancellation(cts.Token);
            }
        }

        /// <summary>
        /// Запрос ключа с сервера
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CheckKey()
        {
            try
            {
                string result = await ExecuteCommandAsync<string>("auth", "My_example_KEY");
                return result == OK_RESPONSE;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка отмены команды
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns></returns>
        private bool IsCommandCancelled(string commandId) => !_commandQueueManager.ContainsCommand(commandId);

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
                    if (Interlocked.CompareExchange(ref _disposed, 0, 0) == 0)
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

        /// <summary>
        /// Название программы
        /// </summary>
        /// <returns></returns>
        public async Task<string> Pro_NameAsync() => await this.ExecuteCommandAsync<string>("Var_ShowVar", "$PRO_NAME[]");

        /// <summary>
        /// Запрос состояния входов
        /// </summary>
        /// <returns></returns>
        public async Task<string> InAsync() => await this.ExecuteCommandAsync<string>("Var_ShowVar", "$IN[]");

        /// <summary>
        /// Вывод содержимого файла
        /// </summary>
        /// <param name="sFilePath">Путь файла</param>
        /// <returns></returns>
        public async Task<string> CopyFile2MemAsync(string sFilePath) => await this.ExecuteCommandAsync<string>("File_CopyFile2Mem", sFilePath);

        /// <summary>
        /// Копирование файла в дерективу робота
        /// </summary>
        /// <param name="sFilePath">Директория файла</param>
        /// <param name="sNewPath">Новый путь для файла</param>
        /// <returns></returns>
        public async Task<bool> CopyAsync(string sFilePath, string sNewPath)
        {
            if (string.IsNullOrWhiteSpace(sFilePath) || string.IsNullOrWhiteSpace(sNewPath))
                throw new ArgumentException("File paths cannot be null or empty");

            this.OnLoggingEvent($"Копирование файла программы в директорию робота {sNewPath} . . .");
            object[] args = { sFilePath, sNewPath, 64 };
            string result = await this.ExecuteCommandAsync<string>("File_Copy", args);
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

            if (!File.Exists(sFilePath))
                throw new FileNotFoundException($"Файл {sFilePath} не найден!", sFilePath);

            string content;
            using (StreamReader reader = new StreamReader(sFilePath))
            {
                content = await reader.ReadToEndAsync();
            }

            object[] args = { content, sFinalPath, 64 };
            string result = await this.ExecuteCommandAsync<string>("File_CopyMem2File", args);

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
                        this.JsonRpc.Dispose();
                        lock (this.Client?.Client)
                        {
                            this.Client?.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Не удалось закрыть TCP-клиент", ex);
                    }
                    finally
                    {
                        this.Client.Dispose();
                        this._disposed = 1;
                    }
                    GC.SuppressFinalize(this);
                }
            }
        }

        #endregion
    }
}
