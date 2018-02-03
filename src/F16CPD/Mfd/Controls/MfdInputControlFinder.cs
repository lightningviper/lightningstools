
namespace F16CPD.Mfd.Controls
{
    public interface IMfdInputControlFinder
    {
        MfdInputControl GetControl(MfdMenuPage page, CpdInputControls control);
    }
    class MfdInputControlFinder:IMfdInputControlFinder
    {
        private F16CpdMfdManager _mfdManager;
        public MfdInputControlFinder(F16CpdMfdManager mfdManager)
        {
            _mfdManager = mfdManager;
        }
        public MfdInputControl GetControl(MfdMenuPage page, CpdInputControls control)
        {
            MfdInputControl toReturn = null;
            switch (control)
            {
                case CpdInputControls.Unknown:
                    break;
                case CpdInputControls.OsbButton1:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(1);
                    }
                    break;
                case CpdInputControls.OsbButton2:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(2);
                    }
                    break;
                case CpdInputControls.OsbButton3:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(3);
                    }
                    break;
                case CpdInputControls.OsbButton4:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(4);
                    }
                    break;
                case CpdInputControls.OsbButton5:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(5);
                    }
                    break;
                case CpdInputControls.OsbButton6:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(6);
                    }
                    break;
                case CpdInputControls.OsbButton7:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(7);
                    }
                    break;
                case CpdInputControls.OsbButton8:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(8);
                    }
                    break;
                case CpdInputControls.OsbButton9:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(9);
                    }
                    break;
                case CpdInputControls.OsbButton10:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(10);
                    }
                    break;
                case CpdInputControls.OsbButton11:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(11);
                    }
                    break;
                case CpdInputControls.OsbButton12:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(12);
                    }
                    break;
                case CpdInputControls.OsbButton13:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(13);
                    }
                    break;
                case CpdInputControls.OsbButton14:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(14);
                    }
                    break;
                case CpdInputControls.OsbButton15:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(15);
                    }
                    break;
                case CpdInputControls.OsbButton16:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(16);
                    }
                    break;
                case CpdInputControls.OsbButton17:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(17);
                    }
                    break;
                case CpdInputControls.OsbButton18:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(18);
                    }
                    break;
                case CpdInputControls.OsbButton19:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(19);
                    }
                    break;
                case CpdInputControls.OsbButton20:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(20);
                    }
                    break;
                case CpdInputControls.OsbButton21:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(21);
                    }
                    break;
                case CpdInputControls.OsbButton22:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(22);
                    }
                    break;
                case CpdInputControls.OsbButton23:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(23);
                    }
                    break;
                case CpdInputControls.OsbButton24:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(24);
                    }
                    break;
                case CpdInputControls.OsbButton25:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(25);
                    }
                    break;
                case CpdInputControls.OsbButton26:
                    if (page != null)
                    {
                        toReturn = page.FindOptionSelectButtonByPositionNumber(26);
                    }
                    break;
                case CpdInputControls.HsiModeControl:
                    toReturn = _mfdManager.HsiModeSelectorSwitch;
                    break;
                case CpdInputControls.HsiModeTcn:
                    toReturn = _mfdManager.HsiModeSelectorSwitch.GetPositionByName("TCN");
                    break;
                case CpdInputControls.HsiModeIlsTcn:
                    toReturn = _mfdManager.HsiModeSelectorSwitch.GetPositionByName("ILS/TCN");
                    break;
                case CpdInputControls.HsiModeNav:
                    toReturn = _mfdManager.HsiModeSelectorSwitch.GetPositionByName("NAV");
                    break;
                case CpdInputControls.HsiModeIlsNav:
                    toReturn = _mfdManager.HsiModeSelectorSwitch.GetPositionByName("ILS/NAV");
                    break;
                case CpdInputControls.ParameterAdjustKnob:
                    toReturn = _mfdManager.ParamAdjustKnob;
                    break;
                case CpdInputControls.ParameterAdjustKnobIncrease:
                    toReturn = _mfdManager.ParamAdjustKnob.ClockwiseMomentaryInputControl;
                    break;
                case CpdInputControls.ParameterAdjustKnobDecrease:
                    toReturn = _mfdManager.ParamAdjustKnob.CounterclockwiseMomentaryInputControl;
                    break;
                case CpdInputControls.FuelSelectControl:
                    toReturn = _mfdManager.FuelSelectSwitch;
                    break;
                case CpdInputControls.FuelSelectTest:
                    toReturn = _mfdManager.FuelSelectSwitch.GetPositionByName("TEST");
                    break;
                case CpdInputControls.FuelSelectNorm:
                    toReturn = _mfdManager.FuelSelectSwitch.GetPositionByName("NORM");
                    break;
                case CpdInputControls.FuelSelectRsvr:
                    toReturn = _mfdManager.FuelSelectSwitch.GetPositionByName("RSVR");
                    break;
                case CpdInputControls.FuelSelectIntWing:
                    toReturn = _mfdManager.FuelSelectSwitch.GetPositionByName("INT WING");
                    break;
                case CpdInputControls.FuelSelectExtWing:
                    toReturn = _mfdManager.FuelSelectSwitch.GetPositionByName("EXT WING");
                    break;
                case CpdInputControls.FuelSelectExtCtr:
                    toReturn = _mfdManager.FuelSelectSwitch.GetPositionByName("EXT CTR");
                    break;
                case CpdInputControls.ExtFuelTransSwitch:
                    toReturn = _mfdManager.ExtFuelTransSwitch;
                    break;
                case CpdInputControls.ExtFuelSwitchTransNorm:
                    toReturn = _mfdManager.ExtFuelTransSwitch.GetPositionByName("NORM");
                    break;
                case CpdInputControls.ExtFuelSwitchTransWingFirst:
                    toReturn = _mfdManager.ExtFuelTransSwitch.GetPositionByName("WING FIRST");
                    break;
                default:
                    break;
            }
            return toReturn;
        }
    }
}
