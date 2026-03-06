using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

using ForRobot.Libr.Modeling;

namespace ForRobot.Models.File3D
{
    public class MeshModelFile3D : File3D
    {
        #region Private variables

        private Model3DGroup _currentModel = new Model3DGroup();
        //private IList<DependencyObject> _sceneItems = new List<DependencyObject>();

        #endregion private variables

        #region Public variables
            
        //public override IList<SceneItem> SceneItems
        //{
        //    get => this._sceneItems;
        //    protected set
        //    {
        //        this._sceneItems = value;
        //        this.OnSceneChanged();
        //    }
        //}

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

        public MeshModelFile3D(string path) : base(path)
        {
            //this.SceneItems.Add(this.LoadModel3D(path) as Model3DGroup);
            this.CurrentModel = this.LoadModel3D(path);
        }

        #endregion Constructors

        #region Private functions

        private Model3DGroup LoadModel3D(string path) => Model3DManager.LoadModel3D(path);

        #endregion Private functions

        #region Public functions

        public override void Save(string path)
        {
            Model3DManager.ExportModel3D(this.CurrentModel, path);
        }

        protected override string GetFilter()
        {
            string filter = string.Empty;
            string allFormat = string.Empty;
            foreach (var format in Libr.Registry.FileFormatRegistry.GetByCategory(FormatCategories.MeshFile).ToList())
            {
                filter = string.Format("{0} Files {1}|{1}", format.FormatsName, format.Extensions.Select(item => String.Join(";", $"*{item}")));
                allFormat += format.Extensions.Select(item => String.Join(";", $"*{item}"));
            }
            return string.Join("|", filter, $"All Supported Files {allFormat}|{allFormat}");
        }
        protected override void HandleValueChangedEvent(object sender, ForRobot.Libr.ValueChangedEventArgs e) { }
        protected override void HandleUndoRedoStateChangedEvent(object sender, EventArgs e) { }

        #endregion Public functions
    }
}
