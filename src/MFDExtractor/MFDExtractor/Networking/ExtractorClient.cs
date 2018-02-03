using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Windows.Forms;
using Common.Imaging;
using F4SharedMem;

namespace MFDExtractor.Networking
{
    public static class ExtractorClient
    {
        private static BackgroundWorker _connectionTestingBackgroundWorker;
        private static DateTime _lastConnectionCheckTime = DateTime.UtcNow.Subtract(new TimeSpan(0, 5, 0));
        private static ExtractorServer _server;
        private static bool _wasConnected;

        [DebuggerHidden]
        public static IPEndPoint ServerEndpoint { get; internal set; }
        public static string ServiceName { get; internal set; }
        public static bool IsConnected
        {
            get
            {
                var toReturn = false;
                var secondsSinceLastCheck = (int) DateTime.UtcNow.Subtract(_lastConnectionCheckTime).TotalSeconds;
                if (secondsSinceLastCheck > 0 && secondsSinceLastCheck < 5)
                {
                    return _wasConnected;
                }
                if (_server == null) return toReturn;
                try
                {
                    _lastConnectionCheckTime = DateTime.UtcNow;
                    Application.DoEvents();
                    if (_connectionTestingBackgroundWorker == null)
                    {
                        _connectionTestingBackgroundWorker = new BackgroundWorker();
                        _connectionTestingBackgroundWorker.DoWork += ConnectionTestingBackgroundWorker_DoWork;
                    }
                    if (_connectionTestingBackgroundWorker != null && !_connectionTestingBackgroundWorker.IsBusy)
                    {
                        _connectionTestingBackgroundWorker.RunWorkerAsync();
                    }
                    toReturn = _wasConnected;
                }
                catch { }
                return toReturn;
            }
        }

        public static Image GetInstrumentImage(InstrumentType instrumentType)
        {
            EnsureConnected();
            if (_server == null || !IsConnected ) return null;
            try
            {
                var raw = _server.GetInstrumentImageBytes(instrumentType);
                return Util.BitmapFromBytes(raw);
            }
            catch { }
            return null;
        }

        public static FlightData GetFlightData()
        {
            EnsureConnected();
            if (_server == null || !IsConnected) return null;
            try
            {
                return _server.GetFlightData();
            }
            catch {}
            return null;

        }

        public static void SendMessageToServer(Message message)
        {
            EnsureConnected();
            if (!IsConnected) return;
            try
            {
                if (_server != null)
                {
                    _server.SubmitMessageToServerFromClient(message);
                }
            }
            catch { }
        }

        public static void ClearPendingMessagesToClientFromServer()
        {
            EnsureConnected();
            if (!IsConnected) return;
            try
            {
                if (_server != null)
                {
                    _server.ClearPendingMessagesToClientFromServer();
                }
            }
            catch { }
        }

        public static Message GetNextMessageToClientFromServer()
        {
            EnsureConnected();
            Message toReturn = null;
            if (!IsConnected) return toReturn;
            try
            {
                if (_server != null)
                {
                    toReturn = _server.GetNextPendingMessageToClientFromServer();
                }
            }
            catch { }
            return toReturn;
        }

        [DebuggerHidden]
        private static void EnsureConnected()
        {
            if (ServerEndpoint == null || ServiceName == null) return;
            if (IsConnected) return;
            IDictionary prop = new Hashtable();
            prop["port"] = ServerEndpoint.Port;
            prop["machineName"] = ServerEndpoint.Address.ToString();
            prop["priority"] = 100;
            prop["timeout"] = (uint) 1;
            prop["retryCount"] = 0;
            prop["useIpAddress"] = 1;
            TcpClientChannel chan = null;
            try
            {
                chan = new TcpClientChannel();
            }
            catch { }
            try
            {
                if (chan != null)
                {
                    ChannelServices.RegisterChannel(chan, false);
                }
            }
            catch { }
            try
            {
                RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
            }
            catch { }
            try
            {
                // Create an instance of the remote object
                _server = (ExtractorServer) Activator.GetObject(
                    typeof (ExtractorServer),
                    "tcp://"
                    + ServerEndpoint.Address
                    + ":"
                    + ServerEndpoint.Port.ToString(CultureInfo.InvariantCulture)
                    + "/"
                    + ServiceName);
            }
            catch { }
        }

        [DebuggerHidden]
        private static void ConnectionTestingBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_server == null) return;
            try
            {
                _wasConnected = _server.TestConnection();
            }
            catch { }
        }
    }
}