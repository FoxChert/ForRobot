using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.ComponentModel;

namespace ForRobot.Libr
{
    /// <summary>
    /// Представляет изменяемую версию <see cref="KeyValuePair{TKey, TValue}"/>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class MutableKeyValuePair<TKey, TValue> : INotifyPropertyChanged
    {
        private TKey _key;
        private TValue _value;

        public TKey Key
        {
            get => _key;
            set
            {
                _key = value;
                OnPropertyChanged();
            }
        }

        public TValue Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public MutableKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public static implicit operator MutableKeyValuePair<TKey, TValue>(KeyValuePair<TKey, TValue> kvp)
        {
            return new MutableKeyValuePair<TKey, TValue>(kvp.Key, kvp.Value);
        }
    }
}
