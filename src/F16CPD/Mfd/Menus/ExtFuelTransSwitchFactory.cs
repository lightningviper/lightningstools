using F16CPD.Mfd.Controls;

namespace F16CPD.Mfd.Menus
{
    internal interface IExtFuelTransSwitchFactory
    {
        ToggleSwitchMfdInputControl BuildExtFuelTransSwitch();
    }
    class ExtFuelTransSwitchFactory:IExtFuelTransSwitchFactory
    {
        private F16CpdMfdManager _mfdManager;
        public ExtFuelTransSwitchFactory(F16CpdMfdManager mfdManager)
        {
            _mfdManager = mfdManager;
        }
        public ToggleSwitchMfdInputControl BuildExtFuelTransSwitch()
        {
            var extFuelTransSwitch = new ToggleSwitchMfdInputControl();
            extFuelTransSwitch.PositionChanged += _extFuelTransSwitch_PositionChanged;
            extFuelTransSwitch.AddPosition("NORM");
            extFuelTransSwitch.AddPosition("WING FIRST");
            return extFuelTransSwitch;
        }
        private void _extFuelTransSwitch_PositionChanged(object sender, ToggleSwitchPositionChangedEventArgs e)
        {
            if (e == null) return;
            switch (e.NewPosition.PositionName)
            {
                case "NORM":
                    if (_mfdManager.SimSupportModule != null)
                        _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.ExtFuelSwitchTransNorm, e.NewPosition);
                    break;
                case "WING FIRST":
                    if (_mfdManager.SimSupportModule != null)
                        _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.ExtFuelSwitchTransWingFirst,
                                                                  e.NewPosition);
                    break;
            }
        }
    }
}
