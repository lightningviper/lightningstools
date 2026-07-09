using F4SharedMem;
using F4SharedMem.Headers;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface ICautionPanelFlightDataAdapter
    {
        void Adapt(ICautionPanel cautionPanel, FlightData flightData);
    }

    class CautionPanelFlightDataAdapter : ICautionPanelFlightDataAdapter
    {
        public void Adapt(ICautionPanel cautionPanel, FlightData flightData)
        {
            cautionPanel.InstrumentState.AftFuelLow = ((flightData.lightBits2 &(int) LightBits2.AftFuelLow) ==(int) LightBits2.AftFuelLow);
            cautionPanel.InstrumentState.AntiSkid = ((flightData.lightBits2 &(int) LightBits2.ANTI_SKID) ==(int) LightBits2.ANTI_SKID);
            cautionPanel.InstrumentState.AvionicsFault = ((flightData.lightBits &(int) LightBits.Avionics) ==(int) LightBits.Avionics);
            cautionPanel.InstrumentState.BUC = ((flightData.lightBits2 &(int) LightBits2.BUC) ==(int) LightBits2.BUC);
            cautionPanel.InstrumentState.CabinPress = ((flightData.lightBits &(int) LightBits.CabinPress) ==(int) LightBits.CabinPress);
            cautionPanel.InstrumentState.CADC = ((flightData.lightBits3 &(int) LightBits3.cadc) ==(int) LightBits3.cadc);
			cautionPanel.InstrumentState.InletIcing = ((flightData.lightBits3 & (int)LightBits3.Inlet_Icing) == (int)LightBits3.Inlet_Icing);
			cautionPanel.InstrumentState.ECM = ((flightData.lightBits &(int) LightBits.ECM) ==(int) LightBits.ECM);
            cautionPanel.InstrumentState.ElecSys = ((flightData.lightBits3 &(int) LightBits3.Elec_Fault) ==(int) LightBits3.Elec_Fault);
            cautionPanel.InstrumentState.EngineFault = ((flightData.lightBits &(int) LightBits.EngineFault) ==(int) LightBits.EngineFault);
            cautionPanel.InstrumentState.EquipHot = ((flightData.lightBits &(int) LightBits.EQUIP_HOT) ==(int) LightBits.EQUIP_HOT);
            cautionPanel.InstrumentState.FLCSFault = ((flightData.lightBits &(int) LightBits.FltControlSys) ==(int) LightBits.FltControlSys);
            cautionPanel.InstrumentState.FuelOilHot = ((flightData.lightBits2 &(int) LightBits2.FUEL_OIL_HOT) ==(int) LightBits2.FUEL_OIL_HOT);
            cautionPanel.InstrumentState.FwdFuelLow = ((flightData.lightBits2 &(int) LightBits2.FwdFuelLow) ==(int) LightBits2.FwdFuelLow);
            cautionPanel.InstrumentState.Hook = ((flightData.lightBits &(int) LightBits.Hook) ==(int) LightBits.Hook);
            cautionPanel.InstrumentState.IFF = ((flightData.lightBits &(int) LightBits.IFF) ==(int) LightBits.IFF);
            cautionPanel.InstrumentState.NWSFail = ((flightData.lightBits &(int) LightBits.NWSFail) ==(int) LightBits.NWSFail);
            cautionPanel.InstrumentState.Overheat = ((flightData.lightBits &(int) LightBits.Overheat) ==(int) LightBits.Overheat);
            cautionPanel.InstrumentState.OxyLow = ((flightData.lightBits2 &(int) LightBits2.OXY_LOW) ==(int) LightBits2.OXY_LOW);
            cautionPanel.InstrumentState.ProbeHeat = ((flightData.lightBits2 &(int) LightBits2.PROBEHEAT) ==(int) LightBits2.PROBEHEAT);
            cautionPanel.InstrumentState.RadarAlt = ((flightData.lightBits &(int) LightBits.RadarAlt) ==(int) LightBits.RadarAlt);
            cautionPanel.InstrumentState.SeatNotArmed = ((flightData.lightBits2 &(int) LightBits2.SEAT_ARM) ==(int) LightBits2.SEAT_ARM);
            cautionPanel.InstrumentState.SEC = ((flightData.lightBits2 &(int) LightBits2.SEC) ==(int) LightBits2.SEC);
            cautionPanel.InstrumentState.StoresConfig = ((flightData.lightBits &(int) LightBits.CONFIG) ==(int) LightBits.CONFIG);

			// Force these lights on if test is being run
			cautionPanel.InstrumentState.Nuclear = ((flightData.lightBits & (uint)LightBits.AllLampBitsOn) == (uint)LightBits.AllLampBitsOn);
			cautionPanel.InstrumentState.ATFNotEngaged = ((flightData.lightBits & (uint)LightBits.AllLampBitsOn) == (uint)LightBits.AllLampBitsOn);
			cautionPanel.InstrumentState.EEC = ((flightData.lightBits & (uint)LightBits.AllLampBitsOn) == (uint)LightBits.AllLampBitsOn);
			cautionPanel.InstrumentState.Slot27 = ((flightData.lightBits & (uint)LightBits.AllLampBitsOn) == (uint)LightBits.AllLampBitsOn);
			cautionPanel.InstrumentState.Slot28 = ((flightData.lightBits & (uint)LightBits.AllLampBitsOn) == (uint)LightBits.AllLampBitsOn);
			cautionPanel.InstrumentState.Slot30 = ((flightData.lightBits & (uint)LightBits.AllLampBitsOn) == (uint)LightBits.AllLampBitsOn);
			cautionPanel.InstrumentState.Slot31 = ((flightData.lightBits & (uint)LightBits.AllLampBitsOn) == (uint)LightBits.AllLampBitsOn);
			cautionPanel.InstrumentState.Slot32 = ((flightData.lightBits & (uint)LightBits.AllLampBitsOn) == (uint)LightBits.AllLampBitsOn);
			
            //TODO: implement MLU cautions
		}
	}
}
