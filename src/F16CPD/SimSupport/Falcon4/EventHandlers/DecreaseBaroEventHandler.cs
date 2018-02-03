using F16CPD.Networking;
using F16CPD.Properties;

namespace F16CPD.SimSupport.Falcon4.EventHandlers
{
    internal interface IDecreaseBaroEventHandler
    {
        void DecreaseBaro();
    }
    class DecreaseBaroEventHandler:IDecreaseBaroEventHandler
    {
        private readonly F16CpdMfdManager _mfdManager;
        private readonly IFalconCallbackSender _falconCallbackSender;
        public DecreaseBaroEventHandler(F16CpdMfdManager mfdManager, IFalconCallbackSender falconCallbackSender)
        {
            _mfdManager = mfdManager;
            _falconCallbackSender = falconCallbackSender;

        }
        public void DecreaseBaro()
        {
            if (Settings.Default.RunAsClient)
            {
                var message = new Message("Falcon4DecreaseBaro", null);
                _mfdManager.Client.SendMessageToServer(message);
            }
            else
            {
                _falconCallbackSender.SendCallbackToFalcon("SimAltPressDec");
            }
        }

    }
}
