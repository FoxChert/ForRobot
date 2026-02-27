using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

using GalaSoft.MvvmLight.Messaging;

using ForRobot.Models;
using ForRobot.Models.RoboticComplex;
using ForRobot.Libr.Collections;

namespace ForRobot.ViewModels
{
    public class MainPageViewModel3_new : BaseClass
    {
        #region Private variables

        private FullyObservableCollection<Robot> _robotsCollection;
        private ObservableCollection<ForRobot.Models.Message> _messagesCollection = new ObservableCollection<ForRobot.Models.Message>();

        #endregion Private variables

        #region Public variables

        #region Collections

        /// <summary>
        /// Коллекция всех добаленнных роботов
        /// </summary>
        public FullyObservableCollection<Robot> RobotsCollection
        {
            get => this._robotsCollection;
            set
            {
                if (this._robotsCollection != null)
                {
                    this._robotsCollection.ItemPropertyChanged -= HandleRobotPropertyChanged;
                    this._robotsCollection.CollectionChanged -= HandleRobotsCollectionChanged;
                }

                Set(ref this._robotsCollection, value);

                if (this._robotsCollection != null)
                {
                    this._robotsCollection.ItemPropertyChanged += HandleRobotPropertyChanged;
                    this._robotsCollection.CollectionChanged += HandleRobotsCollectionChanged;
                }
            }
        }
        /// <summary>
        /// Коллекция сообщений
        /// </summary>
        public ObservableCollection<Message> MessagesCollection { get => this._messagesCollection; set => Set(ref this._messagesCollection, value); }

        #endregion Collections

        #region Commands

        /// <summary>
        /// Выгрузка макета
        /// </summary>
        public ICommand LoadedCommand { get; } = new RelayCommand(_ => Messenger.Default.Send(new Libr.Messages.LoadLayoutMessage()));
        /// <summary>
        /// Сброс фокуса
        /// </summary>
        public ICommand LostFocusCommand { get; } = new RelayCommand(obj => LostFocus(obj as FrameworkElement));
        /// <summary>
        /// Команда создания файла
        /// </summary>
        public ICommand CreateNewFileCommand { get; }
        /// <summary>
        /// Команда открытия файла
        /// </summary>
        public ICommand OpenedFileCommand { get; }
        /// <summary>
        /// Команда сохранения выбранного файла
        /// </summary>
        public ICommand SaveFileCommand { get; }
        /// <summary>
        /// Команда сохранения файла как
        /// </summary>
        public ICommand SaveAsFileCommand { get; }
        /// <summary>
        /// Команда сохранения всех открытых файлов
        /// </summary>
        public ICommand SaveAllFilesCommand { get; }
        /// <summary>
        /// Команда выхода из приложения
        /// </summary>
        public ICommand ExitCommand { get; }
        /// <summary>
        /// Команда отмены последнего изменения в выбранном файле
        /// </summary>
        public ICommand UndoCommand { get; }
        /// <summary>
        /// Команда возврата последнего изменения в выбранном файле
        /// </summary>
        public ICommand RedoCommand { get; }
        /// <summary>
        /// Команда генерации программы и её выбор на роботе/ах
        /// </summary>
        public ICommand GenerateProgramCommand { get; }
        /// <summary>
        ///  Команда сброса параметров детали до стандартных
        /// </summary>
        public ICommand StandartParametrsCommand { get; }
        /// <summary>
        /// Команда изменения видимости панели
        /// </summary>
        public ICommand ChangeVisibleLayoutAnchorableCommand { get; }
        /// <summary>
        /// Команда добавления робота
        /// </summary>
        public ICommand AddRobotCommand { get; }
        /// <summary>
        /// УКоманда удаление робота
        /// </summary>
        public ICommand DeleteRobotCommand { get; }
        /// <summary>
        /// Команда (повторного) открытия соединения с роботом
        /// </summary>
        public ICommand ConnectedRobotCommand { get; }
        /// <summary>
        /// Команда закрытия соединения с роботом
        /// </summary>
        public ICommand DisconnectedRobotCommand { get; }
        /// <summary>
        /// Команда переименования робота
        /// </summary>
        public ICommand RenameRobotCommand { get; }
        /// <summary>
        /// Команда запуска программы на роботе
        /// </summary>
        public ICommand RunProgramCommand { get; }
        /// <summary>
        /// Команда удержания кнопки запуска
        /// </summary>
        public ICommand RetentionRunButtonCommand { get; }
        /// <summary>
        /// Команда остановки программы на роботе
        /// </summary>
        public ICommand PauseProgramCommand { get; }
        /// <summary>
        /// Команда аннулирования программы на роботе
        /// </summary>
        public IAsyncCommand CancelProgramCommand { get; }
        /// <summary>
        /// Команда открытия окна настроек
        /// </summary>
        public ICommand PropertiesCommand { get; }
        /// <summary>
        /// Команда открытия chm-справки
        /// </summary>
        public ICommand HelpCommand { get; } = new RelayCommand(_ => System.Windows.Forms.Help.ShowHelp(null, "Help/HelpManual.chm"));

        #endregion Commands

        #endregion Public variables

        public MainPageViewModel3_new()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            ForRobot.Libr.Logging.Logger.LoggingEvent += (s, o) => System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => this.MessagesCollection.Add(new Models.Message(o))));

        }

        #region Private functions

        /// <summary>
        /// Сброс фокуса на заданный элемент
        /// </summary>
        /// <param name="frameworkElement"></param>
        private static void LostFocus(FrameworkElement frameworkElement)
        {
            System.Windows.Input.Keyboard.ClearFocus();
            System.Windows.Input.FocusManager.SetFocusedElement(System.Windows.Input.FocusManager.GetFocusScope(frameworkElement), null);
        }

        #endregion Private functions

        #region Public functions



        #endregion Public functions
    }
}
