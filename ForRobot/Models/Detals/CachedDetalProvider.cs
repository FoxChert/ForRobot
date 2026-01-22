using System;
using System.Collections.Generic;

using ForRobot.Libr.Services.Providers;

namespace ForRobot.Models.Detals
{
    public class CachedDetalProvider : ForRobot.Libr.Services.Providers.IDetalProvider
    {
        private readonly IDetalProvider _innerProvider;
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();
        private readonly object _lock = new object();

        public CachedDetalProvider(IDetalProvider innerProvider)
        {
            _innerProvider = innerProvider ?? throw new ArgumentNullException(nameof(innerProvider));
        }

        private T GetCached<T>(string key, Func<T> factory) where T : class
        {
            lock (_lock)
            {
                if (!_cache.ContainsKey(key))
                {
                    _cache[key] = factory();
                }
                return (T)_cache[key];
            }
        }

        public Plita CreatePlita() => GetCached(typeof(Plita).FullName, () => _innerProvider.CreatePlita());

        public PlitaStringer CreatePlitaStringer()
        {
            throw new NotImplementedException();
        }

        public PlitaTreygolnik CreatePlitaTreygolnik()
        {
            throw new NotImplementedException();
        }

        public void ClearCache()
        {
            lock (_lock)
            {
                _cache.Clear();
            }
        }

    }
}
