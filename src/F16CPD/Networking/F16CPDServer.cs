using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using log4net;

namespace F16CPD.Networking
{
    public class F16CPDServer : MarshalByRefObject, IF16CPDServer
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (F16CPDServer));
        private static F16CPDServer _server;
        private static readonly Dictionary<string, object> _simProperties = new Dictionary<string, object>();
        private static readonly List<Message> _serverMessages = new List<Message>();
        private static readonly List<Message> _clientMessages = new List<Message>();
        private static bool _serviceEstablished;

        private F16CPDServer()
        {
        }

        #region IF16CPDServer Members

        public object GetSimProperty(string propertyName)
        {
            if (_simProperties.ContainsKey(propertyName))
            {
                return _simProperties[propertyName];
            }
            return null;
        }

        public void SubmitMessageToServer(Message message)
        {
            if (_serverMessages != null)
            {
                if (_serverMessages.Count >= 1000)
                {
                    _serverMessages.RemoveRange(999, _serverMessages.Count - 1000);
                }
                if (message != null)
                {
                    if (message.MessageType == "RequestNewMapImage")
                    {
                        //only allow one of these in the queue at a time
                        ClearPendingServerMessagesOfType(message.MessageType);
                    }
                    _serverMessages.Add(message);
                }
            }
        }

        public void ClearPendingClientMessages()
        {
            if (_clientMessages != null)
            {
                _clientMessages.Clear();
            }
        }

        public Message GetNextPendingClientMessage()
        {
            Message toReturn = null;
            if (_clientMessages != null)
            {
                if (_clientMessages.Count > 0)
                {
                    toReturn = _clientMessages[0];
                    _clientMessages.RemoveAt(0);
                }
            }
            return toReturn;
        }

        public bool TestConnection()
        {
            return _serviceEstablished;
        }

        #endregion

        public static F16CPDServer GetInstance()
        {
            return _server ?? (_server = new F16CPDServer());
        }

        [DebuggerHidden]
        internal static void CreateService(string serviceName, int port)
        {
            IDictionary prop = new Hashtable();
            prop["port"] = port;
            prop["priority"] = 100;
            try
            {
                RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
            }
            catch (Exception e)
            {
                _log.Debug(e.Message, e);
            }
            TcpServerChannel channel = null;
            try
            {
                channel = new TcpServerChannel(prop, null, null);
            }
            catch (Exception e)
            {
                _log.Debug(e.Message, e);
            }
            try
            {
                if (channel != null)
                {
                    ChannelServices.RegisterChannel(channel, false);
                }
            }
            catch (Exception e)
            {
                _log.Debug(e.Message, e);
            }
            try
            {
                // Register as an available service with the name HelloWorld     
                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof (F16CPDServer), serviceName,
                    WellKnownObjectMode.Singleton);
            }
            catch (Exception e)
            {
                _log.Debug(e.Message, e);
            }
            if (_serverMessages != null)
            {
                _serverMessages.Clear();
            }
            _serviceEstablished = true;
        }

        [DebuggerHidden]
        internal static void TearDownService(int port)
        {
            ClearPendingServerMessages();
            IDictionary prop = new Hashtable();
            prop["port"] = port;
            TcpServerChannel channel = null;
            try
            {
                channel = new TcpServerChannel(prop, null, null);
            }
            catch (Exception e)
            {
                _log.Debug(e.Message, e);
            }

            try
            {
                ChannelServices.UnregisterChannel(channel);
            }
            catch (Exception e)
            {
                _log.Debug(e.Message, e);
            }
            _serviceEstablished = false;
        }

        public static void ClearSimProperties()
        {
            _simProperties.Clear();
        }

        public static void SetSimProperty(string propertyName, object value)
        {
            if (_simProperties.ContainsKey(propertyName))
            {
                _simProperties[propertyName] = value;
            }
            else
            {
                _simProperties.Add(propertyName, value);
            }
        }

        public static void ClearPendingServerMessagesOfType(string messageType)
        {
            var messagesToRemove = _serverMessages.Where(message => message.MessageType == messageType).ToList();
            foreach (var message in messagesToRemove)
            {
                _serverMessages.Remove(message);
            }
        }

        public static void ClearPendingServerMessages()
        {
            if (_serverMessages != null)
            {
                _serverMessages.Clear();
            }
        }

        public static void SubmitMessageToClient(Message message)
        {
            if (_clientMessages != null)
            {
                if (_clientMessages.Count >= 1000)
                {
                    _clientMessages.RemoveRange(999, _clientMessages.Count - 1000);
                    //limit the message queue size to 1000 messages
                }
                _clientMessages.Add(message);
            }
        }

        public static Message GetNextPendingServerMessage()
        {
            Message toReturn = null;
            if (_serverMessages != null)
            {
                if (_serverMessages.Count > 0)
                {
                    toReturn = _serverMessages[0];
                    _serverMessages.RemoveAt(0);
                }
            }
            return toReturn;
        }
    }
}