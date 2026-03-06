using System;
using System.Collections.Generic;
using ForRobot.Models.File3D;

namespace ForRobot.Libr.Registry
{
    public interface IFileFormatRegistry
    {
        void RegisterFormat(FileFormatInfo fileFormat);
        void RegisterFormat(string formatName, FormatCategories category, params string[] extensions);

        bool IsExtensionSupported(string extension);
        FileFormatInfo GetFormatByName(string formatName);
        FileFormatInfo GetFormatByExtension(string extension);
        IEnumerable<string> GetExtensionsByCategory(FormatCategories category);
        IEnumerable<string> GetAllExtensions();
        IEnumerable<FileFormatInfo> GetAllFormats();
        FormatCategories GetFormatCategory(string extension);
    }
}
