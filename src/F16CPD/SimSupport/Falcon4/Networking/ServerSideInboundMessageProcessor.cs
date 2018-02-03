using F16CPD.Networking;
using F16CPD.Properties;
using F16CPD.SimSupport.Falcon4.EventHandlers;

namespace F16CPD.SimSupport.Falcon4.Networking
{
    internal interface IServerSideInboundMessageProcessor
    {
        bool ProcessPendingMessage(Message message);
    }

    internal class ServerSideInboundMessageProcessor : IServerSideInboundMessageProcessor
    {
        private readonly IDecreaseAlowEventHandler _decreaseAlowEventHandler;
        private readonly IDecreaseBaroEventHandler _decreaseBaroEventHandler;
        private readonly IFalconCallbackSender _falconCallbackSender;
        private readonly IIncreaseAlowEventHandler _increaseAlowEventHandler;
        private readonly IIncreaseBaroEventHandler _increaseBaroEventHandler;

        public ServerSideInboundMessageProcessor(
            F16CpdMfdManager mfdManager,
            IFalconCallbackSender falconCallbackSender = null,
            IIncreaseAlowEventHandler increaseAlowEventHandler = null,
            IDecreaseAlowEventHandler decreaseAlowEventHandler = null,
            IIncreaseBaroEventHandler increaseBaroEventHandler = null,
            IDecreaseBaroEventHandler decreaseBaroEventHandler = null)
        {
            F16CpdMfdManager mfdManager1 = mfdManager;
            _falconCallbackSender = falconCallbackSender ?? new FalconCallbackSender(mfdManager1);
            _increaseAlowEventHandler = increaseAlowEventHandler ??
                                        new IncreaseAlowEventHandler(mfdManager1, _falconCallbackSender);
            _decreaseAlowEventHandler = decreaseAlowEventHandler ??
                                        new DecreaseAlowEventHandler(mfdManager1, _falconCallbackSender);
            _increaseBaroEventHandler = increaseBaroEventHandler ??
                                        new IncreaseBaroEventHandler(mfdManager1, _falconCallbackSender);
            _decreaseBaroEventHandler = decreaseBaroEventHandler ??
                                        new DecreaseBaroEventHandler(mfdManager1, _falconCallbackSender);
        }

        public bool ProcessPendingMessage(Message message)
        {
            if (!Settings.Default.RunAsServer) return false;
            var toReturn = false;
            if (message == null) return false;
            var messageType = message.MessageType;
            if (messageType == null) return false;
            switch (messageType)
            {
                case "Falcon4SendCallbackMessage":
                    var callback = (string) message.Payload;
                    _falconCallbackSender.SendCallbackToFalcon(callback);
                    toReturn = true;
                    break;
                case "Falcon4IncreaseALOW":
                {
                    _increaseAlowEventHandler.IncreaseAlow();
                    toReturn = true;
                }
                    break;

                case "Falcon4DecreaseALOW":
                {
                    _decreaseAlowEventHandler.DecreaseAlow();
                    toReturn = true;
                }
                    break;
                case "Falcon4IncreaseBaro":
                {
                    _increaseBaroEventHandler.IncreaseBaro();
                    toReturn = true;
                }
                    break;

                case "Falcon4DecreaseBaro":
                {
                    _decreaseBaroEventHandler.DecreaseBaro();
                    toReturn = true;
                }
                    break;
            }
            return toReturn;
        }
    }
}