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
        
        private string _selectedWeldingSchema = WeldingSchemas.GetDescription(WeldingSchemas.SchemasTypes.LeftEvenOdd_RightEvenOdd);

        private Model3DGroup _model = new Model3DGroup();

        private Detal _currentDetal;

        private FullyObservableCollection<WeldingSchemas.SchemaItem> _weldingSchema;

        #endregion private variables

        #region Public variables

        public override string Filter { get; }

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

        public override Model3DGroup Model
        {
            get => this._model;
            protected set
            {
                this._model = value;
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

        public NativeFile3D() : base()
        {

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
            if (this._currentDetal == value)
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
    }
}
