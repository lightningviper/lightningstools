using Common.Math;
using Common.Strings;
using F4SharedMem;
using F4SharedMem.Headers;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
namespace F4SharedMemViewer
{
    public partial class SharedMemoryViewer : Form
    {
        private Reader _sharedMemReader = new Reader();
        private FlightData _lastFlightData;
        private Timer _timer = new Timer();
        public SharedMemoryViewer()
        {
            InitializeComponent();
            SetTabPanelBackgroundColors(tabControl1, SystemColors.ButtonHighlight);
            DisableControlsAllTabs();
            InitializeDEDGridView();
            InitializePFLGridView();
            InitializeFD2RWRGridView();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _timer.Tick += _timer_Tick;
            _timer.Interval = 20;
            _timer.Start();
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (ReadSharedMem() != null)
            {
                BindSharedMemoryDataToFormElements();
            }
            else
            {
                DisableControlsAllTabs();
            }
        }
        private FlightData ReadSharedMem()
        {
            return _lastFlightData = _sharedMemReader.GetCurrentData();
        }
        private void DisableControlsAllTabs()
        {
            foreach (var tabPage in tabControl1.TabPages)
            {
                DisableChildControls(tabPage as TabPage);
            }
        }
        private void BindSharedMemoryDataToFormElements()
        {
            this.SuspendLayout();
            if (tabControl1.SelectedTab == tabFDBits)
            {
                EnableChildControls(tabFDBits);
                BindLightBits1ToFormElements();
                BindLightBits2ToFormElements();
                BindLightBits3ToFormElements();
                BindHsiBitsToFormElements();
            }
            else if (tabControl1.SelectedTab == tabFDVars)
            {
                EnableChildControls(tabFDVars);
                BindFDVarsToFormElements();
            }
            else if (tabControl1.SelectedTab == tabRWR)
            {
                EnableChildControls(tabRWR);
                BindRWRToFormElements();
            }
            else if (tabControl1.SelectedTab == tabDED_PFL)
            {
                EnableChildControls(tabDED_PFL);
                BindDEDToFormElements();
                BindPFLToFormElements();
            }
            else if (tabControl1.SelectedTab == tabFD2Bits)
            {
                EnableChildControls(tabFD2Bits);
                BindFD2VarsToFormElements();
            }
            else if (tabControl1.SelectedTab == tabFD2Vars)
            {
                EnableChildControls(tabFD2Vars);
                BindFD2VarsToFormElements();
            }
            else if (tabControl1.SelectedTab == tabFD2RWRPilots)
            {
                EnableChildControls(tabFD2RWRPilots);
                BindFD2RWRInfoToFormElements();
                BindFD2PilotInfoToFormElements();
            }
            else if (tabControl1.SelectedTab == tabOSB)
            {
                EnableChildControls(tabOSB);
                BindOSBDataToFormElements();
            }
            else if (tabControl1.SelectedTab == tabIVibe)
            {
                EnableChildControls(tabIVibe);
                BindIntelliVibeDataToFormElements();
            }
            else if (tabControl1.SelectedTab == tabIVC_RCS_RCC)
            {
                EnableChildControls(tabIVC_RCS_RCC);
                Bind_IVC_RCS_RCC_DataToFormElements();
            }
            else if (tabControl1.SelectedTab == tabStrings)
            {
                EnableChildControls(tabStrings);
                BindStringsVarsToFormElements();
            }
            else if (tabControl1.SelectedTab == tabRawBits)
            {
                EnableChildControls(tabRawBits);
                BindRawBitsToFormElements();
            }
            this.ResumeLayout();
        }
        private void BindLightBits1ToFormElements()
        {
            var lightBits = (LightBits)_lastFlightData.lightBits;
            chkCAN.Checked = (lightBits & LightBits.CAN) == LightBits.CAN;
            chkFLCS.Checked = (lightBits & LightBits.FLCS) == LightBits.FLCS;
            chkFlcs_ABCD.Checked = (lightBits & LightBits.Flcs_ABCD) == LightBits.Flcs_ABCD;
            chkHYD.Checked = (lightBits & LightBits.HYD) == LightBits.HYD;
            chkCONFIG.Checked = (lightBits & LightBits.CONFIG) == LightBits.CONFIG;
            chkENG_FIRE.Checked = (lightBits & LightBits.ENG_FIRE) == LightBits.ENG_FIRE;
            chkONGROUND.Checked = (lightBits & LightBits.ONGROUND) == LightBits.ONGROUND;
            chkEQUIP_HOT.Checked = (lightBits & LightBits.EQUIP_HOT) == LightBits.EQUIP_HOT;
            chkOXY_BROW.Checked = (lightBits & LightBits.OXY_BROW) == LightBits.OXY_BROW;
            chkTF.Checked = (lightBits & LightBits.TF) == LightBits.TF;
            chkMasterCaution.Checked = (lightBits & LightBits.MasterCaution) == LightBits.MasterCaution;
            chkFuelLow.Checked = (lightBits & LightBits.FuelLow) == LightBits.FuelLow;
            chkOverheat.Checked = (lightBits & LightBits.Overheat) == LightBits.Overheat;
            chkEngineFault.Checked = (lightBits & LightBits.EngineFault) == LightBits.EngineFault;
            chkLEFlaps.Checked = (lightBits & LightBits.LEFlaps) == LightBits.LEFlaps;
            chkFltControlSys.Checked = (lightBits & LightBits.FltControlSys) == LightBits.FltControlSys;
            chkRefuelDSC.Checked = (lightBits & LightBits.RefuelDSC) == LightBits.RefuelDSC;
            chkRefuelAR.Checked = (lightBits & LightBits.RefuelAR) == LightBits.RefuelAR;
            chkRefuelRDY.Checked = (lightBits & LightBits.RefuelRDY) == LightBits.RefuelRDY;
            chkAOAAbove.Checked = (lightBits & LightBits.AOAAbove) == LightBits.AOAAbove;
            chkAOAOn.Checked = (lightBits & LightBits.AOAOn) == LightBits.AOAOn;
            chkAOABelow.Checked = (lightBits & LightBits.AOABelow) == LightBits.AOABelow;
            chkT_L_CFG.Checked = (lightBits & LightBits.T_L_CFG) == LightBits.T_L_CFG;
            chkTFR_STBY.Checked = (lightBits & LightBits.TFR_STBY) == LightBits.TFR_STBY;
            chkAutoPilotOn.Checked = (lightBits & LightBits.AutoPilotOn) == LightBits.AutoPilotOn;
            chkCabinPress.Checked = (lightBits & LightBits.CabinPress) == LightBits.CabinPress;
            chkNWSFail.Checked = (lightBits & LightBits.NWSFail) == LightBits.NWSFail;
            chkHook.Checked = (lightBits & LightBits.Hook) == LightBits.Hook;
            chkECM.Checked = (lightBits & LightBits.ECM) == LightBits.ECM;
            chkIFF.Checked = (lightBits & LightBits.IFF) == LightBits.IFF;
            chkRadarAlt.Checked = (lightBits & LightBits.RadarAlt) == LightBits.RadarAlt;
            chkAvionics.Checked = (lightBits & LightBits.Avionics) == LightBits.Avionics;
        }
        private void BindLightBits2ToFormElements()
        {
            var lightBits2 = (LightBits2)_lastFlightData.lightBits2;
            chkHandoff.Checked = (lightBits2 & LightBits2.HandOff) == LightBits2.HandOff;
            chkLaunch.Checked = (lightBits2 & LightBits2.Launch) == LightBits2.Launch;
            chkPriMode.Checked = (lightBits2 & LightBits2.PriMode) == LightBits2.PriMode;
            chkNaval.Checked = (lightBits2 & LightBits2.Naval) == LightBits2.Naval;
            chkUnk.Checked = (lightBits2 & LightBits2.Unk) == LightBits2.Unk;
            chkTgtSep.Checked = (lightBits2 & LightBits2.TgtSep) == LightBits2.TgtSep;
            chkGo.Checked = (lightBits2 & LightBits2.Go) == LightBits2.Go;
            chkNoGo.Checked = (lightBits2 & LightBits2.NoGo) == LightBits2.NoGo;
            chkDegr.Checked = (lightBits2 & LightBits2.Degr) == LightBits2.Degr;
            chkRdy.Checked = (lightBits2 & LightBits2.Rdy) == LightBits2.Rdy;
            chkChaffLo.Checked = (lightBits2 & LightBits2.ChaffLo) == LightBits2.ChaffLo;
            chkFlareLo.Checked = (lightBits2 & LightBits2.FlareLo) == LightBits2.FlareLo;
            chkAuxSrch.Checked = (lightBits2 & LightBits2.AuxSrch) == LightBits2.AuxSrch;
            chkAuxAct.Checked = (lightBits2 & LightBits2.AuxAct) == LightBits2.AuxAct;
            chkAuxLow.Checked = (lightBits2 & LightBits2.AuxLow) == LightBits2.AuxLow;
            chkAuxPwr.Checked = (lightBits2 & LightBits2.AuxPwr) == LightBits2.AuxPwr;
            chkEcmPwr.Checked = (lightBits2 & LightBits2.EcmPwr) == LightBits2.EcmPwr;
            chkEcmFail.Checked = (lightBits2 & LightBits2.EcmFail) == LightBits2.EcmFail;
            chkFwdFuelLow.Checked = (lightBits2 & LightBits2.FwdFuelLow) == LightBits2.FwdFuelLow;
            chkAftFuelLow.Checked = (lightBits2 & LightBits2.AftFuelLow) == LightBits2.AftFuelLow;
            chkEPUOn.Checked = (lightBits2 & LightBits2.EPUOn) == LightBits2.EPUOn;
            chkJFSOn.Checked = (lightBits2 & LightBits2.JFSOn) == LightBits2.JFSOn;
            chkSEC.Checked = (lightBits2 & LightBits2.SEC) == LightBits2.SEC;
            chkOXY_LOW.Checked = (lightBits2 & LightBits2.OXY_LOW) == LightBits2.OXY_LOW;
            chkPROBEHEAT.Checked = (lightBits2 & LightBits2.PROBEHEAT) == LightBits2.PROBEHEAT;
            chkSEAT_ARM.Checked = (lightBits2 & LightBits2.SEAT_ARM) == LightBits2.SEAT_ARM;
            chkBUC.Checked = (lightBits2 & LightBits2.BUC) == LightBits2.BUC;
            chkFUEL_OIL_HOT.Checked = (lightBits2 & LightBits2.FUEL_OIL_HOT) == LightBits2.FUEL_OIL_HOT;
            chkANTI_SKID.Checked = (lightBits2 & LightBits2.ANTI_SKID) == LightBits2.ANTI_SKID;
            chkTFR_ENGAGED.Checked = (lightBits2 & LightBits2.TFR_ENGAGED) == LightBits2.TFR_ENGAGED;
            chkGEARHANDLE.Checked = (lightBits2 & LightBits2.GEARHANDLE) == LightBits2.GEARHANDLE;
            chkENGINE.Checked = (lightBits2 & LightBits2.ENGINE) == LightBits2.ENGINE;

        }
        private void BindLightBits3ToFormElements()
        {
            var lightBits3 = (LightBits3)_lastFlightData.lightBits3;
            chkFlcsPmg.Checked = (lightBits3 & LightBits3.FlcsPmg) == LightBits3.FlcsPmg;
            chkMainGen.Checked = (lightBits3 & LightBits3.MainGen) == LightBits3.MainGen;
            chkStbyGen.Checked = (lightBits3 & LightBits3.StbyGen) == LightBits3.StbyGen;
            chkEpuGen.Checked = (lightBits3 & LightBits3.EpuGen) == LightBits3.EpuGen;
            chkEpuPmg.Checked = (lightBits3 & LightBits3.EpuPmg) == LightBits3.EpuPmg;
            chkToFlcs.Checked = (lightBits3 & LightBits3.ToFlcs) == LightBits3.ToFlcs;
            chkFlcsRly.Checked = (lightBits3 & LightBits3.FlcsRly) == LightBits3.FlcsRly;
            chkBatFail.Checked = (lightBits3 & LightBits3.BatFail) == LightBits3.BatFail;
            chkHydrazine.Checked = (lightBits3 & LightBits3.Hydrazine) == LightBits3.Hydrazine;
            chkAir.Checked = (lightBits3 & LightBits3.Air) == LightBits3.Air;
            chkElec_Fault.Checked = (lightBits3 & LightBits3.Elec_Fault) == LightBits3.Elec_Fault;
            chkLef_Fault.Checked = (lightBits3 & LightBits3.Lef_Fault) == LightBits3.Lef_Fault;
            chkOnGround_.Checked = (lightBits3 & LightBits3.OnGround) == LightBits3.OnGround;
            chkFlcsBitRun.Checked = (lightBits3 & LightBits3.FlcsBitRun) == LightBits3.FlcsBitRun;
            chkFlcsBitFail.Checked = (lightBits3 & LightBits3.FlcsBitFail) == LightBits3.FlcsBitFail;
            chkDbuWarn.Checked = (lightBits3 & LightBits3.DbuWarn) == LightBits3.DbuWarn;
            chkNoseGearDown.Checked = (lightBits3 & LightBits3.NoseGearDown) == LightBits3.NoseGearDown;
            chkLeftGearDown.Checked = (lightBits3 & LightBits3.LeftGearDown) == LightBits3.LeftGearDown;
            chkRightGearDown.Checked = (lightBits3 & LightBits3.RightGearDown) == LightBits3.RightGearDown;
            chkParkBrakeOn.Checked = (lightBits3 & LightBits3.ParkBrakeOn) == LightBits3.ParkBrakeOn;
            chkPower_Off.Checked = (lightBits3 & LightBits3.Power_Off) == LightBits3.Power_Off;
            chkCadc.Checked = (lightBits3 & LightBits3.cadc) == LightBits3.cadc;
            chkSpeedbrake.Checked = (lightBits3 & LightBits3.SpeedBrake) == LightBits3.SpeedBrake;
            chkSysTest.Checked = (lightBits3 & LightBits3.SysTest) == LightBits3.SysTest;
            chkMCAnnounced.Checked = (lightBits3 & LightBits3.MCAnnounced) == LightBits3.MCAnnounced;
            chkMLGWOW.Checked = (lightBits3 & LightBits3.MLGWOW) == LightBits3.MLGWOW;
            chkNLGWOW.Checked = (lightBits3 & LightBits3.NLGWOW) == LightBits3.NLGWOW;
            chkATF_Not_Engaged.Checked = (lightBits3 & LightBits3.ATF_Not_Engaged) == LightBits3.ATF_Not_Engaged;
            chkInlet_Icing.Checked = (lightBits3 & LightBits3.Inlet_Icing) == LightBits3.Inlet_Icing;
        }
        private void BindHsiBitsToFormElements()
        {
            var hsiBits = (HsiBits)_lastFlightData.hsiBits;
            chkToTrue.Checked = (hsiBits & HsiBits.ToTrue) == HsiBits.ToTrue;
            chkIlsWarning.Checked = (hsiBits & HsiBits.IlsWarning) == HsiBits.IlsWarning;
            chkCourseWarning.Checked = (hsiBits & HsiBits.CourseWarning) == HsiBits.CourseWarning;
            chkInit.Checked = (hsiBits & HsiBits.Init) == HsiBits.Init;
            chkTotalFlags.Checked = (hsiBits & HsiBits.TotalFlags) == HsiBits.TotalFlags;
            chkADI_OFF.Checked = (hsiBits & HsiBits.ADI_OFF) == HsiBits.ADI_OFF;
            chkADI_AUX.Checked = (hsiBits & HsiBits.ADI_AUX) == HsiBits.ADI_AUX;
            chkADI_GS.Checked = (hsiBits & HsiBits.ADI_GS) == HsiBits.ADI_GS;
            chkADI_LOC.Checked = (hsiBits & HsiBits.ADI_LOC) == HsiBits.ADI_LOC;
            chkHSI_OFF.Checked = (hsiBits & HsiBits.HSI_OFF) == HsiBits.HSI_OFF;
            chkBUP_ADI_OFF.Checked = (hsiBits & HsiBits.BUP_ADI_OFF) == HsiBits.BUP_ADI_OFF;
            chkVVI.Checked = (hsiBits & HsiBits.VVI) == HsiBits.VVI;
            chkAOA.Checked = (hsiBits & HsiBits.AOA) == HsiBits.AOA;
            chkAVTR.Checked = (hsiBits & HsiBits.AVTR) == HsiBits.AVTR;
            chkOuterMarker.Checked = (hsiBits & HsiBits.OuterMarker) == HsiBits.OuterMarker;
            chkMiddleMarker.Checked = (hsiBits & HsiBits.MiddleMarker) == HsiBits.MiddleMarker;
            chkFromTrue.Checked = (hsiBits & HsiBits.FromTrue) == HsiBits.FromTrue;
            chkFlying.Checked = (hsiBits & HsiBits.Flying) == HsiBits.Flying;
        }
        private void BindFDVarsToFormElements()
        {
            txtVersionNum.Text = _lastFlightData.VersionNum.ToString();
            txtX.Text = _lastFlightData.x.FormatDecimal(decimalPlaces: 2);
            txtY.Text = _lastFlightData.y.FormatDecimal(decimalPlaces: 2);
            txtZ.Text = _lastFlightData.z.FormatDecimal(decimalPlaces: 2);
            txtXDot.Text = _lastFlightData.xDot.FormatDecimal(decimalPlaces: 2);
            txtYDot.Text = _lastFlightData.yDot.FormatDecimal(decimalPlaces: 2);
            txtZDot.Text = _lastFlightData.zDot.FormatDecimal(decimalPlaces: 2);
            txtAlpha.Text = _lastFlightData.alpha.FormatDecimal(decimalPlaces: 2);
            txtBeta.Text = _lastFlightData.beta.FormatDecimal(decimalPlaces: 2);
            txtGamma.Text = _lastFlightData.gamma.FormatDecimal(decimalPlaces: 2);
            txtPitch.Text = _lastFlightData.pitch.FormatDecimal(decimalPlaces: 2);
            txtRoll.Text = _lastFlightData.roll.FormatDecimal(decimalPlaces: 2);
            txtYaw.Text = _lastFlightData.yaw.FormatDecimal(decimalPlaces: 2);
            txtMach.Text = _lastFlightData.mach.FormatDecimal(decimalPlaces: 2);
            txtKias.Text = _lastFlightData.kias.FormatDecimal(decimalPlaces: 2);
            txtVt.Text = _lastFlightData.vt.FormatDecimal(decimalPlaces: 2);
            txtGs.Text = _lastFlightData.gs.FormatDecimal(decimalPlaces: 2);
            txtWindOffset.Text = _lastFlightData.windOffset.FormatDecimal(decimalPlaces: 2);
            txtTrimPitch.Text = _lastFlightData.TrimPitch.FormatDecimal(decimalPlaces: 2);
            txtTrimRoll.Text = _lastFlightData.TrimRoll.FormatDecimal(decimalPlaces: 2);
            txtTrimYaw.Text = _lastFlightData.TrimYaw.FormatDecimal(decimalPlaces: 2);
            txtNozzlePos.Text = _lastFlightData.nozzlePos.FormatDecimal(decimalPlaces: 2);
            txtRpm.Text = _lastFlightData.rpm.FormatDecimal(decimalPlaces: 2);
            txtFtit.Text = _lastFlightData.ftit.FormatDecimal(decimalPlaces: 2);
            txtOilPressure.Text = _lastFlightData.oilPressure.FormatDecimal(decimalPlaces: 2);
            txtInternalFuel.Text = _lastFlightData.internalFuel.FormatDecimal(decimalPlaces: 2);
            txtExternalFuel.Text = _lastFlightData.externalFuel.FormatDecimal(decimalPlaces: 2);
            txtFuelFlow.Text = _lastFlightData.fuelFlow.FormatDecimal(decimalPlaces: 2);
            txtFwd.Text = _lastFlightData.fwd.FormatDecimal(decimalPlaces: 2);
            txtAft.Text = _lastFlightData.aft.FormatDecimal(decimalPlaces: 2);
            txtTotal.Text = _lastFlightData.total.FormatDecimal(decimalPlaces: 2);
            txtEpuFuel.Text = _lastFlightData.epuFuel.FormatDecimal(decimalPlaces: 2);
            txtSpeedBrake.Text = _lastFlightData.speedBrake.FormatDecimal(decimalPlaces: 2);
            txtChaffCount.Text = _lastFlightData.ChaffCount.FormatDecimal(decimalPlaces: 2);
            txtFlareCount.Text = _lastFlightData.FlareCount.FormatDecimal(decimalPlaces: 2);
            txtGearPos.Text = _lastFlightData.gearPos.FormatDecimal(decimalPlaces: 2);
            txtNoseGearPos.Text = _lastFlightData.NoseGearPos.FormatDecimal(decimalPlaces: 2);
            txtLeftGearPos.Text = _lastFlightData.LeftGearPos.FormatDecimal(decimalPlaces: 2);
            txtRightGearPos.Text = _lastFlightData.RightGearPos.FormatDecimal(decimalPlaces: 2);
            txtMainPower.Text = string.Format("({0}) {1}",
                _lastFlightData.MainPower.ToString(),
                ((TriStateSwitchStates)_lastFlightData.MainPower).ToString()
            );
            txtUFCTChan.Text = _lastFlightData.UFCTChan.ToString();
            txtAUXTChan.Text = _lastFlightData.AUXTChan.ToString();
            txtAdiIlsHorPos.Text = _lastFlightData.AdiIlsHorPos.FormatDecimal(decimalPlaces: 2);
            txtAdiIlsVerPos.Text = _lastFlightData.AdiIlsVerPos.FormatDecimal(decimalPlaces: 2);
            txtCourseState.Text = _lastFlightData.courseState.ToString();
            txtHeadingState.Text = _lastFlightData.headingState.ToString();
            txtTotalStates.Text = _lastFlightData.totalStates.ToString();
            txtCourseDeviation.Text = _lastFlightData.courseDeviation.FormatDecimal(decimalPlaces: 2);
            txtDesiredCourse.Text = _lastFlightData.desiredCourse.FormatDecimal(decimalPlaces: 2);
            txtDistanceToBeacon.Text = _lastFlightData.distanceToBeacon.FormatDecimal(decimalPlaces: 2);
            txtBearingToBeacon.Text = _lastFlightData.bearingToBeacon.FormatDecimal(decimalPlaces: 2);
            txtCurrentHeading.Text = _lastFlightData.currentHeading.FormatDecimal(decimalPlaces: 2);
            txtDesiredHeading.Text = _lastFlightData.desiredHeading.FormatDecimal(decimalPlaces: 2);
            txtDeviationLimit.Text = _lastFlightData.deviationLimit.FormatDecimal(decimalPlaces: 2);
            txtHalfDeviationLimit.Text = _lastFlightData.halfDeviationLimit.FormatDecimal(decimalPlaces: 2);
            txtLocalizerCourse.Text = _lastFlightData.localizerCourse.FormatDecimal(decimalPlaces: 2);
            txtAirbaseX.Text = _lastFlightData.airbaseX.FormatDecimal(decimalPlaces: 2);
            txtAirbaseY.Text = _lastFlightData.airbaseY.FormatDecimal(decimalPlaces: 2);
            txtTotalValues.Text = _lastFlightData.totalValues.FormatDecimal(decimalPlaces: 2);
        }

        private void BindRWRToFormElements()
        {
            txtRwrObjectCount.Text = _lastFlightData.RwrObjectCount.ToString();
            gvRWR.SuspendLayout();
            if (gvRWR.RowCount > _lastFlightData.RWRsymbol.Length)
            {
                gvRWR.Rows.RemoveAt(gvRWR.RowCount - 1);
            }
            while (gvRWR.Rows.Count < _lastFlightData.RWRsymbol.Length)
            {
                gvRWR.Rows.Add();
                gvRWR.RowHeadersVisible = true;
            }
            for (var i = 0; i < _lastFlightData.RWRsymbol.Length; i++)
            {
                var row = gvRWR.Rows[i];
                row.HeaderCell.Value = (i + 1).ToString();
                row.Cells[0].Value = _lastFlightData.RWRsymbol[i];
                row.Cells[1].Value = _lastFlightData.bearing[i].FormatDecimal(decimalPlaces: 2);
                row.Cells[2].Value = _lastFlightData.missileActivity[i];
                row.Cells[3].Value = _lastFlightData.missileLaunch[i];
                row.Cells[4].Value = _lastFlightData.selected[i];
                row.Cells[5].Value = _lastFlightData.lethality[i].FormatDecimal(decimalPlaces: 2);
                row.Cells[6].Value = _lastFlightData.newDetection[i];
                row.Cells[7].Value = $"({ (int)_lastFlightData.RWRjammingStatus[i]}) { _lastFlightData.RWRjammingStatus[i].ToString().Replace("JAMMED_", "")}";
            }
            gvRWR.CurrentCell = null;
            gvRWR.ResumeLayout();
        }
        private void BindDEDToFormElements()
        {
            BindDEDorPFLToFormElements(_lastFlightData.DEDLines, _lastFlightData.Invert, gvDED);
        }
        private void BindPFLToFormElements()
        {
            BindDEDorPFLToFormElements(_lastFlightData.PFLLines, _lastFlightData.PFLInvert, gvPFL);
        }
        private void BindDEDorPFLToFormElements(string[] lines, string[] invert, DataGridView dataGridView)
        {
            dataGridView.SuspendLayout();
            if (dataGridView.RowCount > lines.Length)
            {
                dataGridView.Rows.RemoveAt(dataGridView.RowCount - 1);
            }
            while (dataGridView.RowCount < lines.Length)
            {
                dataGridView.Rows.Add();
            }
            for (var i = 0; i < lines.Length; i++)
            {
                var thisLineBytes = Encoding.Default.GetBytes(lines[i] ?? string.Empty);
                var thisLineInvertBytes = Encoding.Default.GetBytes(invert[i] ?? string.Empty);

                dataGridView.Rows[i].HeaderCell.Value = (i + 1).ToString();
                for (var j = 0; j < lines[i].Length; j++)
                {
                    var thisByte = thisLineBytes[j];
                    var thisChar = (char)thisByte;
                    if (thisByte == 0x01) thisChar = '\u2195';
                    if (thisByte == 0x02) thisChar = '*';
                    if (thisByte == 0x5E) thisChar = '\u00B0'; //degree symbol
                    var thisByteInvert = thisLineInvertBytes.Length > j ? (byte)thisLineInvertBytes[j] : (byte)0;
                    var inverted = thisByteInvert > 0 && thisByteInvert != 32;
                    dataGridView.Rows[i].Cells[j].Value = thisChar;

                    if (inverted)
                    {
                        dataGridView.Rows[i].Cells[j].Style.ForeColor = Color.White;
                        dataGridView.Rows[i].Cells[j].Style.BackColor = Color.Black;
                    }
                    else
                    {
                        dataGridView.Rows[i].Cells[j].Style.ForeColor = Color.Black;
                        dataGridView.Rows[i].Cells[j].Style.BackColor = Color.White;
                    }
                }
            }
            dataGridView.CurrentCell = null;
            dataGridView.ResumeLayout();
        }
        private void BindFD2VarsToFormElements()
        {
            chkLB_OuterMarker.Checked = (((HsiBits)_lastFlightData.hsiBits) & HsiBits.OuterMarker) == HsiBits.OuterMarker;
            chkBB_OuterMarker.Checked = (((BlinkBits)_lastFlightData.blinkBits) & BlinkBits.OuterMarker) == BlinkBits.OuterMarker;
            chkLB_MiddleMarker.Checked = (((HsiBits)_lastFlightData.hsiBits) & HsiBits.MiddleMarker) == HsiBits.MiddleMarker;
            chkBB_MiddleMarker.Checked = (((BlinkBits)_lastFlightData.blinkBits) & BlinkBits.MiddleMarker) == BlinkBits.MiddleMarker;
            chkLB_PROBEHEAT.Checked = (((LightBits2)_lastFlightData.lightBits2) & LightBits2.PROBEHEAT) == LightBits2.PROBEHEAT;
            chkBB_PROBEHEAT.Checked = (((BlinkBits)_lastFlightData.blinkBits) & BlinkBits.PROBEHEAT) == BlinkBits.PROBEHEAT;
            chkLB_AuxSrch.Checked = (((LightBits2)_lastFlightData.lightBits2) & LightBits2.AuxSrch) == LightBits2.AuxSrch;
            chkBB_AuxSrch.Checked = (((BlinkBits)_lastFlightData.blinkBits) & BlinkBits.AuxSrch) == BlinkBits.AuxSrch;
            chkLB_Launch.Checked = (((LightBits2)_lastFlightData.lightBits2) & LightBits2.Launch) == LightBits2.Launch;
            chkBB_Launch.Checked = (((BlinkBits)_lastFlightData.blinkBits) & BlinkBits.Launch) == BlinkBits.Launch;
            chkLB_PriMode.Checked = (((LightBits2)_lastFlightData.lightBits2) & LightBits2.PriMode) == LightBits2.PriMode;
            chkBB_PriMode.Checked = (((BlinkBits)_lastFlightData.blinkBits) & BlinkBits.PriMode) == BlinkBits.PriMode;
            chkLB_Unk.Checked = (((LightBits2)_lastFlightData.lightBits2) & LightBits2.Unk) == LightBits2.Unk;
            chkBB_Unk.Checked = (((BlinkBits)_lastFlightData.blinkBits) & BlinkBits.Unk) == BlinkBits.Unk;
            chkLB_Elec_Fault.Checked = (((LightBits3)_lastFlightData.lightBits3) & LightBits3.Elec_Fault) == LightBits3.Elec_Fault;
            chkBB_Elec_Fault.Checked = (((BlinkBits)_lastFlightData.blinkBits) & BlinkBits.Elec_Fault) == BlinkBits.Elec_Fault;
            chkLB_OXY_BROW.Checked = (((LightBits)_lastFlightData.lightBits) & LightBits.OXY_BROW) == LightBits.OXY_BROW;
            chkBB_OXY_BROW.Checked = (((BlinkBits)_lastFlightData.blinkBits) & BlinkBits.OXY_BROW) == BlinkBits.OXY_BROW;
            chkLB_EPUOn.Checked = (((LightBits2)_lastFlightData.lightBits2) & LightBits2.EPUOn) == LightBits2.EPUOn;
            chkBB_EPUOn.Checked = (((BlinkBits)_lastFlightData.blinkBits) & BlinkBits.EPUOn) == BlinkBits.EPUOn;
            chkLB_JFSOn_Slow.Checked = (((LightBits2)_lastFlightData.lightBits2) & LightBits2.JFSOn) == LightBits2.JFSOn;
            chkBB_JFSOn_Slow.Checked = (((BlinkBits)_lastFlightData.blinkBits) & BlinkBits.JFSOn_Slow) == BlinkBits.JFSOn_Slow;
            chkLB_JFSOn_Fast.Checked = (((LightBits2)_lastFlightData.lightBits2) & LightBits2.JFSOn) == LightBits2.JFSOn;
            chkBB_JFSOn_Fast.Checked = (((BlinkBits)_lastFlightData.blinkBits) & BlinkBits.JFSOn_Fast) == BlinkBits.JFSOn_Fast;
            chkBusPowerBattery.Checked = (((PowerBits)_lastFlightData.powerBits) & PowerBits.BusPowerBattery) == PowerBits.BusPowerBattery;
            chkBusPowerEmergency.Checked = (((PowerBits)_lastFlightData.powerBits) & PowerBits.BusPowerEmergency) == PowerBits.BusPowerEmergency;
            chkBusPowerEssential.Checked = (((PowerBits)_lastFlightData.powerBits) & PowerBits.BusPowerEssential) == PowerBits.BusPowerEssential;
            chkBusPowerNonEssential.Checked = (((PowerBits)_lastFlightData.powerBits) & PowerBits.BusPowerNonEssential) == PowerBits.BusPowerNonEssential;
            chkMainGenerator.Checked = (((PowerBits)_lastFlightData.powerBits) & PowerBits.MainGenerator) == PowerBits.MainGenerator;
            chkStandbyGenerator.Checked = (((PowerBits)_lastFlightData.powerBits) & PowerBits.StandbyGenerator) == PowerBits.StandbyGenerator;
            chkJetFuelStarter.Checked = (((PowerBits)_lastFlightData.powerBits) & PowerBits.JetFuelStarter) == PowerBits.JetFuelStarter;

            chkUfcTacanIsAA.Checked = _lastFlightData.UfcTacanIsAA;
            chkUfcTacanIsX.Checked = _lastFlightData.UfcTacanIsX;
            chkAuxTacanIsAA.Checked = _lastFlightData.AuxTacanIsAA;
            chkAuxTacanIsX.Checked = _lastFlightData.AuxTacanIsX;

            chkCalType.Checked = (((AltBits)_lastFlightData.altBits) & AltBits.CalType) == AltBits.CalType;
            chkPneuFlag.Checked = (((AltBits)_lastFlightData.altBits) & AltBits.PneuFlag) == AltBits.PneuFlag;

            chkRALT_Valid.Checked = (((MiscBits)_lastFlightData.miscBits) & MiscBits.RALT_Valid) == MiscBits.RALT_Valid;
            chkFlcs_flcc_A.Checked = (((MiscBits)_lastFlightData.miscBits) & MiscBits.Flcs_Flcc_A) == MiscBits.Flcs_Flcc_A;
            chkFlcs_flcc_B.Checked = (((MiscBits)_lastFlightData.miscBits) & MiscBits.Flcs_Flcc_B) == MiscBits.Flcs_Flcc_B;
            chkFlcs_flcc_C.Checked = (((MiscBits)_lastFlightData.miscBits) & MiscBits.Flcs_Flcc_C) == MiscBits.Flcs_Flcc_C;
            chkFlcs_flcc_D.Checked = (((MiscBits)_lastFlightData.miscBits) & MiscBits.Flcs_Flcc_D) == MiscBits.Flcs_Flcc_D;
            chkSolenoidStatus.Checked = (((MiscBits)_lastFlightData.miscBits) & MiscBits.SolenoidStatus) == MiscBits.SolenoidStatus;

            chkAllWords.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_Allwords) == BettyBits.Betty_Allwords;
            chkPullup.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_Pullup) == BettyBits.Betty_Pullup;
            chkAltitude.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_Altitude) == BettyBits.Betty_Altitude;
            chkWarning.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_Warning) == BettyBits.Betty_Warning;
            chkJammer.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_Jammer) == BettyBits.Betty_Jammer;
            chkCounter.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_Counter) == BettyBits.Betty_Counter;
            chkChaffFlare.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_ChaffFlare) == BettyBits.Betty_ChaffFlare;
            chkChaffFlare_Low.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_ChaffFlare_Low) == BettyBits.Betty_ChaffFlare_Low;
            chkChaffFlareOut.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_ChaffFlare_Out) == BettyBits.Betty_ChaffFlare_Out;
            chkLock.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_Lock) == BettyBits.Betty_Lock;
            chkCaution.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_Caution) == BettyBits.Betty_Caution;
            chkBingo.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_Bingo) == BettyBits.Betty_Bingo;
            chkData.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_Data) == BettyBits.Betty_Data;
            chkBettyIFF.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_IFF) == BettyBits.Betty_IFF;
            chkLowSpeed.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_Lowspeed) == BettyBits.Betty_Lowspeed;
            chkBeeps.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_Beeps) == BettyBits.Betty_Beeps;
            chkAOA.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_AOA) == BettyBits.Betty_AOA;
            chkMaxG.Checked = (((BettyBits)_lastFlightData.bettyBits) & BettyBits.Betty_MaxG) == BettyBits.Betty_MaxG;

            txtFD2_VersionNum.Text = _lastFlightData.VersionNum2.ToString();
            txtNozzlePos2.Text = _lastFlightData.nozzlePos2.FormatDecimal(decimalPlaces: 2);
            txtRpm2.Text = _lastFlightData.rpm2.FormatDecimal(decimalPlaces: 2);
            txtFtit2.Text = _lastFlightData.ftit2.FormatDecimal(decimalPlaces: 2);
            txtOilPressure2.Text = _lastFlightData.oilPressure2.FormatDecimal(decimalPlaces: 2);
            txtHydPressureA.Text = _lastFlightData.hydPressureA.FormatDecimal(decimalPlaces: 2);
            txtHydPressureB.Text = _lastFlightData.hydPressureB.FormatDecimal(decimalPlaces: 2);
            txtFuelFlow2.Text = _lastFlightData.fuelFlow2.FormatDecimal(decimalPlaces: 2);
            txtLefPos.Text = _lastFlightData.lefPos.FormatDecimal(decimalPlaces: 2);
            txtTefPos.Text = _lastFlightData.tefPos.FormatDecimal(decimalPlaces: 2);
            txtLatitude.Text = _lastFlightData.latitude.FormatDecimal(decimalPlaces: 4).PadLeft(8, '0');
            txtLongitude.Text = _lastFlightData.longitude.FormatDecimal(decimalPlaces: 4).PadLeft(8, '0');
            txtRALT.Text = _lastFlightData.RALT.FormatDecimal(decimalPlaces: 2);
            txtStringAreaSize.Text = _lastFlightData.StringAreaSize.ToString();
            txtStringAreaTime.Text = _lastFlightData.StringAreaTime.ToString();



            txtNavMode.Text = string.Format("({0}) {1}",
                _lastFlightData.navMode.ToString(),
                ((NavModes)_lastFlightData.navMode).ToString()
            );
            txtCmdsMode.Text = string.Format("({0}) {1}",
                _lastFlightData.cmdsMode.ToString(),
                ((CmdsModes)_lastFlightData.cmdsMode).ToString()
            );
            txtAAUZ.Text = _lastFlightData.aauz.FormatDecimal(decimalPlaces: 2);
            txtAltCalReading.Text = _lastFlightData.AltCalReading.ToString();
            txtCabinAlt.Text = _lastFlightData.cabinAlt.FormatDecimal(decimalPlaces: 2);
            txtBupUhfPreset.Text = _lastFlightData.BupUhfPreset.ToString();
            txtBupUhfFreq.Text = _lastFlightData.BupUhfFreq.ToString();
            txtCurrentTime.Text = _lastFlightData.currentTime.ToString();
            txtVehicleACD.Text = _lastFlightData.vehicleACD.ToString();
            txtVtolPos.Text = _lastFlightData.vtolPos.FormatDecimal(decimalPlaces: 2);
            txtBumpIntensity.Text = _lastFlightData.bumpIntensity.FormatDecimal(decimalPlaces: 4);
            txtInstrLight.Text = string.Format("({0}) {1}",
                _lastFlightData.instrLight.ToString(),
                ((InstrLight)_lastFlightData.instrLight).ToString().Replace("INSTR_LIGHT_", "")
            );
            txtCaraAlow.Text = _lastFlightData.caraAlow.FormatDecimal(decimalPlaces: 2);
            txtBMSVersion.Text = string.Format("{0}.{1}.{2}.{3}",
                _lastFlightData.BMSVersionMajor.ToString(),
                _lastFlightData.BMSVersionMinor.ToString(),
                _lastFlightData.BMSVersionMicro.ToString(),
                _lastFlightData.BMSBuildNumber.ToString());
            txtDrawingAreaSize.Text = _lastFlightData.DrawingAreaSize.ToString();





            txtRTT_size.Text = string.Format("{0} {1}",
                _lastFlightData.RTT_size[0].ToString(),
                _lastFlightData.RTT_size[1].ToString()
            );
            txtRTT_area0.Text = string.Format("{0} {1} {2} {3}",
                _lastFlightData.RTT_area[0].ToString(),
                _lastFlightData.RTT_area[1].ToString(),
                _lastFlightData.RTT_area[2].ToString(),
                _lastFlightData.RTT_area[3].ToString()
            );
            txtRTT_area1.Text = string.Format("{0} {1} {2} {3}",
                _lastFlightData.RTT_area[4].ToString(),
                _lastFlightData.RTT_area[5].ToString(),
                _lastFlightData.RTT_area[6].ToString(),
                _lastFlightData.RTT_area[7].ToString()
            );
            txtRTT_area2.Text = string.Format("{0} {1} {2} {3}",
                _lastFlightData.RTT_area[8].ToString(),
                _lastFlightData.RTT_area[9].ToString(),
                _lastFlightData.RTT_area[10].ToString(),
                _lastFlightData.RTT_area[11].ToString()
            );
            txtRTT_area3.Text = string.Format("{0} {1} {2} {3}",
                _lastFlightData.RTT_area[12].ToString(),
                _lastFlightData.RTT_area[13].ToString(),
                _lastFlightData.RTT_area[14].ToString(),
                _lastFlightData.RTT_area[15].ToString()
            );
            txtRTT_area4.Text = string.Format("{0} {1} {2} {3}",
                _lastFlightData.RTT_area[16].ToString(),
                _lastFlightData.RTT_area[17].ToString(),
                _lastFlightData.RTT_area[18].ToString(),
                _lastFlightData.RTT_area[19].ToString()
            );
            txtRTT_area5.Text = string.Format("{0} {1} {2} {3}",
                _lastFlightData.RTT_area[20].ToString(),
                _lastFlightData.RTT_area[21].ToString(),
                _lastFlightData.RTT_area[22].ToString(),
                _lastFlightData.RTT_area[23].ToString()
            );
            txtRTT_area6.Text = string.Format("{0} {1} {2} {3}",
                _lastFlightData.RTT_area[24].ToString(),
                _lastFlightData.RTT_area[25].ToString(),
                _lastFlightData.RTT_area[26].ToString(),
                _lastFlightData.RTT_area[27].ToString()
            );
            txtIFF_BackupModeDigits.Text = string.Format("{0} {1} {2} {3}",
                ((byte)(_lastFlightData.iffBackupMode1Digit1)).ToString(),
                ((byte)(_lastFlightData.iffBackupMode1Digit2)).ToString(),
                ((byte)(_lastFlightData.iffBackupMode3ADigit1)).ToString(),
                ((byte)(_lastFlightData.iffBackupMode3ADigit2)).ToString()
            );

            txtBingoFuel.Text = _lastFlightData.bingoFuel.FormatDecimal(decimalPlaces: 2);
            txtBullseyeX.Text = _lastFlightData.bullseyeX.FormatDecimal(decimalPlaces: 2);
            txtBullseyeY.Text = _lastFlightData.bullseyeY.FormatDecimal(decimalPlaces: 2);
            txtTurnRate.Text = _lastFlightData.turnRate.FormatDecimal(decimalPlaces: 2);

            txtFloodConsole.Text = string.Format("({0}) {1}",
                ((int)_lastFlightData.floodConsole).ToString(),
                _lastFlightData.floodConsole.ToString().Replace("FLOOD_CONSOLE_", ""));

            txtMagDeviationSystem.Text = _lastFlightData.magDeviationSystem.FormatDecimal(decimalPlaces: 2);
            txtMagDeviationReal.Text = _lastFlightData.magDeviationReal.FormatDecimal(decimalPlaces: 2);

            txtECMBits0.Text =  _lastFlightData.ecmOper == EcmOperStates.ECM_OPER_NO_LIT ? "(invalid)" :
                string.Format("({0}) {1}",
                _lastFlightData.ecmBits[0].ToString(),
                ((EcmBits)_lastFlightData.ecmBits[0]).ToString());

            txtECMBits1.Text = _lastFlightData.ecmOper == EcmOperStates.ECM_OPER_NO_LIT ? "(invalid)" : 
                string.Format("({0}) {1}",
                _lastFlightData.ecmBits[1].ToString(),
                ((EcmBits)_lastFlightData.ecmBits[1]).ToString());

            txtECMBits2.Text = _lastFlightData.ecmOper == EcmOperStates.ECM_OPER_NO_LIT ? "(invalid)" : 
                string.Format("({0}) {1}",
                _lastFlightData.ecmBits[2].ToString(),
                ((EcmBits)_lastFlightData.ecmBits[2]).ToString());

            txtECMBits3.Text = _lastFlightData.ecmOper == EcmOperStates.ECM_OPER_NO_LIT ? "(invalid)" : 
                string.Format("({0}) {1}",
                _lastFlightData.ecmBits[3].ToString(),
                ((EcmBits)_lastFlightData.ecmBits[3]).ToString());

            txtECMBits4.Text = _lastFlightData.ecmOper == EcmOperStates.ECM_OPER_NO_LIT ? "(invalid)" : 
                string.Format("({0}) {1}",
                _lastFlightData.ecmBits[4].ToString(),
                ((EcmBits)_lastFlightData.ecmBits[4]).ToString());

            txtECMOper.Text = string.Format("({0}) {1}",
                ((int)_lastFlightData.ecmOper).ToString(),
                (_lastFlightData.ecmOper).ToString());

            chkLB_ECM_Opr.Checked = _lastFlightData.ecmOper != EcmOperStates.ECM_OPER_NO_LIT;
            chkBB_ECM_Opr.Checked = (((BlinkBits)_lastFlightData.blinkBits) & BlinkBits.ECM_Oper) == BlinkBits.ECM_Oper;


        }

        private void BindFD2RWRInfoToFormElements()
        {
            var dataGridView = gvRwrInfo;
            if (_lastFlightData == null || _lastFlightData.RwrInfo == null)
            {
                return;
            }
            dataGridView.SuspendLayout();
            if (dataGridView.RowCount > 52)
            {
                dataGridView.Rows.RemoveAt(dataGridView.RowCount - 1);
            }
            while (dataGridView.RowCount < 52)
            {
                dataGridView.Rows.Add();
            }
            for (var i = 0; i < 52; i++)
            {
                var rowNumberHex = i.ToString() + ("x");
                if (rowNumberHex.Length < 3)
                {
                    rowNumberHex = new String('0', 3 - rowNumberHex.Length) + rowNumberHex;
                }
                dataGridView.Rows[i].HeaderCell.Value = rowNumberHex;
                byte[] thisLineBytes;
                if (i < 51)
                {
                    thisLineBytes = _lastFlightData.RwrInfo.Skip(i * 10).Take(10).ToArray();
                }
                else
                {
                    thisLineBytes = _lastFlightData.RwrInfo.Skip(i * 10).Take(2).ToArray();
                    for (var n = 2; n < 10; n++)
                    {
                        dataGridView.Rows[i].Cells[n].Style.BackColor = SystemColors.ControlDark;
                    }
                }
                var thisLineString = Encoding.Default.GetString(thisLineBytes);

                for (var j = 0; j < thisLineBytes.Length; j++)
                {
                    dataGridView.Rows[i].Cells[j].Value = thisLineString.Substring(j, 1);
                }
            }
            dataGridView.CurrentCell = null;
            dataGridView.ResumeLayout();
        }
        private void BindFD2PilotInfoToFormElements()
        {
            if (_lastFlightData == null || _lastFlightData.pilotsCallsign == null)
            {
                return;
            }
            txtPilotsInSession.Text = _lastFlightData.pilotsOnline.ToString();
            var dataGridView = gvPilotsInSession;
            
            dataGridView.SuspendLayout();
            if (dataGridView.RowCount > _lastFlightData.pilotsCallsign.Length)
            {
                dataGridView.Rows.RemoveAt(dataGridView.RowCount - 1);
            }
            while (dataGridView.RowCount < _lastFlightData.pilotsCallsign.Length)
            {
                dataGridView.Rows.Add();
            }
            for (var i = 0; i < _lastFlightData.pilotsCallsign.Length; i++)
            {
                dataGridView.Rows[i].HeaderCell.Value = (i + 1).ToString();
                dataGridView.Rows[i].Cells[0].Value = _lastFlightData.pilotsOnline > i ? _lastFlightData.pilotsCallsign[i] : string.Empty;
                dataGridView.Rows[i].Cells[1].Value = _lastFlightData.pilotsOnline > i ? ((FlyStates)_lastFlightData.pilotsStatus[i]).ToString() : string.Empty;
            }
            dataGridView.CurrentCell = null;
            dataGridView.ResumeLayout();

        }
        private void BindOSBDataToFormElements()
        {
            BindOSBDataToFormElements(_lastFlightData.leftMFD, gvLeftMfd);
            BindOSBDataToFormElements(_lastFlightData.rightMFD, gvRightMfd);
        }
        private void BindOSBDataToFormElements(F4SharedMem.FlightData.OptionSelectButtonLabel[] buttonLabels, DataGridView dataGridView)
        {
            if (buttonLabels == null)
            {
                return;
            }
            dataGridView.SuspendLayout();
            if (dataGridView.RowCount > buttonLabels.Length)
            {
                dataGridView.Rows.RemoveAt(dataGridView.RowCount - 1);
            }
            while (dataGridView.RowCount < buttonLabels.Length)
            {
                dataGridView.Rows.Add();
            }
            for (var i = 0; i < buttonLabels.Length; i++)
            {
                var rowNum = (i + 1).ToString();
                if (rowNum.Length < 2) rowNum = "0" + rowNum;
                dataGridView.Rows[i].HeaderCell.Value = rowNum;
                dataGridView.Rows[i].Cells[0].Value = buttonLabels[i].Line1 ?? string.Empty;
                dataGridView.Rows[i].Cells[1].Value = buttonLabels[i].Line2 ?? string.Empty;
                if (buttonLabels[i].Inverted)
                {
                    dataGridView.Rows[i].Cells[0].Style.BackColor = Color.Black;
                    dataGridView.Rows[i].Cells[0].Style.ForeColor = Color.White;

                    dataGridView.Rows[i].Cells[1].Style.BackColor = Color.Black;
                    dataGridView.Rows[i].Cells[1].Style.ForeColor = Color.White;

                }
                else
                {
                    dataGridView.Rows[i].Cells[0].Style.BackColor = Color.White;
                    dataGridView.Rows[i].Cells[0].Style.ForeColor = Color.Black;

                    dataGridView.Rows[i].Cells[1].Style.BackColor = Color.White;
                    dataGridView.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                }
            }
            dataGridView.CurrentCell = null;
            dataGridView.ResumeLayout();
        }
        private void BindIntelliVibeDataToFormElements()
        {
            chkIsFiringGun.Checked = _lastFlightData.IntellivibeData.IsFiringGun;
            chkIsEndFlight.Checked = _lastFlightData.IntellivibeData.IsEndFlight;
            chkIsEjecting.Checked = _lastFlightData.IntellivibeData.IsEjecting;
            chkIn3D.Checked = _lastFlightData.IntellivibeData.In3D;
            chkIsPaused.Checked = _lastFlightData.IntellivibeData.IsPaused;
            chkIsFrozen.Checked = _lastFlightData.IntellivibeData.IsFrozen;
            chkIsOverG.Checked = _lastFlightData.IntellivibeData.IsOverG;
            chkIsOnGround.Checked = _lastFlightData.IntellivibeData.IsOnGround;
            chkIsExitGame.Checked = _lastFlightData.IntellivibeData.IsExitGame;
            txtAAMissileFired.Text = _lastFlightData.IntellivibeData.AAMissileFired.ToString();
            txtAGMissileFired.Text = _lastFlightData.IntellivibeData.AGMissileFired.ToString();
            txtBombDropped.Text = _lastFlightData.IntellivibeData.BombDropped.ToString();
            txtFlareDropped.Text = _lastFlightData.IntellivibeData.FlareDropped.ToString();
            txtChaffDropped.Text = _lastFlightData.IntellivibeData.ChaffDropped.ToString();
            txtBulletsFired.Text = _lastFlightData.IntellivibeData.BulletsFired.ToString();
            txtCollisionCounter.Text = _lastFlightData.IntellivibeData.CollisionCounter.ToString();
            txtGForce.Text = _lastFlightData.IntellivibeData.Gforce.FormatDecimal(decimalPlaces: 2);
            txtLastDamage.Text = _lastFlightData.IntellivibeData.lastdamage.ToString();
            txtDamageforce.Text = _lastFlightData.IntellivibeData.damageforce.FormatDecimal(decimalPlaces: 2);
            txtWhenDamage.Text = _lastFlightData.IntellivibeData.whendamage.ToString();
            txtEyeX.Text = _lastFlightData.IntellivibeData.eyex.FormatDecimal(decimalPlaces: 2);
            txtEyeY.Text = _lastFlightData.IntellivibeData.eyey.FormatDecimal(decimalPlaces: 2);
            txtEyeZ.Text = _lastFlightData.IntellivibeData.eyez.FormatDecimal(decimalPlaces: 2);

        }

        private void Bind_IVC_RCS_RCC_DataToFormElements()
        {
            if (_lastFlightData == null || _lastFlightData.RadioClientControlData.Radios ==null)
            {
                return;
            }
            BindRadioClientStatusDataToFormElements();
            BindRadioClientControlDataToFormElements();
        }
        private void BindRadioClientStatusDataToFormElements()
        {
            chkClientActive.Checked = (((ClientFlags)_lastFlightData.RadioClientStatus.ClientFlags) & ClientFlags.ClientActive) == ClientFlags.ClientActive;
            chkConnected.Checked = (((ClientFlags)_lastFlightData.RadioClientStatus.ClientFlags) & ClientFlags.Connected) == ClientFlags.Connected;
            chkConnectionFail.Checked = (((ClientFlags)_lastFlightData.RadioClientStatus.ClientFlags) & ClientFlags.ConnectionFail) == ClientFlags.ConnectionFail;
            chkHostUnknown.Checked = (((ClientFlags)_lastFlightData.RadioClientStatus.ClientFlags) & ClientFlags.HostUnknown) == ClientFlags.HostUnknown;
            chkBadPassword.Checked = (((ClientFlags)_lastFlightData.RadioClientStatus.ClientFlags) & ClientFlags.BadPassword) == ClientFlags.BadPassword;
            chkNoMicrophone.Checked = (((ClientFlags)_lastFlightData.RadioClientStatus.ClientFlags) & ClientFlags.NoMicrophone) == ClientFlags.NoMicrophone;
            chkNoSpeakers.Checked = (((ClientFlags)_lastFlightData.RadioClientStatus.ClientFlags) & ClientFlags.NoSpeakers) == ClientFlags.NoSpeakers;
        }
        private void BindRadioClientControlDataToFormElements()
        {
            txtUhfFrequency.Text = _lastFlightData.RadioClientControlData.Radios[(int)Radios.UHF].Frequency.ToString();
            txtUhfRxVolume.Text = _lastFlightData.RadioClientControlData.Radios[(int)Radios.UHF].RxVolume.ToString();
            chkUhfPttDepressed.Checked = _lastFlightData.RadioClientControlData.Radios[(int)Radios.UHF].PttDepressed;
            chkUhfIsOn.Checked = _lastFlightData.RadioClientControlData.Radios[(int)Radios.UHF].IsOn;

            txtVhfFrequency.Text = _lastFlightData.RadioClientControlData.Radios[(int)Radios.VHF].Frequency.ToString();
            txtVhfRxVolume.Text = _lastFlightData.RadioClientControlData.Radios[(int)Radios.VHF].RxVolume.ToString();
            chkVhfPttDepressed.Checked = _lastFlightData.RadioClientControlData.Radios[(int)Radios.VHF].PttDepressed;
            chkVhfIsOn.Checked = _lastFlightData.RadioClientControlData.Radios[(int)Radios.VHF].IsOn;

            txtGuardFrequency.Text = _lastFlightData.RadioClientControlData.Radios[(int)Radios.GUARD].Frequency.ToString();
            txtGuardRxVolume.Text = _lastFlightData.RadioClientControlData.Radios[(int)Radios.GUARD].RxVolume.ToString();
            chkGuardPttDepressed.Checked = _lastFlightData.RadioClientControlData.Radios[(int)Radios.GUARD].PttDepressed;
            chkGuardIsOn.Checked = _lastFlightData.RadioClientControlData.Radios[(int)Radios.GUARD].IsOn;

            txtNickname.Text = Encoding.Default.GetString(_lastFlightData.RadioClientControlData.Nickname).TrimAtNull();
            txtAddress.Text = Encoding.Default.GetString(_lastFlightData.RadioClientControlData.Address).TrimAtNull();
            txtPortNumber.Text = _lastFlightData.RadioClientControlData.PortNumber.ToString();
            txtPassword.Text = Encoding.Default.GetString(_lastFlightData.RadioClientControlData.Password).TrimAtNull();
            txtPlayerCount.Text = _lastFlightData.RadioClientControlData.PlayerCount.ToString();
            chkSignalConnect.Checked = _lastFlightData.RadioClientControlData.SignalConnect;
            chkTerminateClient.Checked = _lastFlightData.RadioClientControlData.TerminateClient;
            chkFlightMode.Checked = _lastFlightData.RadioClientControlData.FlightMode;
            chkUseAGC.Checked = _lastFlightData.RadioClientControlData.UseAGC;
            txtIcVolume.Text = _lastFlightData.RadioClientControlData.Devices[(int)Devices.MAIN].IcVolume.ToString();
            BindTelemetryDataToFormElements();
        }
        private void BindTelemetryDataToFormElements()
        {
            var dataGridView = gvTelemetry;

            dataGridView.SuspendLayout();
            if (dataGridView.RowCount > _lastFlightData.RadioClientControlData.PlayerMap.Length)
            {
                dataGridView.Rows.RemoveAt(dataGridView.RowCount - 1);
            }
            while (dataGridView.RowCount < _lastFlightData.RadioClientControlData.PlayerMap.Length)
            {
                dataGridView.Rows.Add();
            }
            for (var i = 0; i < _lastFlightData.RadioClientControlData.PlayerMap.Length; i++)
            {
                dataGridView.Rows[i].HeaderCell.Value = (i + 1).ToString();
                var logbookName = Encoding.Default.GetString(_lastFlightData.RadioClientControlData.PlayerMap[i].LogbookName).TrimAtNull();
                dataGridView.Rows[i].Cells[0].Value = logbookName;
                dataGridView.Rows[i].Cells[1].Value = _lastFlightData.RadioClientControlData.PlayerMap[i].Agl.FormatDecimal(decimalPlaces: 2);
                dataGridView.Rows[i].Cells[2].Value = _lastFlightData.RadioClientControlData.PlayerMap[i].Range.FormatDecimal(decimalPlaces: 2);
                var telemetryFlags =_lastFlightData.RadioClientControlData.PlayerMap[i].Flags;
                var telemetryFlagsString = string.Format(
                    "Los({0}) AC({1})",
                        (((TelemetryFlags)telemetryFlags & TelemetryFlags.HasPlayerLoS) == TelemetryFlags.HasPlayerLoS) ? "1" : "0",
                        (((TelemetryFlags)telemetryFlags & TelemetryFlags.IsAircraft) == TelemetryFlags.IsAircraft) ? "1" : "0"
                );
                dataGridView.Rows[i].Cells[3].Value = telemetryFlagsString.ToString();
            }
            dataGridView.CurrentCell = null;
            dataGridView.ResumeLayout();
        }
        private void BindRawBitsToFormElements()
        {
            txtLightBitsUint.Text = ((uint)_lastFlightData.lightBits).ToString();
            txtLightBits2Uint.Text = ((uint)_lastFlightData.lightBits2).ToString();
            txtLightBits3Uint.Text = ((uint)_lastFlightData.lightBits3).ToString();
            txtHsiBitsUint.Text = ((uint)_lastFlightData.hsiBits).ToString();
            txtAltBitsUint.Text = ((uint)_lastFlightData.altBits).ToString();
            txtPowerBitsUint.Text = ((uint)_lastFlightData.powerBits).ToString();
            txtBlinkBitsUint.Text = ((uint)_lastFlightData.blinkBits).ToString();
            txtMiscBitsUint.Text = ((uint)_lastFlightData.miscBits).ToString();
            txtBettyBitsUint.Text = ((uint)_lastFlightData.bettyBits).ToString();

            txtLightBitsHex.Text = "0x" + _lastFlightData.lightBits.ToString("X").PadLeft(8,'0');
            txtLightBits2Hex.Text = "0x" + _lastFlightData.lightBits2.ToString("X").PadLeft(8, '0');
            txtLightBits3Hex.Text = "0x" + _lastFlightData.lightBits3.ToString("X").PadLeft(8, '0');
            txtHsiBitsHex.Text = "0x" + _lastFlightData.hsiBits.ToString("X").PadLeft(8, '0');
            txtAltBitsHex.Text = "0x" + _lastFlightData.altBits.ToString("X").PadLeft(8, '0');
            txtPowerBitsHex.Text = "0x" + _lastFlightData.powerBits.ToString("X").PadLeft(8, '0');
            txtBlinkBitsHex.Text = "0x" + _lastFlightData.blinkBits.ToString("X").PadLeft(8, '0');
            txtMiscBitsHex.Text = "0x" + _lastFlightData.miscBits.ToString("X").PadLeft(8, '0');
            txtBettyBitsHex.Text = "0x" + _lastFlightData.bettyBits.ToString("X").PadLeft(8, '0');
        }
        private uint _lastStringAreaTime;
        private void BindStringsVarsToFormElements()
        {
            txtStringsVersionNum.Text = ((int)_lastFlightData.StringData.VersionNum).ToString();
            txtNoOfStrings.Text = ((int)_lastFlightData.StringData.NoOfStrings).ToString();
            txtStringsDataSize.Text = ((uint)_lastFlightData.StringData.dataSize).ToString();
            if (_lastFlightData.StringAreaTime != _lastStringAreaTime)
            {
                txtStrings.Clear();
                foreach (var thisString in _lastFlightData.StringData.data)
                {
                    txtStrings.AppendText(string.Format("[id {0} ({1}), len {2}] {3}\r\n", thisString.strId, ((F4SharedMem.Headers.StringIdentifier)thisString.strId).ToString(), thisString.strLength, thisString.value));
                }
                _lastStringAreaTime = _lastFlightData.StringAreaTime;
            }

        }
        private void InitializeFD2RWRGridView()
        {
            var toInitialize = gvRwrInfo;
            toInitialize.SuspendLayout();
            for (var i = 0; i < 10; i++)
            {
                var columnTemplate = (DataGridViewColumn)toInitialize.Columns[0].Clone();
                columnTemplate.HeaderText = (i).ToString();
                toInitialize.Columns.Add(columnTemplate);
            }
            toInitialize.Columns.RemoveAt(0);
            toInitialize.ResumeLayout();
        }

        private void InitializeDEDGridView()
        {
            InitializeDEDorPFLGridView(gvDED);
        }
        private void InitializePFLGridView()
        {
            InitializeDEDorPFLGridView(gvPFL);
        }
        private void InitializeDEDorPFLGridView(DataGridView toInitialize)
        {
            toInitialize.SuspendLayout();
            for (var i = 0; i < 26; i++)
            {
                var columnTemplate = (DataGridViewColumn)toInitialize.Columns[0].Clone();
                columnTemplate.HeaderText = (i + 1).ToString();
                toInitialize.Columns.Add(columnTemplate);
            }
            toInitialize.Columns.RemoveAt(0);
            toInitialize.ResumeLayout();
        }

        private void EnableChildControls(Control control, bool enabled=true)
        {
            foreach (var thisControl in control.Controls)
            {
                (thisControl as Control).Enabled = enabled;
            }
        }
        private void DisableChildControls(Control control)
        {
            EnableChildControls(control, false);
        }
        private void SetTabPanelBackgroundColors(TabControl tabControl, Color color)
        {
            foreach (TabPage tabPage in tabControl.TabPages)
            {
                tabPage.BackColor = color;

            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            _timer.Tick -= _timer_Tick;
            _timer.Stop();
            Common.Util.DisposeObject(_sharedMemReader);
            base.OnClosing(e);
        }

    }
        
}
