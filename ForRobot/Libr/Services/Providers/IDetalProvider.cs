using System;
using ForRobot.Models.Detals;

namespace ForRobot.Libr.Services.Providers
{
    public interface IDetalProvider
    {
        Plate CreatePlita();
        PlateStringer CreatePlitaStringer();
        PlateTreygolnik CreatePlitaTreygolnik();
    }
}
