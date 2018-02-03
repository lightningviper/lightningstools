using F4SharedMem;

namespace MFDExtractor
{
    internal interface ISharedmemReaderFactory
    {
        Reader Create();
    }

    class SharedmemReaderFactory : ISharedmemReaderFactory
    {
        private Reader _cachedReader;
        public SharedmemReaderFactory(Reader falconSmReader =null)
        {
            _cachedReader = falconSmReader  ?? new Reader();
        }

        public Reader Create()
        {
            if (_cachedReader != null)
            {
                DisposeReader();
            }
            if (F4Utils.Process.Util.IsFalconRunning())
            {
                _cachedReader = new Reader();
            }
            return _cachedReader;
        }

        private void DisposeReader()
        {
            Common.Util.DisposeObject(_cachedReader);
            _cachedReader = null;
        }

    }
}
