using System;
using System.Windows;
using System.Windows.Media.Imaging;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media.Media3D;

namespace ForRobot.Models.Detals
{
    public class PlateTreygolnik : Detal
    {
        [JsonIgnore]
        /// <summary>
        /// Тип детали
        /// </summary>
        public override string DetalType { get => DetalTypes.Treygolnik; }

        //public override sealed BitmapImage GenericImage { get => (BitmapImage)Application.Current.FindResource("ImagePlitaTreygolnikFull"); }

        #region Constructors

        public PlateTreygolnik() { }

        #endregion
    }
}
