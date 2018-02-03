using System;
using System.IO;
using System.Text;
using System.Threading;
using Common.Win32;
using F4KeyFile;
using log4net;

namespace F4Utils.Process
{
    public static class KeyFileUtils
    {
        private const string KEYSTROKE_FILE_NAME__DEFAULT = "BMS - Full.key";
        private const string KEYSTROKE_FILE_NAME__FALLBACK = "BMS.key";
        private const string KEYSTROKE_FILE_NAME__FALLBACK2 = "keystrokes.key";
        private const string PLAYER_OPTS_FILENAME__DEFAULT = "viper.pop";
        private const string PLAYER_OPTS_FILE_EXTENSION = ".pop";
        private const string KEYFILE_EXENSION_DEFAULT = ".key";
        private const string CONFIG_DIRECTORY_NAME = "config";
        private const string USEROPTS_DIRECTORY_NAME = "User";
        private static readonly ILog Log = LogManager.GetLogger(typeof (KeyFileUtils));
        private static KeyFile _keyFile;
        private static readonly object KeySenderLock = new object();

        public static void ResetCurrentKeyFile()
        {
            _keyFile = null;
        }

        public static KeyBinding FindKeyBinding(string callback)
        {
            if (_keyFile == null) _keyFile = GetCurrentKeyFile();
            if (_keyFile == null) return null;
           return _keyFile.GetBindingForCallback(callback) as KeyBinding;
        }

        public static void SendCallbackToFalcon(string callback)
        {
            var keyBinding = FindKeyBinding(callback);
            if (keyBinding == null) return;
            Util.ActivateFalconWindow();
            lock (KeySenderLock)
            {
                keyBinding.SendCallback();
            }
        }

        public static KeyFile GetCurrentKeyFile()
        {
            KeyFile toReturn = null;
            var exeFilePath = Util.GetFalconExePath();

            if (exeFilePath == null) return null;
            var callsign = CallsignUtils.DetectCurrentCallsign();


            var configFolder = Path.GetDirectoryName(exeFilePath) + Path.DirectorySeparatorChar
                               + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + 
                               USEROPTS_DIRECTORY_NAME + Path.DirectorySeparatorChar + CONFIG_DIRECTORY_NAME;

            var pilotOptionsPath = configFolder + Path.DirectorySeparatorChar + callsign +
                                   PLAYER_OPTS_FILE_EXTENSION;
            if (!new FileInfo(pilotOptionsPath).Exists)
            {
                pilotOptionsPath = configFolder + Path.DirectorySeparatorChar + PLAYER_OPTS_FILENAME__DEFAULT;
            }
                

            string keyFileName = null;
            if (new FileInfo(pilotOptionsPath).Exists)
            {
                keyFileName = GetKeyFileNameFromPlayerOpts(pilotOptionsPath);
            }
            if (keyFileName == null) keyFileName = KEYSTROKE_FILE_NAME__DEFAULT;

            var falconKeyFilePath = configFolder + Path.DirectorySeparatorChar + keyFileName;
            var keyFileInfo = new FileInfo(falconKeyFilePath);

            if (!keyFileInfo.Exists)
            {
                keyFileName = KEYSTROKE_FILE_NAME__FALLBACK;
                falconKeyFilePath = configFolder + Path.DirectorySeparatorChar + keyFileName;
                keyFileInfo = new FileInfo(falconKeyFilePath);
            }

            if (!keyFileInfo.Exists)
            {
                keyFileName = KEYSTROKE_FILE_NAME__FALLBACK2;
                falconKeyFilePath = configFolder + Path.DirectorySeparatorChar + keyFileName;
                keyFileInfo = new FileInfo(falconKeyFilePath);
            }

            if (!keyFileInfo.Exists) return null;
            try
            {
                toReturn = KeyFile.Load(falconKeyFilePath);
            }
            catch (IOException e)
            {
                Log.Error(e.Message, e);
            }
            return toReturn;
        }
        private static string GetKeyFileNameFromPlayerOpts(string playerOptionsFilePath)
        {
            PlayerOp.PlayerOp playerOptionsFile = null;
            try
            {
                using (var fs = new FileStream(playerOptionsFilePath, FileMode.Open, FileAccess.Read))
                {
                      playerOptionsFile= new PlayerOp.PlayerOp(fs);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
            if (playerOptionsFile == null || playerOptionsFile.keyfile == null || playerOptionsFile.keyfile.Length <= 0)
                return null;
            var keyFileName = Encoding.ASCII.GetString(playerOptionsFile.keyfile, 0, playerOptionsFile.keyfile.Length);
            var firstNull = keyFileName.IndexOf('\0');
            if (firstNull <= 0) return null;
            keyFileName = keyFileName.Substring(0, firstNull);
            keyFileName += KEYFILE_EXENSION_DEFAULT;
            return keyFileName;
        }
       

    }
}