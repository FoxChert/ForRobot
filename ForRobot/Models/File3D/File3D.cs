using System;
using System.Windows.Threading;
using System.Windows.Media.Media3D;
using System.ComponentModel;

namespace ForRobot.Models.File3D
{
    /// <summary>
    /// Базовый класс представляющий файл в приложении
    /// </summary>
    public abstract class File3D : IFile3D, IDisposable
    {
        #region Private variables

        private readonly Dispatcher dispatcher;

        private bool _isSaved = true;
        private string _path;
        private ForRobot.Libr.Clipboard.UndoRedoManager _undoRedoManager;

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

        /// <summary>
        /// Директория файла
        /// </summary>
        public string Path
        {
            get => this._path;
            private set
            {
                this._path = value;
                this.OnPropertyChanged(nameof(this.Path), nameof(this.Name));
            }
        }
        /// <summary>
        /// Наименование файла
        /// </summary>
        public string Name => System.IO.Path.GetFileName(this.Path);
        /// <summary>
        /// Фильтер файлового диалога для поиска файлов данного типа
        /// </summary>
        public abstract string Filter { get; }

        /// <summary>
        /// Можно ли отменить изменения отслеживаемого свойства
        /// </summary>
        public bool CanUndo => this._undoRedoManager == null ? false : this._undoRedoManager.CanUndo;
        /// <summary>
        /// Можно ли вернуть изменения отслеживаемого свойства
        /// </summary>
        public bool CanRedo => this._undoRedoManager == null ? false : this._undoRedoManager.CanRedo;

        public abstract Model3DGroup CurrentModel { get; protected set; }
        //public abstract System.Collections.Generic.IList<SceneItem> SceneItems { get; protected set; }

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        public event ForRobot.Libr.ValueChangedEventHandler ValueChangedEvent;
        //public event EventHandler SceneChangedEvent;
        public event EventHandler ModelChangedEvent;
        public event EventHandler SaveEvent;

        #endregion Events

        #endregion Public variables

        #region Constructors

        public File3D()
        {
            this.dispatcher = Dispatcher.CurrentDispatcher;
            //this.SceneChangedEvent += (s, e) => this.OnPropertyChanged(nameof(this.SceneItems));
            this.ModelChangedEvent += (s, e) => this.OnPropertyChanged(nameof(this.CurrentModel));
            this.PropertyChanged += (s, e) => { if(e.PropertyName != nameof(this.IsSaved)) this.IsSaved = false; };
            this.ValueChangedEvent += HandleValueChangedEvent;
        }

        public File3D(string path) : this()
        {
            this.Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        #endregion Constructors

        #region Private functions
        
        private void OnPropertyChanged(params string[] propertyNames)
        {
            foreach (var prop in propertyNames)
            {
                this.OnPropertyChanged(prop);
            }
        }

        #endregion Private functions

        #region Protected functions

        /// <summary>
        /// Делегат события изменения значения отслеживаемого свойства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void HandleValueChangedEvent(object sender, ForRobot.Libr.ValueChangedEventArgs e);
        /// <summary>
        /// Делегат события фиксации изменения отслеживаемого свойства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void HandleUndoRedoStateChangedEvent(object sender, EventArgs e);

        /// <summary>
        /// Событие фиксации изменения отслеживаемого свойства
        /// </summary>
        /// <param name="command"></param>
        protected virtual void AddUndoCommand(ForRobot.Libr.Clipboard.UndoRedo.PropertyChangeCommand command) => this._undoRedoManager.AddUndoCommand(command);
        /// <summary>
        /// Вызов события изменения свойства класса <see cref="File3D"/>
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        /// <summary>
        /// Вызов события изменения отслеживаемого свойства
        /// </summary>
        /// <param name="oldValue">Старое значение</param>
        /// <param name="newValue">Новое значение</param>
        /// <param name="propertyName">Имя свойства</param>
        protected virtual void OnValueChanged(object target, object oldValue, object newValue, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            this.ValueChangedEvent?.Invoke(target, new ForRobot.Libr.ValueChangedEventArgs(oldValue, newValue, propertyName));
        }

        /// <summary>
        /// Вызов события изменения <see cref="CurrentModel"/>
        /// </summary>
        protected virtual void OnModelChanged() => this.ModelChangedEvent?.Invoke(this, null);


        //protected virtual void OnSceneChanged() => this.SceneChangedEvent?.Invoke(this, null);

        /// <summary>
        /// Вызов события сохранения файла
        /// </summary>
        protected virtual void OnSave() => this.SaveEvent?.Invoke(this, null);

        #endregion Protected functions

        #region Public functions
        
        public static File3D Load(string path) => ForRobot.Libr.Factories.File3DFactory.Create(path);

        public static void Save(params File3D[] files)
        {
            foreach (File3D file in files) file.Save();
        }
        public abstract void Save(string path);
        public virtual void Save() => this.Save(this.Path);

        public virtual void Undo() => this._undoRedoManager?.Undo();
        public virtual void Redo() => this._undoRedoManager?.Redo();

        //public void SetUndoRedoManager(ForRobot.Libr.Clipboard.CacheClipboardProvider cacheClipboardProvider)
        //{
        //    this._undoRedoManager = new Libr.Clipboard.UndoRedoManager(cacheClipboardProvider, this.Path);
        //    this._undoRedoManager.UndoRedoStateChanged += this.HandleUndoRedoStateChangedEvent;
        //}

        #endregion Public functions

        #region Implementations of IDisposable

        private volatile bool _disposed = false;

        ~File3D() => this.Dispose(false);

        public void Dispose() => this.Dispose(true);

        public virtual void Dispose(bool disposing)
        {
            if (this._disposed)
                return;

            if (disposing)
            {
                if(this._undoRedoManager != null)
                {
                    this._undoRedoManager.ClearUndoRedoHistory();
                    this._undoRedoManager.UndoRedoStateChanged -= this.HandleUndoRedoStateChangedEvent;
                }
                //this.SceneChangedEvent -= (s, e) => this.OnPropertyChanged(nameof(this.SceneItems));
                this.ModelChangedEvent -= (s, e) => this.OnPropertyChanged(nameof(this.CurrentModel));
                this.PropertyChanged -= (s, e) => { if (e.PropertyName != nameof(this.IsSaved)) this.IsSaved = false; };
                this.ValueChangedEvent -= HandleValueChangedEvent;
            }
            this._disposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
