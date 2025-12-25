using System;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

using ForRobot.Models.Detals;
using ForRobot.Libr.Collections;

namespace ForRobot.Models.File3D
{
    public class NativeFile3D : File3D
    {
        #region Private variables

        private readonly ForRobot.Libr.Factories.DetalFactory.IDetalFactory _detalFactory;

        private string _selectedWeldingSchema = WeldingSchemas.GetDescription(WeldingSchemas.SchemasTypes.LeftEvenOdd_RightEvenOdd);

        private Model3DGroup _currentModel = new Model3DGroup();

        private Detal _currentDetal;

        private FullyObservableCollection<WeldingSchemas.SchemaItem> _weldingSchema;

        #endregion private variables

        #region Public variables

        public override string Filter { get; } = "Text Files (*.txt)|*.txt|Json Files (*.json)|*.json|All Supported Files (*.txt;*.json)|*.txt;*.json";

        /// <summary>
        /// Выбранная схема сварки рёбер
        /// </summary>
        public string SelectedWeldingSchema
        {
            get => this._selectedWeldingSchema;
            set
            {
                this._selectedWeldingSchema = value;
                this.OnPropertyChanged(nameof(this.SelectedWeldingSchema));
            }
        }

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

        public FullyObservableCollection<WeldingSchemas.SchemaItem> WeldingSchema
        {
            get => this._weldingSchema;
            private set
            {
                this._weldingSchema = value;
                this.OnPropertyChanged(nameof(this.WeldingSchema));
            }
        }

        #endregion Public variables

        #region Constructors

        //public NativeFile3D() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public NativeFile3D(string path, ForRobot.Libr.Factories.DetalFactory.IDetalFactory detalFactory) : base(path)
        {
            this._detalFactory = detalFactory;

            this.PropertyChanged += HandlePropertyChange;

            string jsonString = System.IO.File.ReadAllText(path);
            this.CurrentDetal = this._detalFactory.Deserialize(jsonString);
        }

        public NativeFile3D(string path, DetalType detalType, ForRobot.Libr.Factories.DetalFactory.IDetalFactory detalFactory) : base(path)
        {
            this._detalFactory = detalFactory;

            this.PropertyChanged += HandlePropertyChange;

            this.CurrentDetal = this._detalFactory.CreateDetal(detalType);
        }

        #endregion Constructors

        #region Private functions

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
                case nameof(SelectedWeldingSchema):
                    if (this.SelectedWeldingSchema == ForRobot.Models.Detals.WeldingSchemas.GetDescription(ForRobot.Models.Detals.WeldingSchemas.SchemasTypes.Edit))
                        break;

                    this.WeldingSchema = ForRobot.Models.Detals.WeldingSchemas.BuildingSchema(ForRobot.Models.Detals.WeldingSchemas.GetSchemaType(this.SelectedWeldingSchema), (this.CurrentDetal as Plita).RibsCount) as FullyObservableCollection<WeldingSchemas.SchemaItem>;
                    this.WeldingSchema.CollectionChanged += (s, o) => this.OnPropertyChanged(nameof(this.WeldingSchema));
                    this.WeldingSchema.ItemPropertyChanged += (s, o) =>
                    {
                        this.SelectedWeldingSchema = ForRobot.Models.Detals.WeldingSchemas.GetDescription(WeldingSchemas.SchemasTypes.Edit);
                        this.OnPropertyChanged(nameof(this.WeldingSchema));
                    };
                    break;
            }
        }

        /// <summary>
        /// Делегат изменения свойства класса <see cref="Detal"/> объекта <see cref="CurrentDetal"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleCurrentDetalPropertyChange(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Plita.RibsCount):
                    break;
            }
            this.OnModelChanged();
        }

        #endregion
        
        private void SetDetal(object value)
        {
            if (this._currentDetal == value) // Изменить на сравнение объеков
                return;

            if (this._currentDetal != null) // Отписка событий
            {
                this._currentDetal.ChangePropertyEvent -= HandleCurrentDetalPropertyChange;
            }

            this._currentDetal = value as Detal;

            if (this._currentDetal == null)
                return;

            this._currentDetal.ChangePropertyEvent += HandleCurrentDetalPropertyChange;
            this._currentDetal.OnChangeProperty();
        }

        #endregion Private functions

        #region Public functions

        public override void Save(string path)
        {
            string jsonString = this._detalFactory.Serialize(this.CurrentDetal);

            if (jsonString == string.Empty) return;

            System.IO.File.WriteAllText(path, jsonString);
        }

        public void StandartParamertrs()
        {
            if (this.CurrentDetal == null)
                return;

            var detal = this._detalFactory.CreateDetal(DetalTypes.StringToEnum(this.CurrentDetal.DetalType));

            switch (this.CurrentDetal.DetalType)
            {
                case DetalTypes.Plita:
                    Plita plita = this.CurrentDetal as Plita;
                    (detal as Plita).ScoseType = plita.ScoseType;
                    (detal as Plita).DiferentDistance = plita.DiferentDistance;
                    (detal as Plita).ParalleleRibs = plita.ParalleleRibs;
                    (detal as Plita).DiferentDissolutionLeft = plita.DiferentDissolutionLeft;
                    (detal as Plita).DiferentDissolutionRight = plita.DiferentDissolutionRight;
                    break;

                default:
                    return;
            }
            this.CurrentDetal = detal;
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
