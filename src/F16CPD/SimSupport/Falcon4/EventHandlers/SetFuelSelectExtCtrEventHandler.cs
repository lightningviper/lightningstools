namespace F16CPD.SimSupport.Falcon4.EventHandlers
{
    internal interface ISetFuelSelectExtCtrEventHandler
    {
        void SetFuelSelectExtCtr();
    }
    class SetFuelSelectExtCtrEventHandler:ISetFuelSelectExtCtrEventHandler
    {
        private readonly IFalconCallbackSender _falconCallbackSender;
        public SetFuelSelectExtCtrEventHandler(IFalconCallbackSender falconCallbackSender)
        {
            _falconCallbackSender = falconCallbackSender;
        }
            
        public void SetFuelSelectExtCtr()
        {
            _falconCallbackSender.SendCallbackToFalcon("SimFuelSwitchCenterExt");
        }
    }
}
