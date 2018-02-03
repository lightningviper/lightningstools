using System;
using Common.Drawing;

namespace MFDExtractor
{
    public interface IInstrumentFormSettings
    {
        bool Enabled { get; set; }
        string OutputDisplay { get; set; }
        bool StretchToFit { get; set; }
        int ULX { get; set; }
        int ULY { get; set; }
        int LRX { get; set; }
        int LRY { get; set; }
        bool AlwaysOnTop { get; set; }
        bool Monochrome { get; set; }
        RotateFlipType RotateFlipType { get; set; }
    }

    [Serializable]
	class InstrumentFormSettings : IInstrumentFormSettings
    {
		public bool Enabled { get; set; }
		public string OutputDisplay { get; set; }
		public bool StretchToFit { get; set; }
		public int ULX { get; set; }
		public int ULY { get; set; }
		public int LRX { get; set; }
		public int LRY { get; set; }
		public bool AlwaysOnTop { get; set; }
		public bool Monochrome { get; set; }
		public RotateFlipType RotateFlipType { get; set; }
	}
}
