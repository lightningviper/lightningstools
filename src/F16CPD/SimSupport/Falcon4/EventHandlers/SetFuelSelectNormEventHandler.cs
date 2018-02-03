
namespace F16CPD.SimSupport.Falcon4.EventHandlers
{
    internal interface ISetFuelSelectNormEventHandler
    {
        void SetFuelSelectNorm();
    }
    class SetFuelSelectNormEventHandler : ISetFuelSelectNormEventHandler
    {
        private readonly IFalconCallbackSender _falconCallbackSender;
        public SetFuelSelectNormEventHandler(IFalconCallbackSender falconCallbackSender)
        {
            _falconCallbackSender = falconCallbackSender;
        }
        public void SetFuelSelectNorm()
        {
            _falconCallbackSender.SendCallbackToFalcon("SimFuelSwitchNorm");
        }
    }
}
