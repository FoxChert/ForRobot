using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace ForRobot.Libr.Modeling
{
    public static class Model3DManager
    {
        private static readonly Dictionary<string, List<IModelFileHandler>> _handlersByFormat = new Dictionary<string, List<IModelFileHandler>>(StringComparer.OrdinalIgnoreCase)
        {
            [".stl"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },
            [".obj"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },
            //[".fbx"] = new List<IModelFileHandler> { new AssimpModelHandler() },
            //[".callada"] = new List<IModelFileHandler> { new AssimpModelHandler() },
            //[".3ds"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },
            //[".gltf"] = new List<IModelFileHandler> { new AssimpModelHandler() },
            //[".glb"] = new List<IModelFileHandler> { new AssimpModelHandler() },
            [".ply"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },
            //[".off"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },
            //[".lwo"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() }
            [".step"] = new List<IModelFileHandler> { new OpenCascadeModelHandler() }
        };

        /// <summary>
        /// Загрузка 3д модели
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Model3DGroup LoadModel3D(string path)
        {
            Model3DGroup model = null;
            try
            {
                if (!System.IO.File.Exists(path))
                    throw new FileNotFoundException("Не удалось найти файл", path);

                string extension = System.IO.Path.GetExtension(path).ToLower();

                if (_handlersByFormat.TryGetValue(extension, out var handlers))
                {
                    for (int i = 0; i < handlers.Count; i++)
                    {
                        var modelFileHandler = handlers[i];
                        model = modelFileHandler.LoadModel(path) as System.Windows.Media.Media3D.Model3DGroup;

                        if (model != null) break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка выгрузки модели робота: {ex.Message}", ex);
            }
            return model;
        }
    }
}
