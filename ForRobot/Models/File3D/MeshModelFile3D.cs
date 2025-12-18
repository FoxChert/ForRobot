using System;
using System.Windows.Media.Media3D;

namespace ForRobot.Models.File3D
{
    public class MeshModelFile3D : File3D
    {
        #region Private variables

        private Model3DGroup _currentModel = new Model3DGroup();

        #endregion private variables

        #region Public variables

        public override string Filter { get; } = "3D Mesh Files (*.stl;*.obj;*.ply)|*.stl;*.obj;*.ply";

        public override Model3DGroup CurrentModel
        {
            get => this._currentModel;
            protected set
            {
                this._currentModel = value;
                this.OnModelChanged();
            }
        }

        #endregion Public variables

        #region Constructors

        public MeshModelFile3D(string path) : base(path) { }

        #endregion Constructors

        #region Public functions

        public override void Save(string path)
        {

        }

        #endregion Public functions
    }
}
