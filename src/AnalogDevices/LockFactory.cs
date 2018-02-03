using System.Collections.Concurrent;

namespace AnalogDevices
{
    internal interface ILockFactory
    {
        IObservableLock GetLock(LockType lockType);
    }

    internal class LockFactory : ILockFactory
    {
        internal static ConcurrentDictionary<LockType, object> Locks = new ConcurrentDictionary<LockType, object>();

        public IObservableLock GetLock(LockType lockType)
        {
            return new ObservableLock(Locks.GetOrAdd(lockType, new object()));
        }
    }
}