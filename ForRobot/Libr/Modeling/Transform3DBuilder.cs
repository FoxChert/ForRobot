using System;
using System.Linq;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace ForRobot.Libr.Modeling
{
    public static class Transform3DBuilder
    {
        /// <summary>
        /// Создание нового экземпляра <see cref="Transform3DGroup"/>
        /// </summary>
        /// <returns></returns>
        public static Transform3DGroup Create()
        {
            return new Transform3DGroup();
        }

        /// <summary>
        /// Маштабирование
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Transform3DGroup Scale(this Transform3DGroup transform, double x, double y, double z)
        {
            transform.Children.Add(new ScaleTransform3D(x, y, z));
            return transform;
        }

        public static Transform3DGroup Scale(this Transform3DGroup transform, double scale) => Scale(transform, scale, scale, scale);

        public static Transform3DGroup Translate(this Transform3DGroup transform, double x, double y, double z)
        {
            transform.Children.Add(new TranslateTransform3D(x, y, z));
            return transform;
        }

        public static Transform3DGroup Translate(this Transform3DGroup transform, Vector3D vector3D) => Translate(transform, vector3D.X, vector3D.Y, vector3D.Z);

        public static Transform3DGroup Rotate(this Transform3DGroup transform, Vector3D axis, double angle, Point3D center = new Point3D())
        {
            transform.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(axis, angle), center));
            return transform;
        }

        public static Transform3DGroup Rotate(this Transform3DGroup transform, AxisAngleRotation3D axisAngleRotation3D, Point3D center = new Point3D())
        {
            transform.Children.Add(new RotateTransform3D(axisAngleRotation3D, center));
            return transform;
        }

        /// <summary>
        /// Проверка использования в <see cref="Transform3D"/> <see cref="ScaleTransform3D"/>
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static bool HasScalationApplied(this Transform3D transform, out IEnumerable<ScaleTransform3D> scaleTransform3D)
        {
            scaleTransform3D = null;
            switch (transform)
            {
                case Transform3DGroup transform3DGroup:
                    bool result = transform3DGroup.Children.OfType<ScaleTransform3D>().Any();
                    scaleTransform3D = result ? transform3DGroup.Children.OfType<ScaleTransform3D>() : null;
                    return result;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Проверка использования в <see cref="Transform3D"/> <see cref="TranslateTransform3D"/>
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static bool HasTranslationApplied(this Transform3D transform, out IEnumerable<TranslateTransform3D> translateTransform3D)
        {
            translateTransform3D = null;
            switch (transform)
            {
                case Transform3DGroup transform3DGroup:
                    bool result = transform3DGroup.Children.OfType<TranslateTransform3D>().Any();
                    translateTransform3D = result ? transform3DGroup.Children.OfType<TranslateTransform3D>() : null;
                    return result;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Проверка использования в <see cref="Transform3D"/> <see cref="RotateTransform3D"/>
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static bool HasRotationApplied(this Transform3D transform, out IEnumerable<RotateTransform3D> rotateTransform3D)
        {
            rotateTransform3D = null;
            switch (transform)
            {
                case Transform3DGroup transform3DGroup:
                    bool result = transform3DGroup.Children.OfType<RotateTransform3D>().Any();
                    rotateTransform3D = result ? transform3DGroup.Children.OfType<RotateTransform3D>() : null;
                    return result;

                default:
                    return false;
            }
        }
    }
}
