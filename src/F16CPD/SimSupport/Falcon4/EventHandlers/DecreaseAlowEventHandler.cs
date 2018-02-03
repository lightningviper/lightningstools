using F16CPD.Networking;
using F16CPD.Properties;

namespace F16CPD.SimSupport.Falcon4.EventHandlers
{
    internal interface IDecreaseAlowEventHandler
    {
        void DecreaseAlow();
    }
    class DecreaseAlowEventHandler:IDecreaseAlowEventHandler
    {
        private readonly F16CpdMfdManager _mfdManager;
        private readonly IFalconCallbackSender _falconCallbackSender;
        public DecreaseAlowEventHandler(F16CpdMfdManager mfdManager, IFalconCallbackSender falconCallbackSender)
        {
            _mfdManager = mfdManager;
            _falconCallbackSender = falconCallbackSender;
        }
        public void DecreaseAlow()
        {
            if (Settings.Default.RunAsClient)
            {
                var message = new Message("Falcon4DecreaseALOW", _mfdManager.FlightData.AutomaticLowAltitudeWarningInFeet);
                _mfdManager.Client.SendMessageToServer(message);
            }
            else
            {
                if (_mfdManager.FlightData.AutomaticLowAltitudeWarningInFeet > 1000)
                {
                    _mfdManager.FlightData.AutomaticLowAltitudeWarningInFeet -= 1000;
                }
                else
                {
                    _mfdManager.FlightData.AutomaticLowAltitudeWarningInFeet -= 100;
                }
                _falconCallbackSender.SendCallbackToFalcon("DecreaseAlow");
            }
        }
    }
}
