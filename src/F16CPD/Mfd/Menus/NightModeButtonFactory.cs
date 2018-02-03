using Common.Drawing;
using F16CPD.Mfd.Controls;

namespace F16CPD.Mfd.Menus
{
    internal interface INightModeButtonFactory
    {
        OptionSelectButton CreateNightModeButton(MfdMenuPage mfdMenuPage);
    }
    class NightModeButtonFactory:INightModeButtonFactory
    {
        private F16CpdMfdManager _mfdManager;
        private IOptionSelectButtonFactory _optionSelectButtonFactory;
        public NightModeButtonFactory(
            F16CpdMfdManager mfdManager,
            IOptionSelectButtonFactory optionSelectButtonFactory = null
        )
        {
            _mfdManager = mfdManager;
            _optionSelectButtonFactory = optionSelectButtonFactory ?? new OptionSelectButtonFactory();
        }
        public OptionSelectButton CreateNightModeButton(MfdMenuPage mfdMenuPage)
        {
            var nightModeButton = _optionSelectButtonFactory.CreateOptionSelectButton(mfdMenuPage, 6, "NGT", false);
            nightModeButton.FunctionName = "NightMode";
            nightModeButton.Pressed += (s,e)=>_mfdManager.NightMode = true;
            nightModeButton.LabelLocation = new Point(-10000, -10000);
            nightModeButton.LabelSize = new Size(0, 0);
            return nightModeButton;
        }
    }
}
