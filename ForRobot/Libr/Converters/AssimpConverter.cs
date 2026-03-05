using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using Assimp;

namespace ForRobot.Libr.Converters
{
    public static class AssimpConverter
    {
        public static Model3DGroup ConvertSceneToModel3DGroup(Scene scene)
        {
            Model3DGroup modelGroup = new Model3DGroup();

            ProcessNode(scene.RootNode, scene, modelGroup);

            return modelGroup;
        }

        public static GeometryModel3D ConvertMeshToGeometryModel3D(Assimp.Mesh mesh)
        {
            var meshGeometry = new MeshGeometry3D();

            // Заполняем позиции вершин (с учетом смены системы координат)
            foreach (var vertex in mesh.Vertices)
            {
                meshGeometry.Positions.Add(new Point3D(vertex.X,  vertex.Z, vertex.Y));
            }
            
            foreach (var face in mesh.Faces)
            {
                if (face.IndexCount == 3)
                {
                    meshGeometry.TriangleIndices.Add(face.Indices[0]);
                    meshGeometry.TriangleIndices.Add(face.Indices[1]);
                    meshGeometry.TriangleIndices.Add(face.Indices[2]);
                }
            }

            var material = new DiffuseMaterial(Brushes.Gray);
            return new GeometryModel3D(meshGeometry, material);
        }

        public static MaterialGroup ConvertMaterial(Assimp.Material assimpMaterial)
        {
            var materialGroup = new MaterialGroup();
            var diffuseColor = new Color()
            {
                R = (byte)(assimpMaterial.ColorDiffuse.R * 255),
                G = (byte)(assimpMaterial.ColorDiffuse.G * 255),
                B = (byte)(assimpMaterial.ColorDiffuse.B * 255)
            };
            materialGroup.Children.Add(new DiffuseMaterial(new SolidColorBrush(diffuseColor)));
            return materialGroup;
        }

        private static void ProcessNode(Node node, Scene scene, Model3DGroup modelGroup)
        {
            foreach (var meshIndex in node.MeshIndices)
            {
                Assimp.Mesh mesh = scene.Meshes[meshIndex];
                GeometryModel3D geometryModel = ConvertMeshToGeometryModel3D(mesh);
                modelGroup.Children.Add(geometryModel);
            }

            foreach (var childNode in node.Children)
            {
                ProcessNode(childNode, scene, modelGroup);
            }
        }
    }
}
