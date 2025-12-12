using System;
using System.Linq;
using System.Collections.Generic;

using Assimp;

namespace ForRobot.Models.File3D
{
    public class AssimpModelHandler : IModelFileHandler
    {
        //3D Files|*.stl;*.obj;*.fbx;*.gltf;*.glb;*.3mf|STL Files|*.stl|OBJ Files|*.obj|FBX Files|*.fbx|glTF Files|*.gltf;*.glb|3MF Files|*.3mf|

        /// <summary>
        /// 
        /// </summary>
        public HashSet<string[]> SupportedExtensions { get; } = new HashSet<string[]>()
        {
            new Tuple<string, string[]>("STL Files", new string[]{ ".stl" }),
            new Tuple<string, string[]>("OBJ Files", new string[]{ ".obj" }),
            new Tuple<string, string[]>("FBX Files", new string[]{ ".fbx" }),
            new Tuple<string, string[]>("glTF Files", new string[]{ ".gltf", ".glb" })

            //".stl", ".obj", ".fbx", ".callada", ".3ds", ".gltf", ".glb", ".ply", ".off", ".lwo"
        };

        public object LoadModel(string filePath) => new AssimpContext().ImportFile(filePath);

        public void SaveModel(object model, string filePath)
        {
            Scene scene = model as Scene;
            AssimpContext exporter = new AssimpContext();
            string extension = System.IO.Path.GetExtension(filePath).ToLower();

            //if(!this.SupportedExtensions.Contains(extension))
            //    throw new Exception(string.Format("Расширение {0} не поддерживается", extension));

            exporter.ExportFile(scene, filePath, extension); 
        }
    }
}
