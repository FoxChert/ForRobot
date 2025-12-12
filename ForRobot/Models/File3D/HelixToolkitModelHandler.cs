using System;
using System.IO;
using System.Collections.Generic;

using HelixToolkit.Wpf;

namespace ForRobot.Models.File3D
{
    public class HelixToolkitModelHandler : IModelFileHandler
    {
        public HashSet<string[]> SupportedExtensions { get; } = new HashSet<Tuple<string[]>()
        {
            new Tuple<string, string[]>("STL Files", new string[]{ ".3ds" }),
            new Tuple<string, string[]>("STL Files", new string[]{ ".obj" }),
            new Tuple<string, string[]>("STL Files", new string[]{ ".objz" }),
            new Tuple<string, string[]>("STL Files", new string[]{ ".off" }),
            new Tuple<string, string[]>("STL Files", new string[]{ ".lwo" }),
            new Tuple<string, string[]>("STL Files", new string[]{ ".stl" }),
            new Tuple<string, string[]>("STL Files", new string[]{ ".ply" }),

            //".3ds", ".obj", ".objz", ".off", ".lwo", ".stl", ".ply"
        };

        public object LoadModel(string filePath) => new ModelImporter().Load(filePath);

        public void SaveModel(object model, string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath).ToLower();

            //if (!this.SupportedExtensions.Contains(extension))
            //    throw new Exception(string.Format("Расширение {0} не поддерживается", extension));

            using (Stream StreamFile = new FileStream(filePath, FileMode.Create))
            {
                //exp.Export(model, StreamFile);
            }

            throw new NotImplementedException();
        }
    }
}
