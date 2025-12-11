using System;
using System.Collections.Generic;

namespace ForRobot.Models.File3D
{
    public interface IModelFileHandler
    {
        HashSet<Tuple<string, string[]>> SupportedExtensions { get; }
        object LoadModel(string filePath);
        void SaveModel(object model, string filePath);
    }
}
