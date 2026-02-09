using System;
using System.Windows.Media.Media3D;

namespace ForRobot.Models.RoboticComplex
{
    /// <summary>
    /// Сварочная ячейка
    /// </summary>
    public class WeldingCell
    {
        public string Name { get; }

        public double Width { get; set; }
        public double Length { get; set; }
        //public double Height { get; set; }
        public double Diagonal { get => Math.Sqrt(Math.Pow(this.Length, 2) + Math.Pow(this.Width, 2)); }

        public Model3DGroup GetModel()
        {
            Model3DGroup model = new Model3DGroup();
            return model;
        }
    }
}
