using System;
using System.Windows.Media.Media3D;

namespace ForRobot.Models.File3D
{
    public class HelixFile3D : File3D
    {
        #region Private variables

        private Model3DGroup _model = new Model3DGroup();

        #endregion private variables

        #region Public variables

        public override string Filter { get; } = "3ds Max Files (*.3ds)|.3ds|" +
                                                 "Wavefront Object Files (*.obj)|*.obj|" +
                                                 "Wavefront Object Files zipped (*.objz)|*.objz|" +
                                                 "Object File Format (*.off)|*.off|" +
                                                 "LightWave 3D Object Files (*.lwo)|*.lwo|" +
                                                 "STereoLithography (*.stl)|*.stl|" +
                                                 "Polygon Model Files (*.ply)|*.ply|" +
                                                 "All Supported Files (*.3ds, *.obj, *.objz, *.off, *.lwo, *.stl, *ply)|*.3ds;*.obj;*.objz;*.off;*.lwo;*.stl;*ply";

        public override Model3DGroup Model
        {
            get => this._model;
            protected set
            {
                this._model = value;
                this.OnModelChanged();
            }
        }

        #endregion Public variables
    }
}
