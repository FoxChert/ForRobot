using System;
using System.Windows.Media.Media3D;

namespace ForRobot.Models.File3D
{
    public class MeshModelFile3D : File3D
    {
        #region Private variables

        private Model3DGroup _model = new Model3DGroup();

        #endregion private variables

        #region Public variables

        public override string Filter { get; } = "3D Mesh Files (*.stl;*.obj;*.ply)|*.stl;*.obj;*.ply";

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

        #region Constructors

        public MeshModelFile3D(string path) : base(path) { }

        #endregion Constructors
    }
}
