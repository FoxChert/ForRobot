using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace ForRobot.Models.File3D
{
    public class MeshModelFile3D : File3D
    {
        #region Private variables

        private static readonly Dictionary<string, List<IModelFileHandler>> _handlersByFormat;

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

        static MeshModelFile3D()
        {
            _handlersByFormat = new Dictionary<string, List<IModelFileHandler>>(StringComparer.OrdinalIgnoreCase)
            {
                [".stl"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },
                [".obj"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },
                //[".fbx"] = new List<IModelFileHandler> { new AssimpModelHandler() },
                //[".callada"] = new List<IModelFileHandler> { new AssimpModelHandler() },
                //[".3ds"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },
                //[".gltf"] = new List<IModelFileHandler> { new AssimpModelHandler() },
                //[".glb"] = new List<IModelFileHandler> { new AssimpModelHandler() },
                //[".ply"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },
                //[".off"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() },
                //[".lwo"] = new List<IModelFileHandler> { new AssimpModelHandler(), new HelixToolkitModelHandler() }
                //[".step"] = new List<IModelFileHandler> { new OpenCascadeModelHandler() }
            };
        }

        public MeshModelFile3D(string path) : base(path)
        {
            this.CurrentModel = this.LoadModel3D(path);
        }

        #endregion Constructors

        #region Private functions

        private Model3DGroup LoadModel3D(string path)
        {
            if (!System.IO.File.Exists(path))
                throw new FileNotFoundException("Не удалось найти файл", path);

            string extension = System.IO.Path.GetExtension(path).ToLower();
            System.Windows.Media.Media3D.Model3DGroup model = null;

            if (_handlersByFormat.TryGetValue(extension, out var handlers))
            {
                for (int i = 0; i < handlers.Count; i++)
                {
                    var modelFileHandler = handlers[i];
                    model = modelFileHandler.LoadModel(path) as System.Windows.Media.Media3D.Model3DGroup;

                    if (model != null) break;
                }
            }
            return model;
        }

        #endregion Private functions

        #region Public functions

        public override void Save(string path)
        {

        }

        protected override void HandleValueChangedEvent(object sender, ForRobot.Libr.ValueChangedEventArgs e) { }
        protected override void HandleUndoRedoStateChangedEvent(object sender, EventArgs e) { }

        #endregion Public functions
    }
}
