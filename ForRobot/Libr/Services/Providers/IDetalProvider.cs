using System;
using ForRobot.Models.Detals;

namespace ForRobot.Libr.Services.Providers
{
    public interface IDetalProvider
    {
        Plita CreatePlita();
        PlitaStringer CreatePlitaStringer();
        PlitaTreygolnik CreatePlitaTreygolnik();
    }
}
