using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Common.Drawing;
using Common.Drawing.Imaging;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using Common.Imaging;
using Common.Networking;
using F4SharedMem;

namespace MFDExtractor.Networking
{
    [SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
    public class ExtractorServer : MarshalByRefObject
    {
        private static readonly object FlightDataLock = new object();
        private static FlightData _flightData;
        private static readonly ConcurrentDictionary<InstrumentType, Image> LatestTexSharedmemImages = new ConcurrentDictionary<InstrumentType, Image>(); 
        private static long _flightDataSequenceNum;
        private static FlightData _lastRetrievedFlightData;
        private static string _compressionType = "LZW";
        private static string _imageFormat = "TIFF";
        private static readonly List<Message> MessagesToServerFromClient = new List<Message>();
        private static readonly List<Message> MessagesToClientFromServer = new List<Message>();
        private static bool _serviceEstablished;

        private ExtractorServer()
        {
        }

        public FlightData GetFlightData()
        {
            if (!Extractor.State.SimRunning || Extractor.State.NetworkMode != NetworkMode.Server)
            {
                return null;
            }
            if (_flightData == null)
            {
                return null;
            }
            FlightData toReturn;
            lock (FlightDataLock)
            {
                toReturn = _flightData;
                Interlocked.Exchange(ref _lastRetrievedFlightData, toReturn);
            }
            return toReturn;
        }



        public byte[] GetInstrumentImageBytes(InstrumentType instrumentType)
        {
            if (!Extractor.State.SimRunning || Extractor.State.NetworkMode != NetworkMode.Server)
            {
                return null;
            }
            Image image;
            LatestTexSharedmemImages.TryGetValue(instrumentType, out image);
            Util.ConvertPixelFormat(ref image, PixelFormat.Format16bppRgb565);
            var toReturn = Util.BytesFromBitmap(image, _compressionType, _imageFormat);
            return toReturn;
        }

        public void SubmitMessageToServerFromClient(Message message)
        {
            if (MessagesToServerFromClient == null) return;
            if (MessagesToServerFromClient.Count >= 1000)
            {
                MessagesToServerFromClient.RemoveRange(999, MessagesToServerFromClient.Count - 1000);
            }
            MessagesToServerFromClient.Add(message);
        }

        public void ClearPendingMessagesToClientFromServer()
        {
            if (MessagesToClientFromServer != null)
            {
                MessagesToClientFromServer.Clear();
            }
        }

        public Message GetNextPendingMessageToClientFromServer()
        {
            if (MessagesToClientFromServer == null || MessagesToClientFromServer.Count <= 0) return null;
            var toReturn = MessagesToClientFromServer[0];
            MessagesToClientFromServer.RemoveAt(0);
            return toReturn;
        }

        public bool TestConnection()
        {
            if (Extractor.State.NetworkMode != NetworkMode.Server) return false;
            return _serviceEstablished;
        }

        internal static void CreateService(string serviceName, int port, string compressionType, string imageFormat)
        {
            _compressionType = compressionType;
            _imageFormat = imageFormat;

            IDictionary prop = new Hashtable();
            prop["port"] = port;
            prop["priority"] = 100;
            try
            {
                RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
            }
            catch {}
            TcpServerChannel channel = null;
            try
            {
                channel = new TcpServerChannel(prop, null, null);
            }
            catch {}
            try
            {
                if (channel != null)
                {
                    ChannelServices.RegisterChannel(channel, false);
                }
            }
            catch {}
            try
            {
                // Register as an available service with the name HelloWorld     
                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof (ExtractorServer), serviceName,
                    WellKnownObjectMode.Singleton);
            }
            catch {}
            if (MessagesToServerFromClient != null)
            {
                MessagesToServerFromClient.Clear();
            }
            _serviceEstablished = true;
        }

        [DebuggerHidden]
        internal static void TearDownService(int port)
        {
            IDictionary prop = new Hashtable();
            prop["port"] = port;
            TcpServerChannel channel = null;
            try
            {
                channel = new TcpServerChannel(prop, null, null);
            }
            catch {}

            try
            {
                ChannelServices.UnregisterChannel(channel);
            }
            catch {}
        }

        internal static void SetFlightData(FlightData flightData)
        {
            lock (FlightDataLock)
            {
                Interlocked.Exchange(ref _flightData, flightData);
            }
            Interlocked.Increment(ref _flightDataSequenceNum);
        }

        

        internal static void SetInstrumentImage(Image bitmap, InstrumentType instrumentType)
        {
            if (Extractor.State.NetworkMode != NetworkMode.Server) return;
            var cloned = Util.CloneBitmap(bitmap);
            LatestTexSharedmemImages.AddOrUpdate(instrumentType, x => cloned, (x, y) => cloned);
        }

        public static void ClearPendingMessagesToServerFromClientOfType(MessageType messageType)
        {
            var messagesToRemove = MessagesToServerFromClient.Where(message => message.MessageType == messageType).ToList();
	        foreach (var message in messagesToRemove)
            {
                MessagesToServerFromClient.Remove(message);
            }
        }

        public static void ClearPendingMessagesToServerFromClient()
        {
            if (MessagesToServerFromClient != null)
            {
                MessagesToServerFromClient.Clear();
            }
        }

        public static void SubmitMessageToClientFromServer(Message message)
        {
            if (Extractor.State.NetworkMode != NetworkMode.Server) return;
            if (MessagesToClientFromServer == null) return;
            if (MessagesToClientFromServer.Count >= 1000)
            {
                MessagesToClientFromServer.RemoveRange(999, MessagesToClientFromServer.Count - 1000);
                //limit the message queue size to 1000 messages
            }
            MessagesToClientFromServer.Add(message);
        }

        public static Message GetNextPendingMessageToServerFromClient()
        {
            if (Extractor.State.NetworkMode != NetworkMode.Server) return null;
            if (MessagesToServerFromClient == null || MessagesToServerFromClient.Count <= 0) return null;
            var toReturn = MessagesToServerFromClient[0];
            MessagesToServerFromClient.RemoveAt(0);
            return toReturn;
        }
    }
}