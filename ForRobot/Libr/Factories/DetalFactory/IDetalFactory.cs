using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ForRobot.Models.Detals;

namespace ForRobot.Libr.Factories.DetalFactory
{
    public interface IDetalFactory
    {
        T CreateDetal<T>(DetalType type) where T : Detal;
        Detal CreateDetal(DetalType type);

        T Deserialize<T>(string jsonString, JsonSerializerSettings jsonSerializerSettings = null, JsonLoadSettings jsonLoadSettings = null) where T : Detal;
        Detal Deserialize(string jsonString, JsonSerializerSettings jsonSerializerSettings = null, JsonLoadSettings jsonLoadSettings = null);

        string Serialize<T>(T detal, JsonSerializerSettings jsonSerializerSettings = null, bool isValidate = false) where T : Detal;
        string Serialize(Detal detal, JsonSerializerSettings jsonSerializerSettings = null, bool isValidate = false);

        void ClearCache();
    }
}
