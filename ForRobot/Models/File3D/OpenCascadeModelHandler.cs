using System;
using System.Collections.Generic;


namespace ForRobot.Models.File3D
{
    public class OpenCascadeModelHandler : IModelFileHandler
    {
        public HashSet<string> SupportedExtensions { get; } = new HashSet<string>() { ".stl", ".obj", ".fbx", ".callada", ".3ds", ".gltf", ".glb", ".ply", ".off", ".lwo" };

        public object LoadModel(string filePath)
        {
            var importer = new AssimpContext();
            return importer.ImportFile(filePath);
        }

        public void SaveModel(object model, string filePath)
        {
            var scene = model as Scene;
            var exporter = new AssimpContext();
            exporter.ExportFile(scene, filePath, "obj");
        }
    }
}
