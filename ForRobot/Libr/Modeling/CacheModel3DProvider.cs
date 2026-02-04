using System;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

using ForRobot.Libr.Services.Providers;

namespace ForRobot.Libr.Modeling
{
    public class CacheModel3DProvider : IModelProvider
    {
        private readonly IModelProvider _innerProvider;
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();
        private readonly object _lock = new object();

        public CacheModel3DProvider(IModelProvider innerProvider)
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

        public Model3DGroup GetMansModel() => GetCached("man", () => _innerProvider.GetMansModel());

        public Model3DGroup GetPcModel() => GetCached("pc", () => _innerProvider.GetMansModel());

        public Model3DGroup GetRobotModel() => GetCached("robot", () => _innerProvider.GetRobotModel());

        public void ClearCache()
        {
            lock (_lock)
            {
                _cache.Clear();
            }
        }
    }
}
