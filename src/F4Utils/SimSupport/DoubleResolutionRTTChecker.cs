using System;
using System.Linq;
using F4Utils.Process;
using Util = Common.Strings.Util;

namespace F4Utils.SimSupport
{
    public interface IDoubleResolutionRTTChecker
    {
        bool IsDoubleResolutionRtt { get; }
    }

    public class DoubleResolutionRTTChecker : IDoubleResolutionRTTChecker
    {
        private readonly IBMSConfigFileReader _bmsConfigFileReader;

        public DoubleResolutionRTTChecker(IBMSConfigFileReader bmsConfigFileReader = null) { _bmsConfigFileReader = bmsConfigFileReader ?? new BMSConfigFileReader(); }


        public bool IsDoubleResolutionRtt
        {
            get
            {
                return _bmsConfigFileReader.ConfigLines.Select(Util.Tokenize)
                    .Where(tokens => tokens.Count > 2)
                    .Any(
                        tokens => tokens[0].ToLowerInvariant() == "set"
                                  && string.Equals(tokens[1], "g_bDoubleRTTResolution", StringComparison.InvariantCultureIgnoreCase)
                                  && string.Equals(tokens[2], "1", StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}