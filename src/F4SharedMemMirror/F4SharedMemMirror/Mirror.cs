using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows.Forms;
using F4SharedMem;
using F4SharedMemMirror.Properties;

namespace F4SharedMemMirror
{
    public enum NetworkingMode
    {
        Client,
        Server
    }

    public class Mirror : IDisposable
    {
        private readonly Writer _writer = new Writer();
        private volatile bool _disposed;
        private volatile bool _running;
        private Reader _smReader;

        public NetworkingMode NetworkingMode { get; set; }
        public ushort PortNumber { get; set; }
        public IPAddress ClientIPAddress { get; set; }

        public bool IsRunning
        {
            get { return _running; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public void StartMirroring()
        {
            switch (NetworkingMode)
            {
                case NetworkingMode.Client:
                    RunClient();
                    break;
                case NetworkingMode.Server:
                    RunServer();
                    break;
                default:
                    break;
            }
        }

        public void StopMirroring()
        {
            _running = false;
        }

        private void RunClient()
        {
            if (_running) throw new InvalidOperationException();
            _running = true;
            SharedMemoryMirrorClient client;
            try
            {
                var serverIPAddress = Settings.Default.ServerIPAddress;
                var serverPortNum = Settings.Default.ServerPortNum;
                var portNum = 21142;
                Int32.TryParse(Settings.Default.ServerPortNum, out portNum);
                var serverAddress = IPAddress.Parse(serverIPAddress);
                var endpoint = new IPEndPoint(serverAddress, portNum);
                client = new SharedMemoryMirrorClient(endpoint, "F4SharedMemoryMirrorService");
            }
            catch (RemotingException)
            {
                client = null;
            }

            while (_running)
            {
                try
                {
                    if (client == null)
                    {
                        try
                        {
                            var serverIPAddress = Settings.Default.ServerIPAddress;
                            var serverPortNum = Settings.Default.ServerPortNum;
                            var portNum = 21142;
                            Int32.TryParse(Settings.Default.ServerPortNum, out portNum);
                            var serverAddress = IPAddress.Parse(serverIPAddress);
                            var endpoint = new IPEndPoint(serverAddress, portNum);
                            client = new SharedMemoryMirrorClient(endpoint, "F4SharedMemoryMirrorService");
                        }
                        catch (RemotingException)
                        {
                            client = null;
                        }
                    }
                    byte[] primaryFlightData = null;
                    byte[] flightData2 = null;
                    byte[] osbData = null;
                    byte[] intellivibeData = null;
                    byte[] radioClientControlData = null;
                    byte[] radioClientStatusData = null;

                    if (client != null)
                    {
                        try
                        {
                            primaryFlightData = client.GetPrimaryFlightData();
                            flightData2 = client.GetFlightData2();
                            osbData = client.GetOSBData();
                            intellivibeData = client.GetIntellivibeData();
                            radioClientControlData = client.GetRadioClientControlData();
                            radioClientStatusData = client.GetRadioClientStatusData();
                        }
                        catch (RemotingException e)
                        {
                            Debug.WriteLine(e);
                        }

                        _writer.WritePrimaryFlightData(primaryFlightData);
                        _writer.WriteFlightData2(flightData2);
                        _writer.WriteOSBData(osbData);
                        _writer.WriteIntellivibeData(intellivibeData);
                        _writer.WriteRadioClientControlData(radioClientControlData);
                        _writer.WriteRadioClientStatusData(radioClientStatusData);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                Thread.Sleep(Settings.Default.PollingFrequencyMillis);
                Application.DoEvents();
            }
        }

        private void RunServer()
        {
            if (_running) throw new InvalidOperationException();
            _running = true;
            _smReader = new Reader();

            var serverPortNum = Settings.Default.ServerPortNum;
            var portNum = 21142;
            Int32.TryParse(Settings.Default.ServerPortNum, out portNum);

            try
            {
                SharedMemoryMirrorServer.TearDownService(portNum);
            }
            catch (RemotingException)
            {
            }

            try
            {
                SharedMemoryMirrorServer.CreateService("F4SharedMemoryMirrorService", portNum);
            }
            catch (RemotingException)
            {
            }

            while (_running)
            {
                try
                {
                    var primaryFlightData = _smReader.GetRawPrimaryFlightData();
                    var flightData2 = _smReader.GetRawFlightData2();
                    var osbData = _smReader.GetRawOSBData();
                    var intellivibeData = _smReader.GetRawIntelliVibeData();
                    var radioClientControlData = _smReader.GetRawRadioClientControlData();
                    var radioClientStatusData = _smReader.GetRawRadioClientStatusData();

                    SharedMemoryMirrorServer.SetPrimaryFlightData(primaryFlightData);
                    SharedMemoryMirrorServer.SetFlightData2(flightData2);
                    SharedMemoryMirrorServer.SetOSBData(osbData);
                    SharedMemoryMirrorServer.SetIntellivibeData(intellivibeData);
                    SharedMemoryMirrorServer.SetRadioClientControlData(radioClientControlData);
                    SharedMemoryMirrorServer.SetRadioClientStatusData(radioClientStatusData);
                    Thread.Sleep(Settings.Default.PollingFrequencyMillis);
                    Application.DoEvents();
                }
                catch (RemotingException e)
                {
                    Debug.WriteLine(e);
                }
            }

            try
            {
                SharedMemoryMirrorServer.TearDownService(21142);
            }
            catch (RemotingException)
            {
            }
        }

        internal void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_smReader != null)
                    {
                        try
                        {
                            _smReader.Dispose();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if (_writer != null)
                    {
                        try
                        {
                            if (_smReader != null) _smReader.Dispose();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                _disposed = true;
            }
        }
    }
}