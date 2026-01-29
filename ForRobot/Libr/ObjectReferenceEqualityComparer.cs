using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace ForRobot.Libr
{
    /// <summary>
    /// Класс для сравнения объектов по ссылке
    /// </summary>
    public class ObjectReferenceEqualityComparer : IEqualityComparer<object>
    {
        public new bool Equals(object x, object y) => ReferenceEquals(x, y);

        public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
}
