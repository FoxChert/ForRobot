using System;
using System.IO;

using ForRobot.Models.File3D;
using ForRobot.Libr.Services.Providers;

namespace ForRobot.Libr.Factories
{
    public static class File3DFactory
    {
        private static IConfigurationProvider _configurationProvider;
        private static IJsonSchemaProvider _jsonSchemaProvider;

        static File3DFactory()
        {
            _configurationProvider = new ForRobot.Libr.Configuration.ConfigurationProvider(); // Проверить не инициализируется ли больше 1 раза
            _jsonSchemaProvider = new ForRobot.Libr.Json.Schemas.JsonSchemaProvider();
        }

        public static File3D Create(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Не удалось найти файл", path);

            string extension = System.IO.Path.GetExtension(path).ToLower();

            switch (extension)
            {
                case ".stl":
                case ".obj":
                case ".ply":
                    return new MeshModelFile3D(path);

                case ".json":
                case ".txt":
                    return new NativeFile3D(path, new ForRobot.Libr.Factories.DetalFactory.DetalFactory(_configurationProvider, _jsonSchemaProvider));

                default:
                    throw new Exception(string.Format("Расширение {0} не поддерживается", extension));
            }
        }
    }
}
