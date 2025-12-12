using System;
using System.Windows.Media.Media3D;

namespace ForRobot.Models.File3D
{
    public class AssimpFile : File3D
    {
        #region Private variables

        private Model3DGroup _model = new Model3DGroup();

        #endregion private variables

        #region Public variables

        public override string Filter { get; } = "STereoLithography (*.stl)|*.stl|" +
                                                 "Wavefront Object Files (*.obj)|*.obj|" +
                                                 "Wavefront Object Files zipped (*.objz)|*.objz|" +
                                                 "Autodesk FBX Interchange Files (*.fbx)|.fbx|" +
                                                 "Collada Files (*.dae)|.dae|" +
                                                 "3ds Max Files (*.3ds)|.3ds|" +
                                                 "glTF Files (*.gltf;*.glb)|*.gltf;*.glb|" +
                                                 "Polygon Model Files (*.ply)|*.ply|" +
                                                 "Object File Format (*.off)|*.off|" +
                                                 "LightWave 3D Object Files (*.lwo)|*.lwo|" +
                                                 "All Supported Files (*.stl;*.obj;*.fbx;*.dae;*.3ds;*.gltf;*.glb;*.ply;*.off;*.lwo)|*.stl;*.obj;*.fbx;*.dae;*.3ds;*.gltf;*.glb;*.ply;*.off;*.lwo";

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