using System;
using System.Linq;
using System.Collections.Generic;

namespace ForRobot.Models.File3D
{
    /// <summary>
    /// Формат файлов
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

        /// <summary>
        /// Поддерживает ли формат файлов расширение
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        internal bool SupportsExtension(string extension) => Extensions.Contains(extension);
    }
}
