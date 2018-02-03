
namespace F16CPD.SimSupport.Falcon4.EventHandlers
{
    internal interface ISetHsiModeNavEventHandler
    {
        void SetHsiModeNav();
    }
    class SetHsiModeNavEventHandler : ISetHsiModeNavEventHandler
    {
        private readonly IFalconCallbackSender _falconCallbackSender;
        public SetHsiModeNavEventHandler(IFalconCallbackSender falconCallbackSender)
        {
            _falconCallbackSender = falconCallbackSender;
        }
        public void SetHsiModeNav()
        {
            _falconCallbackSender.SendCallbackToFalcon("SimHSINav");
        }
    }
}
