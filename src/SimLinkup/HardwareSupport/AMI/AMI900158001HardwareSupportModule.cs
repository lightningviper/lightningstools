using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using Common.Math;
using LightningGauges.Renderers.F16.HSI;

namespace SimLinkup.HardwareSupport.AMI
{
    //AMI 9001580-01 HSI
    public class AMI900158001HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly IHorizontalSituationIndicator _renderer = new HorizontalSituationIndicator();
        private AnalogSignal _bearingCOSOutputSignal;
        private AnalogSignal _bearingInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _bearingInputSignalChangedEventHandler;
        private AnalogSignal _bearingSINOutputSignal;
        private AnalogSignal _compassCOSOutputSignal;

        private AnalogSignal _compassInputSignal;

        private AnalogSignal.AnalogSignalChangedEventHandler _compassInputSignalChangedEventHandler;

        private AnalogSignal _compassSINOutputSignal;
        private AnalogSignal _courseCOSOutputSignal;
        private AnalogSignal _courseDeviationInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _courseDeviationInputSignalChangedEventHandler;
        private AnalogSignal _courseDeviationLimitInputSignal;
        private AnalogSignal _courseDeviationOutputSignal;
        private AnalogSignal _courseInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _courseInputSignalChangedEventHandler;
        private AnalogSignal _courseSINOutputSignal;
        private DigitalSignal _deviationFlagInputSignal;
        private DigitalSignal.SignalChangedEventHandler _deviationFlagInputSignalChangedEventHandler;
        private DigitalSignal _deviationFlagOutputSignal;
        private AnalogSignal _DMEInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _DMEInputSignalChangedEventHandler;
        private DigitalSignal _dmeShutterInputSignal;
        private DigitalSignal.SignalChangedEventHandler _dmeShutterInputSignalChangedEventHandler;
        private DigitalSignal _dmeShutterOutputSignal;
        private AnalogSignal _DMEx100COSOutputSignal;
        private AnalogSignal _DMEx100SINOutputSignal;
        private AnalogSignal _DMEx10COSOutputSignal;
        private AnalogSignal _DMEx10SINOutputSignal;
        private AnalogSignal _DMEx1COSOutputSignal;
        private AnalogSignal _DMEx1SINOutputSignal;
        private DigitalSignal _fromFlagInputSignal;
        private DigitalSignal.SignalChangedEventHandler _fromFlagInputSignalChangedEventHandler;
        private DigitalSignal _fromFlagOutputSignal;
        private AnalogSignal _headingCOSOutputSignal;
        private AnalogSignal _headingInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _headingInputSignalChangedEventHandler;
        private AnalogSignal _headingSINOutputSignal;

        private bool _isDisposed;

        private DigitalSignal _offFlagInputSignal;

        private DigitalSignal.SignalChangedEventHandler _offFlagInputSignalChangedEventHandler;

        private DigitalSignal _offFlagOutputSignal;
        private DigitalSignal _toFlagInputSignal;
        private DigitalSignal.SignalChangedEventHandler _toFlagInputSignalChangedEventHandler;
        private DigitalSignal _toFlagOutputSignal;

        private AMI900158001HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[]
        {
            _compassInputSignal, _headingInputSignal, _courseInputSignal, _bearingInputSignal, _DMEInputSignal,
            _courseDeviationInputSignal, _courseDeviationLimitInputSignal
        };

        public override AnalogSignal[] AnalogOutputs => new[]
        {
            _compassSINOutputSignal, _compassCOSOutputSignal, _headingSINOutputSignal, _headingCOSOutputSignal,
            _courseSINOutputSignal, _courseCOSOutputSignal, _bearingSINOutputSignal, _bearingCOSOutputSignal,
            _DMEx100SINOutputSignal, _DMEx100COSOutputSignal, _DMEx10SINOutputSignal, _DMEx10COSOutputSignal,
            _DMEx1SINOutputSignal, _DMEx1COSOutputSignal, _courseDeviationOutputSignal
        };

        public override DigitalSignal[] DigitalInputs => new[]
        {
            _offFlagInputSignal, _deviationFlagInputSignal, _dmeShutterInputSignal, _toFlagInputSignal,
            _fromFlagInputSignal
        };

        public override DigitalSignal[] DigitalOutputs => new[]
        {
            _offFlagOutputSignal, _deviationFlagOutputSignal, _dmeShutterOutputSignal, _toFlagOutputSignal,
            _fromFlagOutputSignal
        };

        public override string FriendlyName =>
            "AMI P/N 900150-01 - Indicator - Simulated Horizontal Situation Indicator";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AMI900158001HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule> {new AMI900158001HardwareSupportModule()};
            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.BearingToBeaconDegrees = (float) _bearingInputSignal.State;
            _renderer.InstrumentState.CourseDeviationDegrees = (float) _courseDeviationInputSignal.State;
            _renderer.InstrumentState.CourseDeviationLimitDegrees = (float) _courseDeviationLimitInputSignal.State;
            _renderer.InstrumentState.DesiredCourseDegrees = (int) _courseInputSignal.State;
            _renderer.InstrumentState.DesiredHeadingDegrees = (int) _headingInputSignal.State;
            _renderer.InstrumentState.DeviationInvalidFlag = _deviationFlagInputSignal.State;
            _renderer.InstrumentState.DistanceToBeaconNauticalMiles = (float) _DMEInputSignal.State;
            _renderer.InstrumentState.DmeInvalidFlag = _dmeShutterInputSignal.State;
            _renderer.InstrumentState.FromFlag = _fromFlagInputSignal.State;
            _renderer.InstrumentState.MagneticHeadingDegrees = (float) _compassInputSignal.State;
            _renderer.InstrumentState.OffFlag = _offFlagInputSignal.State;
            _renderer.InstrumentState.ShowToFromFlag = true;
            _renderer.InstrumentState.ToFlag = _toFlagInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _offFlagInputSignalChangedEventHandler = null;
            _dmeShutterInputSignalChangedEventHandler = null;
            _deviationFlagInputSignalChangedEventHandler = null;
            _toFlagInputSignalChangedEventHandler = null;
            _fromFlagInputSignalChangedEventHandler = null;

            _compassInputSignalChangedEventHandler = null;
            _headingInputSignalChangedEventHandler = null;
            _courseInputSignalChangedEventHandler = null;
            _bearingInputSignalChangedEventHandler = null;
            _DMEInputSignalChangedEventHandler = null;
            _courseDeviationInputSignalChangedEventHandler = null;
        }

        private AnalogSignal CreateBearingCOSOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Bearing (COS)",
                Id = "900158001_Bearing_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00, //volts
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateBearingInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Bearing to Beacon",
                Id = "900158001_Bearing_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = 0,
                MaxValue = 360
            };
            return thisSignal;
        }

        private AnalogSignal CreateBearingSINOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Bearing (SIN)",
                Id = "900158001_Bearing_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00, //volts;
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateCompassCOSOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Compass (COS)",
                Id = "900158001_Compass_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00, //volts
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateCompassInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Compass",
                Id = "900158001_Compass_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = 0,
                MaxValue = 360
            };
            return thisSignal;
        }

        private AnalogSignal CreateCompassSINOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Compass (SIN)",
                Id = "900158001_Compass_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00, //volts
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateCourseCOSOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Course (COS)",
                Id = "900158001_Course_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00, //volts
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateCourseDeviationInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Course Deviation",
                Id = "900158001_Course_Deviation_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateCourseDeviationLimitInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Course Deviation Limit",
                Id = "900158001_Course_Deviation_Limit_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = 0,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateCourseDeviationOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Course Deviation",
                Id = "900158001_Course_Deviation_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00, //volts;
                IsVoltage = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateCourseInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Course",
                Id = "900158001_Course_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = 0,
                MaxValue = 360
            };
            return thisSignal;
        }

        private AnalogSignal CreateCourseSINOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Course (SIN)",
                Id = "900158001_Course_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00, //volts;
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private DigitalSignal CreateDeviationFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "Deviation flag",
                Id = "900158001_Deviation_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private DigitalSignal CreateDeviationFlagOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "Deviation flag",
                Id = "900158001_Deviation_Flag_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private AnalogSignal CreateDMEInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "DME",
                Id = "900158001_DME_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = 0,
                MaxValue = 999.9999
            };
            return thisSignal;
        }

        private DigitalSignal CreateDMEShutterInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "DME Shutter flag",
                Id = "900158001_DME_Shutter_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private DigitalSignal CreateDMEShutterOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "DME Shutter",
                Id = "900158001_DME_Shutter_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private AnalogSignal CreateDMEx100COSOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "DME x100 (COS)",
                Id = "900158001_DME_x100_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00, //volts
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateDMEx100SINOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "DME x100 (SIN)",
                Id = "900158001_DME_x100_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00, //volts;
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateDMEx10COSOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "DME x10 (COS)",
                Id = "900158001_DME_x10_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00, //volts
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateDMEx10SINOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "DME x10 (SIN)",
                Id = "900158001_DME_x10_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00, //volts;
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateDMEx1COSOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "DME x1 (COS)",
                Id = "900158001_DME_x1_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00, //volts
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateDMEx1SINOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "DME x1 (SIN)",
                Id = "900158001_DME_x1_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00, //volts;
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private DigitalSignal CreateFromFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "FROM flag",
                Id = "900158001_FROM_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private DigitalSignal CreateFromFlagOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "FROM Flag",
                Id = "900158001_FROM_Flag_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private AnalogSignal CreateHeadingCOSOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Heading (COS)",
                Id = "900158001_Heading_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00, //volts
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateHeadingInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Heading",
                Id = "900158001_Heading_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = 0,
                MaxValue = 360
            };
            return thisSignal;
        }

        private AnalogSignal CreateHeadingSINOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Heading (SIN)",
                Id = "900158001_Heading_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00, //volts;
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private void CreateInputEventHandlers()
        {
            _offFlagInputSignalChangedEventHandler =
                offFlag_InputSignalChanged;
            _dmeShutterInputSignalChangedEventHandler =
                dmeShutter_InputSignalChanged;
            _deviationFlagInputSignalChangedEventHandler =
                deviationFlag_InputSignalChanged;
            _toFlagInputSignalChangedEventHandler =
                toFlag_InputSignalChanged;
            _fromFlagInputSignalChangedEventHandler =
                fromFlag_InputSignalChanged;

            _compassInputSignalChangedEventHandler =
                HSI_Directional_InputSignalsChanged;
            _headingInputSignalChangedEventHandler =
                HSI_Directional_InputSignalsChanged;
            _courseInputSignalChangedEventHandler =
                HSI_Directional_InputSignalsChanged;
            _bearingInputSignalChangedEventHandler =
                HSI_Directional_InputSignalsChanged;
            _DMEInputSignalChangedEventHandler =
                dme_InputSignalChanged;
            _courseDeviationInputSignalChangedEventHandler =
                HSI_Directional_InputSignalsChanged;
        }

        private void CreateInputSignals()
        {
            _offFlagInputSignal = CreateOffFlagInputSignal();
            _deviationFlagInputSignal = CreateDeviationFlagInputSignal();
            _dmeShutterInputSignal = CreateDMEShutterInputSignal();
            _toFlagInputSignal = CreateToFlagInputSignal();
            _fromFlagInputSignal = CreateFromFlagInputSignal();
            _compassInputSignal = CreateCompassInputSignal();
            _headingInputSignal = CreateHeadingInputSignal();
            _courseInputSignal = CreateCourseInputSignal();
            _bearingInputSignal = CreateBearingInputSignal();
            _DMEInputSignal = CreateDMEInputSignal();
            _courseDeviationInputSignal = CreateCourseDeviationInputSignal();
            _courseDeviationLimitInputSignal = CreateCourseDeviationLimitInputSignal();
        }

        private DigitalSignal CreateOffFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "Off flag",
                Id = "900158001_Off_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private DigitalSignal CreateOffFlagOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "OFF flag",
                Id = "900158001_OFF_Flag_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private void CreateOutputSignals()
        {
            _compassSINOutputSignal = CreateCompassSINOutputSignal();
            _compassCOSOutputSignal = CreateCompassCOSOutputSignal();
            _headingSINOutputSignal = CreateHeadingSINOutputSignal();
            _headingCOSOutputSignal = CreateHeadingCOSOutputSignal();
            _courseSINOutputSignal = CreateCourseSINOutputSignal();
            _courseCOSOutputSignal = CreateCourseCOSOutputSignal();
            _bearingSINOutputSignal = CreateBearingSINOutputSignal();
            _bearingCOSOutputSignal = CreateBearingCOSOutputSignal();
            _DMEx100SINOutputSignal = CreateDMEx100SINOutputSignal();
            _DMEx100COSOutputSignal = CreateDMEx100COSOutputSignal();
            _DMEx10SINOutputSignal = CreateDMEx10SINOutputSignal();
            _DMEx10COSOutputSignal = CreateDMEx10COSOutputSignal();
            _DMEx1SINOutputSignal = CreateDMEx1SINOutputSignal();
            _DMEx1COSOutputSignal = CreateDMEx1COSOutputSignal();
            _courseDeviationOutputSignal = CreateCourseDeviationOutputSignal();
            _offFlagOutputSignal = CreateOffFlagOutputSignal();
            _deviationFlagOutputSignal = CreateDeviationFlagOutputSignal();
            _dmeShutterOutputSignal = CreateDMEShutterOutputSignal();
            _toFlagOutputSignal = CreateToFlagOutputSignal();
            _fromFlagOutputSignal = CreateFromFlagOutputSignal();
        }

        private DigitalSignal CreateToFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "TO flag",
                Id = "900158001_TO_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private DigitalSignal CreateToFlagOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "TO Flag",
                Id = "900158001_TO_Flag_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private void deviationFlag_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateDeviationFlagOutputValue();
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    UnregisterForInputEvents();
                    AbandonInputEventHandlers();
                    Common.Util.DisposeObject(_renderer);
                }
            }
            _isDisposed = true;
        }

        private void dme_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateDMEOutputValue();
        }

        private void dmeShutter_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateDMEShutterOutputValue();
        }

        private void fromFlag_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateFromFlagOutputValue();
        }

        private void HSI_Directional_InputSignalsChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateCompassOutputValue();
            UpdateHeadingOutputValue();
            UpdateCourseOutputValue();
            UpdateBearingOutputValue();
            UpdateCourseDeviationOutputValue();
        }

        private void offFlag_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateOffFlagOutputValue();
        }

        private void RegisterForInputEvents()
        {
            if (_offFlagInputSignal != null)
            {
                _offFlagInputSignal.SignalChanged += _offFlagInputSignalChangedEventHandler;
            }
            if (_dmeShutterInputSignal != null)
            {
                _dmeShutterInputSignal.SignalChanged += _dmeShutterInputSignalChangedEventHandler;
            }
            if (_deviationFlagInputSignal != null)
            {
                _deviationFlagInputSignal.SignalChanged += _deviationFlagInputSignalChangedEventHandler;
            }
            if (_toFlagInputSignal != null)
            {
                _toFlagInputSignal.SignalChanged += _toFlagInputSignalChangedEventHandler;
            }
            if (_fromFlagInputSignal != null)
            {
                _fromFlagInputSignal.SignalChanged += _fromFlagInputSignalChangedEventHandler;
            }
            if (_compassInputSignal != null)
            {
                _compassInputSignal.SignalChanged += _compassInputSignalChangedEventHandler;
            }
            if (_headingInputSignal != null)
            {
                _headingInputSignal.SignalChanged += _headingInputSignalChangedEventHandler;
            }
            if (_courseInputSignal != null)
            {
                _courseInputSignal.SignalChanged += _courseInputSignalChangedEventHandler;
            }
            if (_bearingInputSignal != null)
            {
                _bearingInputSignal.SignalChanged += _bearingInputSignalChangedEventHandler;
            }
            if (_DMEInputSignal != null)
            {
                _DMEInputSignal.SignalChanged += _DMEInputSignalChangedEventHandler;
            }
            if (_courseDeviationInputSignal != null)
            {
                _courseDeviationInputSignal.SignalChanged += _courseDeviationInputSignalChangedEventHandler;
            }
        }

        private void toFlag_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateToFlagOutputValue();
        }

        private void UnregisterForInputEvents()
        {
            if (_offFlagInputSignal != null && _offFlagInputSignalChangedEventHandler != null)
            {
                try
                {
                    _offFlagInputSignal.SignalChanged -= _offFlagInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_dmeShutterInputSignal != null && _dmeShutterInputSignalChangedEventHandler != null)
            {
                try
                {
                    _dmeShutterInputSignal.SignalChanged -= _dmeShutterInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_deviationFlagInputSignal != null && _deviationFlagInputSignalChangedEventHandler != null)
            {
                try
                {
                    _deviationFlagInputSignal.SignalChanged -= _deviationFlagInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_toFlagInputSignal != null && _toFlagInputSignalChangedEventHandler != null)
            {
                try
                {
                    _toFlagInputSignal.SignalChanged -= _toFlagInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_fromFlagInputSignal != null && _fromFlagInputSignalChangedEventHandler != null)
            {
                try
                {
                    _fromFlagInputSignal.SignalChanged -= _fromFlagInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_compassInputSignal != null && _compassInputSignalChangedEventHandler != null)
            {
                try
                {
                    _compassInputSignal.SignalChanged -= _compassInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_headingInputSignal != null && _headingInputSignalChangedEventHandler != null)
            {
                try
                {
                    _headingInputSignal.SignalChanged -= _headingInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_courseInputSignal != null && _courseInputSignalChangedEventHandler != null)
            {
                try
                {
                    _courseInputSignal.SignalChanged -= _courseInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_bearingInputSignal != null && _bearingInputSignalChangedEventHandler != null)
            {
                try
                {
                    _bearingInputSignal.SignalChanged -= _bearingInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_DMEInputSignal != null && _DMEInputSignalChangedEventHandler != null)
            {
                try
                {
                    _DMEInputSignal.SignalChanged -= _DMEInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_courseDeviationInputSignal == null || _courseDeviationInputSignalChangedEventHandler == null) return;
            try
            {
                _courseDeviationInputSignal.SignalChanged += _courseDeviationInputSignalChangedEventHandler;
            }
            catch (RemotingException)
            {
            }
        }

        private void UpdateBearingOutputValue()
        {
            if (_bearingInputSignal == null || _bearingSINOutputSignal == null ||
                _bearingCOSOutputSignal == null)
            {
                return;
            }
            var bearingToBeaconDegrees = _bearingInputSignal.State;
            var magneticHeadingDegrees = _compassInputSignal.State;

            var bearingSINOutputValue = 10.0000 *
                                        Math.Sin(-(magneticHeadingDegrees - bearingToBeaconDegrees) *
                                                 Constants.RADIANS_PER_DEGREE);
            if (bearingSINOutputValue < -10)
            {
                bearingSINOutputValue = -10;
            }
            if (bearingSINOutputValue > 10)
            {
                bearingSINOutputValue = 10;
            }
            _bearingSINOutputSignal.State = bearingSINOutputValue;

            var bearingCOSOutputValue = 10.0000 *
                                        Math.Cos(-(magneticHeadingDegrees - bearingToBeaconDegrees) *
                                                 Constants.RADIANS_PER_DEGREE);
            if (bearingCOSOutputValue < -10)
            {
                bearingCOSOutputValue = -10;
            }
            if (bearingCOSOutputValue > 10)
            {
                bearingCOSOutputValue = 10;
            }
            _bearingCOSOutputSignal.State = bearingCOSOutputValue;
        }

        private void UpdateCompassOutputValue()
        {
            if (_compassInputSignal == null || _compassSINOutputSignal == null ||
                _compassCOSOutputSignal == null)
            {
                return;
            }
            var compassDegrees = _compassInputSignal.State;
            var compassSINOutputValue = 10.0000 * Math.Sin(compassDegrees * Constants.RADIANS_PER_DEGREE);
            if (compassSINOutputValue < -10)
            {
                compassSINOutputValue = -10;
            }
            if (compassSINOutputValue > 10)
            {
                compassSINOutputValue = 10;
            }
            _compassSINOutputSignal.State = compassSINOutputValue;

            var compassCOSOutputValue = 10.0000 * Math.Cos(compassDegrees * Constants.RADIANS_PER_DEGREE);
            if (compassCOSOutputValue < -10)
            {
                compassCOSOutputValue = -10;
            }
            if (compassCOSOutputValue > 10)
            {
                compassCOSOutputValue = 10;
            }
            _compassCOSOutputSignal.State = compassCOSOutputValue;
        }

        private void UpdateCourseDeviationOutputValue()
        {
            const double DEFAULT_COURSE_DEVIATION_LIMIT_DEGREES = 5.0d;

            if (_courseDeviationInputSignal == null || _courseDeviationLimitInputSignal == null ||
                _courseDeviationOutputSignal == null)
            {
                return;
            }
            var courseDeviationDegrees = _courseDeviationInputSignal.State;
            var courseDeviationLimitDegrees = _courseDeviationLimitInputSignal.State;
            if (courseDeviationLimitDegrees == 0 || double.IsInfinity(courseDeviationLimitDegrees) ||
                double.IsNaN(courseDeviationLimitDegrees))
            {
                courseDeviationLimitDegrees = DEFAULT_COURSE_DEVIATION_LIMIT_DEGREES;
            }
            var courseDeviationPct = courseDeviationDegrees / courseDeviationLimitDegrees;

            var courseDeviationOutputValue = 10.0000 * courseDeviationPct;
            if (courseDeviationOutputValue < -10)
            {
                courseDeviationOutputValue = -10;
            }
            if (courseDeviationOutputValue > 10)
            {
                courseDeviationOutputValue = 10;
            }
            _courseDeviationOutputSignal.State = courseDeviationOutputValue;
        }

        private void UpdateCourseOutputValue()
        {
            if (_courseInputSignal != null && _courseSINOutputSignal != null && _courseCOSOutputSignal != null)
            {
                var desiredCourseDegrees = _courseInputSignal.State;
                var courseSINOutputValue = 10.0000 * Math.Sin(desiredCourseDegrees * Constants.RADIANS_PER_DEGREE);
                if (courseSINOutputValue < -10)
                {
                    courseSINOutputValue = -10;
                }
                if (courseSINOutputValue > 10)
                {
                    courseSINOutputValue = 10;
                }
                _courseSINOutputSignal.State = courseSINOutputValue;

                var courseCOSOutputValue = 10.0000 * Math.Cos(desiredCourseDegrees * Constants.RADIANS_PER_DEGREE);
                if (courseCOSOutputValue < -10)
                {
                    courseCOSOutputValue = -10;
                }
                if (courseCOSOutputValue > 10)
                {
                    courseCOSOutputValue = 10;
                }
                _courseCOSOutputSignal.State = courseCOSOutputValue;
            }
        }

        private void UpdateDeviationFlagOutputValue()
        {
            if (_deviationFlagInputSignal != null && _deviationFlagOutputSignal != null)
            {
                _deviationFlagOutputSignal.State = _deviationFlagInputSignal.State;
            }
        }

        private void UpdateDMEOutputValue()
        {
            const double MAX_DISTANCE_TO_BEACON_NAUTICAL_MILES = 999.9999d;
            if (_DMEInputSignal == null || _DMEx100SINOutputSignal == null || _DMEx100COSOutputSignal == null ||
                _DMEx10SINOutputSignal == null || _DMEx10COSOutputSignal == null || _DMEx1SINOutputSignal == null ||
                _DMEx1COSOutputSignal == null)
            {
                return;
            }
            var distanceToBeaconNauticalMiles = Math.Abs(double.IsInfinity(_DMEInputSignal.State) ||
                                                         double.IsNaN(_DMEInputSignal.State)
                ? 0
                : _DMEInputSignal.State);
            if (distanceToBeaconNauticalMiles > MAX_DISTANCE_TO_BEACON_NAUTICAL_MILES)
            {
                distanceToBeaconNauticalMiles = MAX_DISTANCE_TO_BEACON_NAUTICAL_MILES;
            }
            var distanceToBeaconString = $"{distanceToBeaconNauticalMiles:000}";

            var DMEx100 = byte.Parse(distanceToBeaconString.Substring(0, 1));
            var DMEx10 = byte.Parse(distanceToBeaconString.Substring(1, 1));
            var DMEx1 = byte.Parse(distanceToBeaconString.Substring(2, 1));

            var DMEx100SINOutputValue = 10.0000 * Math.Sin(DMEx100 / 10.0000 * 360.0000 * Constants.RADIANS_PER_DEGREE);
            if (DMEx100SINOutputValue < -10)
            {
                DMEx100SINOutputValue = -10;
            }
            if (DMEx100SINOutputValue > 10)
            {
                DMEx100SINOutputValue = 10;
            }
            _DMEx100SINOutputSignal.State = DMEx100SINOutputValue;

            var DMEx100COSOutputValue = 10.0000 * Math.Cos(DMEx100 / 10.0000 * 360.0000 * Constants.RADIANS_PER_DEGREE);
            if (DMEx100COSOutputValue < -10)
            {
                DMEx100COSOutputValue = -10;
            }
            if (DMEx100COSOutputValue > 10)
            {
                DMEx100COSOutputValue = 10;
            }
            _DMEx100COSOutputSignal.State = DMEx100COSOutputValue;


            var DMEx10SINOutputValue = 10.0000 * Math.Sin(DMEx10 / 10.0000 * 360.0000 * Constants.RADIANS_PER_DEGREE);
            if (DMEx10SINOutputValue < -10)
            {
                DMEx10SINOutputValue = -10;
            }
            if (DMEx10SINOutputValue > 10)
            {
                DMEx10SINOutputValue = 10;
            }
            _DMEx10SINOutputSignal.State = DMEx10SINOutputValue;

            var DMEx10COSOutputValue = 10.0000 * Math.Cos(DMEx10 / 10.0000 * 360.0000 * Constants.RADIANS_PER_DEGREE);
            if (DMEx10COSOutputValue < -10)
            {
                DMEx10COSOutputValue = -10;
            }
            if (DMEx10COSOutputValue > 10)
            {
                DMEx10COSOutputValue = 10;
            }
            _DMEx10COSOutputSignal.State = DMEx10COSOutputValue;


            var DMEx1SINOutputValue = 10.0000 * Math.Sin(DMEx1 / 10.0000 * 360.0000 * Constants.RADIANS_PER_DEGREE);
            if (DMEx1SINOutputValue < -10)
            {
                DMEx1SINOutputValue = -10;
            }
            if (DMEx1SINOutputValue > 10)
            {
                DMEx1SINOutputValue = 10;
            }
            _DMEx1SINOutputSignal.State = DMEx1SINOutputValue;

            var DMEx1COSOutputValue = 10.0000 * Math.Cos(DMEx1 / 10.0000 * 360.0000 * Constants.RADIANS_PER_DEGREE);
            if (DMEx1COSOutputValue < -10)
            {
                DMEx1COSOutputValue = -10;
            }
            if (DMEx1COSOutputValue > 10)
            {
                DMEx1COSOutputValue = 10;
            }
            _DMEx1COSOutputSignal.State = DMEx1COSOutputValue;
        }

        private void UpdateDMEShutterOutputValue()
        {
            if (_dmeShutterInputSignal != null && _dmeShutterOutputSignal != null)
            {
                _dmeShutterOutputSignal.State = _dmeShutterInputSignal.State;
            }
        }

        private void UpdateFromFlagOutputValue()
        {
            if (_fromFlagInputSignal != null && _fromFlagOutputSignal != null)
            {
                _fromFlagOutputSignal.State = _fromFlagInputSignal.State;
            }
        }

        private void UpdateHeadingOutputValue()
        {
            if (_headingInputSignal != null && _headingSINOutputSignal != null && _headingCOSOutputSignal != null)
            {
                var desiredHeadingDegrees = _headingInputSignal.State;
                var magneticHeadingDegrees = _compassInputSignal.State;

                var headingSINOutputValue = 10.0000 *
                                            Math.Sin((desiredHeadingDegrees - magneticHeadingDegrees) *
                                                     Constants.RADIANS_PER_DEGREE);
                if (headingSINOutputValue < -10)
                {
                    headingSINOutputValue = -10;
                }
                if (headingSINOutputValue > 10)
                {
                    headingSINOutputValue = 10;
                }
                _headingSINOutputSignal.State = headingSINOutputValue;

                var headingCOSOutputValue = 10.0000 *
                                            Math.Cos((desiredHeadingDegrees - magneticHeadingDegrees) *
                                                     Constants.RADIANS_PER_DEGREE);
                if (headingCOSOutputValue < -10)
                {
                    headingCOSOutputValue = -10;
                }
                if (headingCOSOutputValue > 10)
                {
                    headingCOSOutputValue = 10;
                }
                _headingCOSOutputSignal.State = headingCOSOutputValue;
            }
        }

        private void UpdateOffFlagOutputValue()
        {
            if (_offFlagInputSignal != null && _offFlagOutputSignal != null)
            {
                _offFlagOutputSignal.State = _offFlagInputSignal.State;
            }
        }

        private void UpdateToFlagOutputValue()
        {
            if (_toFlagInputSignal != null && _toFlagOutputSignal != null)
            {
                _toFlagOutputSignal.State = _toFlagInputSignal.State;
            }
        }
    }
}