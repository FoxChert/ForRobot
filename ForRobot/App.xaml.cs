using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Windows;
using System.Collections.Specialized;
using System.Security.Cryptography;

using NLog;

namespace ForRobot
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Private variables

        private Mutex _mutex;
        private bool _isNewInstance;
        private const string _mutexName = "InterfaceOfRobots_UniqueAppMutex";
        private const string _pipeName = "InterfaceOfRobots_UniqueAppPipe";
        private CancellationTokenSource _pipeServerCts;
        private ForRobot.Libr.Collections.File3DCollection _openedFiles;

        /// <summary>
        /// Путь к программе на сервере
        /// </summary>
        private string UpdatePath { get => ForRobot.Properties.Settings.Default.UpdatePath; }

        /// <summary>
        /// Путь к программе на коммпьютере
        /// </summary>
        private string FilePathOnPC { get => Directory.GetCurrentDirectory(); }
        
        //private ForRobot.Models.Settings.Settings _settings = ForRobot.App.GetSettings();

        ///// <summary>
        ///// Экземпляр app из app.config
        ///// </summary>
        //private ForRobot.Libr.ConfigurationProperties.AppConfigurationSection AppConfig { get; set; } = (ConfigurationManager.GetSection("app") as ForRobot.Libr.ConfigurationProperties.AppConfigurationSection);

        #endregion

        #region Public variables

        public static new App Current => Application.Current as App;

        public static ForRobot.Libr.Services.Providers.IConfigurationProvider ConfigProvider = new ForRobot.Libr.Configuration.CachedConfigurationProvider(new ForRobot.Libr.Configuration.ConfigurationProvider());
        public static ForRobot.Libr.Services.Providers.IJsonSchemaProvider JsonSchemaProvider = new ForRobot.Libr.Json.Schemas.CachedJsonSchemaProvider(new ForRobot.Libr.Json.Schemas.JsonSchemaProvider());
        public static ForRobot.Libr.Services.Providers.IDetalProvider DetalProvider = new ForRobot.Models.Detals.CachedDetalProvider(new ForRobot.Models.Detals.DetalProvider(ConfigProvider));
        public static ForRobot.Libr.Clipboard.CacheClipboardProvider ClipboardProvider = new Libr.Clipboard.CacheClipboardProvider();

        public Version Version { get; } = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;

        /// <summary>
        /// Директория AvalonDock.config файла, в котором сохраняется макет интерфейса.
        /// </summary>
        public string AvalonConfigPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "AvalonDock.config");

        /// <summary>
        /// Общий логер
        /// </summary>
        public Libr.Logging.Logger Logger { get; } = new Libr.Logging.Logger();

        /// <summary>
        /// Настройки приложения
        /// (выгружаются из временных файлов, иначе инициализируются как класс)
        /// </summary>
        public ForRobot.Models.Settings.Settings Settings { get => GetSettings(); }

        /// <summary>
        /// Открытые файлы 3D моделей
        /// </summary>
        public ForRobot.Libr.Collections.File3DCollection OpenedFiles
        {
            get
            {
                if (this._openedFiles == null)
                {
                    this._openedFiles = new ForRobot.Libr.Collections.File3DCollection();
                    this._openedFiles.CollectionChanged += (s, e) =>
                    {
                        switch (e.Action)
                        {
                            case NotifyCollectionChangedAction.Add:
                                for(int i=0; i<e.NewItems.Count; i++)
                                {
                                    var file = e.NewItems[i] as ForRobot.Models.File3D.File3D;
                                    if (file == null)
                                        return;
                                    ClipboardProvider.GetOrAddStacks(file.Path);
                                    file.SetUndoRedoManager(ClipboardProvider);
                                }
                                break;

                            case NotifyCollectionChangedAction.Remove:
                            case NotifyCollectionChangedAction.Replace:
                            case NotifyCollectionChangedAction.Reset:
                                for (int i = 0; i < e.OldItems.Count; i++)
                                {
                                    var file = e.NewItems[i] as ForRobot.Models.File3D.File3D;
                                    if (file == null)
                                        return;
                                    ClipboardProvider.RemoveStacks(file.Path);
                                }

                                for (int i = 0; i < e.NewItems.Count; i++)
                                {
                                    var file = e.NewItems[i] as ForRobot.Models.File3D.File3D;
                                    if (file == null)
                                        return;
                                    ClipboardProvider.GetOrAddStacks(file.Path);
                                    file.SetUndoRedoManager(ClipboardProvider);
                                }
                                break;
                        }
                    };
                }
                return this._openedFiles;
            }
            set => this._openedFiles = value;
        }
        
        #endregion Public variables

        #region Private functions

        /// <summary>
        /// Запуск программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [STAThread]
        private async void OnStartUp(object sender, StartupEventArgs e)
        {
            try
            {
                _mutex = new Mutex(true, _mutexName, out _isNewInstance);

                if (!_isNewInstance)
                {
                    SendArgumentsToExistingInstance(e.Args);
                    Application.Current.Shutdown(0);
                    return;
                }

                this.Logger.Trace("Запуск приложения");

                // Установка провайдеров
                ForRobot.Libr.Factories.File3DFactory.SetDetalProvider(DetalProvider);
                ForRobot.Libr.Factories.File3DFactory.SetJsonSchemaProvider(JsonSchemaProvider);

                RunApplication(e.Args);
                await Task.Run(() => StartPipeServer());

                GC.KeepAlive(_mutex);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                MessageBox.Show(ex.Message + "\t||\t" + ex.Source, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }
        
        /// <summary>
        /// Детектит исключения в течении работы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message + "\t||\t" + e.Exception.Source, "", MessageBoxButton.OK);
            Logger.Fatal(e.Exception, e.Exception.Message);
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            if (((Application.Current.Windows.Count == 0) && (Application.Current.ShutdownMode == ShutdownMode.OnLastWindowClose))
                || (Application.Current.ShutdownMode == ShutdownMode.OnMainWindowClose))
            {
                //if (Settings.SaveDetalProperties)
                //{
                //    Services.File3DService.SaveFiles(OpenedFiles.Where(item => new List<string>() { ForRobot.Properties.Settings.Default.PlitaProgramm,
                //                                                                                  ForRobot.Properties.Settings.Default.PlitaStringerProgramm,
                //                                                                                  ForRobot.Properties.Settings.Default.PlitaTreugolnikProgramm
                //                                                                                }.Contains(item.NameWithoutExtension)));
                //}

                if (this._isNewInstance)
                    this.Logger.Trace("Закрытие приложения\n\n");

                _pipeServerCts?.Cancel(); // Отмена сервера каналов.
                this._mutex?.Dispose();
                Application.Current.Shutdown(0);
            }
        }

        /// <summary>
        /// Запуск приложения
        /// </summary>
        /// <param name="args"></param>
        private void RunApplication(string[] args)
        {
            // Проверка версии файла в папке с обновлением
            if (ValidateUpdateApplication(out string updatePath) &&
                (!Settings.InformUser || MessageBox.Show($"Обнаружено обновление до версии {FileVersionInfo.GetVersionInfo(updatePath).ProductVersion}\nОбновить приложение?", "Обновление интерфейса", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly) == MessageBoxResult.OK))
            {
                this.Logger.Trace($"Обновление приложения до версии {FileVersionInfo.GetVersionInfo(updatePath).ProductVersion}");
                App.Current.UpdateApplication(args);
            }

            // Обновление настроек приложения к пользовательским.
            if (ForRobot.Properties.Settings.Default.IsSettingsUpgradeRequired)
            {
                ForRobot.Properties.Settings.Default.Upgrade();
                ForRobot.Properties.Settings.Default.Reload();
                ForRobot.Properties.Settings.Default.IsSettingsUpgradeRequired = false;
                ForRobot.Properties.Settings.Default.Save();
            }

            Application.Current.MainWindow = Libr.AppWindowManager.AppMainWindowShow();

            // Вход в приложение по пин-коду
            if (this.Settings.LoginByPINCode)
            {
                bool pinResult = false;

                // Выполняем проверку пин-кода в UI потоке
                Application.Current.Dispatcher.Invoke(() =>
                {
                    pinResult = Libr.AppWindowManager.PinCodeInputWindowShow(ForRobot.Properties.Settings.Default.PinCode);
                });

                if (!pinResult)
                {
                    this.Logger.Error("Ошибка при входе: неверный пин-код!");
                    Application.Current.Shutdown(1);
                    return;
                }
            }

            foreach (var i in args) // Исп. для открытия файла модели "с помощью"
                this.OpenedFiles.Add(Models.File3D.File3D.Load(i));

            InitializeGlobalValable();
            Application.Current.MainWindow.Show();
            SelectAppMainWindow();
        }

        /// <summary>
        /// Проверка обновления приложения на сервере
        /// </summary>
        /// <param name="sUpdatePath"></param>
        /// <returns></returns>
        private bool ValidateUpdateApplication(out string sUpdatePath)
        {
            string path = sUpdatePath = Path.Combine(App.Current.UpdatePath, $"{ResourceAssembly.GetName().Name}.exe");

            if (!Settings.AutoUpdate)
                return false;

            var taskExistAppFiles = new Task<bool>(() => File.Exists(path));
            var taskUpdateApp = Task.WhenAny(taskExistAppFiles, Task.Delay(3000)); // Проверка существования файлов для обновления, ограничено по времени.

            bool fileExists = false;
            if (taskUpdateApp.Result == taskExistAppFiles)
            {
                fileExists = taskExistAppFiles.Result;
            }

            return fileExists && new Version(FileVersionInfo.GetVersionInfo(path).ProductVersion) > Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <summary>
        /// Обновление программы.
        /// Копирует файлы из каталога на сервере в нынешнюю директорию программы и перезапускает её
        /// </summary>
        /// <param name="e">Аргументы командной стоки</param>
        private void UpdateApplication(string[] args)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = this.FilePathOnPC,
                    CreateNoWindow = true,
                    FileName = "cmd.exe",
                    Arguments = $"/K taskkill /im {ResourceAssembly.GetName().Name}.exe /f& " +
                                $"xcopy \"{this.UpdatePath + "\\*.*"}\" \"{this.FilePathOnPC}\" /E /Y& " +
                                $"START \"\" \"{this.FilePathOnPC + "\\" + ResourceAssembly.GetName().Name + ".exe"}\" \"{string.Join("\" \"", args)}\"",
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                }
            };
            new Thread(() => process.Start()).Start();
        }

        /// <summary>
        /// Передача аргументов уже существующему экземпляру приложения
        /// </summary>
        /// <param name="args"></param>
        private void SendArgumentsToExistingInstance(string[] args)
        {
            try
            {
                using (var client = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out))
                {
                    client.Connect(2000); // Таймаут 2 секунды
                    using (var writer = new StreamWriter(client))
                    {
                        foreach (var arg in args)
                        {
                            writer.WriteLine(arg);
                        }
                    }
                }
            }
            catch (TimeoutException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Запуск сервера для перехвата аргументов
        /// </summary>
        private void StartPipeServer()
        {
            _pipeServerCts = new CancellationTokenSource();
            while (!_pipeServerCts.IsCancellationRequested)
            {
                try
                {
                    using (var server = new NamedPipeServerStream(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                    {
                        // Асинхронное ожидание соединения с возможностью отмены
                        var asyncResult = server.BeginWaitForConnection(null, null);
                        WaitHandle.WaitAny(new[] { asyncResult.AsyncWaitHandle, _pipeServerCts.Token.WaitHandle });

                        if (_pipeServerCts.IsCancellationRequested)
                        {
                            server.Close();
                            return;
                        }

                        server.EndWaitForConnection(asyncResult);

                        if (Application.Current == null || Application.Current.Dispatcher == null || Application.Current.Dispatcher.HasShutdownStarted) return;

                        using (var reader = new StreamReader(server))
                        {
                            var args = reader.ReadToEnd().Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                            if (args.Length == 0) continue;

                            bool isMainWindowReady = false; // Готовность главного окна.
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                isMainWindowReady = MainWindow != null && MainWindow.IsInitialized;
                            });

                            if (!isMainWindowReady)
                            {
                                var readyWait = new System.Threading.ManualResetEventSlim();
                                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    HandleArguments(args);
                                    readyWait.Set();
                                }));

                                if (!readyWait.Wait(TimeSpan.FromSeconds(5))) Logger.Warn("Таймаут ожидания главного окна");
                            }
                            else
                            {
                                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                                {
                                    try
                                    {
                                        HandleArguments(args);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex, "Ошибка обработки аргументов");
                                    }
                                }));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Ошибка в сервере каналов");
                    Thread.Sleep(1000); // Пауза перед повторной попыткой
                }                
            }
        }

        private void HandleArguments(string[] args)
        {
            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (File.Exists(arg))
                    {
                        this.OpenedFiles.Add(Models.File3D.File3D.Load(arg));
                    }
                }
            }

            if (MainWindow?.IsVisible == true)
            {
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        this.SelectAppMainWindow();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Ошибка активации окна");
                    }
                });
            }
        }

        /// <summary>
        /// Инициализация глобальных типов и т.п.
        /// </summary>
        private void InitializeGlobalValable()
        {
            Libr.Factories.DetalFactory.DetalFactory.ValidatedError += (exception) => Logger.Error(exception);

            if (ForRobot.Properties.Settings.Default.SaveRobots == null)
                ForRobot.Properties.Settings.Default.SaveRobots = new StringCollection();

            // Если нет открываемых файлов, проверяет в настройках - нужно ли создать файл детали.
            if (OpenedFiles.Count == 0 && Settings.CreatedDetalFile)
            {
                string programName = Settings.GetStandartProgramName(Settings.StartedDetalType);
                string path = Path.Combine(Path.GetTempPath(), programName);

                Models.File3D.File3D file3D;
                if (Settings.SaveDetalProperties && File.Exists(path))
                {
                    file3D = ForRobot.Models.File3D.File3D.Load(path);
                }
                else
                {
                    file3D = Models.File3D.NativeFile3D.Create(path, Settings.StartedDetalType);
                }

                if (Settings.SaveDetalProperties)
                    file3D.PropertyChanged += (s, e) => Application.Current.Dispatcher.BeginInvoke(new Action(() => (s as Models.File3D.File3D).Save()));

                OpenedFiles.Add(file3D);
            }
        }

        /// <summary>
        /// Установка настроек приложения
        /// </summary>
        /// <returns></returns>
        private ForRobot.Models.Settings.Settings GetSettings()
        {
            ForRobot.Models.Settings.Settings settings = ForRobot.Models.Settings.Settings.GetSettings();
            var robotConfig = ConfigProvider.GetRobotConfig();
            var plateConfig = ConfigProvider.GetPlateConfig();

            foreach (var names in settings.DetalsProgramNames.Where(x => x.Item1 == Models.Detals.DetalType.Plate).ToList())
                settings.DetalsProgramNames.Remove(names);
            settings.DetalsProgramNames.Add(Tuple.Create(Models.Detals.DetalType.Plate, Libr.EnumExtensions.GetDescription(Models.Detals.DetalType.Plate), plateConfig.PlateProgramName));

            foreach (var names in settings.DetalsScriptNames.Where(x => x.Item1 == Models.Detals.DetalType.Plate).ToList())
                settings.DetalsScriptNames.Remove(names);
            settings.DetalsScriptNames.Add(Tuple.Create(Models.Detals.DetalType.Plate, Libr.EnumExtensions.GetDescription(Models.Detals.DetalType.Plate), plateConfig.PlateScriptName));

            settings.PathFolderOfGeneration = robotConfig.PathFolderGeneration;
            settings.ControlerFolder = robotConfig.ControlFolderPath;
            return settings;
        }

        #endregion Private functions

        #region Public functions

        /// <summary>
        /// Вывод и вокусировка главного окна приложения
        /// </summary>
        public void SelectAppMainWindow()
        {
            if (App.Current.MainWindow.WindowState == WindowState.Minimized)
            {
                App.Current.MainWindow.WindowState = WindowState.Normal;
            }
            App.Current.MainWindow.Topmost = true;
            App.Current.MainWindow.Topmost = false;

            App.Current.MainWindow.Activate();

            App.Current.MainWindow.Focus();
            App.Current.MainWindow.Left = SystemParameters.WorkArea.Left;
            App.Current.MainWindow.Top = SystemParameters.WorkArea.Top;
        }

        #endregion
    }
}