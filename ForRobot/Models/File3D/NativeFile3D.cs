using System;
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

        #endregion private variables

        #region Public variables

        public override string Filter { get; } = "Text Files (*.txt)|*.txt|Json Files (*.json)|*.json|All Supported Files (*.txt;*.json)|*.txt;*.json";

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

        #endregion Public variables

        #region Constructors
        
        /// <summary>
        /// Инициализация объекта <see cref="NativeFile3D"/> для чтения существующего файла
        /// </summary>
        /// <param name="path"></param>
        public NativeFile3D(string path, ForRobot.Libr.Factories.DetalFactory.IDetalFactory detalFactory) : base(path)
        {
            this._detalFactory = detalFactory;

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
        public NativeFile3D(string path, DetalType detalType, ForRobot.Libr.Factories.DetalFactory.IDetalFactory detalFactory) : base(path)
        {
            this._detalFactory = detalFactory;

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
                    if (this._undoBlock)
                    {
                        this.OnValueChanged(this, this._oldDetal, this.CurrentDetal, nameof(CurrentDetal));
                        //break;
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
            //switch (e.PropertyName)
            //{
            //    case nameof(Plate.RibsCount):
            //        break;
            //}

            //this.OnPropertyChanged(nameof(CurrentDetal));

            if (this._undoBlock)
                return;

            string path = ForRobot.Libr.PropertyPathHelper.GetFullPropertyPath(this.CurrentDetal, e.PropertyName);
            var oldValue = ForRobot.Libr.PropertyPathHelper.GetValueFromPath(this._oldDetal, path);
            var newValue = ForRobot.Libr.PropertyPathHelper.GetValueFromPath(this.CurrentDetal, path);

            if (!object.Equals(oldValue, newValue))
                this.OnValueChanged(this.CurrentDetal, oldValue, newValue, e.PropertyName);
        }

        #endregion

        /// <summary>
        /// Делегат изменения значения отслеживаемого свойства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void HandleValueChangedEvent(object sender, ForRobot.Libr.ValueChangedEventArgs e)
        {
            if (e.OldValue != null && e.NewValue != e.OldValue)
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

        /// <summary>
        /// Делегат события фиксации изменения отслеживаемого свойства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// Блокировщих срабатывания фиксации изменений при отмене/возврате (заменить на токен?)
        /// </summary>
        private bool _undoBlock = false;

        /// <summary>
        /// Отмена последнего изменения параметра <see cref="CurrentDetal"/>
        /// </summary>
        public override void Undo()
        {
            this._undoBlock = true;
            base.Undo();
            this._undoBlock = false;
        }

        /// <summary>
        /// Возврат отменённого изменения параметра <see cref="CurrentDetal"/>
        /// </summary>
        public override void Redo()
        {
            this._undoBlock = true;
            base.Redo();
            this._undoBlock = false;
        }

        /// <summary>
        /// Присвоение параметрам <see cref="CurrentDetal"/> стандартных значений
        /// </summary>
        public void StandartParamertrs()
        {
            if (this.CurrentDetal == null)
                return;

            var detal = this._detalFactory.CreateDetal(DetalTypes.StringToEnum(this.CurrentDetal.DetalType));

            switch (this.CurrentDetal.DetalType)
            {
                case DetalTypes.Plate:
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
            this._undoBlock = true;
            this.CurrentDetal = detal;
            this._undoBlock = false;
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
