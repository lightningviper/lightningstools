using F16CPD.Networking;
using F16CPD.Properties;
using System;

namespace F16CPD.SimSupport.Falcon4.Networking
{
    internal interface IClientSideInboundMessageProcessor
    {
        bool ProcessPendingMessage(Message message);
    }
    class ClientSideInboundMessageProcessor:IClientSideInboundMessageProcessor
    {
        public bool ProcessPendingMessage(Message message)
        {
            if (!Settings.Default.RunAsClient) return false;
            var toReturn = false;
            if (message == null) return false;
            var messageType = message.MessageType;
            if (messageType == null) return false;
            switch (messageType)
            {
                case "Falcon4CallbackOccurredMessage":
                    var callback = (string)message.Payload;
                    ProcessDetectedCallback(callback);
                    toReturn = true;
                    break;
            }
            return toReturn;
        }
        private static void ProcessDetectedCallback(string callback)
        {
            if (String.IsNullOrEmpty(callback) || String.IsNullOrEmpty(callback.Trim())) return;
            if (!Settings.Default.RunAsServer) return;
            var message = new Message("Falcon4CallbackOccurredMessage", callback.Trim());
            F16CPDServer.SubmitMessageToClient(message);
        }
    }
}
