using System;
using System.IO;
using System.Collections.Generic;

using HelixToolkit.Wpf;

namespace ForRobot.Models.File3D
{
    /// <summary>
    /// Обработчик файлов 3D моделей, использующий <see cref="HelixToolkit.Wpf"/> для загрузки и сохранения моделей
    /// </summary>
    public class HelixToolkitModelHandler : IModelFileHandler
    {
        /// <summary>
        /// Хэшированная коллекция поддерживаемых расширений файлов для экспорта
        /// </summary>
        public HashSet<string> SupportedImportExtensions => throw new NotImplementedException();

        /// <summary>
        /// Хэшированная коллекция поддерживаемых расширений файлов для экспорта
        /// </summary>
        public HashSet<string> SupportedExportExtensions { get; } = new HashSet<string>() { ".3ds", ".obj", ".objz", ".off", ".lwo", ".stl", ".ply"  };
        
        /// <summary>
        /// Загрузка 3D модели из указанного файла
        /// </summary>
        /// <param name="filePath">Путь к файлу модели</param>
        /// <returns>Загруженная 3D модель</returns>
        /// <exception cref="ArgumentNullException">Если путь к файлу равен null</exception>
        /// <exception cref="FileNotFoundException">Если файл не найден</exception>
        /// <exception cref="InvalidOperationException">Если произошла ошибка при загрузке модели</exception>
        public object LoadModel(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath), "Путь к файлу не может быть null или пустым");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

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
                throw new Exception(string.Format("Расширение {0} не поддерживается", extension));

            try
            {
                using (Stream StreamFile = new FileStream(filePath, FileMode.Create))
                {

                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при сохранении модели в файл {filePath}", ex);
            }

            //using (Stream StreamFile = new FileStream(filePath, FileMode.Create))
            //{
            //    //exp.Export(model, StreamFile);
            //}

            //throw new NotImplementedException();
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
                case ".3ds":
                    return new Exporters.Create;
                case ".obj":
                case ".objz":
                    return new ObjExporter();
                case ".off":
                    return new Exporters.Create;
                case ".lwo":
                    return new Exporters.Create;
                case ".stl":
                    return new StlExporter();
                case ".ply":
                    return new Exporters.Create;
                default:
                    throw new NotSupportedException($"Экспортер для формата {extension} не реализован");
            }
        }
    }
}
