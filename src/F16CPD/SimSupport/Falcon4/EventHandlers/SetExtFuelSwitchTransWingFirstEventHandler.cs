
namespace F16CPD.SimSupport.Falcon4.EventHandlers
{
    internal interface ISetExtFuelSwitchTransWingFirstEventHandler
    {
        void SetExtFuelSwitchTransWingFirst();
    }
    class SetExtFuelSwitchTransWingFirstEventHandler : ISetExtFuelSwitchTransWingFirstEventHandler
    {
        private readonly IFalconCallbackSender _falconCallbackSender;
        public SetExtFuelSwitchTransWingFirstEventHandler(IFalconCallbackSender falconCallbackSender)
        {
            _falconCallbackSender = falconCallbackSender;
        }
        public void SetExtFuelSwitchTransWingFirst()
        {
            _falconCallbackSender.SendCallbackToFalcon("SimFuelTransWing");
        }
    }
}
