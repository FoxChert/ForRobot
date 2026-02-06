using System;
using System.Linq;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

using HelixToolkit.Wpf;

namespace ForRobot.Libr.Modeling
{
    /// <summary>
    /// Класс расширения класса <see cref="Model3DCollection"/>
    /// </summary>
    public static partial class Model3DCollectionExtensions
    {
        /// <summary>
        /// Добавление модели робота
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="scaleFactor">Маштабный коэффициент</param>
        public static void AddRobotModel(this ICollection<Model3D> source, double x = 0, double y = 0, double z = 0, Transform3DGroup transform3DGroup = null)
        {
            Model3DGroup robotModel = ModelingService.GetRobotModel(x, y, z, transform3DGroup);
            robotModel.ApplyCustomColor(ForRobot.Themes.Colors.RobotColor);
            robotModel.SetName(string.Format("Robot {0}", source.Count(item => item.GetName().Contains("Robot")) + 1));
            source.Add(robotModel);
        }

        /// <summary>
        /// Добавление модели компьютера
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="transform3DGroup"></param>
        public static void AddPcModel(this ICollection<Model3D> source, double x = 0, double y = 0, double z = 0, Transform3DGroup transform3DGroup = null)
        {
            Model3DGroup pcModel = ModelingService.GetPcModel(x, y, z, transform3DGroup);
            pcModel.ApplyCustomColor(ForRobot.Themes.Colors.PcColor);
            pcModel.SetName(string.Format("PC {0}", source.Count(item => item.GetName().Contains("PC")) + 1));
            source.Add(pcModel);
        }

        /// <summary>
        /// Добавление модели человека
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="scaleFactor">Маштабный коэффициент</param>
        /// <param name="modelTransform"></param>
        public static void AddMansModel(this ICollection<Model3D> source, double x = 0, double y = 0, double z = 0, Transform3DGroup transform3DGroup = null)
        {
            Model3DGroup manModel = ModelingService.GetMansModel(x, y, z, transform3DGroup);
            manModel.ApplyCustomColor(ForRobot.Themes.Colors.WatcherColor);
            manModel.SetName(string.Format("Man {0}", source.Count(item => item.GetName().Contains("Man")) + 1));
            source.Add(manModel);
        }
    }
}
