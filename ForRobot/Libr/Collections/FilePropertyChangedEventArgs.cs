using System;

using ForRobot.Models.File3D;

namespace ForRobot.Libr.Collections
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