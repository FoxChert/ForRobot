using System;
using System.IO;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

using HelixToolkit.Wpf;

namespace ForRobot.Libr.Modeling
{
    /// <summary>
    /// Обработчик файлов 3D моделей, использующий <see cref="HelixToolkit.Wpf"/> для загрузки и сохранения моделей
    /// </summary>
    public class HelixToolkitModelHandler : IModelFileHandler
    {
        /// <summary>
        /// Хэшированная коллекция поддерживаемых расширений файлов для импорта
        /// </summary>
        public HashSet<string> SupportedImportExtensions { get; } = new HashSet<string>()
        {
            ".3ds", ".obj", ".objz", ".off", ".lwo", ".stl", ".ply"
        };

        /// <summary>
        /// Хэшированная коллекция поддерживаемых расширений файлов для экспорта
        /// </summary>
        public HashSet<string> SupportedExportExtensions { get; } = new HashSet<string>()
        {
            ".png", ".jpg", ".obj", ".objz", ".xaml", ".xml", ".x3d", ".dae", ".stl"
        };

        public bool CanHandle(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Выгрузка 3D модели из указанного файла
        /// </summary>
        /// <param name="filePath">Путь к файлу модели</param>
        /// <returns>3D модель</returns>
        /// <exception cref="ArgumentNullException">Если путь к файлу равен null</exception>
        /// <exception cref="FileNotFoundException">Если файл не найден</exception>
        /// <exception cref="InvalidOperationException">Если произошла ошибка при загрузке модели</exception>
        public object LoadModel(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath), "Путь к файлу не может быть null или пустым");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

            string extension = System.IO.Path.GetExtension(filePath)?.ToLower();
            if (string.IsNullOrEmpty(extension) || !this.SupportedImportExtensions.Contains(extension))
                throw new Exception(string.Format("Расширение {0} не поддерживается для импорта", extension));

            try
            {
                return new ModelImporter().Load(filePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при загрузке модели из файла {filePath}", ex);
            }
        }
        
        /// <summary>
        /// Сохранение 3D модели по указанному пути
        /// </summary>
        /// <param name="model">3D модель для сохранения</param>
        /// <param name="filePath">Путь к файлу для сохранения</param>
        /// <exception cref="ArgumentNullException">Если модель или путь к файлу равен null</exception>
        /// <exception cref="ArgumentException">Если расширение файла не поддерживается</exception>
        /// <exception cref="InvalidOperationException">Если произошла ошибка при сохранении модели</exception>
        public void SaveModel(object model, string filePath)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Модель не может быть null");

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath), "Путь к файлу не может быть null или пустым");

            string extension = System.IO.Path.GetExtension(filePath)?.ToLower();

            if (string.IsNullOrEmpty(extension) || !this.SupportedExportExtensions.Contains(extension))
                throw new Exception(string.Format("Расширение {0} не поддерживается экспорта", extension));

            try
            {
                var exporter = this.CreateExporterForExtension(extension);
                using (Stream streamFile = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    exporter.Export(model as Model3D, streamFile);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при сохранении модели в файл {filePath}", ex);
            }
        }

        /// <summary>
        /// Создание экспортера для указанного расширения файла
        /// </summary>
        /// <param name="extension">Расширение файла</param>
        /// <returns>Экспортер для заданного формата</returns>
        private IExporter CreateExporterForExtension(string extension)
        {
            switch (extension)
            {
                case ".png":
                case ".jpeg":
                    return new BitmapExporter();
                case ".obj":
                case ".objz":
                    return new ObjExporter();
                case ".xaml":
                    return new XamlExporter();
                case ".xml":
                    return new KerkytheaExporter();
                case ".dae":
                    return new ColladaExporter();
                case ".stl":
                    return new StlExporter();
                default:
                    throw new NotSupportedException($"Экспортер для формата {extension} не реализован");
            }
        }
    }
}
