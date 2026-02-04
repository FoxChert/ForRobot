using System;
using System.Windows.Media.Media3D;

namespace ForRobot.Models
{
    public interface ISceneItem
    {
        string Name { get; }

        //bool IsVisible { get; set; }

        //Type ObjectType { get; }

        Model3DGroup GetModel();
    }
}
