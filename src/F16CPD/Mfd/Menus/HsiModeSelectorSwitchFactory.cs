using F16CPD.Mfd.Controls;

namespace F16CPD.Mfd.Menus
{
    internal interface IHsiModeSlectorSwitchFactory
    {
        ToggleSwitchMfdInputControl CreateHsiModeSelectorSwitch();
    }
    class HsiModeSelectorSwitchFactory:IHsiModeSlectorSwitchFactory
    {
        private F16CpdMfdManager _mfdManager;
        public HsiModeSelectorSwitchFactory(F16CpdMfdManager mfdManager)
        {
            _mfdManager = mfdManager;
        }
        public ToggleSwitchMfdInputControl CreateHsiModeSelectorSwitch() 
        {
            var toggleSwitch = new ToggleSwitchMfdInputControl();
            toggleSwitch.PositionChanged += _hsiModeSelectorSwitch_PositionChanged;
            toggleSwitch.AddPosition(@"ILS/TCN");
            toggleSwitch.AddPosition("TCN");
            toggleSwitch.AddPosition("NAV");
            toggleSwitch.AddPosition(@"ILS/NAV");
            return toggleSwitch;
        }
        private void _hsiModeSelectorSwitch_PositionChanged(object sender, ToggleSwitchPositionChangedEventArgs e)
        {
            if (e == null) return;
            switch (e.NewPosition.PositionName)
            {
                case "ILS/TCN":
                    if (_mfdManager.SimSupportModule != null)
                        _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.HsiModeIlsTcn, e.NewPosition);
                    break;
                case "TCN":
                    if (_mfdManager.SimSupportModule != null)
                        _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.HsiModeTcn, e.NewPosition);
                    break;
                case "NAV":
                    if (_mfdManager.SimSupportModule != null)
                        _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.HsiModeNav, e.NewPosition);
                    break;
                case "ILS/NAV":
                    if (_mfdManager.SimSupportModule != null)
                        _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.HsiModeIlsNav, e.NewPosition);
                    break;
            }
        }
    }
}
