using System;
using System.Collections.Generic;
using System.Linq;
using ForRobot.Models.File3D;

namespace ForRobot.Libr.Registry
{
    public class FileFormatRegistry : IFileFormatRegistry
    {
        private static FileFormatRegistry _instance;
        private static readonly object _lock = new object();

        public static FileFormatRegistry Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new FileFormatRegistry();
                    }
                }
                return _instance;
            }
        }
            //=> _instance ?? (_instance new FileFormatRegistry();

        private readonly Dictionary<string, FileFormatInfo> _formats = new Dictionary<string, FileFormatInfo>(StringComparer.OrdinalIgnoreCase);

        private FileFormatRegistry() { }

        /// <summary>
        /// Поддерживается ли расширение
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public bool IsExtensionSupported(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return false;

            return _formats.Values.Any(format => format.SupportsExtension(extension));
        }

        /// <summary>
        /// Получение категории формата по его расширению
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public FormatCategories GetFormatCategory(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                throw new ArgumentNullException(nameof(extension));

            var format = GetFormatByExtension(extension);
            return format.Category;
        }

        /// <summary>
        /// Возвращает формат по наименованию
        /// </summary>
        /// <param name="formatName"></param>
        /// <returns></returns>
        public FileFormatInfo GetFormatByName(string formatName)
        {
            if (string.IsNullOrEmpty(formatName))
                throw new ArgumentNullException(nameof(formatName));

            return _formats.TryGetValue(formatName, out var format) ? format : throw new InvalidOperationException($"Не найдено формата с именем '{formatName}'");
        }

        /// <summary>
        /// Возвращает формат по расширению
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public FileFormatInfo GetFormatByExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                throw new ArgumentNullException(nameof(extension));

            return _formats.Values.FirstOrDefault(format => format.SupportsExtension(extension));
        }

        /// <summary>
        /// Возвращает все зарегестрированные форматы
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FileFormatInfo> GetAllFormats() => _formats.Values;

        /// <summary>
        /// Возвращает все форматы определенной категории
        /// </summary>
        public IEnumerable<FileFormatInfo> GetFormatsByCategory(FormatCategories category) => _formats.Values.Where(f => f.Category == category);

        /// <summary>
        /// Возвращает расширения всех форматов одной категории
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public IEnumerable<string> GetExtensionsByCategory(FormatCategories category)
        {
            List<string> extensions = new List<string>();
            foreach(var format in _formats.Values.Where(item => item.Category == category))
            {
                extensions.AddRange(format.Extensions);
            }
            return extensions.Count == 0 ? null : extensions;
        }

        /// <summary>
        /// Возврат всех поддерживаемых расширений
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllExtensions() => _formats.Values.SelectMany(format => format.Extensions).Distinct(StringComparer.OrdinalIgnoreCase);

        public void RegisterFormat(FileFormatInfo fileFormat) => _formats[fileFormat.FormatsName] = fileFormat;

        public void RegisterFormat(string formatName, FormatCategories category, params string[] extensions)
        {
            var info = new FileFormatInfo(formatName, category, extensions);
            RegisterFormat(info);
        }

        public static void Register(FileFormatInfo formatInfo) => Instance.RegisterFormat(formatInfo);
        public static void Register(string formatName, FormatCategories category, params string[] extensions) => Instance.RegisterFormat(formatName, category, extensions);
        public static bool Supports(string extension) => Instance.IsExtensionSupported(extension);
        public static FormatCategories Category(string extension) => Instance.GetFormatCategory(extension);
        public static FileFormatInfo GetByName(string name) => Instance.GetFormatByName(name);
        public static FileFormatInfo GetByExtension(string extension) => Instance.GetFormatByExtension(extension);
        public static IEnumerable<FileFormatInfo> GetByCategory(FormatCategories category) => Instance.GetFormatsByCategory(category);
    }
}
