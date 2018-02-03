using System;
using System.Threading;

namespace AnalogDevices
{
    internal interface IObservableLock : IDisposable
    {
        void Enter();
        void Exit();
    }

    internal sealed class ObservableLock : IObservableLock
    {
        private readonly Action _onDisposeAction;
        private readonly Action _onEnterAction;
        private readonly Action _onExitAction;
        private readonly object _toLock;

        public ObservableLock(
            object toLock = null,
            Action onEnterAction = null,
            Action onExitAction = null,
            Action onDisposeAction = null)
        {
            _toLock = toLock ?? new object();
            _onEnterAction = onEnterAction;
            _onExitAction = onExitAction;
            _onDisposeAction = onDisposeAction;
            Enter();
        }

        public void Enter()
        {
            Monitor.Enter(_toLock);
            _onEnterAction?.Invoke();
        }

        public void Exit()
        {
            Monitor.Exit(_toLock);
            _onExitAction?.Invoke();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Exit();
                _onDisposeAction?.Invoke();
            }
        }

        ~ObservableLock()
        {
            Dispose(false);
        }
    }
}