using System;
using System.Collections.Generic;

namespace ForRobot.Models.File3D
{
    public interface IModelFileHandler
    {
        HashSet<string> SupportedImportExtensions { get; }
        HashSet<string> SupportedExportExtensions { get; }
        object LoadModel(string filePath);
        void SaveModel(object model, string filePath);
    }
}
