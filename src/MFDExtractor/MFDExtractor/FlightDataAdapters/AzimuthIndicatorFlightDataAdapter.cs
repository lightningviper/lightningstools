using System;
using Common.Math;
using F4SharedMem;
using F4SharedMem.Headers;
using LightningGauges.Renderers.F16.AzimuthIndicator;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IAzimuthIndicatorFlightDataAdapter
    {
        void Adapt(IAzimuthIndicator azimuthIndicator, FlightData flightData);
    }

    class AzimuthIndicatorFlightDataAdapter : IAzimuthIndicatorFlightDataAdapter
    {
        public void Adapt(IAzimuthIndicator azimuthIndicator, FlightData flightData)
        {
            azimuthIndicator.InstrumentState.MagneticHeadingDegrees = (360 + (flightData.yaw / Common.Math.Constants.RADIANS_PER_DEGREE)) % 360;
            azimuthIndicator.InstrumentState.RollDegrees = ((flightData.roll / Common.Math.Constants.RADIANS_PER_DEGREE));
            var rwrObjectCount = flightData.RwrObjectCount;
            if (flightData.RWRsymbol != null)
            {
                var blips = new Blip[flightData.RWRsymbol.Length];
                azimuthIndicator.InstrumentState.Blips = blips;
                if (flightData.RWRsymbol != null)
                {
                    for (var i = 0; i < flightData.RWRsymbol.Length; i++)
                    {
                        var thisBlip = new Blip();
                        if (i < rwrObjectCount) thisBlip.Visible = true;
                        if (flightData.bearing != null)
                        {
                            thisBlip.BearingDegrees = (flightData.bearing[i] / Common.Math.Constants.RADIANS_PER_DEGREE);
                        }
                        if (flightData.lethality != null)
                        {
                            thisBlip.Lethality = thisBlip.Lethality = 1.0f - flightData.lethality[i];
                        }
                        if (flightData.missileActivity != null)
                        {
                            thisBlip.MissileActivity = flightData.missileActivity[i];
                        }
                        if (flightData.missileLaunch != null)
                        {
                            thisBlip.MissileLaunch = flightData.missileLaunch[i];
                        }
                        if (flightData.newDetection != null)
                        {
                            thisBlip.NewDetection = flightData.newDetection[i];
                        }
                        if (flightData.selected != null)
                        {
                            thisBlip.Selected = flightData.selected[i];
                        }
                        thisBlip.SymbolID = flightData.RWRsymbol[i];
                        blips[i] = thisBlip;
                    }
                }
            }
            azimuthIndicator.InstrumentState.Activity = ((flightData.lightBits2 & (int)LightBits2.AuxAct) == (int)LightBits2.AuxAct);
            azimuthIndicator.InstrumentState.ChaffCount = (int)flightData.ChaffCount;
            azimuthIndicator.InstrumentState.ChaffLow = ((flightData.lightBits2 & (int)LightBits2.ChaffLo) == (int)LightBits2.ChaffLo);
            azimuthIndicator.InstrumentState.EWSDegraded = ((flightData.lightBits2 & (int)LightBits2.Degr) == (int)LightBits2.Degr);
            azimuthIndicator.InstrumentState.EWSDispenseReady = ((flightData.lightBits2 & (int)LightBits2.Rdy) == (int)LightBits2.Rdy);
            azimuthIndicator.InstrumentState.EWSNoGo = (
                ((flightData.lightBits2 & (int)LightBits2.NoGo) == (int)LightBits2.NoGo)
                    ||
                ((flightData.lightBits2 & (int)LightBits2.Degr) == (int)LightBits2.Degr)
                );
            azimuthIndicator.InstrumentState.EWSGo =
                (
                    ((flightData.lightBits2 & (int)LightBits2.Go) == (int)LightBits2.Go)
                        &&
                    !(
                        ((flightData.lightBits2 & (int)LightBits2.NoGo) == (int)LightBits2.NoGo)
                            ||
                        ((flightData.lightBits2 & (int)LightBits2.Degr) == (int)LightBits2.Degr)
                            ||
                        ((flightData.lightBits2 & (int)LightBits2.Rdy) == (int)LightBits2.Rdy)
                        )
                    );


            azimuthIndicator.InstrumentState.FlareCount = (int)flightData.FlareCount;
            azimuthIndicator.InstrumentState.FlareLow = ((flightData.lightBits2 & (int)LightBits2.FlareLo) == (int)LightBits2.FlareLo);
            azimuthIndicator.InstrumentState.Handoff = ((flightData.lightBits2 & (int)LightBits2.HandOff) == (int)LightBits2.HandOff);
            azimuthIndicator.InstrumentState.Launch = ((flightData.lightBits2 & (int)LightBits2.Launch) == (int)LightBits2.Launch);
            azimuthIndicator.InstrumentState.LowAltitudeMode = ((flightData.lightBits2 & (int)LightBits2.AuxLow) == (int)LightBits2.AuxLow);
            azimuthIndicator.InstrumentState.NavalMode = ((flightData.lightBits2 & (int)LightBits2.Naval) == (int)LightBits2.Naval);
            azimuthIndicator.InstrumentState.Other1Count = 0;
            azimuthIndicator.InstrumentState.Other1Low = true;
            azimuthIndicator.InstrumentState.Other2Count = 0;
            azimuthIndicator.InstrumentState.Other2Low = true;
            azimuthIndicator.InstrumentState.cmdsMode = flightData.cmdsMode;
            if (((flightData.powerBits & (int)PowerBits.BusPowerNonEssential) == (int)PowerBits.BusPowerNonEssential) || Extractor.State.OptionsFormIsShowing)
            {
                azimuthIndicator.InstrumentState.RWRPowerOn = ((flightData.lightBits2 & (int)LightBits2.AuxPwr) == (int)LightBits2.AuxPwr) || Extractor.State.OptionsFormIsShowing;
                azimuthIndicator.InstrumentState.RWRTest1 = ((flightData.lightBits3 & (int)LightBits3.SysTest) == (int)LightBits3.SysTest);
                if ((flightData.lightBits3 & (int)LightBits3.SysTest) == (int)LightBits3.SysTest) //Added Falcas 07-11-2012
                {
                    //Set test start time
                    if (azimuthIndicator.InstrumentState.TestStartTime == DateTime.MinValue)
                    {
                        azimuthIndicator.InstrumentState.TestStartTime = DateTime.UtcNow;
                    }

                    DateTime thisTestTime = DateTime.UtcNow;
                    TimeSpan TimeDiff = thisTestTime.Subtract(azimuthIndicator.InstrumentState.TestStartTime);
                    if (TimeDiff >= TimeSpan.FromSeconds(5))
                    {
                        azimuthIndicator.InstrumentState.RWRTest1 = false;
                        azimuthIndicator.InstrumentState.RWRTest2 = true;
                    }
                }
                else
                {
                    //Reset test time.
                    azimuthIndicator.InstrumentState.TestStartTime = DateTime.MinValue;
                    azimuthIndicator.InstrumentState.RWRTest1 = false;
                    azimuthIndicator.InstrumentState.RWRTest2 = false;
                }
            }
            else
            {
                azimuthIndicator.InstrumentState.RWRPowerOn = false;
                azimuthIndicator.InstrumentState.RWRTest1 = false;
                azimuthIndicator.InstrumentState.RWRTest2 = false;
            }
            azimuthIndicator.InstrumentState.PriorityMode = ((flightData.lightBits2 & (int)LightBits2.PriMode) == (int)LightBits2.PriMode);
            azimuthIndicator.InstrumentState.SearchMode = ((flightData.lightBits2 & (int)LightBits2.AuxSrch) == (int)LightBits2.AuxSrch);
            azimuthIndicator.InstrumentState.SeparateMode = ((flightData.lightBits2 & (int)LightBits2.TgtSep) == (int)LightBits2.TgtSep);
            azimuthIndicator.InstrumentState.UnknownThreatScanMode = ((flightData.lightBits2 & (int)LightBits2.Unk) == (int)LightBits2.Unk);

        }
    }
}
