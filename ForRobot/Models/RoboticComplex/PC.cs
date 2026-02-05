using System;
using System.Windows.Media.Media3D;

using ForRobot.Libr.Modeling;

namespace ForRobot.Models.RoboticComplex
{
    public class PC : SceneItem
    {
        //public Type ObjectType { get => this.GetType(); }

        /// <summary>
        /// Смещение по осям
        /// </summary>
        public double[] XYZ { get => new double[3] { X, Y, Z }; }

        /// <summary>
        /// Смещение по оси x, мм.
        /// </summary>
        public double X { get; set; } // Поменять на вывод из соединения.
        /// <summary>
        /// Смещение по оси y, мм.
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// Смещение по оси z, мм.
        /// </summary>
        public double Z { get; set; }
        
        /// <summary>
        /// Вращение по осям
        /// </summary>
        public double[] ABC { get => new double[3] { RotationX, RotationY, RotationZ }; }
        
        /// <summary>
        /// Вращение по оси x
        /// </summary>
        public double RotationX { get; set; } // A
        /// <summary>
        /// Вращение по оси y
        /// </summary>
        public double RotationY { get; set; } // B
        /// <summary>
        /// Вращение по оси z
        /// </summary>
        public double RotationZ { get; set; } // C

        public PC()
        {
            this.Children.Add(this.GetModel());
        }

        public override Model3DGroup GetModel()
        {
            //Vector3D pcTranslate = new Vector3D(this.X, this.Y, this.Z);
            Transform3DGroup transform3DGroup = Transform3DBuilder.Create().Translate(this.X, this.Y, this.Z);
            Model3DGroup model = ModelingService.GetPcModel(this.X, this.Y, this.Z, transform3DGroup);
            return model;
        }
    }
}
