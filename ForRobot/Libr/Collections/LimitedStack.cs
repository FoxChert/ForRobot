using System;
using System.Collections;
using System.Collections.Generic;

namespace ForRobot.Libr.Collections
{
    public class LimitedStack<T> : IEnumerable<T>
    {
        private readonly int _maxSize;
        private readonly LinkedList<T> _items = new LinkedList<T>();
        /// <summary>
        /// Блокируется ли добавление новых записей в стек по достижению лимита
        /// </summary>
        private bool _blockAdding;

        public LimitedStack(int maxSize, bool blockAdding = false)
        {
            if (maxSize <= 0)
                throw new ArgumentException("Максимальный размер должен натуральным числом", nameof(maxSize));

            this._maxSize = maxSize;
            this._blockAdding = blockAdding;
        }

        public void Push(T item)
        {
            if (this._items.Count == _maxSize)
            {
                if (this._blockAdding)
                    return;
                else
                    this._items.RemoveLast(); // Удаление самого старого элемента.
            }
            this._items.AddFirst(item);
        }

        public T Pop()
        {
            if (this._items.First == null)
                throw new InvalidOperationException("Stack is empty");

            var value = this._items.First.Value;
            _items.RemoveFirst();
            return value;
        }

        public T Peek()
        {
            if (this._items.First == null)
                throw new InvalidOperationException("Stack is empty");

            return _items.First.Value;
        }

        public int Count => this._items.Count;
        public void Clear() => this._items.Clear();
        public bool IsEmpty => this._items.Count == 0;

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class LinkedListExtensions
    {
        public static T RemoveFirstAndReturn<T>(this LinkedList<T> list)
        {
            if (list.First == null)
                throw new InvalidOperationException("List is empty");

            var value = list.First.Value;
            list.RemoveFirst();
            return value;
        }
    }
}
