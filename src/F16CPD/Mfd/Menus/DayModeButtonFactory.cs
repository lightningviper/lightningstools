using Common.Drawing;
using F16CPD.Mfd.Controls;

namespace F16CPD.Mfd.Menus
{
    internal interface IDayModeButtonFactory
    {
        OptionSelectButton BuildDayModeButton(MfdMenuPage mfdMenuPage);
    }
    class DayModeButtonFactory:IDayModeButtonFactory
    {
        private F16CpdMfdManager _mfdManager;
        private IOptionSelectButtonFactory _optionSelectButtonFactory;
        public DayModeButtonFactory(
            F16CpdMfdManager mfdManager,
            IOptionSelectButtonFactory optionSelectButtonFactory = null
        )
        {
            _mfdManager = mfdManager;
            _optionSelectButtonFactory = optionSelectButtonFactory ?? new OptionSelectButtonFactory();
        }
        public OptionSelectButton BuildDayModeButton(MfdMenuPage mfdMenuPage)
        {
            var dayModeButton = _optionSelectButtonFactory.CreateOptionSelectButton(mfdMenuPage, 26, "DAY", false);
            dayModeButton.FunctionName = "DayMode";
            dayModeButton.Pressed += (s,e)=>_mfdManager.NightMode=false;
            dayModeButton.LabelSize = new Size(0, 0);
            dayModeButton.LabelLocation = new Point(-10000, -10000);
            return dayModeButton;
        }
    }
}
