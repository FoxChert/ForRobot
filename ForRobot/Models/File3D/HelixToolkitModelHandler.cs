using System;
using System.IO;
using System.Collections.Generic;

using HelixToolkit.Wpf;

namespace ForRobot.Models.File3D
{
    public class HelixToolkitModelHandler : IModelFileHandler
    {
        public HashSet<string> SupportedExportExtensions { get; } = new HashSet<string>() { ".3ds", ".obj", ".objz", ".off", ".lwo", ".stl", ".ply"  };

        public object LoadModel(string filePath) => new ModelImporter().Load(filePath);

        public void SaveModel(object model, string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath).ToLower();

            if (!this.SupportedExportExtensions.Contains(extension))
                throw new Exception(string.Format("Расширение {0} не поддерживается", extension));

            using (Stream StreamFile = new FileStream(filePath, FileMode.Create))
            {
                //exp.Export(model, StreamFile);
            }

            throw new NotImplementedException();
        }
    }
}
