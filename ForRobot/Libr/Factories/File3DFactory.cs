using System;
using System.IO;
using System.Linq;

using ForRobot.Models.File3D;
using ForRobot.Libr.Services.Providers;

namespace ForRobot.Libr.Factories
{
    /// <summary>
    /// Файбрика для создания и загрузки 3D файлов различных форматов
    /// </summary>
    /// <remarks>
    /// Поддерживаемые форматы:
    /// - Меш-файлы: .stl, .obj, .ply
    /// - Нативные файлы: .json, .txt
    /// </remarks>
    public static class File3DFactory
    {
        /// <summary>
        /// Провайдер json-схем для валидации структуры json-строк
        /// </summary>
        private static IJsonSchemaProvider _jsonSchemaProvider;
        /// <summary>
        /// Провайдер создания объектов <see cref="ForRobot.Models.Detals.Detal"/>
        /// </summary>
        private static IDetalProvider _detalProvider;
        /// <summary>
        /// Кэширование фабрики создания детали
        /// </summary>
        private static DetalFactory.IDetalFactory _cachedDetalFactory;

        /// <summary>
        /// Статический конструктор для инициализации провайдеров по умолчанию
        /// </summary>
        /// <remarks>
        /// Выполняется один раз при первом обращении к классу
        /// </remarks>
        static File3DFactory()
        {
            InitializeDefaultProviders();
            CreateCachedDetalFactory();
        }

        #region Private functions

        private static void InitializeDefaultProviders()
        {
            _detalProvider = new ForRobot.Models.Detals.DetalProvider(new ForRobot.Libr.Configuration.ConfigurationProvider());
            _jsonSchemaProvider = new ForRobot.Libr.Json.Schemas.JsonSchemaProvider();
        }

        private static void CreateCachedDetalFactory()
        {
            _cachedDetalFactory = new ForRobot.Libr.Factories.DetalFactory.DetalFactory(_detalProvider, _jsonSchemaProvider);
        }

        /// <summary>
        /// Валидация пути к файлу
        /// </summary>
        /// <param name="path">Путь к файлу для валидации</param>
        /// <exception cref="ArgumentNullException">Если путь равен null</exception>
        /// <exception cref="ArgumentException">Если путь пустой или состоит только из пробелов</exception>
        private static bool ValidatePath(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path), "Путь к файлу не может быть null");
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Путь к файлу не может быть пустым или состоять только из пробелов", nameof(path));
            }

            return true;
        }

        #endregion Private functions

        #region Public functions

        //private static bool CanCreate(this File3D file3D, string extension) => file3D.Extensions.Select(item => item.FilesExtensions).Contains(extension);

        /// <summary>
        /// Создание объекта File3D на основе расширения файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Соответствующий объект File3D</returns>
        /// <exception cref="ArgumentNullException">Если путь равен null</exception>
        /// <exception cref="FileNotFoundException">Если файл не найден</exception>
        /// <exception cref="NotSupportedException">Если формат файла не поддерживается</exception>
        /// <exception cref="ArgumentException">Если путь пустой или состоит только из пробелов</exception>
        public static File3D Create(string path)
        {
            ValidatePath(path);
            
            string extension = System.IO.Path.GetExtension(path)?.ToLowerInvariant();

            switch (extension)
            {
                case ".stl":
                case ".obj":
                case ".ply":
                case ".3ds":
                case ".off":
                case ".dae":
                    return new MeshModelFile3D(path);

                case ".step":
                    return new CadFile3D(path);

                case ".json":
                case ".txt":
                    return new NativeFile3D(path, _cachedDetalFactory);

                default:
                    throw new Exception(string.Format("Расширение '{0}' не поддерживается", extension));
            }
        }

        /// <summary>
        /// Создание <see cref="NativeFile3D"/> с указанным типом детали
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="detalType">Тип детали</param>
        /// <returns>Объект NativeFile3D</returns>
        /// <exception cref="ArgumentNullException">Если путь равен null</exception>
        /// <exception cref="ArgumentException">Если путь пустой или состоит только из пробелов</exception>
        public static File3D Create(string path, ForRobot.Models.Detals.DetalType detalType)
        {
            ValidatePath(path);

            return new NativeFile3D(path, detalType, _cachedDetalFactory);
        }

        /// <summary>
        /// Установка провайдера деталей
        /// </summary>
        /// <param name="detalProvider">Провайдер деталей</param>
        /// <exception cref="ArgumentNullException">Если configurationProvider равен null</exception>
        public static void SetDetalProvider(IDetalProvider detalProvider)
        {
            _detalProvider = _detalProvider ?? throw new ArgumentNullException(nameof(detalProvider));
            CreateCachedDetalFactory();
        }
        /// <summary>
        /// Установка провайдера json-схем
        /// </summary>
        /// <param name="jsonSchemaProvider">Провайдер json-схем</param>
        /// <exception cref="ArgumentNullException">Если jsonSchemaProvider равен null</exception>
        public static void SetJsonSchemaProvider(IJsonSchemaProvider jsonSchemaProvider)
        {
            _jsonSchemaProvider = jsonSchemaProvider ?? throw new ArgumentNullException(nameof(jsonSchemaProvider));
            CreateCachedDetalFactory();
        }

        #endregion Public functions
    }
}
