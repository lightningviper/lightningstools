using System;
using System.IO;
using System.Text;
using log4net;
using Microsoft.Win32;

namespace F4Utils.Process
{
    public static class CallsignUtils
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (CallsignUtils));

        public static string DetectCurrentCallsign()
        {
            string callsign = null;
            var exePath = Util.GetFalconExePath();

            callsign = DetectBMSCallsign(exePath, callsign);
            return callsign;
        }

        private static string DetectBMSCallsign(string exePath, string callsign)
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Benchmark Sims");
                if (key != null)
                {
                    var subkeys = key.GetSubKeyNames();
                    if (subkeys != null && subkeys.Length > 0)
                    {
                        var callsignFound = false;
                        foreach (var subkey in subkeys)
                        {
                            var toRead = key.OpenSubKey(subkey, false);
                            if (toRead != null)
                            {
                                var baseDir = (string) toRead.GetValue("baseDir", null);
                                var exePathFI = new FileInfo(exePath);
                                var exeDir = exePathFI.Directory.FullName;

                                if (baseDir != null && exeDir != null && (string.Compare(baseDir, exeDir, true) == 0) ||
                                    exeDir.StartsWith(baseDir))
                                {
                                    callsign = Encoding.ASCII.GetString((byte[]) toRead.GetValue("PilotCallsign"));
                                    var firstNull = callsign.IndexOf('\0');
                                    callsign = callsign.Substring(0, firstNull);
                                    callsignFound = true;
                                    break;
                                }
                            }
                        }
                        if (!callsignFound)
                        {
                            callsign = "Viper";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                callsign = "Viper";
            }
            return callsign;
        }
    }
}