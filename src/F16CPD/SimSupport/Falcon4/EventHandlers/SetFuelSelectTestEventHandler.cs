
namespace F16CPD.SimSupport.Falcon4.EventHandlers
{
    internal interface ISetFuelSelectTestEventHandler 
    {
        void SetFuelSelectTest();
    }
    class SetFuelSelectTestEventHandler : ISetFuelSelectTestEventHandler
    {
        private readonly IFalconCallbackSender _falconCallbackSender;
        public SetFuelSelectTestEventHandler(IFalconCallbackSender falconCallbackSender)
        {
            _falconCallbackSender = falconCallbackSender;
        }
        public void SetFuelSelectTest()
        {
            _falconCallbackSender.SendCallbackToFalcon("SimFuelSwitchTest");
        }
    }
}
