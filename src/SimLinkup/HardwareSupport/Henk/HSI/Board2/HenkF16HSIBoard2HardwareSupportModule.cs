using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.HardwareSupport.Calibration;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16.HSI;
using Henkie.HSI.Board2;
using System.IO;
using System.Linq;
using Henkie.Common;
using System.Globalization;
using log4net;
using p = Phcc;

namespace SimLinkup.HardwareSupport.Henk.HSI.Board2
{
    //Henk HSI 
    public class HenkF16HSIBoard2HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(HenkF16HSIBoard2HardwareSupportModule));
        private readonly IHorizontalSituationIndicator _renderer = new HorizontalSituationIndicator();
        private bool _isDisposed;

        //INPUT SIGNALS
        private AnalogSignal _magneticHeadingInputSignal;
        private AnalogSignal _bearingInputSignal;
        private AnalogSignal _rangeInputSignal;
        private DigitalSignal _rangeInvalidFlagInputSignal;
        private AnalogSignal _courseInputSignal;
        private AnalogSignal _courseDeviationInputSignal;
        private AnalogSignal _courseDeviationLimitInputSignal;
        private DigitalSignal _deviationInvalidFlagInputSignal;
        private AnalogSignal _desiredHeadingFromSimInputSignal;
        private DigitalSignal _toFlagInputSignal;
        private DigitalSignal _fromFlagInputSignal;
        private DigitalSignal _offFlagInputSignal;

        //OUTPUT SIGNALS
        private AnalogSignal _courseDeviationIndicatorOutputSignal;
        private DigitalSignal _deviationInvalidFlagOutputSignal;
        private AnalogSignal _toFromFlagsOutputSignal;
        private AnalogSignal _courseArrowPositionOutputSignal;
        private List<DigitalSignal> _digitalOutputs = new List<DigitalSignal>();

        //DEVICE CONFIG
        private Device _hsiBoard2DeviceInterface;
        private readonly DeviceConfig _hsiBoard2DeviceConfig;
        private byte _hsiBoard2DeviceAddress;

        private CalibrationPoint[] _courseDeviationIndicatorCalibrationData;

        // Editor-authored calibration plumbing — see Board 1's matching
        // fields for the contract. Board 2's unified file owns just the
        // single course-deviation-indicator breakpoint table; everything
        // else (identity, stator offsets, hysteresis thresholds,
        // DIG_OUTs) stays in the legacy file.
        private string _legacyConfigPath;
        private string _unifiedConfigPath;
        private ConfigFileReloadWatcher _legacyConfigWatcher;
        private ConfigFileReloadWatcher _unifiedConfigWatcher;

        private HenkF16HSIBoard2HardwareSupportModule(DeviceConfig hsiBoard2DeviceConfig, string legacyConfigPath, string unifiedConfigPath)
        {
            _hsiBoard2DeviceConfig = hsiBoard2DeviceConfig;
            _legacyConfigPath = legacyConfigPath;
            _unifiedConfigPath = unifiedConfigPath;
            if (_hsiBoard2DeviceConfig != null)
            {
                ConfigureDevice();
                CreateInputSignals();
                CreateOutputSignals();
                RegisterForEvents();
                SetInitialState();
                StartConfigWatchers();
            }
        }

        public override AnalogSignal[] AnalogInputs => new[]
        {
            _magneticHeadingInputSignal,
            _desiredHeadingFromSimInputSignal,
            _courseInputSignal,
            _courseDeviationInputSignal,
            _courseDeviationLimitInputSignal,
            _bearingInputSignal,
            _rangeInputSignal,
        };

        public override AnalogSignal[] AnalogOutputs => new[]
        {
            _courseDeviationIndicatorOutputSignal,
            _toFromFlagsOutputSignal,
            _courseArrowPositionOutputSignal
        };

        public override DigitalSignal[] DigitalInputs => new[]
        {
            _offFlagInputSignal, 
            _deviationInvalidFlagInputSignal, 
            _rangeInvalidFlagInputSignal, 
            _toFlagInputSignal,
            _fromFlagInputSignal
        };

        private static OutputChannels[] DigitalOutputChannels => new[]
        {
            OutputChannels.DIG_OUT_A,
            OutputChannels.DIG_OUT_B,
            OutputChannels.DIG_OUT_X,
        };

        public override DigitalSignal[] DigitalOutputs
        {
            get
            {
                return _digitalOutputs
                    .OrderBy(x => x.FriendlyName)
                    .ToArray();
            }
        }

        public override string FriendlyName =>
                    $"Henkie F-16 HSI Interface Board #2: 0x{_hsiBoard2DeviceAddress.ToString("X").PadLeft(2, '0')} on {_hsiBoard2DeviceConfig.ConnectionType?.ToString() ?? "UNKNOWN"} [ {_hsiBoard2DeviceConfig.COMPort ?? "<UNKNOWN>"} ]";


        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>();

            try
            {
                var legacyPath  = Path.Combine(Util.CurrentMappingProfileDirectory, "HenkF16HSIBoard2HardwareSupportModule.config");
                var unifiedPath = Path.Combine(Util.CurrentMappingProfileDirectory, "HenkF16HSIBoard2Calibration.config");
                var hsmConfig = HenkieF16HSIBoard2HardwareSupportModuleConfig.Load(legacyPath);

                // Try the unified-schema calibration file (authored by the
                // SimLinkup Profile Editor). When present, its single
                // breakpoint table overrides the legacy file's
                // <CourseDeviationIndicatorCalibrationData> block. See
                // HenkF16HSIBoard2CalibrationConfig.cs for the rationale.
                var unifiedCalibration = TryLoadUnifiedCalibration(unifiedPath);

                if (hsmConfig != null)
                {
                    foreach (var deviceConfiguration in hsmConfig.Devices)
                    {
                        if (unifiedCalibration != null)
                        {
                            deviceConfiguration.CourseDeviationIndicatorCalibrationData = unifiedCalibration;
                        }
                        var hsmInstance = new HenkF16HSIBoard2HardwareSupportModule(deviceConfiguration, legacyPath, unifiedPath);
                        toReturn.Add(hsmInstance);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }

            return toReturn.ToArray();
        }

        // Load + map the unified-schema calibration file's course-deviation
        // breakpoints onto Henkie.Common.CalibrationPoint. Returns null when
        // the file is absent, malformed, or doesn't contain the expected
        // channel — caller falls back to the legacy <CourseDeviation
        // IndicatorCalibrationData> block.
        private static CalibrationPoint[] TryLoadUnifiedCalibration(string unifiedPath)
        {
            try
            {
                if (!File.Exists(unifiedPath)) return null;
                var unified = GaugeCalibrationConfig.Load<HenkF16HSIBoard2CalibrationConfig>(unifiedPath);
                if (unified == null) return null;
                var ch = unified.FindChannel("Henk_F16_HSI_Board2_Course_Deviation_Indicator_Position_To_Instrument");
                var bps = ch?.Transform?.Breakpoints;
                if (bps == null || bps.Length < 2) return null;
                var result = new CalibrationPoint[bps.Length];
                for (var i = 0; i < bps.Length; i++)
                {
                    result[i] = new CalibrationPoint(bps[i].Input, bps[i].Output);
                }
                return result;
            }
            catch (Exception e)
            {
                Log.Warn("Failed to load unified-schema calibration; falling back to legacy <CourseDeviationIndicatorCalibrationData>: " + e.Message);
                return null;
            }
        }

        // Watch both calibration files. Either changing triggers a reload
        // of just the calibration table — same pattern as Board 1 / FuelFlow.
        private void StartConfigWatchers()
        {
            try
            {
                if (!string.IsNullOrEmpty(_legacyConfigPath))
                {
                    _legacyConfigWatcher = new ConfigFileReloadWatcher(_legacyConfigPath, ReloadCalibration);
                }
            }
            catch (Exception e) { Log.Error(e.Message, e); }
            try
            {
                if (!string.IsNullOrEmpty(_unifiedConfigPath))
                {
                    _unifiedConfigWatcher = new ConfigFileReloadWatcher(_unifiedConfigPath, ReloadCalibration);
                }
            }
            catch (Exception e) { Log.Error(e.Message, e); }
        }

        // Re-evaluate the course-deviation calibration without touching the
        // live device connection. Preference: unified file's value when
        // present (editor's source of truth); otherwise the legacy file's
        // <CourseDeviationIndicatorCalibrationData> block.
        private void ReloadCalibration()
        {
            CalibrationPoint[] next = null;
            try
            {
                next = TryLoadUnifiedCalibration(_unifiedConfigPath);
                if (next == null && !string.IsNullOrEmpty(_legacyConfigPath) && File.Exists(_legacyConfigPath))
                {
                    var legacy = HenkieF16HSIBoard2HardwareSupportModuleConfig.Load(_legacyConfigPath);
                    var dev = legacy?.Devices?.FirstOrDefault(d =>
                        d != null && string.Equals(d.Address, _hsiBoard2DeviceConfig?.Address, StringComparison.OrdinalIgnoreCase));
                    if (dev?.CourseDeviationIndicatorCalibrationData != null
                        && dev.CourseDeviationIndicatorCalibrationData.Length >= 2)
                    {
                        next = dev.CourseDeviationIndicatorCalibrationData;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warn("Calibration reload failed; keeping previous table: " + e.Message);
                return;
            }
            if (next != null && next.Length >= 2)
            {
                _courseDeviationIndicatorCalibrationData = next;
                // Re-fire the course-deviation update with the cached input
                // values so the user sees new calibration immediately.
                CourseDeviationOrCourseDeviationLimitInputSignalsChanged();
            }
        }

        private List<DigitalSignal> CreateOutputSignalsForDigitalOutputChannels()
        {
            return DigitalOutputChannels
                .Select(x => CreateOutputSignalForOutputChannelConfiguredAsDigital(x))
                .ToList();
        }
        private DigitalSignal CreateOutputSignalForOutputChannelConfiguredAsDigital(OutputChannels outputChannel)
        {
            var thisSignal = new DigitalSignal
            {

                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = $"{outputChannel} (0=OFF, 1=ON)",
                Id = $"Henk_F16_HSI_Board2[{"0x" + _hsiBoard2DeviceAddress.ToString("X").PadLeft(2, '0')}]__{outputChannel}",
                Index = (int)outputChannel,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = OutputChannelInitialValue(outputChannel)
            };
            return thisSignal;
        }

        private void ConfigureDevice()
        {
            ConfigureDeviceConnection();
            ConfigureStatorOffsets();
            ConfigureDiagnosticLEDBehavior();
            ConfigureOutputChannels();
            ConfigureCalibration();
            ConfigureUsbMessagingOptions();
            ConfigureCourseAndHeadingValueHysteresisThresholds();
            ConfigureCourse45DegreeSinCosCrossoverValue();
        }

        private void ConfigureCourse45DegreeSinCosCrossoverValue()
        {
            if (_hsiBoard2DeviceInterface == null || _hsiBoard2DeviceConfig == null) return;
            _hsiBoard2DeviceInterface.SetCourse45DegreeSinCosCrossoverValue(_hsiBoard2DeviceConfig.Course45DegreeSinCosCrossover);
        }

        private void ConfigureCourseAndHeadingValueHysteresisThresholds()
        {
            if (_hsiBoard2DeviceInterface == null || _hsiBoard2DeviceConfig == null) return;
            _hsiBoard2DeviceInterface.SetCourseValueHysteresisThreshold(_hsiBoard2DeviceConfig.CourseValueHysteresisThreshold);
            _hsiBoard2DeviceInterface.SetHeadingValueHysteresisThreshold(_hsiBoard2DeviceConfig.HeadingValueHysteresisThreshold);
        }

        private void ConfigureUsbMessagingOptions()
        {
            if (_hsiBoard2DeviceInterface == null) return;
            _hsiBoard2DeviceInterface.SetUsbMessagingOption(UsbMessagingOption.SendOnlyOnRequest);
        }

        private void ConfigureStatorOffsets()
        {
            if (_hsiBoard2DeviceInterface == null || _hsiBoard2DeviceConfig?.StatorOffsetsConfig == null) return;

            //COURSE EXCITER stator offsets
            try
            {
                var courseExciterS1StatorOffset = _hsiBoard2DeviceConfig?.StatorOffsetsConfig?.CourseExciterS1Offset ?? 241;
                var courseExciterS2StatorOffset = _hsiBoard2DeviceConfig?.StatorOffsetsConfig?.CourseExciterS2Offset ?? 582;
                var courseExciterS3StatorOffset = _hsiBoard2DeviceConfig?.StatorOffsetsConfig?.CourseExciterS3Offset ?? 923;

                _hsiBoard2DeviceInterface.SetCourseSynchroExciterStatorCoilOffset((short)courseExciterS1StatorOffset);
                _hsiBoard2DeviceInterface.LoadCourseSynchroExciterStatorCoilOffset(StatorSignals.S1);
                _hsiBoard2DeviceInterface.SetCourseSynchroExciterStatorCoilOffset((short)courseExciterS2StatorOffset);
                _hsiBoard2DeviceInterface.LoadCourseSynchroExciterStatorCoilOffset(StatorSignals.S2);
                _hsiBoard2DeviceInterface.SetCourseSynchroExciterStatorCoilOffset((short)courseExciterS3StatorOffset);
                _hsiBoard2DeviceInterface.LoadCourseSynchroExciterStatorCoilOffset(StatorSignals.S3);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }
        private void ConfigureCalibration()
        {
            if (_hsiBoard2DeviceInterface == null) return;
            _courseDeviationIndicatorCalibrationData = _hsiBoard2DeviceConfig?.CourseDeviationIndicatorCalibrationData;
        }

        private void ConfigureDeviceConnection()
        {
            try
            {
                if (
                    _hsiBoard2DeviceConfig?.ConnectionType != null &&
                    _hsiBoard2DeviceConfig.ConnectionType.Value == ConnectionType.USB &&
                    !string.IsNullOrWhiteSpace(_hsiBoard2DeviceConfig.COMPort)
                )
                {
                    ConfigureUSBConnection();
                }
                else if (
                    _hsiBoard2DeviceConfig?.ConnectionType != null &&
                    _hsiBoard2DeviceConfig.ConnectionType.Value == ConnectionType.PHCC &&
                    !string.IsNullOrWhiteSpace(_hsiBoard2DeviceConfig.COMPort) &&
                    !string.IsNullOrWhiteSpace(_hsiBoard2DeviceConfig.Address)
                )
                {
                    ConfigurePhccConnection();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private void ConfigureDiagnosticLEDBehavior()
        {
            if (_hsiBoard2DeviceInterface == null) return;

            var diagnosticLEDBehavior = _hsiBoard2DeviceConfig?.DiagnosticLEDMode != null &&
                _hsiBoard2DeviceConfig.DiagnosticLEDMode.HasValue
                ? _hsiBoard2DeviceConfig.DiagnosticLEDMode.Value
                : DiagnosticLEDMode.Heartbeat;
            try
            {
                _hsiBoard2DeviceInterface.ConfigureDiagnosticLEDBehavior(diagnosticLEDBehavior);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }
        private void ConfigureOutputChannels()
        {
            if (_hsiBoard2DeviceInterface == null) return;

            try
            {
                _hsiBoard2DeviceInterface.SetDigitalOutputChannelValue(OutputChannels.DIG_OUT_A,
                    OutputChannelInitialValue(OutputChannels.DIG_OUT_A));
                _hsiBoard2DeviceInterface.SetDigitalOutputChannelValue(OutputChannels.DIG_OUT_B,
                    OutputChannelInitialValue(OutputChannels.DIG_OUT_B));
                _hsiBoard2DeviceInterface.SetDigitalOutputChannelValue(OutputChannels.DIG_OUT_X,
                    OutputChannelInitialValue(OutputChannels.DIG_OUT_X));
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private bool OutputChannelInitialValue(OutputChannels outputChannel)
        {
            switch (outputChannel)
            {
                case OutputChannels.DIG_OUT_A:
                    return _hsiBoard2DeviceConfig?.OutputChannelsConfig?.DIG_OUT_A?.InitialValue ?? false;
                case OutputChannels.DIG_OUT_B:
                    return _hsiBoard2DeviceConfig?.OutputChannelsConfig?.DIG_OUT_B?.InitialValue ?? false;
                case OutputChannels.DIG_OUT_X:
                    return _hsiBoard2DeviceConfig?.OutputChannelsConfig?.DIG_OUT_X?.InitialValue ?? false;
            }
            return false;
        }
        private void ConfigurePhccConnection()
        {
            if
            (
                _hsiBoard2DeviceConfig?.ConnectionType == null ||
                _hsiBoard2DeviceConfig.ConnectionType.Value != ConnectionType.PHCC ||
                string.IsNullOrWhiteSpace(_hsiBoard2DeviceConfig.COMPort) || string.IsNullOrWhiteSpace(_hsiBoard2DeviceConfig.Address)
            )
            {
                return;
            }

            var addressString = (_hsiBoard2DeviceConfig.Address ?? "").ToLowerInvariant().Replace("0x", string.Empty).Trim();
            var addressIsValid = byte.TryParse(addressString, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out var addressByte);
            if (!addressIsValid) return;

            _hsiBoard2DeviceAddress = addressByte;
            var comPort = (_hsiBoard2DeviceConfig.COMPort ?? "").Trim();

            try
            {
                var phccDevice = new p.Device(comPort, false);
                _hsiBoard2DeviceInterface = new Device(phccDevice, addressByte);
                _hsiBoard2DeviceInterface.DisableWatchdog();
            }
            catch (Exception e)
            {
                Common.Util.DisposeObject(_hsiBoard2DeviceInterface);
                _hsiBoard2DeviceInterface = null;
                Log.Error(e.Message, e);
            }
        }

        private void ConfigureUSBConnection()
        {
            if (
                _hsiBoard2DeviceConfig?.ConnectionType == null || _hsiBoard2DeviceConfig.ConnectionType.Value !=
                ConnectionType.USB &&
                string.IsNullOrWhiteSpace(_hsiBoard2DeviceConfig.COMPort)
            )
            {
                return;
            }

            var addressString = (_hsiBoard2DeviceConfig.Address ?? "").ToLowerInvariant().Replace("0x", string.Empty).Trim();
            var addressIsValid = byte.TryParse(addressString, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out var addressByte);
            if (!addressIsValid) return;
            _hsiBoard2DeviceAddress = addressByte;

            try
            {
                var comPort = _hsiBoard2DeviceConfig.COMPort;
                _hsiBoard2DeviceInterface = new Device(comPort);
                _hsiBoard2DeviceInterface.DisableWatchdog();
                _hsiBoard2DeviceInterface.ConfigureUsbDebug(false);
            }
            catch (Exception e)
            {
                Common.Util.DisposeObject(_hsiBoard2DeviceInterface);
                _hsiBoard2DeviceInterface = null;
                Log.Error(e.Message, e);
            }
        }
        private void SetInitialState()
        {
            SetDeviationInvalidFlagVisibility(isVisible: false);
        }
        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.BearingToBeaconDegrees = (float)_bearingInputSignal.CorrelatedState;
            _renderer.InstrumentState.CourseDeviationDegrees = (float)_courseDeviationInputSignal.State;
            _renderer.InstrumentState.CourseDeviationLimitDegrees = (float)_courseDeviationLimitInputSignal.State;
            _renderer.InstrumentState.DesiredCourseDegrees = (int)_courseInputSignal.CorrelatedState;
            _renderer.InstrumentState.DesiredHeadingDegrees = (int)_desiredHeadingFromSimInputSignal.CorrelatedState;
            _renderer.InstrumentState.DeviationInvalidFlag = _deviationInvalidFlagInputSignal.State;
            _renderer.InstrumentState.DistanceToBeaconNauticalMiles = (float)_rangeInputSignal.State;
            _renderer.InstrumentState.DmeInvalidFlag = _rangeInvalidFlagInputSignal.State;
            _renderer.InstrumentState.FromFlag = _fromFlagInputSignal.State;
            _renderer.InstrumentState.MagneticHeadingDegrees = (float)_magneticHeadingInputSignal.CorrelatedState;
            _renderer.InstrumentState.OffFlag = _offFlagInputSignal.State;
            _renderer.InstrumentState.ShowToFromFlag = true;
            _renderer.InstrumentState.ToFlag = _toFlagInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }
        private void CreateInputSignals()
        {
            _offFlagInputSignal = CreateOffFlagInputSignal();
            _deviationInvalidFlagInputSignal = CreateDeviationInvalidFlagInputSignal();
            _rangeInvalidFlagInputSignal = CreateRangeInvalidFlagInputSignal();
            _toFlagInputSignal = CreateToFlagInputSignal();
            _fromFlagInputSignal = CreateFromFlagInputSignal();
            _magneticHeadingInputSignal = CreateMagneticHeadingInputSignal();
            _desiredHeadingFromSimInputSignal = CreateDesiredHeadingFromSimInputSignal();
            _courseInputSignal = CreateCourseInputSignal();
            _bearingInputSignal = CreateBearingInputSignal();
            _rangeInputSignal = CreateRangeInputSignal();
            _courseDeviationInputSignal = CreateCourseDeviationInputSignal();
            _courseDeviationLimitInputSignal = CreateCourseDeviationLimitInputSignal();
        }

        private AnalogSignal CreateMagneticHeadingInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Current Heading (from sim)",
                Id = $"Henk_F16_HSI_Board2__Magnetic_Heading_From_Sim",
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

        private AnalogSignal CreateDesiredHeadingFromSimInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Desired Heading (from sim)",
                Id = $"Henk_F16_HSI_Board2__Desired_Heading_From_Sim",
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


        private AnalogSignal CreateBearingInputSignal()
        {

            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Bearing to Beacon (from sim)",
                Id = $"Henk_F16_HSI_Board2__Bearing_From_Sim",
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

        private AnalogSignal CreateCourseInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Desired Course (from sim)",
                Id = $"Henk_F16_HSI_Board2__Course_From_Sim",
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

        private AnalogSignal CreateCourseDeviationInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Course Deviation (from sim)",
                Id = $"Henk_F16_HSI_Board2__Course_Deviation_From_Sim",
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
                FriendlyName = "Course Deviation Limit (from sim)",
                Id = $"Henk_F16_HSI_Board2__Course_Deviation_Limit_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 10,
                IsAngle = true,
                MinValue = 0,
                MaxValue = 10
            };
            return thisSignal;
        }
        private DigitalSignal CreateDeviationInvalidFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "Deviation Invalid Flag (from sim)",
                Id = $"Henk_F16_HSI_Board2__Deviation_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private AnalogSignal CreateRangeInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Range (from sim)",
                Id = $"Henk_F16_HSI_Board2__Range_From_Sim",
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

        private DigitalSignal CreateRangeInvalidFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "Range Invalid Flag (from sim)",
                Id = $"Henk_F16_HSI_Board2__Range_Invalid_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private DigitalSignal CreateToFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "TO flag (from sim)",
                Id = $"Henk_F16_HSI_Board2__TO_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private DigitalSignal CreateFromFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "FROM Flag (from sim)",
                Id = $"Henk_F16_HSI_Board2__FROM_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private DigitalSignal CreateOffFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "OFF flag (from sim)",
                Id = $"Henk_F16_HSI_Board2__Off_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };
            return thisSignal;
        }

        private AnalogSignal CreateCourseDeviationIndicatorPositionOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Course Deviation Indicator Position (0-1023)",
                Id = $"Henk_F16_HSI_Board2[{"0x" + _hsiBoard2DeviceAddress.ToString("X").PadLeft(2, '0')}]__Course_Deviation_Indicator_Position_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 10,
                IsVoltage = false,
                IsSine = false,
                MinValue = 0,
                MaxValue = 1023
            };
            return thisSignal;
        }

        private AnalogSignal CreateCourseArrowPositionOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Course Arrow Position (0-1023)",
                Id = $"Henk_F16_HSI_Board2[{"0x" + _hsiBoard2DeviceAddress.ToString("X").PadLeft(2, '0')}]__Course_Arrow_Position_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsVoltage = false,
                IsSine = false,
                MinValue = 0,
                MaxValue = 1023
            };
            return thisSignal;
        }

        private DigitalSignal CreateDeviationInvalidFlagOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "Deviation Invalid Flag (0 = visible, 1 = not visible)",
                Id = $"Henk_F16_HSI_Board2[{"0x" + _hsiBoard2DeviceAddress.ToString("X").PadLeft(2, '0')}]__Deviation_Invalid_Flag_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private AnalogSignal CreateToFromFlagsOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "TO/FROM Indication (0 = none, 1 = TO, 2 = FROM)",
                Id = $"Henk_F16_HSI_Board2[{"0x" + _hsiBoard2DeviceAddress.ToString("X").PadLeft(2, '0')}]__TO_FROM_Indication_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsVoltage = false,
                IsSine = false,
                MinValue = 0,
                MaxValue = 2
            };
            return thisSignal;
        }

        private void CreateOutputSignals()
        {
            _courseDeviationIndicatorOutputSignal = CreateCourseDeviationIndicatorPositionOutputSignal();
            _deviationInvalidFlagOutputSignal = CreateDeviationInvalidFlagOutputSignal();
            _toFromFlagsOutputSignal = CreateToFromFlagsOutputSignal();
            _courseArrowPositionOutputSignal = CreateCourseArrowPositionOutputSignal();
            _digitalOutputs = CreateOutputSignalsForDigitalOutputChannels();
            _digitalOutputs.Add(_deviationInvalidFlagOutputSignal);
        }

        private void RegisterForEvents()
        {
            if (_courseDeviationInputSignal != null)
            {
                _courseDeviationInputSignal.SignalChanged += CourseDeviationInputSignal_SignalChanged;
            }
            if (_courseDeviationLimitInputSignal != null)
            {
                _courseDeviationLimitInputSignal.SignalChanged += CourseDeviationLimitInputSignal_SignalChanged;
            }

            if (_courseDeviationIndicatorOutputSignal != null)
            {
                _courseDeviationIndicatorOutputSignal.SignalChanged += CourseDeviationIndicatorOutputSignal_SignalChanged;
            }

            if (_deviationInvalidFlagInputSignal != null)
            {
                _deviationInvalidFlagInputSignal.SignalChanged += DeviationInvalidFlagInputSignal_SignalChanged;
            }
            if (_deviationInvalidFlagOutputSignal != null)
            {
                _deviationInvalidFlagOutputSignal.SignalChanged += DeviationInvalidFlagOutputSignal_SignalChanged;
            }

            if (_toFlagInputSignal != null)
            {
                _toFlagInputSignal.SignalChanged += ToFlagInputSignal_SignalChanged;
            }
            if (_fromFlagInputSignal != null)
            {
                _fromFlagInputSignal.SignalChanged += FromFlagInputSignal_SignalChanged;
            }
            if (_toFromFlagsOutputSignal != null)
            {
                _toFromFlagsOutputSignal.SignalChanged += ToFromFlagsOutputSignal_SignalChanged;
            }

            foreach (var digitalSignal in _digitalOutputs)
            {
                digitalSignal.SignalChanged += OutputSignalForDigitalOutputChannel_SignalChanged;
            }
        }

        private void UnregisterForEvents()
        {
            if (_courseDeviationInputSignal != null)
            {
                try
                {
                    _courseDeviationInputSignal.SignalChanged -= CourseDeviationInputSignal_SignalChanged;
                }
                catch (RemotingException) { }
            }

            if (_courseDeviationLimitInputSignal != null)
            {
                try
                {
                    _courseDeviationLimitInputSignal.SignalChanged -= CourseDeviationLimitInputSignal_SignalChanged;
                }
                catch (RemotingException) { }
            }
            if (_courseDeviationIndicatorOutputSignal != null)
            {
                try
                {
                    _courseDeviationIndicatorOutputSignal.SignalChanged -= CourseDeviationIndicatorOutputSignal_SignalChanged;
                }
                catch (RemotingException) { }
            }

            if (_deviationInvalidFlagInputSignal != null)
            {
                try
                {
                    _deviationInvalidFlagInputSignal.SignalChanged -= DeviationInvalidFlagInputSignal_SignalChanged;
                }
                catch (RemotingException) { }
            }
            if (_deviationInvalidFlagOutputSignal != null)
            {
                try
                {
                    _deviationInvalidFlagOutputSignal.SignalChanged -= DeviationInvalidFlagOutputSignal_SignalChanged;
                }
                catch (RemotingException) { }
            }

            if (_toFlagInputSignal != null)
            {
                try
                {
                    _toFlagInputSignal.SignalChanged -= ToFlagInputSignal_SignalChanged;
                }
                catch (RemotingException) { }
            }
            if (_fromFlagInputSignal != null)
            {
                try
                {
                    _fromFlagInputSignal.SignalChanged -= FromFlagInputSignal_SignalChanged;
                }
                catch (RemotingException) { }
            }
            if (_toFromFlagsOutputSignal != null)
            {
                try
                {
                    _toFromFlagsOutputSignal.SignalChanged -= ToFromFlagsOutputSignal_SignalChanged;
                }
                catch (RemotingException) { }
            }

            foreach (var digitalSignal in _digitalOutputs)
            {
                try
                {
                    digitalSignal.SignalChanged -= OutputSignalForDigitalOutputChannel_SignalChanged;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void CourseDeviationInputSignal_SignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            CourseDeviationOrCourseDeviationLimitInputSignalsChanged();
        }

        private void CourseDeviationLimitInputSignal_SignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            CourseDeviationOrCourseDeviationLimitInputSignalsChanged();
        }

        private void CourseDeviationOrCourseDeviationLimitInputSignalsChanged()
        {
            if (_hsiBoard2DeviceInterface != null && _courseDeviationIndicatorOutputSignal != null && _courseDeviationInputSignal != null && _courseDeviationLimitInputSignal != null)
            {
                var calibratedValue = CalibratedCourseDeviationIndicatorPositionValue(_courseDeviationInputSignal.State, _courseDeviationLimitInputSignal.State);
                _courseDeviationIndicatorOutputSignal.State = calibratedValue;
            }
        }

        private short? _lastCourseDeviationIndicatorState = null;
        private void CourseDeviationIndicatorOutputSignal_SignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            if (_hsiBoard2DeviceInterface != null && _courseDeviationIndicatorOutputSignal != null)
            {
                var newCourseDeviationIndicatorState = (short)_courseDeviationIndicatorOutputSignal.State;
                if (newCourseDeviationIndicatorState != _lastCourseDeviationIndicatorState)
                {
                    _hsiBoard2DeviceInterface.SetCourseDeviationIndication(newCourseDeviationIndicatorState);
                    _lastCourseDeviationIndicatorState = newCourseDeviationIndicatorState;
                }
            }
        }

        private void DeviationInvalidFlagInputSignal_SignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            if (_hsiBoard2DeviceInterface != null && _deviationInvalidFlagOutputSignal != null && _deviationInvalidFlagInputSignal != null)
            {
                _deviationInvalidFlagOutputSignal.State = _deviationInvalidFlagInputSignal.State;
            }
        }

        private void DeviationInvalidFlagOutputSignal_SignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            if (_deviationInvalidFlagOutputSignal != null)
            {
                SetDeviationInvalidFlagVisibility(args.CurrentState);
            }
        }

        private void SetDeviationInvalidFlagVisibility(bool isVisible)
        {
            if (_hsiBoard2DeviceInterface != null )
            {
                _hsiBoard2DeviceInterface.SetDeviationInvalidFlagVisible(isVisible);
            }
        }

        private void FromFlagInputSignal_SignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            ToFromFlagInputSignalsChanged();
        }

        private void ToFlagInputSignal_SignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            ToFromFlagInputSignalsChanged();
        }

        private void ToFromFlagInputSignalsChanged()
        {
            if (_hsiBoard2DeviceInterface != null && _toFromFlagsOutputSignal != null && _toFlagInputSignal != null && _fromFlagInputSignal != null)
            {
                _toFromFlagsOutputSignal.State = (double)
                    (_toFlagInputSignal.State
                        ? ToFromFlagsState.To
                        : _fromFlagInputSignal.State
                            ? ToFromFlagsState.From
                            : ToFromFlagsState.None);
            }
        }

        private void ToFromFlagsOutputSignal_SignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            if (_hsiBoard2DeviceInterface != null && _toFromFlagsOutputSignal != null)
            {
                _hsiBoard2DeviceInterface.SetToFromFlagsVisible((ToFromFlagsState)(byte)_toFromFlagsOutputSignal.State);
            }
        }

        private void OutputSignalForDigitalOutputChannel_SignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            if (_hsiBoard2DeviceInterface == null) return;
            var signal = (DigitalSignal)sender;
            var outputChannel = (OutputChannels)signal.Index;
            try
            {
                if (outputChannel > 0)
                {
                    _hsiBoard2DeviceInterface.SetDigitalOutputChannelValue(outputChannel, args.CurrentState);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private ushort CalibratedCourseDeviationIndicatorPositionValue(double courseDeviationDegrees, double courseDeviationLimitDegrees)
        {
            if (double.IsNaN(courseDeviationDegrees) || double.IsInfinity(courseDeviationDegrees)) courseDeviationDegrees = 0;
            if (double.IsNaN(courseDeviationLimitDegrees) || double.IsInfinity(courseDeviationLimitDegrees) || courseDeviationLimitDegrees == 0) courseDeviationLimitDegrees = 10;

            var courseDeviationPct = (courseDeviationDegrees / courseDeviationLimitDegrees);
            if (_courseDeviationIndicatorCalibrationData == null)
            {
                return (ushort)(511.5 + courseDeviationPct * 511.5);
            }

            var lowerPoint = _courseDeviationIndicatorCalibrationData.OrderBy(x => x.Input).LastOrDefault(x => x.Input <= courseDeviationPct) ??
                             new CalibrationPoint(-1.0, 0);
            var upperPoint =
                _courseDeviationIndicatorCalibrationData
                    .OrderBy(x => x.Input)
                    .FirstOrDefault(x => x != lowerPoint && x.Input >= lowerPoint.Input) ?? new CalibrationPoint(1.0, 1023);
            var inputRange = Math.Abs(upperPoint.Input - lowerPoint.Input);
            var outputRange = Math.Abs(upperPoint.Output - lowerPoint.Output);
            var inputPct = inputRange != 0
                ? (courseDeviationPct - lowerPoint.Input) / inputRange
                : 1.00;
            return (ushort)((inputPct * outputRange) + lowerPoint.Output);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    UnregisterForEvents();
                    Common.Util.DisposeObject(_renderer);
                    if (_legacyConfigWatcher != null)
                    {
                        try { _legacyConfigWatcher.Dispose(); } catch { }
                        _legacyConfigWatcher = null;
                    }
                    if (_unifiedConfigWatcher != null)
                    {
                        try { _unifiedConfigWatcher.Dispose(); } catch { }
                        _unifiedConfigWatcher = null;
                    }
                }
            }
            _isDisposed = true;
        }

        ~HenkF16HSIBoard2HardwareSupportModule()
        {
            Dispose(false);
        }
    }
}