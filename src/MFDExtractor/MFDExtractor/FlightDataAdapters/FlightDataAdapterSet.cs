using MFDExtractor.FlightDataAdapters.MFDExtractor.FlightDataAdapters;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IFlightDataAdapterSet
    {
        ICMDSFlightDataAdapter CMDS { get; }
        INWSFlightDataAdapter NWS { get; }
        IDEDFlightDataAdapter DED { get; }
        IPFLFlightDataAdapter PFL { get; }
        ISpeedbrakeFlightDataAdapter Speedbrake { get; }
        IVVIFlightDataAdapter VVI { get; }
        IAltimeterFlightDataAdapter Altimeter { get; }
        IEPUFuelFlightDataAdapter EPUFuel { get; }
        ICautionPanelFlightDataAdapter CautionPanel { get; }
        ILandingGearLightsFlightDataAdapter LandingGearLights { get; }
        IAzimuthIndicatorFlightDataAdapter AzimuthIndicator { get; }
        IRWRFlightDataAdapter RWR { get; }
        IAirspeedIndicatorFlightDataAdapter AirspeedIndicator { get; }
        ICompassFlightDataAdapter Compass { get; }
        IAngleOfAttackIndicatorFlightDataAdapter AOAIndicator { get; }
        IAngleOfAttackIndexerFlightDataAdapter AOAIndexer { get; }
        IISISFlightDataAdapter ISIS { get; }
        IRPM1FlightDataAdapter RPM1 { get; }
        IRPM2FlightDataAdapter RPM2 { get; }
        ICabinPressureAltitudeIndicatorFlightDataAdapter CabinPress { get; }
        IRollTrimIndicatorFlightDataAdapter RollTrim { get; }
        IPitchTrimIndicatorFlightDataAdapter PitchTrim { get; }
        IFuelFlowFlightDataAdapter FuelFlow { get; }
        IOIL1FlightDataAdapter OIL1 { get; }
        IOIL2FlightDataAdapter OIL2 { get; }
        IAccelerometerFlightDataAdapter Accelerometer { get; }
        INOZ1FlightDataAdapter NOZ1 { get; }
        INOZ2FlightDataAdapter NOZ2 { get; }
        IHYDAFlightDataAdapter HYDA { get; }
        IHYDBFlightDataAdapter HYDB { get; }
        IFTIT1FlightDataAdapter FTIT1 { get; }
        IFTIT2FlightDataAdapter FTIT2 { get; }
        IFuelQuantityFlightDataAdapter FuelQuantity { get; }
        IStandbyADIFlightDataAdapter StandbyADI { get; }
		IMFDFlightDataAdapter LMFD { get;}
		IMFDFlightDataAdapter RMFD { get;}
		IMFDFlightDataAdapter MFD3 { get;}
		IMFDFlightDataAdapter MFD4 { get;}
		IMFDFlightDataAdapter HUD { get;}
	}

    class FlightDataAdapterSet : IFlightDataAdapterSet
    {
        public FlightDataAdapterSet()
        {
            CMDS = new CMDSFlightDataAdapter();
            NWS = new NWSFlightDataAdapter();
            DED = new DEDFlightDataAdapter();
            PFL = new PFLFlightDataAdapter();
            Speedbrake = new SpeedbrakeFlightDataAdapter();
            VVI = new VVIFlightDataAdapter();
            Altimeter = new AltimeterFlightDataAdapter();
            EPUFuel = new EPUFuelFlightDataAdapter();
            CautionPanel = new CautionPanelFlightDataAdapter();
            LandingGearLights = new LandingGearLightsFlightDataAdapter();
            AzimuthIndicator = new AzimuthIndicatorFlightDataAdapter();
            RWR = new RWRFlightDataAdapter();
            AirspeedIndicator = new AirspeedIndicatorFlightDataAdapter();
            Compass = new CompassFlightDataAdapter();
            AOAIndicator = new AngleOfAttackIndicatorFlightDataAdapter();
            AOAIndexer = new AngleOfAttackIndexerFlightDataAdapter(); 
            ISIS = new ISISFlightDataAdapter();
            RPM1 = new RPM1FlightDataAdapter();
            RPM2 = new RPM2FlightDataAdapter();
            CabinPress = new CabinPressureAltitudeIndicatorFlightDataAdapter();
            RollTrim = new RollTrimIndicatorFlightDataAdapter();
            PitchTrim = new PitchTrimIndicatorFlightDataAdapter();
            FuelFlow = new FuelFlowFlightDataAdapter();
            OIL1 = new OIL1FlightDataAdapter();
            OIL2 = new OIL2FlightDataAdapter();
            Accelerometer = new AccelerometerFlightDataAdapter();
            NOZ1 = new NOZ1FlightDataAdapter();
            NOZ2 = new NOZ2FlightDataAdapter();
            HYDA = new HYDAFlightDataAdapter();
            HYDB = new HYDBFlightDataAdapter();
            FTIT1 = new FTIT1FlightDataAdapter();
            FTIT2 = new FTIT2FlightDataAdapter();
            FuelQuantity = new FuelQuantityFlightDataAdapter();
            StandbyADI = new StandbyADIFlightDataAdapter();
			LMFD = new MFDFlightDataAdapter();
			RMFD = new MFDFlightDataAdapter();
			MFD3 = new MFDFlightDataAdapter();
			MFD4 = new MFDFlightDataAdapter();
			HUD = new MFDFlightDataAdapter();
		}

        public ICMDSFlightDataAdapter CMDS { get; private set; }
        public INWSFlightDataAdapter NWS { get; private set; }
        public IDEDFlightDataAdapter DED { get; private set; }
        public ISpeedbrakeFlightDataAdapter Speedbrake { get; private set; }
        public IVVIFlightDataAdapter VVI { get; private set; }
        public IAltimeterFlightDataAdapter Altimeter { get; private set; }
        public IPFLFlightDataAdapter PFL { get; private set; }
        public IEPUFuelFlightDataAdapter EPUFuel { get; private set; }
        public ICautionPanelFlightDataAdapter CautionPanel { get; private set; }
        public ILandingGearLightsFlightDataAdapter LandingGearLights { get; private set; }
        public IAzimuthIndicatorFlightDataAdapter AzimuthIndicator { get; private set; }
        public IRWRFlightDataAdapter RWR { get; private set; }
        public IAirspeedIndicatorFlightDataAdapter AirspeedIndicator { get; private set; }
        public ICompassFlightDataAdapter Compass { get; private set; }
        public IAngleOfAttackIndicatorFlightDataAdapter AOAIndicator { get; private set; }
        public IAngleOfAttackIndexerFlightDataAdapter AOAIndexer { get; private set; }
        public IISISFlightDataAdapter ISIS { get; private set; }
        public IRPM1FlightDataAdapter RPM1 { get; private set; }
        public IRPM2FlightDataAdapter RPM2 { get; private set; }
        public ICabinPressureAltitudeIndicatorFlightDataAdapter CabinPress { get; private set; }
        public IRollTrimIndicatorFlightDataAdapter RollTrim { get; private set; }
        public IPitchTrimIndicatorFlightDataAdapter PitchTrim { get; private set; }
        public IFuelFlowFlightDataAdapter FuelFlow { get; private set; }
        public IOIL1FlightDataAdapter OIL1 { get; private set; }
        public IOIL2FlightDataAdapter OIL2 { get; private set; }
        public IAccelerometerFlightDataAdapter Accelerometer { get; private set; }
        public INOZ1FlightDataAdapter NOZ1 { get; private set; }
        public INOZ2FlightDataAdapter NOZ2 { get; private set; }
        public IHYDAFlightDataAdapter HYDA { get; private set; }
        public IHYDBFlightDataAdapter HYDB { get; private set; }
        public IFTIT1FlightDataAdapter FTIT1 { get; private set; }
        public IFTIT2FlightDataAdapter FTIT2 { get; private set; }
        public IFuelQuantityFlightDataAdapter FuelQuantity { get; private set; }
        public IStandbyADIFlightDataAdapter StandbyADI { get; private set; }
		public IMFDFlightDataAdapter LMFD { get; private set; }
		public IMFDFlightDataAdapter RMFD { get; private set; }
		public IMFDFlightDataAdapter MFD3 { get; private set; }
		public IMFDFlightDataAdapter MFD4 { get; private set; }
		public IMFDFlightDataAdapter HUD { get; private set; }
    }
}
