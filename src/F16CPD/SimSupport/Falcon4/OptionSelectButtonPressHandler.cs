using F16CPD.Mfd.Controls;
using F16CPD.SimSupport.Falcon4.EventHandlers;
using System;

namespace F16CPD.SimSupport.Falcon4
{
    internal interface IOptionSelectButtonPressHandler
    {
        void HandleOptionSelectButtonPress(OptionSelectButton button);
    }
    class OptionSelectButtonPressHandler:IOptionSelectButtonPressHandler
    {
        private readonly ICourseSelectIncreaseEventHandler _courseSelectIncreaseEventHandler;
        private readonly ICourseSelectDecreaseEventHandler _courseSelectDecreaseEventHandler;
        private readonly IHeadingSelectIncreaseEventHandler _headingSelectIncreaseEventHandler;
        private readonly IHeadingSelectDecreaseEventHandler _headingSelectDecreaseEventHandler;
        private readonly IIncreaseBaroEventHandler _increaseBaroEventHandler;
        private readonly IDecreaseBaroEventHandler _decreaseBaroEventHandler;
        private readonly IIncreaseAlowEventHandler _increaseAlowEventHandler;
        private readonly IDecreaseAlowEventHandler _decreaseAlowEventHandler;
        public OptionSelectButtonPressHandler(
            F16CpdMfdManager mfdManager,
            IFalconCallbackSender falconCallbackSender=null,
            ICourseSelectIncreaseEventHandler courseSelectIncreaseEventHandler=null,
            ICourseSelectDecreaseEventHandler courseSelectDecreaseEventHandler=null,
            IHeadingSelectIncreaseEventHandler headingSelectIncreaseEventHandler=null,
            IHeadingSelectDecreaseEventHandler headingSelectDecreaseEventHandler =null,
            IIncreaseBaroEventHandler increaseBaroEventHandler=null,
            IDecreaseBaroEventHandler decreaseBaroEventHandler=null,
            IIncreaseAlowEventHandler increaseAlowEventHandler=null,
            IDecreaseAlowEventHandler decreaseAlowEventHandler=null)
        {
            IFalconCallbackSender falconCallbackSender1 = falconCallbackSender ?? new FalconCallbackSender(mfdManager);
            _courseSelectIncreaseEventHandler=courseSelectIncreaseEventHandler ?? new CourseSelectIncreaseEventHandler(falconCallbackSender1);
            _courseSelectDecreaseEventHandler = courseSelectDecreaseEventHandler ?? new CourseSelectDecreaseEventHandler(falconCallbackSender1);
            _headingSelectIncreaseEventHandler = headingSelectIncreaseEventHandler ?? new HeadingSelectIncreaseEventHandler(falconCallbackSender1);
            _headingSelectDecreaseEventHandler = headingSelectDecreaseEventHandler ?? new HeadingSelectDecreaseEventHandler(falconCallbackSender1);
            _increaseBaroEventHandler = increaseBaroEventHandler ?? new IncreaseBaroEventHandler(mfdManager, falconCallbackSender1);
            _decreaseBaroEventHandler = decreaseBaroEventHandler ?? new DecreaseBaroEventHandler(mfdManager, falconCallbackSender1);
            _increaseAlowEventHandler = increaseAlowEventHandler ?? new IncreaseAlowEventHandler(mfdManager, falconCallbackSender1);
            _decreaseAlowEventHandler = decreaseAlowEventHandler ?? new DecreaseAlowEventHandler(mfdManager, falconCallbackSender1);
        }
        public void HandleOptionSelectButtonPress(OptionSelectButton button)
        {
            var functionName = button.FunctionName;
            if (!String.IsNullOrEmpty(functionName))
            {
                switch (functionName)
                {
                    case "CourseSelectIncrease":
                        _courseSelectIncreaseEventHandler.CourseSelectIncrease();
                        break;
                    case "CourseSelectDecrease":
                        _courseSelectDecreaseEventHandler.CourseSelectDecrease();
                        break;
                    case "HeadingSelectIncrease":
                        _headingSelectIncreaseEventHandler.HeadingSelectIncrease();
                        break;
                    case "HeadingSelectDecrease":
                        _headingSelectDecreaseEventHandler.HeadingSelectDecrease();
                        break;
                    case "BarometricPressureSettingIncrease":
                        _increaseBaroEventHandler.IncreaseBaro();
                        break;
                    case "BarometricPressureSettingDecrease":
                        _decreaseBaroEventHandler.DecreaseBaro();
                        break;
                    case "LowAltitudeWarningThresholdIncrease":
                        _increaseAlowEventHandler.IncreaseAlow();
                        break;
                    case "LowAltitudeWarningThresholdDecrease":
                        _decreaseAlowEventHandler.DecreaseAlow();
                        break;
                    case "AcknowledgeMessage":
                        //SendCallbackToFalcon("SimICPFAck");
                        break;
                }
            }
        }

    }
}
