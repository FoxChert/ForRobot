using System;
using System.Windows.Media.Media3D;
using System.ComponentModel;

namespace ForRobot.Models
{
    public interface ISceneItem
    {
        string Name { get; }

        //bool IsVisible { get; set; }

        //Type ObjectType { get; }

        Model3DGroup GetModel();

        void UpdateTransform(Matrix3D transform);
    }
}
