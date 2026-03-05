using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace ForRobot.Libr.Modeling
{
    /// <summary>
    /// Класс расширяющий тип <see cref="Model3D"/>
    /// </summary>
    public static class Model3DExtensions
    {
        public static List<MeshGeometry3D> ExtractGeometries(this Model3D model3D)
        {
            if (model3D == null)
                throw new ArgumentNullException(nameof(model3D));

            var geometries = new List<MeshGeometry3D>();
            switch (model3D)
            {
                case Model3DGroup model3DGroup:
                    foreach (var model in model3DGroup.Children)
                        geometries.AddRange(ExtractGeometries(model));
                    break;

                case GeometryModel3D geometryModel:
                    if (geometryModel.Geometry is MeshGeometry3D mesh)
                        geometries.Add(mesh);
                    break;
            }
            return geometries;
        }

        /// <summary>
        /// Окрашивание модели в единый цвет
        /// </summary>
        /// <param name="modelGroup"></param>
        /// <param name="color"></param>
        public static void ApplyCustomColor(this Model3D model3D, Color color)
        {
            if (model3D == null) return;

            switch (model3D)
            {
                case Model3DGroup model3DGroup:
                    foreach (var model in model3DGroup.Children)
                        ApplyCustomColor(model, color);
                    break;

                case GeometryModel3D geometryModel:
                    geometryModel.Material = new DiffuseMaterial(new SolidColorBrush(color));
                    geometryModel.BackMaterial = geometryModel.Material;
                    break;
            }
        }

        /// <summary>
        /// Преобразование модели
        /// </summary>
        /// <param name="modelGroup"></param>
        /// <param name="transform">Матрица преобразования</param>
        public static void ApplyTransformToModel(this Model3D model3D, Matrix3D transform)
        {
            if (model3D == null) return;

            switch (model3D)
            {
                case Model3DGroup model3DGroup:
                    foreach (var model in model3DGroup.Children)
                        ApplyTransformToModel(model, transform);
                    break;

                case GeometryModel3D geometryModel:
                    geometryModel.Transform = new MatrixTransform3D(transform);
                    break;
            }
        }
    }
}
