using Common.Drawing;
using F16CPD.Mfd.Controls;

namespace F16CPD.Mfd.Menus
{
    internal interface IBrightnessIncreaseButtonFactory
    {
        OptionSelectButton BuildBrightnessIncreaseButton(MfdMenuPage mfdMenuPage);
    }
    class BrightnessIncreaseButtonFactory:IBrightnessIncreaseButtonFactory
    {
        private F16CpdMfdManager _mfdManager;
        private IOptionSelectButtonFactory _optionSelectButtonFactory;
        public BrightnessIncreaseButtonFactory(
            F16CpdMfdManager mfdManager,
            IOptionSelectButtonFactory optionSelectButtonFactory= null
        )
        {
            _mfdManager = mfdManager;
            _optionSelectButtonFactory = optionSelectButtonFactory ?? new OptionSelectButtonFactory();
        }
        public OptionSelectButton BuildBrightnessIncreaseButton(MfdMenuPage mfdMenuPage)
        {
            var brightnessIncreaseButton = _optionSelectButtonFactory.CreateOptionSelectButton(mfdMenuPage, 13, "BRT", false);
            brightnessIncreaseButton.FunctionName = "IncreaseBrightness";
            brightnessIncreaseButton.Pressed += brightnessIncreaseButton_Pressed;
            brightnessIncreaseButton.LabelSize = new Size(0, 0);
            brightnessIncreaseButton.LabelLocation = new Point(-10000, -10000);
            return brightnessIncreaseButton;
        }
        private void brightnessIncreaseButton_Pressed(object sender, MomentaryButtonPressedEventArgs e)
        {
            _mfdManager.IncreaseBrightness();
        }
    }
}
