using System;
using System.Linq;
using System.Collections.Generic;

using Assimp;

namespace ForRobot.Models.File3D
{
    public class AssimpModelHandler : IModelFileHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public HashSet<string> SupportedExportExtensions { get; } = new HashSet<string>()  { ".stl", ".obj", ".fbx", ".dae", ".3ds", ".gltf", ".glb", ".ply", ".off", ".lwo" };

        public object LoadModel(string filePath) => new AssimpContext().ImportFile(filePath);

        public void SaveModel(object model, string filePath)
        {
            Scene scene = model as Scene;
            AssimpContext exporter = new AssimpContext();
            string extension = System.IO.Path.GetExtension(filePath).ToLower();

            if (!this.SupportedExportExtensions.Contains(extension))
                throw new Exception(string.Format("Расширение {0} не поддерживается", extension));

            exporter.ExportFile(scene, filePath, extension); 
        }
    }
}
