using System;
using System.Windows.Media.Media3D;

namespace ForRobot.Libr.Modeling
{
    public class Model3DProvider : ForRobot.Libr.Services.Providers.IModelProvider
    {
        public const string RobotModelPath = "pack://application:,,,/InterfaceOfRobots;component/3DModels/kukaRobot.stl";
        public const string PCModelPath = "pack://application:,,,/InterfaceOfRobots;component/3DModels/computer_monitor.stl";
        public const string ManModelPath = "pack://application:,,,/InterfaceOfRobots;component/3DModels/stickman.stl";

        public Model3DGroup GetMansModel() => Model3DManager.LoadModel3D(ManModelPath);
        public Model3DGroup GetPcModel() => Model3DManager.LoadModel3D(PCModelPath);
        public Model3DGroup GetRobotModel() => Model3DManager.LoadModel3D(RobotModelPath);
    }
}
