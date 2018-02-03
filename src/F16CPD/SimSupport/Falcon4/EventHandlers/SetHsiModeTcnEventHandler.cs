
namespace F16CPD.SimSupport.Falcon4.EventHandlers
{
    internal interface ISetHsiModeTcnEventHandler
    {
        void SetHsiModeTcn();
    }
    class SetHsiModeTcnEventHandler : ISetHsiModeTcnEventHandler
    {
        private readonly IFalconCallbackSender _falconCallbackSender;
        public SetHsiModeTcnEventHandler(IFalconCallbackSender falconCallbackSender)
        {
            _falconCallbackSender = falconCallbackSender;
        }
        public void SetHsiModeTcn()
        {
            _falconCallbackSender.SendCallbackToFalcon("SimHSITcn");
        }
    }
}
