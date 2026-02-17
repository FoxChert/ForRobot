using System;
using System.Windows.Media.Media3D;

using ForRobot.Libr.Collections;

namespace ForRobot.Libr.Converters
{
    public static class MatrixConverter
    {
        public static System.Windows.Media.Matrix MatrixToMatrix2D(Matrix matrix)
        {
            return new System.Windows.Media.Matrix(
                    matrix[0, 0], matrix[1, 0], matrix[2, 0],
                    matrix[0, 1], matrix[1, 1], matrix[2, 1]);
        }

        public static Matrix3D MatrixToMatrix3D(Matrix matrix)
        {
            return new System.Windows.Media.Media3D.Matrix3D(
                    matrix[0, 0], matrix[1, 0], matrix[2, 0], matrix[3, 0],
                    matrix[0, 1], matrix[1, 1], matrix[2, 1], matrix[3, 1],
                    matrix[0, 2], matrix[1, 2], matrix[2, 2], matrix[3, 2],
                    matrix[0, 3], matrix[1, 3], matrix[2, 3], matrix[3, 3]);
        }

        public static void ApplyTransformToModel(Model3DGroup modelGroup, Matrix3D transform)
        {
            if (modelGroup == null) return;

            foreach (var model in modelGroup.Children)
            {
                if (model is GeometryModel3D geometryModel)
                {
                    geometryModel.Transform = new MatrixTransform3D(transform);
                }
                else if (model is Model3DGroup childGroup)
                {
                    ApplyTransformToModel(childGroup, transform);
                }
            }
        }
    }
}
