using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

using ForRobot.Libr.Modeling;

namespace ForRobot.Models.File3D
{
    public class CadFile3D : File3D
    {
        #region Private variables

        private Model3DGroup _currentModel = new Model3DGroup();
        
        #endregion Private variables

        #region Public variables

        public override string Filter { get; } = "3D CAD-files (*.step)|*.step";

        public override Model3DGroup CurrentModel
        {
            get => this._currentModel;
            protected set
            {
                this._currentModel = value;
                this.OnModelChanged();
            }
        }

        //public override IDictionary<string, string> Extensions { get; }
        //    = new Dictionary<string, string>()
        //{
        //    ".stp", ".step"
        //};

        #endregion Public variables

        #region Constructors

        public CadFile3D(string path) : base(path)
        {
            //this.SceneItems.Add(this.LoadModel3D(path) as Model3DGroup);
            this.CurrentModel = this.LoadModel3D(path);
        }

        #endregion Constructors

        #region Private functions

        private Model3DGroup LoadModel3D(string path) => Model3DManager.LoadModel3D(path);

        #endregion Private functions

        #region Public functions

        protected override void HandleUndoRedoStateChangedEvent(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void HandleValueChangedEvent(object sender, ForRobot.Libr.ValueChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void Save(string path)
        {
            throw new NotImplementedException();
        }

        #endregion Public functions
    }
}
