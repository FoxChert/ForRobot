using System;
using System.ComponentModel;

namespace ForRobot.Models.Welding
{
    public class WeldingSchemaItem : INotifyPropertyChanged
    {
        public const string DEVAULT_VALUE = "-";

        private string _leftSide = DEVAULT_VALUE;
        private string _rightSide = DEVAULT_VALUE;

        public string LeftSide
        {
            get => this._leftSide;
            set
            {
                this._leftSide = value;
                this.OnChangeProperty(nameof(this.LeftSide));
            }
        }

        public string RightSide
        {
            get => this._rightSide;
            set
            {
                this._rightSide = value;
                this.OnChangeProperty(nameof(this.RightSide));
            }
        }

        public WeldingSchemaItem() { }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Вызов события изменения свойства
        /// </summary>
        /// <param name="propertyName">Наименование свойства</param>
        private void OnChangeProperty([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
