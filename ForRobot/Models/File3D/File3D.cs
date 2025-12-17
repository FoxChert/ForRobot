using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//using HelixToolkit;
using HelixToolkit.Wpf;
using Assimp;
//using HelixToolkit.

using CommunityToolkit.Diagnostics;
//using CommunityToolkit.
//using CommunityToolkit.Mvvm.Input;

using ForRobot.Models.Detals;
using ForRobot.Libr.Clipboard.UndoRedo;
using ForRobot.Libr.Services;

namespace ForRobot.Models.File3D
{
    public abstract class File3D : IFile3D, IDisposable
    {
        #region Private variables

        private static readonly Dictionary<string, List<IModelFileHandler>> _handlersByFormat;

        //private readonly ForRobot.Libr.Clipboard.UndoRedoManager _undoRedoManager;
        private readonly Dispatcher dispatcher;

        private bool _isSaved = true;
        private string _path;

        #endregion Private variables

        #region Public variables

        /// <summary>
        /// Сохранены ли последнии изменения
        /// </summary>
        public bool IsSaved
        {
            get => this._isSaved;
            private set
            {
                this._isSaved = value;
                this.OnPropertyChanged(nameof(IsSaved));
            }
        }

        public string Path
        {
            get => this._path;
            protected set
            {
                this._path = value;
                this.OnPropertyChanged(nameof(this.Path), nameof(this.Name));
            }
        }
        public string Name => System.IO.Path.GetFileName(this.Path);
        public abstract string Filter { get; }

        //public bool CanUndo => this._undoRedoManager.CanUndo;
        //public bool CanRedo => this._undoRedoManager.CanRedo;

        public abstract Model3DGroup Model { get; protected set; }

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ModelChangedEvent;

        #endregion Events

        #endregion Public variables

        #region Constructors

        static File3D()
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

        public File3D()
        {
            //this._undoRedoManager = new ForRobot.Libr.Clipboard.UndoRedoManager(new ForRobot.Libr.Clipboard.CacheClipboardProvider(), this.Path);
            this.dispatcher = Dispatcher.CurrentDispatcher;
            this.ModelChangedEvent += (s, e) => this.OnPropertyChanged(nameof(this.Model));
            this.PropertyChanged += (s, e) => { if(e.PropertyName != nameof(this.IsSaved)) this.IsSaved = false; };
        }

        public File3D(string path) : this()
        {
            this.Path = path;
            this.Model = this.LoadModel3D(path);
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

        private void OnPropertyChanged(params string[] propertyNames)
        {
            foreach (var prop in propertyNames)
            {
                this.OnPropertyChanged(prop);
            }
        }

        #endregion Private functions

        public static File3D Load(string path) => ForRobot.Libr.Factories.File3DFactory.Create(path);

        //public void Undo() => this._undoRedoManager.Undo();
        //public void Redo() => this._undoRedoManager.Redo();

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected virtual void OnModelChanged() => this.ModelChangedEvent?.Invoke(this, null);

        #region Implementations of IDisposable

        private volatile bool _disposed = false;

        ~File3D() => Dispose(false);

        public void Dispose() => this.Dispose(true);

        public virtual void Dispose(bool disposing)
        {
            if (this._disposed)
                return;

            if (disposing)
            {
                //this._undoRedoManager.ClearUndoRedoHistory();
                this.ModelChangedEvent -= (s, e) => this.OnPropertyChanged(nameof(this.Model));
                this.PropertyChanged -= (s, e) => { if (e.PropertyName != nameof(this.IsSaved)) this.IsSaved = false; };
                //this._detalFactory.ClearCache();

                //this.ModelChangedEvent -= (s, o) => GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new Libr.Behavior.HelixSceneTrackerMessage());
                //this.ModelChangedEvent -= new ChangeService().HandleModelChanged;
                //this.DetalChangedEvent -= new ChangeService().HandleDetalChanged_Properties;
                //this.DetalChangedEvent -= new ChangeService().HandleDetalChanged_Modeling;
                //this.FileChangedEvent -= new ChangeService().HandleFileChange;
            }
            this._disposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
