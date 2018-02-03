using F16CPD.Mfd.Controls;

namespace F16CPD.Mfd.Menus
{
    internal interface IParamSelectKnobFactory
    {
        RotaryEncoderMfdInputControl BuildParamSelectKnob();
    }
    class ParamSelectKnobFactory:IParamSelectKnobFactory
    {
        private F16CpdMfdManager _mfdManager;
        public ParamSelectKnobFactory(F16CpdMfdManager mfdManager)
        {
            _mfdManager = mfdManager;
        }

        public RotaryEncoderMfdInputControl BuildParamSelectKnob()
        {
            return new RotaryEncoderMfdInputControl();
        }
    }
}
