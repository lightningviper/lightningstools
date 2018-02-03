using F16CPD.Networking;
using F16CPD.Properties;
using F16CPD.UI.Forms;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace F16CPD.SimSupport.Falcon4
{
    internal interface IFalconCallbackSender
    {
        void SendCallbackToFalcon(string callback);
        void SendCallbackToFalconLocal(string callback);
    }
    class FalconCallbackSender:IFalconCallbackSender
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(FalconCallbackSender));
        private F16CpdMfdManager _mfdManager;
        public FalconCallbackSender(F16CpdMfdManager mfdManager)
        {
            _mfdManager = mfdManager;
        }
        public void SendCallbackToFalcon(string callback)
        {
            if (!Settings.Default.RunAsClient)
            {
                var mainForm = Application.OpenForms.OfType<F16CpdEngine>().FirstOrDefault();
                bool cpdWasInForeground = mainForm !=null && mainForm.ContainsFocus;
                var mousePosition = Cursor.Position;
                SendCallbackToFalconLocal(callback);
                if (cpdWasInForeground)
                {
                    mainForm.Activate();
                    mainForm.Capture = true;
                    Cursor.Position = mousePosition;
                }
            }
            else if (Settings.Default.RunAsClient)
            {
                var message = new F16CPD.Networking.Message("Falcon4SendCallbackMessage", callback);
                _mfdManager.Client.SendMessageToServer(message);
            }
        }

        private static object SyncLock = new Object();
        public void SendCallbackToFalconLocal(string callback)
        {
            lock (SyncLock)
            {
                IsSendingInput = true;
                F4Utils.Process.KeyFileUtils.SendCallbackToFalcon(callback);
                IsSendingInput = false;
            }
        }
        public static bool IsSendingInput { get; private set; }

    }
}
