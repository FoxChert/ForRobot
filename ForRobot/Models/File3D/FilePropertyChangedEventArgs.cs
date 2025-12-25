using System;

namespace ForRobot.Models.File3D
{
    public class FilePropertyChangedEventArgs : EventArgs
    {
        public IFile3D File { get; }
        public string PropertyName { get; }

        public FilePropertyChangedEventArgs(IFile3D file, string propertyName)
        {
            File = file;
            PropertyName = propertyName;
        }
    }
}