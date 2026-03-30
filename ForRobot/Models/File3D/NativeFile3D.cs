using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Media3D;

using ForRobot.Models.Detals;
using ForRobot.Libr.Collections;
using ForRobot.Libr.Clipboard.UndoRedo;

namespace ForRobot.Models.File3D
{
    public static class DetalExtensions
    {
        public static object GetProperty(this Detal detal, string propertyName)
        {
            Queue<object> queue = new Queue<object>();
            HashSet<object> visited = new HashSet<object>(new ForRobot.Libr.ObjectReferenceEqualityComparer());

            queue.Enqueue(detal);
            visited.Add(detal);

            while (queue.Count > 0)
            {
                object current = queue.Dequeue();
                Type currentType = current.GetType();                

                foreach (var property in currentType.GetProperties())
                {
                    if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return property.GetValue(current);
                    }

                    if (property.PropertyType.IsClass &&
                        property.PropertyType != typeof(RibCollection) && property.PropertyType != typeof(WeldCollcetion) &&
                        property.CanRead)
                    {
                        object propertyValue = property.GetValue(current);
                        if (propertyValue != null && !visited.Contains(propertyValue))
                        {
                            queue.Enqueue(propertyValue);
                            visited.Add(propertyValue);
                        }
                    }
                }
            }
            return null;

            //Type type = detal.GetType();
            //System.Reflection.PropertyInfo propertyInfo = type.GetProperty(propertyName);
            //if(propertyInfo == null)
            //{
            //    foreach(var prop in type.GetProperties())
            //    {
            //        var obj = prop.GetValue(detal);
            //    }
            //}
            //return propertyInfo.GetValue(detal);
        }
    }

    public class NativeFile3D : File3D
    {
        #region Private variables

        private readonly ForRobot.Libr.Factories.DetalFactory.IDetalFactory _detalFactory;

        private Model3DGroup _currentModel = new Model3DGroup();

        private Detal _currentDetal;
        /// <summary>
        /// Поле для сохранения объекта <see cref="CurrentDetal"/> после изменения
        /// </summary>
        private Detal _oldDetal;

        private readonly object _updateLock = new object();
        private CancellationTokenSource _undoRedoOperationToken = new CancellationTokenSource();
        private CancellationTokenSource _cancellationTokenSource;

        #endregion private variables

        #region Public variables

        public override Model3DGroup CurrentModel
        {
            get => this._currentModel;
            protected set
            {
                this._currentModel = value;
                this.OnModelChanged();
            }
        }

        public Detal CurrentDetal { get => this._currentDetal; set => this.SetDetal(value); }

        //public override IEnumerable<FileFormats> Extensions { get; } = new List<FileFormats>()
        //{
        //    new FileFormats("Text Files", ".txt"),
        //    new FileFormats("Json Files", ".json")
        //};

        #endregion Public variables

        #region Constructors

        /// <summary>
        /// Инициализация объекта <see cref="NativeFile3D"/> для чтения существующего файла
        /// </summary>
        /// <param name="path"></param>
        public NativeFile3D(string path, ForRobot.Libr.Factories.DetalFactory.IDetalFactory detalFactory = null) : base(path)
        {
            this._detalFactory = detalFactory ?? new ForRobot.Libr.Factories.DetalFactory.DetalFactory(new ForRobot.Models.Detals.DetalProvider(new ForRobot.Libr.Configuration.ConfigurationProvider()), new ForRobot.Libr.Json.Schemas.JsonSchemaProvider());

            this.PropertyChanged += HandlePropertyChange;

            string jsonString = System.IO.File.ReadAllText(path);
            this.CurrentDetal = this._detalFactory.Deserialize(jsonString);
        }

        /// <summary>
        /// Инициализация объекта <see cref="NativeFile3D"/>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="detalType"></param>
        /// <param name="detalFactory"></param>
        public NativeFile3D(string path, DetalType detalType, ForRobot.Libr.Factories.DetalFactory.IDetalFactory detalFactory = null) : base(path)
        {
            this._detalFactory = detalFactory ?? new ForRobot.Libr.Factories.DetalFactory.DetalFactory(new ForRobot.Models.Detals.DetalProvider(new ForRobot.Libr.Configuration.ConfigurationProvider()), new ForRobot.Libr.Json.Schemas.JsonSchemaProvider());

            this.PropertyChanged += HandlePropertyChange;

            this.CurrentDetal = this._detalFactory.CreateDetal(detalType);
        }

        #endregion Constructors

        #region Private functions

        private void SetDetal(object value)
        {
            if (this._currentDetal == value)
                return;

            if (this._currentDetal != null)
            {
                this._currentDetal.PropertyChanged -= this.HandlePropertyChange_CurrentDetal;
            }

            this._currentDetal = value as Detal;

            if (this._currentDetal == null)
                return;

            this._currentDetal.PropertyChanged += this.HandlePropertyChange_CurrentDetal;

            this.OnPropertyChanged(nameof(this.CurrentDetal));
        }

        #region Handle

        /// <summary>
        /// Делегат изменения свойства класса <see cref="NativeFile3D"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandlePropertyChange(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.CurrentDetal):
                    if (this._undoRedoOperationToken.Token.IsCancellationRequested)
                    {
                        this.OnValueChanged(this, this._oldDetal, this.CurrentDetal, nameof(CurrentDetal));
                    }
                    this._oldDetal = this.CurrentDetal.Clone() as Detal;
                    break;
            }
        }

        /// <summary>
        /// Делегат изменения свойства класса <see cref="Detal"/> объекта <see cref="CurrentDetal"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandlePropertyChange_CurrentDetal(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Plate.RibsCount):
                    break;
            }

            Detal detal = sender as Detal;

            if (!this._undoRedoOperationToken.Token.IsCancellationRequested)
            {

                string path = ForRobot.Libr.PropertyPathHelper.GetFullPropertyPath(detal, e.PropertyName);
                var oldValue = ForRobot.Libr.PropertyPathHelper.GetValueFromPath(this._oldDetal, path);
                var newValue = ForRobot.Libr.PropertyPathHelper.GetValueFromPath(detal, path);

                if (!object.Equals(oldValue, newValue))
                    this.OnValueChanged(detal, oldValue, newValue, e.PropertyName);
            }


            lock (this._updateLock)
            {
                this._cancellationTokenSource?.Cancel();
                this._cancellationTokenSource?.Dispose();
                this._cancellationTokenSource = new CancellationTokenSource();
            }
            _ = DebouncedUpdateAsync(_cancellationTokenSource.Token);
        }

        #endregion

        /// <inheritdoc cref="File3D.GetFilter()"/>
        protected override string GetFilter()
        {
            string filter = string.Empty;
            string allFormat = string.Empty;
            foreach (var format in Libr.Registry.FileFormatRegistry.GetByCategory(FormatCategories.NativeFile).ToList())
            {
                filter = string.Format("{0} Files {1}|{1}", format.FormatsName, format.Extensions.Select(item => String.Join(";", $"*{item}")));
                allFormat += format.Extensions.Select(item => String.Join(";", $"*{item}"));
            }
            return string.Join("|", filter, $"All Supported Files {allFormat}|{allFormat}");
        }

        /// <inheritdoc cref="File3D.HandleValueChangedEvent(object, Libr.ValueChangedEventArgs)"/>
        protected override void HandleValueChangedEvent(object sender, ForRobot.Libr.ValueChangedEventArgs e)
        {
            if (e.OldValue != null && !object.Equals(e.OldValue, e.NewValue))
            {
                var command = new PropertyChangeCommand(sender,
                                                        e.PropertyName,
                                                        e.OldValue,
                                                        e.NewValue,
                                                        $"Изменение свойства детали {e.PropertyName}: {e.OldValue} -> {e.NewValue}");
                this.AddUndoCommand(command);
            }
            this.OnPropertyChanged(e.PropertyName);
        }

        /// <inheritdoc cref="File3D.HandleUndoRedoStateChangedEvent(object, EventArgs)"/>
        protected override void HandleUndoRedoStateChangedEvent(object sender, EventArgs e)
        {
            this._oldDetal = this.CurrentDetal.Clone() as Detal;
        }

        #endregion Private functions

        #region Public functions

        public static NativeFile3D Create(string path, DetalType detalType) => ForRobot.Libr.Factories.File3DFactory.Create(path, detalType) as NativeFile3D;

        public override void Save(string path)
        {
            string jsonString = this._detalFactory.Serialize(this.CurrentDetal);

            if (jsonString == string.Empty) return;

            System.IO.File.WriteAllText(path, jsonString);
        }

        /// <summary>
        /// Отмена последнего изменения параметра <see cref="CurrentDetal"/>
        /// </summary>
        public override void Undo()
        {
            this._undoRedoOperationToken.Cancel();
            base.Undo();
            this._undoRedoOperationToken = new CancellationTokenSource();
        }

        /// <summary>
        /// Возврат отменённого изменения параметра <see cref="CurrentDetal"/>
        /// </summary>
        public override void Redo()
        {
            this._undoRedoOperationToken.Cancel();
            base.Redo();
            this._undoRedoOperationToken = new CancellationTokenSource();
        }

        /// <summary>
        /// Присвоение параметрам <see cref="CurrentDetal"/> стандартных значений
        /// </summary>
        public void StandartParamertrs()
        {
            if (this.CurrentDetal == null)
                return;

            var detal = this._detalFactory.CreateDetal(this.CurrentDetal.DetalType);

            switch (this.CurrentDetal.DetalType)
            {
                case DetalType.Plate:
                    Plate plita = this.CurrentDetal as Plate;
                    (detal as Plate).ScoseType = plita.ScoseType;
                    (detal as Plate).DiferentDistance = plita.DiferentDistance;
                    (detal as Plate).ParalleleRibs = plita.ParalleleRibs;
                    (detal as Plate).WeldingProperties.DiferentDissolutionLeft = plita.WeldingProperties.DiferentDissolutionLeft;
                    (detal as Plate).WeldingProperties.DiferentDissolutionRight = plita.WeldingProperties.DiferentDissolutionRight;
                    break;

                default:
                    return;
            }

            this._undoRedoOperationToken.Cancel();
            this.CurrentDetal = detal;
            this._undoRedoOperationToken = new CancellationTokenSource();
        }

        private readonly int _debounceDelayMs = 150;

        private async Task DebouncedUpdateAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(_debounceDelayMs, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return;

            if (this.CurrentDetal == null) return;

            await System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        //var model = ForRobot.Libr.Services.ModelingService.Get3DScene(this.CurrentDetal);
                        //var model = _modelingService.Get3DScene(file.CurrentDetal);
                        //file.CurrentModel.Children.Clear();
                        //file.CurrentModel.Children.Add(model);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Ошибка создания модели: " + ex.Message, ex);
                    }
                }
            }));
        }

        #endregion Public functions

        #region Implementations of IDisposable

        private volatile bool _disposed = false;

        public override void Dispose(bool disposing)
        {
            if (this._disposed)
                return;

            if (disposing)
            {
                this._detalFactory.ClearCache();

                this.PropertyChanged -= HandlePropertyChange;
            }
            this._disposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
