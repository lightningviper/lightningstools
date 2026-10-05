using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common.Collections
{
    public class ConcurrentRollingBuffer<T> : IReadOnlyCollection<T>
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private readonly int _maxCapacity;

        public ConcurrentRollingBuffer(int capacity)
        {
            _maxCapacity = capacity;
        }

        // Thread-safe: returns a snapshot of the current number of elements
        public int Count => _queue.Count;

        public void Add(T item)
        {
            _queue.Enqueue(item);

            // Safely evict older items if the queue overflows
            while (_queue.Count > _maxCapacity)
            {
                _queue.TryDequeue(out _);
            }
        }

        // Thread-safe: returns an enumerator that represents a moment-in-time snapshot
        public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
