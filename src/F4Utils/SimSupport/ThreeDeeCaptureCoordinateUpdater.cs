using System;
using System.IO;
using Common.Drawing;
using Common.UI.Layout;
using log4net;

namespace F4Utils.SimSupport
{
    public interface IThreeDeeCaptureCoordinateUpdater
    {
        void Update3DCoordinatesFromCurrentBmsDatFile(int vehicleACD);
    }

    public class ThreeDeeCaptureCoordinateUpdater : IThreeDeeCaptureCoordinateUpdater
    {
        private readonly ILog _log;
        private readonly TexturesSharedMemoryImageCoordinates _coordinates;
        private readonly IThreeDeeCockpitFileFinder2 _threeDeeCockpitFileFinder;
        private readonly IDoubleResolutionRTTChecker _doubleResolutionRTTChecker;

        public ThreeDeeCaptureCoordinateUpdater(
            TexturesSharedMemoryImageCoordinates coordinates, IThreeDeeCockpitFileFinder2 threeDeeCockpitFileFinder = null, IDoubleResolutionRTTChecker doubleResolutionRTTChecker = null,
            ILog log = null)
        {
            _coordinates = coordinates;
            _threeDeeCockpitFileFinder = threeDeeCockpitFileFinder ?? new ThreeDeeCockpitFileFinder();
            _doubleResolutionRTTChecker = doubleResolutionRTTChecker ?? new DoubleResolutionRTTChecker();

            _log = log ?? LogManager.GetLogger(GetType());
        }

        public void Update3DCoordinatesFromCurrentBmsDatFile(int vehicleACD)
        {
            var threeDeeCockpitFile = _threeDeeCockpitFileFinder.FindThreeDeeCockpitFile(vehicleACD);
            if (threeDeeCockpitFile == null) { return; }

            var isDoubleResolution = _doubleResolutionRTTChecker.IsDoubleResolutionRtt;

            using (var filestream = threeDeeCockpitFile.OpenRead())
            using (var reader = new StreamReader(filestream))
            {
                while (!reader.EndOfStream)
                {
                    var currentLine = reader.ReadLine() ?? string.Empty;
                    if (currentLine.StartsWith("hud\t", StringComparison.OrdinalIgnoreCase)) { _coordinates.HUD = ReadCaptureCoordinates(currentLine); }
                    else if (currentLine.StartsWith("mfd4\t", StringComparison.OrdinalIgnoreCase)) { _coordinates.MFD4 = ReadCaptureCoordinates(currentLine); }
                    else if (currentLine.StartsWith("mfd3\t", StringComparison.OrdinalIgnoreCase)) { _coordinates.MFD3 = ReadCaptureCoordinates(currentLine); }
                    else if (currentLine.StartsWith("mfdleft\t", StringComparison.OrdinalIgnoreCase)) { _coordinates.LMFD = ReadCaptureCoordinates(currentLine); }
                    else if (currentLine.StartsWith("mfdright\t", StringComparison.OrdinalIgnoreCase)) { _coordinates.RMFD = ReadCaptureCoordinates(currentLine); }
                    else if (currentLine.StartsWith("ded\t", StringComparison.OrdinalIgnoreCase)) { _coordinates.DED = ReadCaptureCoordinates(currentLine); }
                    else if (currentLine.StartsWith("pfl\t", StringComparison.OrdinalIgnoreCase)) { _coordinates.PFL = ReadCaptureCoordinates(currentLine); }
                    else if (currentLine.StartsWith("rwr\t", StringComparison.OrdinalIgnoreCase)) { _coordinates.RWR = ReadCaptureCoordinates(currentLine); }
                    else if (currentLine.StartsWith("hms\t", StringComparison.OrdinalIgnoreCase)) { _coordinates.HMS = ReadCaptureCoordinates(currentLine); }
                }
            }

            if (isDoubleResolution)
            {
                _coordinates.LMFD = Util.MultiplyRectangle(_coordinates.LMFD, 2);
                _coordinates.RMFD = Util.MultiplyRectangle(_coordinates.RMFD, 2);
                _coordinates.MFD3 = Util.MultiplyRectangle(_coordinates.MFD3, 2);
                _coordinates.MFD4 = Util.MultiplyRectangle(_coordinates.MFD4, 2);
                _coordinates.HUD = Util.MultiplyRectangle(_coordinates.HUD, 2);
                _coordinates.PFL = Util.MultiplyRectangle(_coordinates.PFL, 2);
                _coordinates.DED = Util.MultiplyRectangle(_coordinates.DED, 2);
                _coordinates.RWR = Util.MultiplyRectangle(_coordinates.RWR, 2);
                _coordinates.HMS = Util.MultiplyRectangle(_coordinates.HMS, 2);
            }
        }

        private Rectangle ReadCaptureCoordinates(string configLine)
        {
            var tokens = Common.Strings.Util.Tokenize(configLine);
            if (tokens.Count <= 12) return Rectangle.Empty;

            try
            {
                var rectangle = new Rectangle {X = Convert.ToInt32(tokens[10]), Y = Convert.ToInt32(tokens[11])};
                rectangle.Width = Math.Abs(Convert.ToInt32(tokens[12]) - rectangle.X);
                rectangle.Height = Math.Abs(Convert.ToInt32(tokens[13]) - rectangle.Y);
                return rectangle;
            }
            catch (Exception e) { _log.Error(e.Message, e); }

            return Rectangle.Empty;
        }
    }
}