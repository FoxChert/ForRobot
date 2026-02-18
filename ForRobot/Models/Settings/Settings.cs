using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using AvalonDock.Themes;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using HelixToolkit.Wpf;

using ForRobot.Libr;
using ForRobot.Models.Detals;

namespace ForRobot.Models.Settings
{
    /// <summary>
    /// Класс представляющий настройки приложения
    /// </summary>
    public class Settings : ICloneable
    {
        #region Private variables
        
        private static string _path = Path.Combine(Path.GetTempPath(), Settings.FileName);

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        private Tuple<string, Theme> _selectedTheme;

        private Dictionary<string, System.Windows.Media.Color> _colors = new Dictionary<string, System.Windows.Media.Color>();
        
        #region Properties

        private bool _showCoordinateSystem = true;
        private bool _showViewCube = true;
        private bool _showTriangleCountInfo = false;
        private bool _orthographic = false;
        private bool _showCameraInfo = false;
        private bool _showCameraTarget = true;
        private bool _rotateAroundMouseDownPoint = false;
        private bool _zoomAroundMouseDownPoint = false;
        private bool _isInertiaEnabled = false;
        private bool _isPanEnabled = true;
        private bool _isMoveEnabled = true;
        private bool _isRotationEnabled = true;
        private bool _isZoomEnabled = true;

        #endregion

        #endregion Private variables

        #region Public variables

        /// <summary>
        /// Наименование файла настроек
        /// </summary>
        public const string FileName = "interfaceOfRobot_settings.json";

        #region Properties

        #region Generic

        public Version Version { get; } = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;

        /// <summary>
        /// Обновляется ли приложение автоматически
        /// </summary>
        public bool AutoUpdate { get; set; } = true;

        /// <summary>
        /// Спрашивать пользователя об обновлении
        /// </summary>
        public bool InformUser { get; set; } = true;

        /// <summary>
        /// Вход в приложение интерфейса по пин-коду
        /// </summary>
        public bool LoginByPINCode { get; set; } = false;

        /// <summary>
        /// Создаётся ли при открытии файл детли. Не работает, если приложение открывает файл "с помощью"
        /// </summary>
        public bool CreatedDetalFile { get; set; } = true;

        /// <summary>
        /// Сохраняются ли параметры детали на выходе
        /// </summary>
        public bool SaveDetalProperties { get; set; } = true;

        /// <summary>
        /// Ограничено ли время ожидания ответа от сервера
        /// </summary>
        public bool LimitedConnectionTimeOut { get; set; } = false; // Не используется, хм

        /// <summary>
        /// Время ожидания ответа от сервера, сек.
        /// </summary>
        public double ConnectionTimeOut { get; set; } = 3; // Не используется

        /// <summary>
        /// Тип детали, для которой создаётся стартовый файл
        /// </summary>
        public DetalType StartedDetalType { get; set; } = Models.Detals.DetalType.Plate;
        
        #endregion Generic

        #region Navigation

        /// <summary>
        /// Отображаютя ли файлы с расширением .dat
        /// </summary>
        public bool AccessDataFile { get; set; } = false;

        /// <summary>
        /// Доступность системных папок в дереве файлов
        /// </summary>
        public SortedDictionary<string, bool> AvailableFolders { get; set; } = new SortedDictionary<string, bool>()
                                                                                    {
                                                                                        { "System", false },
                                                                                        { "Mada", false },
                                                                                        { "TP", false },
                                                                                        { "STEU", false }
                                                                                    };

        #endregion

        #region View

        [JsonIgnore]
        public List<Tuple<string, Theme>> Themes { get; } = new List<Tuple<string, Theme>>()
        {
            new Tuple<string, Theme>(nameof(GenericTheme), new GenericTheme()),
            new Tuple<string, Theme>(nameof(AeroTheme),new AeroTheme()),
            new Tuple<string, Theme>(nameof(ExpressionDarkTheme),new ExpressionDarkTheme()),
            new Tuple<string, Theme>(nameof(ExpressionLightTheme),new ExpressionLightTheme()),
            new Tuple<string, Theme>(nameof(MetroTheme),new MetroTheme()),
            //new Tuple<string, Theme>(nameof(VS2010Theme),new VS2010Theme()),
            new Tuple<string, Theme>(nameof(Vs2013BlueTheme),new Vs2013BlueTheme()),
            new Tuple<string, Theme>(nameof(Vs2013DarkTheme),new Vs2013DarkTheme()),
            new Tuple<string, Theme>(nameof(Vs2013LightTheme),new Vs2013LightTheme())
        };        
        [JsonIgnore]
        public Tuple<string, Theme> SelectedTheme
        {
            get => this._selectedTheme;
            set
            {
                this._selectedTheme = value;
                Properties.Settings.Default.SelectedTheme = this._selectedTheme.Item1;
                Properties.Settings.Default.Save();
            }
        }

        #region 3DView

        /// <summary>
        /// Масштабный коэффициент: 1 единица модели = <see cref="ScaleFactor"/> мм. реальных размеров
        /// </summary>
        public static decimal ScaleFactor { get; set; } = 1.00M / 100.00M;

        /// <summary>
        /// Показ системы координат
        /// </summary>
        public bool ShowCoordinateSystem
        {
            get => this._showCoordinateSystem;
            set
            {
                this._showCoordinateSystem = value;
                this.OnPropertyChanged();
            }
        }
        /// <summary>
        /// Показ пространственного куба
        /// </summary>
        public bool ShowViewCube
        {
            get => this._showViewCube;
            set
            {
                this._showViewCube = value;
                this.OnPropertyChanged();
            }
        }
        /// <summary>
        /// Доступности нажатия рёбер пространственного куба
        /// </summary>
        public bool IsViewCubeEdgeClicksEnabled { get; set; } = false;
        /// <summary>
        /// Показ кол-ва полигонов
        /// </summary>
        public bool ShowTriangleCountInfo
        {
            get => this._showTriangleCountInfo;
            set
            {
                this._showTriangleCountInfo = value;
                this.OnPropertyChanged();
            }
        }

        public bool ShowFieldOfView { get; set; } = false;
        public bool ShowFrameRate { get; set; } = false;

        /// <summary>
        /// Вертикальное положение системы координат
        /// </summary>
        public VerticalAlignment CoordinateSystemVerticalPosition { get; set; } = VerticalAlignment.Top;
        /// <summary>
        /// Горизонтальное положение системы координат
        /// </summary>
        public HorizontalAlignment CoordinateSystemHorizontalPosition { get; set; } = HorizontalAlignment.Right;

        /// <summary>
        /// Перечень цветов
        /// </summary>
        public Dictionary<string, System.Windows.Media.Color> Colors
        {
            get => this._colors;
            set
            {
                this._colors = value;

                foreach(var c in this._colors) // Установка класса Colors.
                {
                    SetColor(c.Key, c.Value);
                }
            }
        }

        #endregion 3DView

        #region Params

        private double _annotationFontSize = 20;
        private double _weldsThickness = 5.0;
        private double _annotationThickness = 2.0;
        //private bool _annotationIsVisibale = true;

        /// <summary>
        /// Толщина линий шва на модели
        /// </summary>
        public double WeldsThickness
        {
            get => this._weldsThickness;
            set
            {
                this._weldsThickness = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Размер шрифта
        /// </summary>
        public double AnnotationFontSize
        {
            get => this._annotationFontSize;
            set
            {
                this._annotationFontSize = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Толщина линий
        /// </summary>
        public double AnnotationThickness
        {
            get => this._annotationThickness;
            set
            {
                this._annotationThickness = value;
                this.OnPropertyChanged();
            }
        }

        ///// <summary>
        ///// Видимы ли параметры
        ///// </summary>
        //public bool AnnotationIsVisibale
        //{
        //    get => this._annotationIsVisibale;
        //    set
        //    {
        //        this._paramsIsVisibale = value;
        //        this.OnPropertyChanged();
        //    }
        //}

        #endregion Params

        #region Camera

        /// <summary>
        /// Включена ли ортоганальная камера
        /// </summary>
        public bool Orthographic
        {
            get => this._orthographic;
            set
            {
                this._orthographic = value;
                this.OnPropertyChanged();
            }
        }
        /// <summary>
        /// Демонстрация информации о камере
        /// </summary>
        public bool ShowCameraInfo
        {
            get => this._showCameraInfo;
            set
            {
                this._showCameraInfo = value;
                this.OnPropertyChanged();
            }
        }
        /// <summary>
        /// Демонстрация курсора камеры
        /// </summary>
        public bool ShowCameraTarget
        {
            get => this._showCameraTarget;
            set
            {
                this._showCameraTarget = value;
                this.OnPropertyChanged();
            }
        }
        /// <summary>
        /// Поворот вокруг мыши
        /// </summary>
        public bool RotateAroundMouseDownPoint
        {
            get => this._rotateAroundMouseDownPoint;
            set
            {
                this._rotateAroundMouseDownPoint = value;
                this.OnPropertyChanged();
            }
        }
        /// <summary>
        /// Приближение около мыши
        /// </summary>
        public bool ZoomAroundMouseDownPoint
        {
            get => this._zoomAroundMouseDownPoint;
            set
            {
                this._zoomAroundMouseDownPoint = value;
                this.OnPropertyChanged();
            }
        }
        /// <summary>
        /// Инерция камеры
        /// </summary>
        public bool IsInertiaEnabled
        {
            get => this._isInertiaEnabled;
            set
            {
                this._isInertiaEnabled = value;
                this.OnPropertyChanged();
            }
        }
        /// <summary>
        /// Вкличено ли панорамирование
        /// </summary>
        public bool IsPanEnabled
        {
            get => this._isPanEnabled;
            set
            {
                this._isPanEnabled = value;
                this.OnPropertyChanged();
            }
        }
        /// <summary>
        /// Вкличено ли перемещение
        /// </summary>
        public bool IsMoveEnabled
        {
            get => this._isMoveEnabled;
            set
            {
                this._isMoveEnabled = value;
                this.OnPropertyChanged();
            }
        }
        /// <summary>
        /// Вкличено ли вращение
        /// </summary>
        public bool IsRotationEnabled
        {
            get => this._isRotationEnabled;
            set
            {
                this._isRotationEnabled = value;
                this.OnPropertyChanged();
            }
        }
        /// <summary>
        /// Вкличено ли маштабирование
        /// </summary>
        public bool IsZoomEnabled
        {
            get => this._isZoomEnabled;
            set
            {
                this._isZoomEnabled = value;
                this.OnPropertyChanged();
            }
        }

        ///// <summary>
        ///// Включено ли панаромирование
        ///// </summary>
        //public bool IsChangeFieldOfViewEnabled { get; set; } = true;

        /// <summary>
        /// Режим поворота камеры
        /// <example>
        /// <para/>Turntable - is constrained to two axes of rotation (model up and right direction).
        /// <para/>Turnball - using three axes (look direction, right direction and up direction (on the left/right edges)).
        /// <para/>Trackball - using a virtual trackball.
        /// </example>
        /// </summary>
        public CameraRotationMode CameraRotationMode { get; set; } = CameraRotationMode.Turntable;

        /// <summary>
        /// Режим камеры
        /// <example>
        /// <para/>Inspect - orbits around a point (fixed target position, move closer target when zooming).
        /// <para/>WalkAround - walk around (fixed camera position when rotating, move in camera direction when zooming).
        /// <para/>FixedPosition - fixed camera target, change field of view when zooming.
        /// </example>
        /// </summary>
        public CameraMode CameraMode { get; set; } = CameraMode.Inspect;

        /// <summary>
        /// Чувствительность вращения
        /// </summary>
        public double RotationSensitivity { get; set; } = 1;
        /// <summary>
        /// Чувствительность маштабирования
        /// </summary>
        public double ZoomSensitivity { get; set; } = 1;
        /// <summary>
        /// Степень инерции камеры
        /// </summary>
        public double CameraInertiaFactor { get; set; } = 0.930;

        #endregion Camera

        #endregion View

        #region Generation

        /// <summary>
        /// Ввод имени итогового файла при каждом запуске
        /// </summary>
        public bool AskNameFile { get; set; } = false;
        /// <summary>
        /// Выбор скрипта-генератора при каждом запуске
        /// </summary>
        public bool AskScriptFile { get; set; } = false;
        /// <summary>
        /// Отправляются ли сгенерированные файлы на робота/ов
        /// </summary>
        public bool SendingGeneratedFiles { get; set; } = true;

        /// <summary>
        /// Наименования для сгенерированных программ (в зависимости от типа детали)
        /// </summary>
        public ObservableCollection<Tuple<DetalType, string, string>> DetalsProgramNames { get; } = new ObservableCollection<Tuple<DetalType, string, string>>(DetalTypeExtensions.DetalTypeCollection().Select(t => new Tuple<DetalType, string, string>(t, t.GetDescription(), string.Empty)).ToList());
        /// <summary>
        /// Наименования скриптов-генератов (зависят от типа детали)
        /// </summary>
        public ObservableCollection<Tuple<DetalType, string, string>> DetalsScriptNames { get; } = new ObservableCollection<Tuple<DetalType, string, string>>(DetalTypeExtensions.DetalTypeCollection().Select(t => new Tuple<DetalType, string, string>(t, t.GetDescription(), string.Empty)).ToList());

        private ObservableCollection<string> _scriptsCollection;
        [JsonIgnore]
        public ObservableCollection<string> ScriptsCollection
        {
            get
            {
                if (this._scriptsCollection == null)
                {
                    this._scriptsCollection = GetScripts();
                    this._scriptsCollection.CollectionChanged += HandleCollectionChanged;
                }

                return this._scriptsCollection;
            }
        }

        ///// <summary>
        ///// Имя сгенерированной программы (настил с рёбрами)
        ///// </summary>
        //public string PlitaProgramName { get; set; }
        ///// <summary>
        ///// Имя сгенерированной программы (плита со стрингерами)
        ///// </summary>
        //public string PlitaStringerProgramName { get; set; }
        ///// <summary>
        ///// Имя сгенерированной программы (плита с треугольником)
        ///// </summary>
        //public string PlitaTreugolnikProgramName { get; set; }

        ///// <summary>
        ///// Имя скрапта-генератора (настил с рёбрами)
        ///// </summary>
        //public string PlitaScriptName { get; set; }
        ///// <summary>
        ///// Имя скрапта-генератора (плита со стрингерами)
        ///// </summary>
        //public string PlitaStringerScriptName { get; set; }
        ///// <summary>
        ///// Имя скрапта-генератора (плита с треугольником)
        ///// </summary>
        //public string PlitaTreugolnikScriptName { get; set; }

        /// <summary>
        /// Путь к папке для генерации
        /// </summary>
        public string PathFolderOfGeneration { get; set; }

        #endregion Generation

        #region Robots

        /// <summary>
        /// Стандартный путь к папке на роботе
        /// </summary>
        public string ControlerFolder { get; set; }

        #endregion

        #endregion Properties

        /// <summary>
        /// Событие изменения свойства настроек
        /// </summary>
        public event EventHandler ChangePropertyEvent;

        #endregion Public variables

        #region Constructors

        public Settings()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.SelectedTheme))
                this.SelectedTheme = this.Themes.First();
            else
                this.SelectedTheme = this.Themes.Where(t => t.Item1 == Properties.Settings.Default.SelectedTheme).First();
            
            if (this.Colors.Count == 0) this.Colors = GetColors();
        }

        #endregion Constructors

        #region Public functions

        /// <summary>
        /// Инициализация настроек (при первой загрузки) или выгрузка из временных файлов
        /// </summary>
        /// <returns></returns>
        public static Settings GetSettings()
        {
            try
            {
                if (!File.Exists(_path))
                    throw new FileNotFoundException("Не найден файл настроек", _path);

                string json = File.ReadAllText(_path);
                Settings settings =  JsonConvert.DeserializeObject<Settings>(json, _jsonSettings) ?? new Settings();

                if (JObject.Parse(json)["Version"].ToObject<Version>() != System.Reflection.Assembly.GetEntryAssembly().GetName().Version)
                    throw new Exception("Версия файла настроек не совпадает с версией приложения. Файл пересоздаётся.");

                settings.Colors = JObject.Parse(json)["Colors"].ToObject<Dictionary<string, System.Windows.Media.Color>>();
                return settings;
            }
            catch (Exception ex) when (LogException(ex))
            {
                Settings settings = new Settings();
                settings.Save();
                return settings;
            }
        }

        /// <summary>
        /// Возврат содержимого папки Scripts
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<string> GetScripts()
        {
            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Scripts");

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("Не найдена папка Scripts!");

            List<string> fileList = new List<string>();
            foreach(var file in Directory.GetFiles(path))
            {
                fileList.Add(file);
            }
            return new ObservableCollection<string>(fileList);
        }

        /// <summary>
        /// Установка цвета объекта 3д сцена
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <param name="color">Значение цвета</param>
        public static void SetColor(string propertyName, System.Windows.Media.Color color)
        {
            foreach (var f in typeof(ForRobot.Themes.Colors).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                var attribute = f.GetCustomAttributes(typeof(ForRobot.Libr.Attributes.PropertyNameAttribute), false).FirstOrDefault() as ForRobot.Libr.Attributes.PropertyNameAttribute;
                if (attribute.PropertyName == propertyName)
                    f.SetValue(null, color);
            }
        }

        /// <summary>
        /// Выгрузка установленных цветов для 3д сцены
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, System.Windows.Media.Color> GetColors()
        {
            Dictionary<string, System.Windows.Media.Color> colors = new Dictionary<string, System.Windows.Media.Color>();

            foreach (var f in typeof(ForRobot.Themes.Colors).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                var attribute = f.GetCustomAttributes(typeof(ForRobot.Libr.Attributes.PropertyNameAttribute), false).FirstOrDefault() as ForRobot.Libr.Attributes.PropertyNameAttribute;
                if (attribute != null)
                {
                    // Извлекаем название из атрибута
                    string name = attribute.PropertyName;

                    // Получаем значение цвета из свойства экземпляра
                    System.Windows.Media.Color colorValue = (System.Windows.Media.Color)f.GetValue(null);

                    colors.Add(name, colorValue);
                }
            }
            return colors;
        }

        public object Clone() => (Settings)this.MemberwiseClone();

        /// <summary>
        /// Возвращает стандартное имя программы
        /// </summary>
        /// <param name="startedDetalType"></param>
        /// <returns></returns>
        public string GetStandartProgramName(DetalType type)
        {
            return App.Current.Settings.DetalsProgramNames.Where(x => x.Item1 == type).FirstOrDefault().Item3;
        }
        
        /// <summary>
        /// Сохранение json-файла настроек во временных файлах
        /// </summary>
        public void Save()
        {
            string filePath = Path.Combine(Path.GetTempPath(), FileName);
            this.Save(filePath);
        }

        /// <summary>
        /// Сохранение файла настроек
        /// </summary>
        /// <param name="filePath">Путь для сохранения</param>
        public void Save(string filePath) => File.WriteAllText(filePath, JsonConvert.SerializeObject(this, _jsonSettings));

        #endregion Public functions

        #region Private functions

        private void HandleCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    string path = e.NewItems.Cast<string>().ToList().First();
                    if (!File.Exists(path))
                        throw new FileNotFoundException($"Файл {path} ненайден для удаления!");
                    //File.Move()
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    path = e.OldItems.Cast<string>().ToList().First();
                    if (!File.Exists(path))
                        throw new FileNotFoundException($"Файл {path} ненайден для удаления!");
                    File.Delete(path);
                    break;
            }
        }

        /// <summary>
        /// Вызов события изменения свойства
        /// </summary>
        public void OnPropertyChanged() => this.ChangePropertyEvent?.Invoke(this, null);

        /// <summary>
        /// Логирование исключений выгрузки настроек
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static bool LogException(Exception ex)
        {
            App.Current.Logger.Error(ex, "Ошибка выгрузки настроек приложения");
            return true;
        }

        #endregion Private functions
    }
}
