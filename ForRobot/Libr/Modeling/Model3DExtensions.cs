using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ForRobot.Libr.Modeling
{
    public static class Model3DGroupExtensions
    {
        /// <summary>
        /// Окрашивание модели в единый цвет
        /// </summary>
        /// <param name="modelGroup"></param>
        /// <param name="color"></param>
        public static void ApplyCustomColor(this Model3D modelGroup, Color color)
        {
            if (modelGroup is Model3DGroup group)
            {
                foreach (var model in group.Children)
                {
                    if (model is GeometryModel3D geometryModel)
                    {
                        geometryModel.Material = new DiffuseMaterial(new SolidColorBrush(color));
                        geometryModel.BackMaterial = geometryModel.Material;
                    }
                    else if (model is Model3DGroup childGroup)
                    {
                        ApplyCustomColor(childGroup, color);
                    }
                }
            }
        }
    }
}
