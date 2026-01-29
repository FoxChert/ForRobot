using System;
using System.Collections.Generic;
using System.Runtime.Caching;

using ForRobot.Libr.Clipboard.UndoRedo;

namespace ForRobot.Libr.Clipboard
{
    public class CacheClipboardProvider : IDisposable
    {
        private readonly Dictionary<string, UndoRedoStacks> _cache = new Dictionary<string, UndoRedoStacks>();
        private readonly object _lock = new object();

        public UndoRedoStacks GetOrAddStacks(string key)
        {
            lock (_lock)
            {
                if (!_cache.TryGetValue(key, out var stacks))
                {
                    stacks = new UndoRedoStacks();
                    _cache[key] = stacks;
                }
                return stacks;
            }
        }

        public void ClearCache()
        {
            lock (_lock)
            {
                _cache.Clear();
            }
        }

        public bool RemoveStacks(string key)
        {
            lock (_lock)
            {
                return _cache.Remove(key);
            }
        }

        #region Implementations of IDisposable

        public void Dispose()
        {
            this._cache.Clear();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
