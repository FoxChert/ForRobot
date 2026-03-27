using AvalonDock.Themes;
using ForRobot.Libr;
using ForRobot.Models.Detals;
using HelixToolkit.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;

namespace ForRobot.Models.Settings
{
    /// <summary>
    /// Класс представляющий настройки приложения
    /// </summary>
    public class Settings : ICloneable
    {
        #region Private variables

        //private List<(string, bool)> _availableFolders;

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        private List<MutableKeyValuePair<DetalType, string>> _detalsProgramNames;
        private List<MutableKeyValuePair<DetalType, string>> _detalsScriptNames;

        private Dictionary<string, System.Windows.Media.Color> _colors = new Dictionary<string, System.Windows.Media.Color>();
        
        #region Properties

        #endregion

        #endregion Private variables

        #region Public variables

        /// <summary>
        /// Наименование файла настроек
        /// </summary>
        public const string FileName = "interfaceOfRobot_settings.json";
        
        #region Generic

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
        /// Время ожидания ответа от сервера, сек.
        /// </summary>
        public double ConnectionTimeOut { get; set; } = 3;

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

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        /// <summary>
        /// Доступность системных папок в дереве файлов
        /// </summary>
        public ObservableCollection<MutableKeyValuePair<string, bool>> AvailableFolders { get; set; }

        #endregion

        #region View

        public static List<Tuple<string, Theme>> Themes { get; } = new List<Tuple<string, Theme>>()
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
        
        public Tuple<string, Theme> SelectedTheme { get; set; }

        #region 3DView

        /// <summary>
        /// Масштабный коэффициент: 1 единица модели = <see cref="ScaleFactor"/> мм. реальных размеров
        /// </summary>
        public decimal ScaleFactor { get; set; } = 1.00M / 100.00M;

        /// <summary>
        /// Показ системы координат
        /// </summary>
        public bool ShowCoordinateSystem { get; set; } = true;
        /// <summary>
        /// Показ пространственного куба
        /// </summary>
        public bool ShowViewCube { get; set; } = true;
        /// <summary>
        /// Доступности нажатия рёбер пространственного куба
        /// </summary>
        public bool IsViewCubeEdgeClicksEnabled { get; set; } = false;
        /// <summary>
        /// Показ кол-ва полигонов
        /// </summary>
        public bool ShowTriangleCountInfo { get; set; } = false;

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
        
        /// <summary>
        /// Толщина линий шва на модели
        /// </summary>
        public double WeldsThickness { get; set; } = 5.0;
        /// <summary>
        /// Размер шрифта
        /// </summary>
        public double AnnotationFontSize { get; set; } = 20;
        /// <summary>
        /// Толщина линий
        /// </summary>
        public double AnnotationThickness { get; set; } = 2.0;

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
        public bool Orthographic { get; set; } = false;
        /// <summary>
        /// Демонстрация информации о камере
        /// </summary>
        public bool ShowCameraInfo { get; set; } = false;
        /// <summary>
        /// Демонстрация курсора камеры
        /// </summary>
        public bool ShowCameraTarget { get; set; } = true;
        /// <summary>
        /// Поворот вокруг мыши
        /// </summary>
        public bool RotateAroundMouseDownPoint { get; set; } = false;
        /// <summary>
        /// Приближение около мыши
        /// </summary>
        public bool ZoomAroundMouseDownPoint { get; set; } = false;
        /// <summary>
        /// Инерция камеры
        /// </summary>
        public bool IsInertiaEnabled { get; set; } = false;
        /// <summary>
        /// Вкличено ли панорамирование
        /// </summary>
        public bool IsPanEnabled { get; set; } = true;
        /// <summary>
        /// Вкличено ли перемещение
        /// </summary>
        public bool IsMoveEnabled { get; set; } = true;
        /// <summary>
        /// Вкличено ли вращение
        /// </summary>
        public bool IsRotationEnabled { get; set; } = true;
        /// <summary>
        /// Вкличено ли маштабирование
        /// </summary>
        public bool IsZoomEnabled { get; set; } = true;

        ///// <summary>
        ///// 
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

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        /// <summary>
        /// Наименования для сгенерированных программ (в зависимости от типа детали)
        /// </summary>
        public List<MutableKeyValuePair<DetalType, string>> DetalsProgramNames 
        {
            get => _detalsProgramNames ?? (_detalsProgramNames = new List<MutableKeyValuePair<DetalType, string>>(DetalTypeExtensions.DetalTypeCollection().Select(t => new MutableKeyValuePair<DetalType, string>(t, string.Empty))));
            private set => _detalsProgramNames = value;
        }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        /// <summary>
        /// Наименования скриптов-генератов (зависят от типа детали)
        /// </summary>
        public List<MutableKeyValuePair<DetalType, string>> DetalsScriptNames 
        {
            get => _detalsScriptNames ?? (_detalsScriptNames = new List<MutableKeyValuePair<DetalType, string>>(DetalTypeExtensions.DetalTypeCollection().Select(t => new MutableKeyValuePair<DetalType, string>(t, string.Empty))));
            private set => _detalsScriptNames = value;
        }

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
                
        #endregion Public variables

        #region Constructors

        public Settings()
        {            
            if (this.Colors.Count == 0) this.Colors = GetColors();

            this.SelectedTheme = Settings.Themes.First();
        }

        #endregion Constructors

        #region Public functions

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

        public object Clone() => (Settings)this.MemberwiseClone();

        /// <summary>
        /// Возвращает стандартное имя программы
        /// </summary>
        /// <param name="startedDetalType"></param>
        /// <returns></returns>
        public string GetStandartProgramName(DetalType type) => this.DetalsProgramNames.Where(x => x.Key == type).FirstOrDefault().Value;

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
                    string name = attribute.PropertyName;

                    System.Windows.Media.Color colorValue = (System.Windows.Media.Color)f.GetValue(null);

                    colors.Add(name, colorValue);
                }
            }
            return colors;
        }

        //public void AddScripts(string path)
        //{
        //    if (!File.Exists(path))
        //        throw new FileNotFoundException($"Исходный файл не найден!", path);

        //    string directory = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Scripts");

        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }

        //    string finalPath = Path.Combine(directory, Path.GetFileName(path));
        //    File.Copy(path, finalPath, true);
        //    this.OnPropertyChanged(nameof(this.ScriptsCollection));
        //}

        ///// <summary>
        ///// Добавляет скрипт в коллекцию
        ///// </summary>
        ///// <param name="sourcePath">Путь к исходному файлу</param>
        //public void AddScript(string sourcePath)
        //{
        //    if (!File.Exists(sourcePath))
        //        throw new FileNotFoundException($"Исходный файл не найден!", sourcePath);

        //    string directory = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Scripts");

        //    if (!Directory.Exists(directory))
        //        Directory.CreateDirectory(directory);

        //    string fileName = Path.GetFileName(sourcePath);
        //    string finalPath = Path.Combine(directory, fileName);

        //    File.Copy(sourcePath, finalPath, true);

        //    this.ScriptsCollection.Add(finalPath);
        //}

        ///// <summary>
        ///// Удаляет скрипт из коллекции
        ///// </summary>
        ///// <param name="scriptPath">Путь к файлу скрипта</param>
        //public void RemoveScript(string scriptPath)
        //{
        //    if (this.ScriptsCollection.Contains(scriptPath))
        //    {
        //        if (!File.Exists(scriptPath))
        //            throw new FileNotFoundException($"Файл для удалния не найден!", scriptPath);

        //        File.Delete(scriptPath);

        //        this.ScriptsCollection.Remove(scriptPath);
        //    }
        //}

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
                    //string directory = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Scripts");

                    //if (!Directory.Exists(directory))
                    //    Directory.CreateDirectory(directory);

                    //foreach (var newItem in e.NewItems)
                    //{
                    //    if (!(newItem is string path))
                    //        return;

                    //    if (!File.Exists(path))
                    //        throw new FileNotFoundException($"Исходный файл не найден!", path);

                    //    string finalPath = Path.Combine(directory, Path.GetFileName(path));
                    //    File.Copy(path, finalPath, true);
                    //}

                    //this._scriptsCollection = GetScripts();


                    //string sourcePath = e.NewItems.Cast<string>().ToList().First();

                    //if (!File.Exists(sourcePath))
                    //    throw new FileNotFoundException($"Исходный файл не найден!", sourcePath);

                    //string directory = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Scripts");

                    //if (!Directory.Exists(directory))
                    //{
                    //    Directory.CreateDirectory(directory);
                    //}

                    //string finalPath = Path.Combine(directory, Path.GetFileName(sourcePath));

                    //foreach (var newItem in e.NewItems)
                    //{
                    //    if (newItem is String item)
                    //        item = finalPath;
                    //}
                    //File.Copy(sourcePath, finalPath, true);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    //string sourcePath = e.OldItems.Cast<string>().ToList().First();

                    //if (!File.Exists(sourcePath))
                    //    throw new FileNotFoundException($"Файл для удалния не найден!", sourcePath);

                    //File.Delete(sourcePath);
                    break;
            }
            //this.OnPropertyChanged(nameof(this.ScriptsCollection));
        }

        //[OnDeserializing]
        //internal void OnDeserializing(StreamingContext context)
        //{
        //    //JObject.Parse(jsonString)["DetalsScriptNames"].ToObject<ObservableCollection<Tuple<ForRobot.Models.Detals.DetalType, string, string>>>()
        //}

        #endregion Private functions
    }
}
