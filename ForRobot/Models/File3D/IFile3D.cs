using System;
using System.Windows.Media.Media3D;
using System.ComponentModel;

namespace ForRobot.Models.File3D
{
    public interface IFile3D : INotifyPropertyChanged
    {
        string Path { get; }
        Model3DGroup Model { get; }
    }
}
