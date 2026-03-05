using System;
using System.Linq;
using System.Collections.Generic;

namespace ForRobot.Models.File3D
{
    /// <summary>
    /// 
    /// </summary>
    public struct FileFormatInfo
    {
        /// <summary>
        /// Наименование формата файлов
        /// </summary>
        public string FormatsName { get; }

        /// <summary>
        /// Категория формата
        /// </summary>
        public FormatCategories Category { get; }

        /// <summary>
        /// Расширения формата файлов
        /// </summary>
        public readonly IReadOnlyList<string> Extensions;

        public FileFormatInfo(string formatName, FormatCategories category, params string[] extension)
        {
            this.FormatsName = formatName;
            this.Category = category;
            this.Extensions = new List<string>(extension);
        }
    }

    public enum FormatCategories
    {
        NativeFile,
        MeshFile,
        CadFile
    }

    public class FileFormatRegistry
    {
        private static FileFormatRegistry _instance;
        public static FileFormatRegistry Instance => _instance ?? new FileFormatRegistry();

        private readonly Dictionary<string, FileFormatInfo> _formats = new Dictionary<string, FileFormatInfo>(StringComparer.OrdinalIgnoreCase);

        private FileFormatRegistry() { }

        public string GetFormatCategory(string extension)
        {
            throw new NotImplementedException();
        }

        public bool IsExtensionSupported(string extension)
        {
            throw new NotImplementedException();
        }

        public FileFormatInfo GetFormatByExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return null;

            return _formats.Values.FirstOrDefault(format => format.SupportsExtension(extension));
            //extension?.ToLower(), out var format) ? format : null;
        }


        public IEnumerable<string> GetAllExtensions()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetExtensionsByCategory(FormatCategories category) => null;
            //this._formats.Select(item => item.Value).Where(item => item.Category == category).ToList();

        public Dictionary<string, string> GetAllFormatDescriptions()
        {
            throw new NotImplementedException();
        }

        //public void RegisterFormat(string extension, string description, string category, object metadata = null)
        //{
        //    throw new NotImplementedException();
        //}

        public void RegisterFormat(string formatName, FormatCategories category, params string[] extensions)
        {
            var info = new FileFormatInfo(formatName, category, extensions);
            _formats[formatName] = info;
        }
    }
}
