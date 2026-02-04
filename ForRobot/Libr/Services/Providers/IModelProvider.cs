using System;
using System.Windows.Media.Media3D;

namespace ForRobot.Libr.Services.Providers
{
    public interface IModelProvider
    {
        Model3DGroup GetPcModel();
        Model3DGroup GetRobotModel();
        Model3DGroup GetMansModel();
    }
}
