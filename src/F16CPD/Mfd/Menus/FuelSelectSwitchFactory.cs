using F16CPD.Mfd.Controls;

namespace F16CPD.Mfd.Menus
{
    internal interface IFuelSelectSwitchFactory
    {
        ToggleSwitchMfdInputControl BuildFuelSelectSwitch();
    }
    class FuelSelectSwitchFactory:IFuelSelectSwitchFactory
    {
        private F16CpdMfdManager _mfdManager;
        public FuelSelectSwitchFactory(F16CpdMfdManager mfdManager)
        {
            _mfdManager = mfdManager;
        }
        public ToggleSwitchMfdInputControl BuildFuelSelectSwitch()
        {
            var fuelSelectSwitch = new ToggleSwitchMfdInputControl();
            fuelSelectSwitch.PositionChanged += _fuelSelectControl_PositionChanged;
            fuelSelectSwitch.AddPosition("TEST");
            fuelSelectSwitch.AddPosition("NORM");
            fuelSelectSwitch.AddPosition("RSVR");
            fuelSelectSwitch.AddPosition("INT WING");
            fuelSelectSwitch.AddPosition("EXT WING");
            fuelSelectSwitch.AddPosition("EXT CTR");
            return fuelSelectSwitch;
        }
        private void _fuelSelectControl_PositionChanged(object sender, ToggleSwitchPositionChangedEventArgs e)
        {
            if (e == null) return;
            switch (e.NewPosition.PositionName)
            {
                case "TEST":
                    if (_mfdManager.SimSupportModule != null)
                        _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.FuelSelectTest, e.NewPosition);
                    break;
                case "NORM":
                    if (_mfdManager.SimSupportModule != null)
                        _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.FuelSelectNorm, e.NewPosition);
                    break;
                case "RSVR":
                    if (_mfdManager.SimSupportModule != null)
                        _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.FuelSelectRsvr, e.NewPosition);
                    break;
                case "INT WING":
                    if (_mfdManager.SimSupportModule != null)
                        _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.FuelSelectIntWing, e.NewPosition);
                    break;
                case "EXT WING":
                    if (_mfdManager.SimSupportModule != null)
                        _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.FuelSelectExtWing, e.NewPosition);
                    break;
                case "EXT CTR":
                    if (_mfdManager.SimSupportModule != null)
                        _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.FuelSelectExtCtr, e.NewPosition);
                    break;
            }
        }
    }

}
