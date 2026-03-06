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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model3D"></param>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public static void Rotate(this Model3D model3D, Vector3D axis, double angle)
        {
            Rect3D bounds = model3D.Bounds;
            Point3D center = new Point3D(bounds.X + bounds.SizeX / 2, bounds.Y + bounds.SizeY / 2, bounds.Z + bounds.SizeZ / 2);
            model3D.Transform = Transform3DBuilder.Create().Rotate(axis, angle, center);
        }

        public static void Translate(this Model3D model3D, double x, double y, double z)
        {
            model3D.Transform = Transform3DBuilder.Create().Translate(x, y, z);
        }

        public static void Scale(this Model3D model3D, double x, double y, double z)
        {
            model3D.Transform = Transform3DBuilder.Create().Scale(x, y, z);
        }
    }
}
