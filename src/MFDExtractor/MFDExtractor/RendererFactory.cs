using System;
using Common.SimSupport;
using LightningGauges.Renderers.F16;
using LightningGauges.Renderers.F16.AzimuthIndicator;
using LightningGauges.Renderers.F16.EHSI;
using LightningGauges.Renderers.F16.HSI;
using LightningGauges.Renderers.F16.ISIS;
using MFDExtractor.Properties;
using MFDExtractor.Renderer;
using MFDExtractor.RendererFactories;
using LightningGauges.Renderers.F16.RWR;

namespace MFDExtractor
{
    internal interface IRendererFactory
    {
        IInstrumentRenderer CreateRenderer(InstrumentType instrumentType);
    }

    internal class RendererFactory : IRendererFactory
    {
        private readonly IRWRRendererFactory _rwrRendererFactory;
	    private readonly IAzimuthIndicatorRendererFactory _azimuthIndicatorFactory;
	    private readonly IAltimeterRendererFactory _altimeterRendererFactory;
	    private readonly IFuelQualityIndicatorRendererFactory _fuelQualityIndicatorRendererFactory;
	    private readonly IISISRendererFactory _isisRendererFactory;
	    private readonly IVVIRendererFactory _vviRendererFactory;
        public RendererFactory(
			IAzimuthIndicatorRendererFactory azimuthIndicatorFactory = null, 
            IRWRRendererFactory rwrRendererFactory=null,
			IFuelQualityIndicatorRendererFactory fuelQualityIndicatorRendererFactory=null, 
			IAltimeterRendererFactory altimeterRendererFactory=null, 
			IISISRendererFactory isisRendererFactory=null, 
			IVVIRendererFactory vviRendererFactory=null
        )
        {
			_azimuthIndicatorFactory = azimuthIndicatorFactory ?? new AzimuthIndicatorRendererFactory();
            _rwrRendererFactory = rwrRendererFactory ?? new RWRRendererFactory();
			_altimeterRendererFactory = altimeterRendererFactory ?? new AltimeterRendererFactory();
			_fuelQualityIndicatorRendererFactory = fuelQualityIndicatorRendererFactory ?? new FuelQualityIndicatorRendererFactory();
	        _isisRendererFactory = isisRendererFactory ?? new ISISRendererFactory();
			_vviRendererFactory = vviRendererFactory ?? new VVIRendererFactory();
        }

        public IInstrumentRenderer CreateRenderer(InstrumentType instrumentType)
        {
            switch (instrumentType)
            {
                case InstrumentType.Accelerometer:
                    return CreateAccelerometerRenderer();
                case InstrumentType.ADI:
                    return CreateADIRenderer();
                case InstrumentType.Altimeter:
                    return CreateAltimeterRenderer();
                case InstrumentType.AOAIndexer:
                    return CreateAOAIndexerRenderer();
                case InstrumentType.AOAIndicator:
                    return CreateAOAIndicatorRenderer();
                case InstrumentType.ASI:
                    return CreateASIRenderer();
                case InstrumentType.AzimuthIndicator:
                    return CreateAzimuthIndicatorRenderer();
                case InstrumentType.BackupADI:
                    return CreateBackupADIRenderer();
                case InstrumentType.CabinPress:
                    return CreateCabinPressRenderer();
                case InstrumentType.CautionPanel:
                    return CreateCautionPanelRenderer();
                case InstrumentType.CMDS:
                    return CreateCMDSPanelRenderer();
                case InstrumentType.Compass:
                    return CreateCompassRenderer();
                case InstrumentType.DED:
                    return CreateDEDRenderer();
                case InstrumentType.EHSI:
                    return CreateEHSIRenderer();
                case InstrumentType.EPUFuel:
                    return CreateEPUFuelRenderer();
                case InstrumentType.FTIT1:
                    return CreateFTIT1Renderer();
                case InstrumentType.FTIT2:
                    return CreateFTIT2Renderer();
                case InstrumentType.FuelFlow:
                    return CreateFuelFlowRenderer();
                case InstrumentType.FuelQuantity:
                    return CreateFuelQuantityRenderer();
                case InstrumentType.HUD:
                    return CreateHUDRenderer();
                case InstrumentType.HSI:
                    return CreateHSIRenderer();
                case InstrumentType.HYDA:
                    return CreateHydARenderer();
                case InstrumentType.HYDB:
                    return CreateHydBRenderer();
                case InstrumentType.ISIS:
                    return CreateISISRenderer();
                case InstrumentType.GearLights:
                    return CreateLandingGearLightsRenderer();
                case InstrumentType.LMFD:
                    return CreateLMFDRenderer();
                case InstrumentType.MFD3:
                    return CreateMFD3Renderer();
                case InstrumentType.MFD4:
                    return CreateMFD4Renderer();
                case InstrumentType.NOZ1:
                    return CreateNOZ1Renderer();
                case InstrumentType.NOZ2:
                    return CreateNOZ2Renderer();
                case InstrumentType.NWSIndexer:
                    return CreateNWSIndexerRenderer();
                case InstrumentType.OIL1:
                    return CreateOil1Renderer();
                case InstrumentType.OIL2:
                    return CreateOil2Renderer();
                case InstrumentType.PFL:
                    return CreatePFLRenderer();
                case InstrumentType.PitchTrim:
                    return CreatePitchTrimRenderer();
                case InstrumentType.RMFD:
                    return CreateRMFDRenderer();
                case InstrumentType.RollTrim:
                    return CreateRollTrimRenderer();
                case InstrumentType.RPM1:
                    return CreateRPM1Renderer();
                case InstrumentType.RPM2:
                    return CreateRPM2Renderer();
                case InstrumentType.RWR:
                    return CreateRWRRenderer();
                case InstrumentType.Speedbrake:
                    return CreateSpeedbrakeRenderer();
                case InstrumentType.VVI:
                    return CreateVVIRenderer();
                default:
                    throw new ArgumentOutOfRangeException("instrumentType");
            }
        }
		private IMfdRenderer CreateLMFDRenderer()
		{
			return new MfdRenderer
			{
				Options =
					new MfdRenderer.MfdRendererOptions
					{
						BlankImage = Resources.leftMFDBlankImage,
						TestAlignmentImage = Resources.leftMFDTestAlignmentImage
					}
			};
		}
        private IMfdRenderer CreateRMFDRenderer()
		{
			return new MfdRenderer
			{
				Options =
					new MfdRenderer.MfdRendererOptions
					{
						BlankImage = Resources.rightMFDBlankImage,
						TestAlignmentImage = Resources.rightMFDTestAlignmentImage
					}
			};
		}
        private IMfdRenderer CreateMFD3Renderer()
		{
			return new MfdRenderer
			{
				Options =
					new MfdRenderer.MfdRendererOptions
					{
						BlankImage = Resources.leftMFDBlankImage,
						TestAlignmentImage = Resources.mfd3TestAlignmentImage
					}
			};
		}
		private IMfdRenderer CreateMFD4Renderer()
		{
			return new MfdRenderer
			{
				Options =
					new MfdRenderer.MfdRendererOptions
					{
						BlankImage = Resources.rightMFDBlankImage,
						TestAlignmentImage = Resources.mfd4TestAlignmentImage
					}
			};
		}
		private IMfdRenderer CreateHUDRenderer()
		{
			return new MfdRenderer
			{
				Options =
					new MfdRenderer.MfdRendererOptions
					{
						BlankImage = Resources.hudBlankImage,
						TestAlignmentImage = Resources.hudTestAlignmentImage
					}
			};
		}
        private IVerticalVelocityIndicator CreateVVIRenderer()
        {
	       return _vviRendererFactory.Create();
        }

        private ITachometer CreateRPM2Renderer()
        {
            return new Tachometer {Options = {IsSecondary = true}};
        }

        private ITachometer CreateRPM1Renderer()
        {
            return new Tachometer {Options = {IsSecondary = false}};
        }

        private ISpeedbrakeIndicator CreateSpeedbrakeRenderer()
        {
            return new SpeedbrakeIndicator();
        }

        private IAzimuthIndicator CreateAzimuthIndicatorRenderer()
        {
	        return _azimuthIndicatorFactory.Create();
        }
        private IRWRRenderer CreateRWRRenderer()
        {
            return _rwrRendererFactory.CreateRenderer(RWRType.CARAPACE);
        }

        private IOilPressureGauge CreateOil2Renderer()
        {
            return new OilPressureGauge {Options = {IsSecondary = true}};
        }

        private IOilPressureGauge CreateOil1Renderer()
        {
            return new OilPressureGauge {Options = {IsSecondary = false}};
        }

        private INozzlePositionIndicator CreateNOZ2Renderer()
        {
            return new NozzlePositionIndicator {Options = {IsSecondary = true}};
        }

        private INozzlePositionIndicator CreateNOZ1Renderer()
        {
            return new NozzlePositionIndicator {Options = {IsSecondary = false}};
        }

        private INosewheelSteeringIndexer CreateNWSIndexerRenderer()
        {
            return new NosewheelSteeringIndexer();
        }

        private ILandingGearWheelsLights CreateLandingGearLightsRenderer()
        {
            return new LandingGearWheelsLights();
        }

        private IHorizontalSituationIndicator CreateHSIRenderer()
        {
            return new HorizontalSituationIndicator();
        }

        private IEHSI CreateEHSIRenderer()
        {
            return new EHSI();
        }

        private IFuelQuantityIndicator CreateFuelQuantityRenderer()
        {
	        return _fuelQualityIndicatorRendererFactory.Create();
        }

        private IFuelFlow CreateFuelFlowRenderer()
        {
            return new FuelFlow();
        }

        private IISIS CreateISISRenderer()
        {
	        return _isisRendererFactory.Create();
        }

        private IAccelerometer CreateAccelerometerRenderer()
        {
            return new Accelerometer();
        }

        private IFanTurbineInletTemperature CreateFTIT2Renderer()
        {
            return new FanTurbineInletTemperature {Options = {IsSecondary = true}};
        }

        private IFanTurbineInletTemperature CreateFTIT1Renderer()
        {
            return new FanTurbineInletTemperature {Options = {IsSecondary = false}};
        }

        private IEPUFuelGauge CreateEPUFuelRenderer()
        {
            return new EPUFuelGauge();
        }

        private IDataEntryDisplayPilotFaultList CreatePFLRenderer()
        {
            return new DataEntryDisplayPilotFaultList();
        }

        private IDataEntryDisplayPilotFaultList CreateDEDRenderer()
        {
            return new DataEntryDisplayPilotFaultList();
        }

        private ICompass CreateCompassRenderer()
        {
           return new Compass();
        }

        private ICMDSPanel CreateCMDSPanelRenderer()
        {
           return new CMDSPanel();
        }

        private ICautionPanel CreateCautionPanelRenderer()
        {
            return new CautionPanel();
        }

        private IAngleOfAttackIndicator CreateAOAIndicatorRenderer()
        {
            return new AngleOfAttackIndicator();
        }

        private IAngleOfAttackIndexer CreateAOAIndexerRenderer()
        {
            return new AngleOfAttackIndexer();
        }

        private IAltimeter CreateAltimeterRenderer()
        {
	       return _altimeterRendererFactory.Create();
        }

        private IAirspeedIndicator CreateASIRenderer()
        {
            return new AirspeedIndicator();
        }

        private IADI CreateADIRenderer()
        {
            return new ADI();
        }

        private IStandbyADI CreateBackupADIRenderer()
        {
            return new StandbyADI();
        }

        private IHydraulicPressureGauge CreateHydARenderer()
        {
            return new HydraulicPressureGauge();
        }

        private IHydraulicPressureGauge CreateHydBRenderer()
        {
           return new HydraulicPressureGauge();
        }

        private ICabinPressureAltitudeIndicator CreateCabinPressRenderer()
        {
            return new CabinPressureAltitudeIndicator();
        }

        private IRollTrimIndicator CreateRollTrimRenderer()
        {
            return new RollTrimIndicator();
        }

        private IPitchTrimIndicator CreatePitchTrimRenderer()
        {
            return new PitchTrimIndicator();
        }
        
    }
}
