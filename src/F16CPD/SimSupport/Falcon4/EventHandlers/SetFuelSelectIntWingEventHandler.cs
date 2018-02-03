namespace F16CPD.SimSupport.Falcon4.EventHandlers
{
    internal interface ISetFuelSelectIntWingEventHandler
    {
        void SetFuelSelectIntWing();
    }
    class SetFuelSelectIntWingEventHandler : ISetFuelSelectIntWingEventHandler
    {
        private readonly IFalconCallbackSender _falconCallbackSender;
        public SetFuelSelectIntWingEventHandler(IFalconCallbackSender falconCallbackSender)
        {
            _falconCallbackSender = falconCallbackSender;
        }
        public void SetFuelSelectIntWing()
        {
            _falconCallbackSender.SendCallbackToFalcon("SimFuelSwitchWingInt");
        }
    }
}
