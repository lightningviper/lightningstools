namespace F16CPD.SimSupport.Falcon4.EventHandlers
{
    internal interface ISetFuelSelectExtWingEventHandler
    {
        void SetFuelSelectExtWing();
    }
    class SetFuelSelectExtWingEventHandler : ISetFuelSelectExtWingEventHandler
    {
        private readonly IFalconCallbackSender _falconCallbackSender;
        public SetFuelSelectExtWingEventHandler(IFalconCallbackSender falconCallbackSender)
        {
            _falconCallbackSender = falconCallbackSender;
        }
        public void SetFuelSelectExtWing()
        {
            _falconCallbackSender.SendCallbackToFalcon("SimFuelSwitchWingExt");
        }
    }
}
