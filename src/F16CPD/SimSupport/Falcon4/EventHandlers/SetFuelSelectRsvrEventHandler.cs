namespace F16CPD.SimSupport.Falcon4.EventHandlers
{
    internal interface ISetFuelSelectRsvrEventHandler
    {
        void SetFuelSelectRsvr();
    }
    class SetFuelSelectRsvrEventHandler : ISetFuelSelectRsvrEventHandler
    {
        private readonly IFalconCallbackSender _falconCallbackSender;
        public SetFuelSelectRsvrEventHandler(IFalconCallbackSender falconCallbackSender)
        {
            _falconCallbackSender = falconCallbackSender;
        }
        public void SetFuelSelectRsvr()
        {
            _falconCallbackSender.SendCallbackToFalcon("SimFuelSwitchResv");
        }
    }
}
