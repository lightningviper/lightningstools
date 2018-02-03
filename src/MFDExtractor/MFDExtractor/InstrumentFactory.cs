using System.Threading;
using System.Windows.Forms;

namespace MFDExtractor
{
    internal interface IInstrumentFactory
    {
        IInstrument Create(InstrumentType instrumentType);
    }

    class InstrumentFactory : IInstrumentFactory
    {
        private readonly IInstrumentStateSnapshotCache _instrumentStateSnapshotCache;
        private readonly IRendererFactory _rendererFactory;
        private readonly IPerformanceCounterInstanceFactory _performanceCounterInstanceFactory;

        public InstrumentFactory(
            InstrumentStateSnapshotCache instrumentStateSnapshotCache = null, 
            IRendererFactory rendererFactory=null,
            IPerformanceCounterInstanceFactory performanceCounterInstanceFactory = null)
        {
            _instrumentStateSnapshotCache = instrumentStateSnapshotCache ?? new InstrumentStateSnapshotCache();
            _rendererFactory = rendererFactory ?? new RendererFactory();
            _performanceCounterInstanceFactory = performanceCounterInstanceFactory ?? new PerformanceCounterInstanceInstanceFactory();
        }

        public IInstrument Create(InstrumentType instrumentType)
        {
            var renderer = _rendererFactory.CreateRenderer(instrumentType);
            var instrument = new Instrument(instrumentType, renderer, _instrumentStateSnapshotCache, _performanceCounterInstanceFactory);
            return instrument;
        }
    }
}
