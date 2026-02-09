using System;
using System.Windows;
using System.Windows.Media.Imaging;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media.Media3D;

namespace ForRobot.Models.Detals
{
    public class PlateStringer : Detal
    {
        [JsonIgnore]
        /// <summary>
        /// Тип детали
        /// </summary>
        public override DetalType DetalType { get => DetalType.Stringer; }

        //public override sealed BitmapImage GenericImage { get => (BitmapImage)Application.Current.FindResource("ImagePlitaStringerFull"); }

        #region Constructor

        public PlateStringer() { }

        #endregion
    }
}
