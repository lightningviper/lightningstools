using System.Diagnostics;
using Common.Networking;

namespace MFDExtractor
{
	public class ExtractorState
	{
		public bool Running { get; set; }
        public bool KeepRunning { get; set; }
		public bool OptionsFormIsShowing { get; set; }
		public bool NightMode { get; set; }
        public NetworkMode NetworkMode { get; set; }
        public bool SimRunning { get { return NetworkMode == NetworkMode.Client || F4Utils.Process.Util.IsFalconRunning(); } }
        public PerformanceCounter RenderedFramesCounter { get; set; }
        public PerformanceCounter SkippedFramesCounter { get; set; }
        public PerformanceCounter TimeoutFramesCounter { get; set; }
        public PerformanceCounter TotalFramesCounter { get; set; }
    }
}
