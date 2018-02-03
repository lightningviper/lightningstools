using Common.Drawing;
using F16CPD.Mfd.Controls;

namespace F16CPD.Mfd.Menus
{
    internal interface IBrightnessDecreaseButtonFactory 
    {
        OptionSelectButton BuildBrightnessDecreaseButton(MfdMenuPage mfdMenuPage);
    }
    class BrightnessDecreaseButtonFactory:IBrightnessDecreaseButtonFactory
    {
        private F16CpdMfdManager _mfdManager;
        private IOptionSelectButtonFactory _optionSelectButtonFactory;
        public BrightnessDecreaseButtonFactory(
            F16CpdMfdManager mfdManager,
            IOptionSelectButtonFactory optionSelectButtonFactory= null
        )
        {
            _mfdManager = mfdManager;
            _optionSelectButtonFactory = optionSelectButtonFactory ?? new OptionSelectButtonFactory();
        }

        public OptionSelectButton BuildBrightnessDecreaseButton(MfdMenuPage mfdMenuPage)
        {
            var brightnessDecreaseButton = _optionSelectButtonFactory.CreateOptionSelectButton(mfdMenuPage, 19, "DIM", false);
            brightnessDecreaseButton.FunctionName = "DecreaseBrightness";
            brightnessDecreaseButton.Pressed += brightnessDecreaseButton_Pressed;
            brightnessDecreaseButton.LabelSize = new Size(0, 0);
            brightnessDecreaseButton.LabelLocation = new Point(-10000, -10000);
            return brightnessDecreaseButton;
        }
        private void brightnessDecreaseButton_Pressed(object sender, MomentaryButtonPressedEventArgs e)
        {
            _mfdManager.DecreaseBrightness();
        }

    }
}
