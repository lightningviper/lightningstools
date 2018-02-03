using F16CPD.Networking;
using F16CPD.Properties;

namespace F16CPD.SimSupport.Falcon4.EventHandlers
{
    internal interface IIncreaseBaroEventHandler
    {
        void IncreaseBaro();
    }
    class IncreaseBaroEventHandler : IIncreaseBaroEventHandler
    {
        private readonly F16CpdMfdManager _mfdManager;
        private readonly IFalconCallbackSender _falconCallbackSender;
        public IncreaseBaroEventHandler(F16CpdMfdManager mfdManager, IFalconCallbackSender falconCallbackSender)
        {
            _mfdManager = mfdManager;
            _falconCallbackSender = falconCallbackSender;
        }
        public void IncreaseBaro()
        {
            if (Settings.Default.RunAsClient)
            {
                var message = new Message("Falcon4IncreaseBaro", null);
                _mfdManager.Client.SendMessageToServer(message);
            }
            else
            {
                _falconCallbackSender.SendCallbackToFalcon("SimAltPressInc");
            }
        }
    }
}
