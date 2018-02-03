using F16CPD.Networking;
using F16CPD.Properties;


namespace F16CPD.SimSupport.Falcon4.EventHandlers
{
    internal interface IIncreaseAlowEventHandler
    {
        void IncreaseAlow();
    }
    class IncreaseAlowEventHandler : IIncreaseAlowEventHandler
    {
        private readonly F16CpdMfdManager _mfdManager;
        private readonly IFalconCallbackSender _falconCallbackSender;
        public IncreaseAlowEventHandler(F16CpdMfdManager mfdManager, IFalconCallbackSender falconCallbackSender)
        {
            _mfdManager = mfdManager;
            _falconCallbackSender = falconCallbackSender;
        }
        public void IncreaseAlow()
        {

            if (Settings.Default.RunAsClient)
            {
                var message = new Message("Falcon4IncreaseALOW", _mfdManager.FlightData.AutomaticLowAltitudeWarningInFeet);
                _mfdManager.Client.SendMessageToServer(message);
            }
            else
            {
                if (_mfdManager.FlightData.AutomaticLowAltitudeWarningInFeet < 1000)
                {
                    _mfdManager.FlightData.AutomaticLowAltitudeWarningInFeet += 100;
                }
                else
                {
                    _mfdManager.FlightData.AutomaticLowAltitudeWarningInFeet += 1000;
                }
                _falconCallbackSender.SendCallbackToFalcon("IncreaseAlow");
            }
        }
    }
}
