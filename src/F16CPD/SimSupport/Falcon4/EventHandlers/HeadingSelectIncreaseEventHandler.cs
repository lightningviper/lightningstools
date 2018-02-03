using F4KeyFile;

namespace F16CPD.SimSupport.Falcon4.EventHandlers
{
    internal interface IHeadingSelectIncreaseEventHandler
    {
        void HeadingSelectIncrease();
    }
    class HeadingSelectIncreaseEventHandler : IHeadingSelectIncreaseEventHandler
    {
        private readonly IFalconCallbackSender _falconCallbackSender;
        public HeadingSelectIncreaseEventHandler(IFalconCallbackSender falconCallbackSender)
        {
            _falconCallbackSender = falconCallbackSender;
        }
        public void HeadingSelectIncrease()
        {
            var useIncrementByOne = false;
            KeyBinding incByOneCallback = F4Utils.Process.KeyFileUtils.FindKeyBinding("SimHsiHdgIncBy1");
            if (incByOneCallback != null &&
                incByOneCallback.Key.ScanCode != (int)ScanCodes.NotAssigned)
            {
                useIncrementByOne = true;
            }
            _falconCallbackSender.SendCallbackToFalcon(useIncrementByOne ? "SimHsiHdgIncBy1" : "SimHsiHeadingInc");

        }
    }
}
