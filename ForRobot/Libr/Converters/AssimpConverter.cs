using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using Assimp;

namespace ForRobot.Libr.Converters
{
    /// <summary>
    /// Класс-преобразователь для типов <see cref="Assimp"/>
    /// </summary>
    public static class AssimpConverter
    {
        /// <summary>
        /// Преобразование всей <see cref="Assimp.Scene"/> в <see cref="Model3DGroup"/>
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="texturePath">Путь к файлу с текстурами</param>
        /// <returns></returns>
        public static Model3DGroup ConvertSceneToModel3DGroup(Scene scene, string texturePath = "")
        {
            Model3DGroup modelGroup = new Model3DGroup();
            ProcessNode(scene.RootNode, scene, modelGroup, texturePath);
            return modelGroup;
        }

        /// <summary>
        /// Рекурсивная обработка узлов <see cref="Assimp.Scene"/>
        /// </summary>
        /// <param name="node">Текущий узел</param>
        /// <param name="scene">Вся сцена</param>
        /// <param name="modelGroup">Родительская группа</param>
        /// <param name="texturePath">Путь к файлу с текстурами</param>
        private static void ProcessNode(Node node, Scene scene, Model3DGroup modelGroup, string texturePath)
        {
            foreach (var meshIndex in node.MeshIndices)
            {
                Assimp.Mesh mesh = scene.Meshes[meshIndex];
                GeometryModel3D geometryModel = ConvertMeshToGeometryModel3D(mesh, scene, texturePath);
                modelGroup.Children.Add(geometryModel);
            }

            foreach (var childNode in node.Children)
            {
                ProcessNode(childNode, scene, modelGroup, texturePath);
            }
        }

        /// <summary>
        /// Преобразование <see cref="Assimp.Mesh"/> в <see cref="GeometryModel3D"/>
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="scene"></param>
        /// <param name="texturePath"></param>
        /// <returns></returns>
        public static GeometryModel3D ConvertMeshToGeometryModel3D(Assimp.Mesh mesh, Scene scene, string texturePath)
        {
            if (!mesh.HasVertices)
                return null;

            var meshGeometry = new MeshGeometry3D();

            var positions = new Point3DCollection();

            foreach (var vertex in mesh.Vertices)
                positions.Add(new Point3D(vertex.X, vertex.Z, vertex.Y));

            meshGeometry.Positions = positions;

            if (!mesh.HasNormals || mesh.Normals.Count != mesh.Vertices.Count)
                GenerateNormals(meshGeometry);
            else
            {
                var normals = new Vector3DCollection();
                foreach (var normal in mesh.Normals)
                {
                    normals.Add(new System.Windows.Media.Media3D.Vector3D(normal.X, normal.Z, normal.Y));
                }
                meshGeometry.Normals = normals;
            }

            if (mesh.HasTextureCoords(0))
            {
                var textureCoordinates = new PointCollection();

                for (int i = 0; i < mesh.TextureCoordinateChannels[0].Count; i++)
                {
                    var texCoord = mesh.TextureCoordinateChannels[0][i];
                    textureCoordinates.Add(new System.Windows.Point(texCoord.X, texCoord.Y));
                }
                meshGeometry.TextureCoordinates = textureCoordinates;
            }

            var triangleIndices = new Int32Collection();
            foreach (var face in mesh.Faces)
            {
                if (face.IndexCount == 3)
                {
                    triangleIndices.Add(face.Indices[0]);
                    triangleIndices.Add(face.Indices[2]);
                    triangleIndices.Add(face.Indices[1]);
                }
            }
            meshGeometry.TriangleIndices = triangleIndices;

            System.Windows.Media.Media3D.Material material = null;
            if (mesh.MaterialIndex < scene.MaterialCount)
            {
                material = ConvertMaterial(scene.Materials[mesh.MaterialIndex], texturePath);
            }
            
            return new GeometryModel3D(meshGeometry, material ?? new DiffuseMaterial(Brushes.Gray));
        }

        /// <summary>
        /// Преобразование <see cref="Assimp.Material"/> в <see cref="System.Windows.Media.Media3D.Material"/>
        /// </summary>
        /// <param name="assimpMaterial"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public static System.Windows.Media.Media3D.Material ConvertMaterial(Assimp.Material assimpMaterial, string basePath)
        {
            try
            {
                if (assimpMaterial.HasTextureDiffuse)
                {
                    var textureSlot = assimpMaterial.TextureDiffuse;
                    var texturePath = textureSlot.FilePath;

                    if (!string.IsNullOrEmpty(texturePath))
                    {
                        try
                        {
                            string fullPath = System.IO.Path.Combine(basePath, texturePath);
                            if (System.IO.File.Exists(fullPath))
                            {
                                var bitmap = new System.Windows.Media.Imaging.BitmapImage(new Uri(fullPath));
                                var brush = new ImageBrush(bitmap);
                                return new DiffuseMaterial(brush);
                            }
                        }
                        catch (Exception ex) { }
                    }
                }
                
                var diffuseColor = GetDiffuseColor(assimpMaterial);
                
                if (diffuseColor.R < 30 && diffuseColor.G < 30 && diffuseColor.B < 30)
                    diffuseColor = System.Drawing.Color.LightGray;

                var opacity = GetOpacity(assimpMaterial);
                var color = System.Windows.Media.Color.FromArgb(
                    (byte)(opacity * 255),
                    diffuseColor.R,
                    diffuseColor.G,
                    diffuseColor.B
                );

                return new DiffuseMaterial(new SolidColorBrush(color));
            }
            catch(Exception ex)
            {
                return new DiffuseMaterial(Brushes.Gray);
            }
        }

        /// <summary>
        /// Извлечение диффузного цвета из материала Assimp
        /// </summary>
        /// <param name="material">Материал Assimp</param>
        /// <returns>Цвет в формате System.Drawing.Color</returns>
        public static System.Drawing.Color GetDiffuseColor(Assimp.Material material)
        {
            if (material.HasColorDiffuse)
            {
                var color = material.ColorDiffuse;
                return System.Drawing.Color.FromArgb(
                    (byte)(color.A * 255),
                    (byte)(color.R * 255),
                    (byte)(color.G * 255),
                    (byte)(color.B * 255)
                );
            }
            return System.Drawing.Color.Gray;
        }

        /// <summary>
        /// Извлечение зеркального цвета из материала Assimp
        /// </summary>
        /// <param name="material">Материал Assimp</param>
        /// <returns>Цвет в формате System.Drawing.Color</returns>
        public static System.Drawing.Color GetSpecularColor(Assimp.Material material)
        {
            if (material.HasColorSpecular)
            {
                var color = material.ColorSpecular;
                return System.Drawing.Color.FromArgb(
                    (byte)(color.A * 255),
                    (byte)(color.R * 255),
                    (byte)(color.G * 255),
                    (byte)(color.B * 255)
                );
            }
            return System.Drawing.Color.White;
        }

        /// <summary>
        /// Извлечение значения прозрачности из материала Assimp
        /// </summary>
        /// <param name="material">Материал Assimp</param>
        /// <returns>Значение прозрачности (0.0 - 1.0)</returns>
        public static float GetOpacity(Assimp.Material material)
        {
            if (material.HasOpacity)
            {
                return material.Opacity;
            }
            return 1.0f; // По умолчанию полностью непрозрачный (1.0)
        }

        /// <summary>
        /// Генерация нормалей для меша
        /// </summary>
        /// <param name="mesh"></param>
        public static void GenerateNormals(MeshGeometry3D mesh)
        {
            if (mesh.Positions == null || mesh.TriangleIndices == null)
                return;

            var normals = new Vector3DCollection(new System.Windows.Media.Media3D.Vector3D[mesh.Positions.Count]);

            for (int i = 0; i < normals.Count; i++)
            {
                normals[i] = new System.Windows.Media.Media3D.Vector3D(0, 0, 0);
            }

            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                int i1 = mesh.TriangleIndices[i];
                int i2 = mesh.TriangleIndices[i + 1];
                int i3 = mesh.TriangleIndices[i + 2];

                Point3D p1 = mesh.Positions[i1];
                Point3D p2 = mesh.Positions[i2];
                Point3D p3 = mesh.Positions[i3];

                System.Windows.Media.Media3D.Vector3D v1 = p2 - p1;
                System.Windows.Media.Media3D.Vector3D v2 = p3 - p1;
                System.Windows.Media.Media3D.Vector3D normal = System.Windows.Media.Media3D.Vector3D.CrossProduct(v1, v2);
                
                if (normal.Length > 0)
                    normal.Normalize();
                
                normals[i1] += normal;
                normals[i2] += normal;
                normals[i3] += normal;
            }
            for (int i = 0; i < normals.Count; i++)
            {
                if (normals[i].Length > 0)
                    normals[i].Normalize();
                else
                    normals[i] = new System.Windows.Media.Media3D.Vector3D(0, 1, 0);
            }

            mesh.Normals = normals;
        }
    }
}
