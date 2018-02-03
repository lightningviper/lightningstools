using System;
using System.Collections.Concurrent;
using Common.SimSupport;
using MFDExtractor.UI;

namespace MFDExtractor
{
	internal interface IInstrumentStateSnapshotCache
	{
		bool CaptureInstrumentStateSnapshotAndCheckIfStale(IInstrumentRenderer renderer, InstrumentForm instrumentForm);
	}

	class InstrumentStateSnapshotCache : IInstrumentStateSnapshotCache
	{
		private readonly ConcurrentDictionary<IInstrumentRenderer, InstrumentStateSnapshot> _instrumentStates = new ConcurrentDictionary<IInstrumentRenderer, InstrumentStateSnapshot>();
        private const int StaleDataTimeoutMilliseconds = 1000;
		public bool CaptureInstrumentStateSnapshotAndCheckIfStale(IInstrumentRenderer renderer, InstrumentForm instrumentForm)
		{
		    if (renderer == null) return false;
			var storedState = _instrumentStates.ContainsKey(renderer) ? _instrumentStates[renderer] : InstrumentStateSnapshot.Default;
			var latestState = CaptureStateSnapshot(renderer);
			var newStateIsDifferent = (storedState.HashCode != latestState.HashCode);
            if (newStateIsDifferent)
            {
                _instrumentStates.AddOrUpdate(renderer, latestState, (x, y) => latestState);
            }
            else
            {
                latestState = storedState;
            }
            var timeSinceLastRendered = instrumentForm !=null ?
                DateTime.UtcNow.Subtract(instrumentForm.LastRenderedOn).TotalMilliseconds
                :0;
            var isStale = instrumentForm != null && (newStateIsDifferent || instrumentForm.LastRenderedOn < latestState.DateTime || timeSinceLastRendered > StaleDataTimeoutMilliseconds); 
            return isStale;
		}

		private static InstrumentStateSnapshot CaptureStateSnapshot(IInstrumentRenderer renderer)
		{
			var newState = renderer.GetState();
			return new InstrumentStateSnapshot
			{
				HashCode = newState?.GetHashCode() ?? 0,
				DateTime = DateTime.UtcNow
			};
		}

		private class InstrumentStateSnapshot
		{
			public DateTime DateTime { get; set; }
			public int HashCode { get; set; }
			public static InstrumentStateSnapshot Default => new InstrumentStateSnapshot { DateTime = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)), HashCode = 0 };
		}
	}
}
