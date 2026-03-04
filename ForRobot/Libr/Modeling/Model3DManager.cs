using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace ForRobot.Libr.Modeling
{
    /// <summary>
    /// Класс-менеджер для загрузки 3д моделей из файлов с разными расширениями
    /// </summary>
    public static class Model3DManager
    {
        private static readonly Dictionary<string, List<IModelFileHandler>> _handlersByFormat = new Dictionary<string, List<IModelFileHandler>>(StringComparer.OrdinalIgnoreCase)
        {
            // Mesh files
            [".stl"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },
            [".obj"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },
            //[".fbx"] = new List<IModelFileHandler> { new AssimpModelHandler() },
            //[".callada"] = new List<IModelFileHandler> { new AssimpModelHandler() },
            [".3ds"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },
            [".dae"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },
            //[".gltf"] = new List<IModelFileHandler> { new AssimpModelHandler() },
            //[".glb"] = new List<IModelFileHandler> { new AssimpModelHandler() },
            [".ply"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },
            [".off"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },
            //[".lwo"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },

            // Cad files
            [".step"] = new List<IModelFileHandler> { new OpenCascadeModelHandler() }
        };

        private static Model3DGroup ConvertToModel3DGroup(object obj)
        {
            switch (obj)
            {
                case Assimp.Scene scene:
                    return Converters.AssimpConverter.ConvertSceneToModel3DGroup(scene);

                default:
                    throw new Exception($"Тип {obj.GetType().FullName} не поддерживается для преобразования в System.Windows.Media.Media3D.Model3DGroup");
            }
        }

        /// <summary>
        /// Загрузка 3д модели
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Model3DGroup LoadModel3D(string path)
        {
            Model3DGroup model = null;

            if (!System.IO.File.Exists(path))
                throw new FileNotFoundException("Не удалось найти файл", path);

            string extension = System.IO.Path.GetExtension(path).ToLower();

            if (_handlersByFormat.TryGetValue(extension, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    if (!handler.SupportedImportExtensions.Contains(extension))
                        continue;

                    try
                    {
                        var obj = handler.LoadModel(path);
                        model = obj is Model3DGroup ? obj as Model3DGroup : ConvertToModel3DGroup(obj);
                        if (model != null) break;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Ошибка выгрузки модели через {handler.GetType().Name}: {ex.Message}", ex);
                    }
                }
                if (model == null)
                    throw new InvalidOperationException($"Ни один из обработчиков не смог загрузить модель из файла '{path}'");
            }
            else
            {
                throw new NotSupportedException($"Формат файла '{extension}' не поддерживается.");
            }
            return model;
        }

        public static void ExportModel3D(Model3D model3D, string path)
        {
            string extension = System.IO.Path.GetExtension(path).ToLower();

            if (_handlersByFormat.TryGetValue(extension, out var handlers))
            {
                bool exported = false;
                foreach (var handler in handlers)
                {
                    if (!handler.SupportedExportExtensions.Contains(extension))
                        continue;

                    try
                    {
                        handler.SaveModel(model3D, path);
                        exported = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Ошибка экспорта модели: {ex.Message}", ex);
                    }
                }
                if (!exported)
                    throw new InvalidOperationException($"Ни один из обработчиков не смог экспортировать модель в файл '{path}'.");
            }
            else
            {
                throw new NotSupportedException($"Формат файла '{extension}' не поддерживается.");
            }
        }
    }
}
