using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Windows.Forms;
using log4net;

namespace F16CPD.Networking
{
    public class F16CPDClient : IF16CPDClient
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (F16CPDClient));
        private readonly IPEndPoint _serverEndpoint;
        private readonly string _serviceName;
        private BackgroundWorker _connectionTestingBackgroundWorker;
        private DateTime _lastConnectionCheckTime = DateTime.MinValue;
        private IF16CPDServer _server;
        private bool _wasConnected;

        public F16CPDClient(IPEndPoint serverEndpoint, string serviceName)
        {
            _serverEndpoint = serverEndpoint;
            _serviceName = serviceName;
            EnsureConnected();
        }

        #region IF16CPDClient Members

        [DebuggerHidden]
        public bool IsConnected
        {
            get
            {
                var toReturn = false;
                var secondsSinceLastCheck = (int) DateTime.UtcNow.Subtract(_lastConnectionCheckTime).TotalSeconds;
                if (secondsSinceLastCheck > 0 && secondsSinceLastCheck < 5)
                {
                    return _wasConnected;
                }
                if (_server != null)
                {
                    try
                    {
                        _lastConnectionCheckTime = DateTime.UtcNow;
                        Application.DoEvents();
                        if (_connectionTestingBackgroundWorker == null)
                        {
                            _connectionTestingBackgroundWorker = new BackgroundWorker();
                            _connectionTestingBackgroundWorker.DoWork += ConnectionTestingBackgroundWorkerDoWork;
                        }
                        if (!_connectionTestingBackgroundWorker.IsBusy)
                        {
                            _connectionTestingBackgroundWorker.RunWorkerAsync();
                        }
                        toReturn = _wasConnected;
                    }
                    catch (Exception e)
                    {
                        _log.Debug(e.Message, e);
                    }
                }
                return toReturn;
            }
        }


        public object GetSimProperty(string propertyName)
        {
            EnsureConnected();
            object toReturn = null;
            if (IsConnected)
            {
                try
                {
                    toReturn = _server.GetSimProperty(propertyName);
                }
                catch (Exception e)
                {
                    _log.Debug(e.Message, e);
                }
            }
            return toReturn;
        }

        public void SendMessageToServer(Message message)
        {
            EnsureConnected();
            if (IsConnected)
            {
                try
                {
                    _server.SubmitMessageToServer(message);
                }
                catch (Exception e)
                {
                    _log.Debug(e.Message, e);
                }
            }
        }

        public void ClearPendingClientMessages()
        {
            EnsureConnected();
            if (IsConnected)
            {
                try
                {
                    _server.ClearPendingClientMessages();
                }
                catch (Exception e)
                {
                    _log.Debug(e.Message, e);
                }
            }
        }

        public Message GetNextPendingClientMessage()
        {
            EnsureConnected();
            Message toReturn = null;
            if (IsConnected)
            {
                try
                {
                    toReturn = _server.GetNextPendingClientMessage();
                }
                catch (Exception e)
                {
                    _log.Debug(e.Message, e);
                }
            }
            return toReturn;
        }

        #endregion

        [DebuggerHidden]
        private void EnsureConnected()
        {
            if (_serverEndpoint == null || _serviceName == null) return;
            if (!IsConnected)
            {
                IDictionary prop = new Hashtable();
                prop["port"] = _serverEndpoint.Port;
                prop["machineName"] = _serverEndpoint.Address.ToString();
                prop["priority"] = 100;
                prop["timeout"] = (uint) 1;
                prop["retryCount"] = 0;
                prop["useIpAddress"] = 1;
                TcpClientChannel chan = null;
                try
                {
                    chan = new TcpClientChannel();
                }
                catch (Exception e)
                {
                    _log.Debug(e.Message, e);
                }
                try
                {
                    if (chan != null)
                    {
                        ChannelServices.RegisterChannel(chan, false);
                    }
                }
                catch (Exception e)
                {
                    _log.Debug(e.Message, e);
                }
                try
                {
                    RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
                }
                catch (Exception e)
                {
                    _log.Debug(e.Message, e);
                }
                try
                {
                    // Create an instance of the remote object
                    _server = (F16CPDServer) Activator.GetObject(
                        typeof (F16CPDServer),
                        "tcp://"
                        + _serverEndpoint.Address
                        + ":"
                        + _serverEndpoint.Port.ToString(CultureInfo.InvariantCulture)
                        + "/"
                        + _serviceName);
                }
                catch (Exception e)
                {
                    _log.Debug(e.Message, e);
                }
            }
        }

        [DebuggerHidden]
        private void ConnectionTestingBackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            if (_server != null)
            {
                try
                {
                    _wasConnected = _server.TestConnection();
                }
                catch (Exception ex)
                {
                    _log.Debug(ex.Message, ex);
                }
            }
        }
    }
}