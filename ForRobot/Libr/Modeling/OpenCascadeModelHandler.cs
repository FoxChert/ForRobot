using System;
using System.IO;
using System.Collections.Generic;

namespace ForRobot.Libr.Modeling
{
    /// <summary>
    /// Обработчик файлов 3D моделей, использующий <see cref=""/> для загрузки и сохранения моделей
    /// </summary>
    public class OpenCascadeModelHandler : IModelFileHandler
    {
        /// <summary>
        /// Хэшированная коллекция поддерживаемых расширений файлов для импорта
        /// </summary>
        public HashSet<string> SupportedImportExtensions { get; } = new HashSet<string>()
        {
            ".stp", ".step", ".brep", ".dxf", ".sat", ".sab", ".jt", ".ifc", ".parasolid", ".rvm", ".igs", ".iges"
        };

        /// <summary>
        /// Хэшированная коллекция поддерживаемых расширений файлов для экспорта
        /// </summary>
        public HashSet<string> SupportedExportExtensions { get; } = new HashSet<string>()
        {
            ".stp", ".step", ".brep", ".dxf", ".sat", ".sab", ".jt", ".igs", ".iges", ".wrl"
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
                return null;

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

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при сохранении модели в файл {filePath}", ex);
            }
        }
    }
}
