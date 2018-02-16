using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using Henkie.Common;
using log4net;
using Phcc;
using SDIDriver = Henkie.SDI;

namespace SimLinkup.HardwareSupport.Henk.SDI
{
    //Henk Synchro Drive Interface Hardware Support Module
    public class HenkSDIHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(HenkSDIHardwareSupportModule));
        private readonly DeviceConfig _deviceConfig;
        private byte _deviceAddress;
        private List<DigitalSignal> _digitalOutputs = new List<DigitalSignal>();
        private List<CalibratedAnalogSignal> _analogOutputs = new List<CalibratedAnalogSignal>();

        private bool _isDisposed;
        private CalibratedAnalogSignal _positionOutputSignal;
        private SDIDriver.Device _sdiDevice;

        private HenkSDIHardwareSupportModule(DeviceConfig deviceConfig)
        {
            _deviceConfig = deviceConfig;
            if (_deviceConfig != null)
            {
                ConfigureDevice();
                CreateOutputSignals();
                RegisterForDataChangeEvents();
            }
        }

        public override AnalogSignal[] AnalogOutputs
        {
            get
            {
                return _analogOutputs
                    .Union(new[] {_positionOutputSignal})
                    .OrderBy(x => x.FriendlyName)
                    .Cast<AnalogSignal>()
                    .ToArray();
            }
        }

        public override AnalogSignal[] AnalogInputs=> null;

        public override DigitalSignal[] DigitalOutputs
        {
            get
            {
                return _digitalOutputs
                    .OrderBy(x => x.FriendlyName)
                    .ToArray();
            }
        }

        public override DigitalSignal[] DigitalInputs => null;

        public override string FriendlyName =>
            $"Henk Synchro Drive Interface: 0x{_deviceAddress.ToString("X").PadLeft(2, '0')} {(string.IsNullOrWhiteSpace(DeviceFunction) ? string.Empty : $"(\"{DeviceFunction}\")")} on {_deviceConfig.ConnectionType?.ToString() ?? "UNKNOWN"} [ {_deviceConfig.COMPort ?? "<UNKNOWN>"} ]";

        private static SDIDriver.OutputChannels[] AllOutputChannels => new[]
        {
            SDIDriver.OutputChannels.DIG_PWM_1,
            SDIDriver.OutputChannels.DIG_PWM_2,
            SDIDriver.OutputChannels.DIG_PWM_3,
            SDIDriver.OutputChannels.DIG_PWM_4,
            SDIDriver.OutputChannels.DIG_PWM_5,
            SDIDriver.OutputChannels.DIG_PWM_6,
            SDIDriver.OutputChannels.DIG_PWM_7,
            SDIDriver.OutputChannels.PWM_OUT
        };

        private string DeviceFunction => _deviceAddress == 0x30 || _deviceAddress == 0x48
            ? "PITCH"
            : _deviceAddress == 0x32 || _deviceAddress == 0x50
                ? "ROLL"
                : string.Empty;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HenkSDIHardwareSupportModule()
        {
            Dispose();
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>();

            try
            {
                var hsmConfigFilePath = Path.Combine(Util.CurrentMappingProfileDirectory, "henksdi.config");
                var hsmConfig = HenkSDIHardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    foreach (var deviceConfiguration in hsmConfig.Devices)
                    {
                        var hsmInstance = new HenkSDIHardwareSupportModule(deviceConfiguration);
                        toReturn.Add(hsmInstance);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }

            return toReturn.ToArray();
        }

        private static int ChannelNumber(SDIDriver.OutputChannels outputChannel)
        {
            var lastCharOfChannelName = outputChannel.ToString().Substring(outputChannel.ToString().Length - 1, 1);
            int.TryParse(lastCharOfChannelName, out var channelNumber);
            return channelNumber > 0 ? channelNumber : 8;
        }

        private void ConfigureDefaultIndicatorPosition()
        {
            if (_sdiDevice == null) return;
            var initialIndicatorPosition = _deviceConfig?.InitialIndicatorPosition ?? 512;
            MoveIndicatorToPositionCoarse(initialIndicatorPosition);
        }

        private void ConfigureDevice()
        {
            ConfigureDeviceConnection();
            //ConfigureStatorBaseAngles();
            ConfigureDiagnosticLEDBehavior();
            ConfigureOutputChannels();
            ConfigureDefaultIndicatorPosition();
            ConfigureMovementLimits();
            ConfigurePowerDown();
            ConfigureUpdateRateControl();
        }

        private void ConfigureDeviceConnection()
        {
            try
            {
                if (
                    _deviceConfig?.ConnectionType != null &&
                    _deviceConfig.ConnectionType.Value == ConnectionType.USB &&
                    !string.IsNullOrWhiteSpace(_deviceConfig.COMPort)
                )
                {
                    ConfigureUSBConnection();
                }
                else if (
                    _deviceConfig?.ConnectionType != null &&
                    _deviceConfig.ConnectionType.Value == ConnectionType.PHCC &&
                    !string.IsNullOrWhiteSpace(_deviceConfig.COMPort) &&
                    !string.IsNullOrWhiteSpace(_deviceConfig.Address)
                )
                {
                    ConfigurePhccConnection();
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private void ConfigureDiagnosticLEDBehavior()
        {
            if (_sdiDevice == null) return;

            var diagnosticLEDBehavior = _deviceConfig?.UpdateRateControlConfig != null &&
                                        _deviceConfig.DiagnosticLEDMode.HasValue
                ? _deviceConfig.DiagnosticLEDMode.Value
                : DiagnosticLEDMode.Heartbeat;
            try
            {
                _sdiDevice.ConfigureDiagnosticLEDBehavior(diagnosticLEDBehavior);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private void ConfigureMovementLimits()
        {
            if (_sdiDevice == null) return;

            var min = _deviceConfig?.MovementLimitsConfig?.Min ?? (DeviceFunction == "PITCH"
                          ? (byte) 35 //default maximum for PITCH device if not specified in config file
                          : (byte) 0); //no minimum by default for anything except PITCH SDI device

            var max = _deviceConfig?.MovementLimitsConfig?.Max ?? (DeviceFunction == "PITCH"
                          ? (byte) 175 //default maximum for PITCH device if not specified in config file
                          : (byte) 255); //255=no maximum by default for anything except PITCH SDI device

            try
            {
                _sdiDevice.SetIndicatorMovementLimitMinimum(min);
                _sdiDevice.SetIndicatorMovementLimitMaximum(max);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private void ConfigureOutputChannels()
        {
            if (_sdiDevice == null) return;

            try
            {
                _sdiDevice.ConfigureOutputChannels(
                    OutputChannelMode(SDIDriver.OutputChannels.DIG_PWM_1),
                    OutputChannelMode(SDIDriver.OutputChannels.DIG_PWM_2),
                    OutputChannelMode(SDIDriver.OutputChannels.DIG_PWM_3),
                    OutputChannelMode(SDIDriver.OutputChannels.DIG_PWM_4),
                    OutputChannelMode(SDIDriver.OutputChannels.DIG_PWM_5),
                    OutputChannelMode(SDIDriver.OutputChannels.DIG_PWM_6),
                    OutputChannelMode(SDIDriver.OutputChannels.DIG_PWM_7)
                );

                _sdiDevice.SetOutputChannelValue(SDIDriver.OutputChannels.DIG_PWM_1,
                    OutputChannelInitialValue(SDIDriver.OutputChannels.DIG_PWM_1));
                _sdiDevice.SetOutputChannelValue(SDIDriver.OutputChannels.DIG_PWM_2,
                    OutputChannelInitialValue(SDIDriver.OutputChannels.DIG_PWM_2));
                _sdiDevice.SetOutputChannelValue(SDIDriver.OutputChannels.DIG_PWM_3,
                    OutputChannelInitialValue(SDIDriver.OutputChannels.DIG_PWM_3));
                _sdiDevice.SetOutputChannelValue(SDIDriver.OutputChannels.DIG_PWM_4,
                    OutputChannelInitialValue(SDIDriver.OutputChannels.DIG_PWM_4));
                _sdiDevice.SetOutputChannelValue(SDIDriver.OutputChannels.DIG_PWM_5,
                    OutputChannelInitialValue(SDIDriver.OutputChannels.DIG_PWM_5));
                _sdiDevice.SetOutputChannelValue(SDIDriver.OutputChannels.DIG_PWM_6,
                    OutputChannelInitialValue(SDIDriver.OutputChannels.DIG_PWM_6));
                _sdiDevice.SetOutputChannelValue(SDIDriver.OutputChannels.DIG_PWM_7,
                    OutputChannelInitialValue(SDIDriver.OutputChannels.DIG_PWM_7));
                _sdiDevice.SetOutputChannelValue(SDIDriver.OutputChannels.PWM_OUT,
                    OutputChannelInitialValue(SDIDriver.OutputChannels.PWM_OUT));
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private void ConfigurePhccConnection()
        {
            if
            (
                _deviceConfig?.ConnectionType == null ||
                _deviceConfig.ConnectionType.Value != ConnectionType.PHCC ||
                string.IsNullOrWhiteSpace(_deviceConfig.COMPort) || string.IsNullOrWhiteSpace(_deviceConfig.Address)
            )
            {
                return;
            }

            var addressString = (_deviceConfig.Address ?? "").ToLowerInvariant().Replace("0x", string.Empty).Trim();
            var addressIsValid = byte.TryParse(addressString, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out var addressByte);
            if (!addressIsValid) return;

            _deviceAddress = addressByte;
            var comPort = (_deviceConfig.COMPort ?? "").Trim();

            try
            {
                var phccDevice = new Device(comPort, false);
                _sdiDevice = new SDIDriver.Device(phccDevice, addressByte);
                _sdiDevice.DisableWatchdog();
            }
            catch (Exception e)
            {
                Common.Util.DisposeObject(_sdiDevice);
                _sdiDevice = null;
                _log.Error(e.Message, e);
            }
        }

        private void ConfigurePowerDown()
        {
            if (_sdiDevice == null) return;

            var powerDownState = _deviceConfig?.PowerDownConfig?.Enabled != null &&
                                 _deviceConfig.PowerDownConfig.Enabled.Value
                ? SDIDriver.PowerDownState.Enabled
                : SDIDriver.PowerDownState.Disabled;

            var powerDownLevel = _deviceConfig?.PowerDownConfig?.Level ?? SDIDriver.PowerDownLevel.Half;

            var delayTimeMilliseconds = _deviceConfig?.PowerDownConfig?.DelayTimeMilliseconds ?? (short) 0;

            try
            {
                _sdiDevice.ConfigurePowerDown(powerDownState, powerDownLevel, delayTimeMilliseconds);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private void ConfigureStatorBaseAngles()
        {
            if (_sdiDevice == null) return;

            var s1StatorBaseAngle = _deviceConfig?.StatorBaseAnglesConfig?.S1BaseAngleDegrees / 360.000 *
                                    SDIDriver.Device.STATOR_BASE_ANGLE_MAX_OFFSET ?? (DeviceFunction == "PITCH"
                                        ? 240.000 / 360.000 * SDIDriver.Device.STATOR_BASE_ANGLE_MAX_OFFSET
                                        : DeviceFunction == "ROLL"
                                            ? 210.000 / 360.000 * SDIDriver.Device.STATOR_BASE_ANGLE_MAX_OFFSET
                                            : 0.000 / 360.000 * SDIDriver.Device.STATOR_BASE_ANGLE_MAX_OFFSET);

            var s2StatorBaseAngle = _deviceConfig?.StatorBaseAnglesConfig?.S2BaseAngleDegrees / 360.000 *
                                    SDIDriver.Device.STATOR_BASE_ANGLE_MAX_OFFSET ?? (DeviceFunction == "PITCH"
                                        ? 0.000 / 360.000 * SDIDriver.Device.STATOR_BASE_ANGLE_MAX_OFFSET
                                        : DeviceFunction == "ROLL"
                                            ? 330.000 / 360.000 * SDIDriver.Device.STATOR_BASE_ANGLE_MAX_OFFSET
                                            : 120.00 / 360.000 * SDIDriver.Device.STATOR_BASE_ANGLE_MAX_OFFSET);

            var s3StatorBaseAngle = _deviceConfig?.StatorBaseAnglesConfig?.S2BaseAngleDegrees / 360.000 *
                                    SDIDriver.Device.STATOR_BASE_ANGLE_MAX_OFFSET ?? (DeviceFunction == "PITCH"
                                        ? 120.000 / 360.000 * SDIDriver.Device.STATOR_BASE_ANGLE_MAX_OFFSET
                                        : DeviceFunction == "ROLL"
                                            ? 90.000 / 360.000 * SDIDriver.Device.STATOR_BASE_ANGLE_MAX_OFFSET
                                            : 240.00 / 360.000 * SDIDriver.Device.STATOR_BASE_ANGLE_MAX_OFFSET);

            try
            {
                _sdiDevice.SetStatorBaseAngle(StatorSignals.S1, (short) s1StatorBaseAngle);
                _sdiDevice.SetStatorBaseAngle(StatorSignals.S2, (short) s2StatorBaseAngle);
                _sdiDevice.SetStatorBaseAngle(StatorSignals.S3, (short) s3StatorBaseAngle);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private void ConfigureUpdateRateControl()
        {
            if (_sdiDevice == null) return;

            ConfigureUpdateRateControlMode();
            ConfigureUpdateRateControlSpeed();
            ConfigureUpdateRateControlMiscellaneous();
        }

        private void ConfigureUpdateRateControlLimitMode()
        {
            if (_sdiDevice == null) return;
            var limitThreshold = (_deviceConfig?.UpdateRateControlConfig?.ModeSettings as LimitModeSettings)
                                 ?.LimitThreshold ?? (byte) 0;
            try
            {
                _sdiDevice.SetUpdateRateControlModeLimit(limitThreshold);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private void ConfigureUpdateRateControlMiscellaneous()
        {
            if (_sdiDevice == null) return;
            var useShortestPath = _deviceConfig?.UpdateRateControlConfig?.UseShortestPath ?? DeviceFunction != "PITCH";
            try
            {
                _sdiDevice.SetUpdateRateControlMiscellaneous(useShortestPath);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private void ConfigureUpdateRateControlMode()
        {
            if (_sdiDevice == null) return;

            var updateRateControlMode = _deviceConfig?.UpdateRateControlConfig?.Mode ?? SDIDriver.UpdateRateControlModes
                                            .Limit; //use limit mode defaults (disabled) if no URC mode is specified in the config file

            switch (updateRateControlMode)
            {
                case SDIDriver.UpdateRateControlModes.Limit:
                    ConfigureUpdateRateControlLimitMode();
                    break;
                case SDIDriver.UpdateRateControlModes.Smooth:
                    ConfigureUpdateRateControlSmoothMode();
                    break;
            }
        }

        private void ConfigureUpdateRateControlSmoothMode()
        {
            if (_sdiDevice == null) return;

            var smoothingMinimumThresholdValue =
                (_deviceConfig?.UpdateRateControlConfig?.ModeSettings as SmoothingModeSettings)
                ?.SmoothingMinimumThreshold ?? (byte) 0;

            var smoothingMode =
                (_deviceConfig?.UpdateRateControlConfig?.ModeSettings as SmoothingModeSettings)?.SmoothingMode ??
                SDIDriver.UpdateRateControlSmoothingMode.Adaptive;
            try
            {
                _sdiDevice.SetUpdateRateControlModeSmooth(smoothingMinimumThresholdValue, smoothingMode);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private void ConfigureUpdateRateControlSpeed()
        {
            if (_sdiDevice == null) return;
            var stepUpdateDelayMillis = _deviceConfig?.UpdateRateControlConfig?.StepUpdateDelayMillis != null
                ? (short) _deviceConfig.UpdateRateControlConfig.StepUpdateDelayMillis.Value
                : SDIDriver.Device
                    .UPDATE_RATE_CONTROL_MIN_STEP_UPDATE_DELAY_MILLIS; //use the fastest updates by default
            try
            {
                _sdiDevice.SetUpdateRateControlSpeed(stepUpdateDelayMillis);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private void ConfigureUSBConnection()
        {
            if (
                _deviceConfig?.ConnectionType == null || _deviceConfig.ConnectionType.Value !=
                ConnectionType.USB &&
                string.IsNullOrWhiteSpace(_deviceConfig.COMPort)
            )
            {
                return;
            }

            var addressString = (_deviceConfig.Address ?? "").ToLowerInvariant().Replace("0x", string.Empty).Trim();
            var addressIsValid = byte.TryParse(addressString, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out var addressByte);
            if (!addressIsValid) return;
            _deviceAddress = addressByte;

            try
            {
                var comPort = _deviceConfig.COMPort;
                _sdiDevice = new SDIDriver.Device(comPort);
                _sdiDevice.DisableWatchdog();
                _sdiDevice.ConfigureUsbDebug(false);
            }
            catch (Exception e)
            {
                Common.Util.DisposeObject(_sdiDevice);
                _sdiDevice = null;
                _log.Error(e.Message, e);
            }
        }

        private DigitalSignal CreateOutputSignalForOutputChannelConfiguredAsDigital(int channelNumber)
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital/PWM Output Channels",
                FriendlyName = $"DIG_PWM_{channelNumber} (Digital) (0=OFF, 1=ON)",
                Id = $"HenkSDI[{"0x" + _deviceAddress.ToString("X").PadLeft(2, '0')}]__DIG_PWM_{channelNumber}",
                Index = channelNumber,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = OutputChannelInitialValue(OutputChannel(channelNumber)) == 1
            };
            return thisSignal;
        }

        private CalibratedAnalogSignal CreateOutputSignalForOutputChannelConfiguredAsPWM(int channelNumber)
        {
            var thisSignal = new CalibratedAnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Digital/PWM Output Channels"
            };
            if (channelNumber < 8)
            {
                thisSignal.FriendlyName = $"DIG_PWM_{channelNumber} (PWM) (% duty cycle)";
                thisSignal.Id =
                    $"HenkSDI[{"0x" + _deviceAddress.ToString("X").PadLeft(2, '0')}]__DIG_PWM_{channelNumber}";
            }
            else
            {
                thisSignal.FriendlyName = "PWM_OUT (PWM) (% duty cycle)";
                thisSignal.Id = $"HenkSDI[{"0x" + _deviceAddress.ToString("X").PadLeft(2, '0')}]__PWM_OUT";
            }

            thisSignal.Index = channelNumber;
            thisSignal.Source = this;
            thisSignal.SourceFriendlyName = FriendlyName;
            thisSignal.SourceAddress = null;
            thisSignal.State = OutputChannelInitialValue(OutputChannel(channelNumber));
            thisSignal.CalibrationData = OutputChannelCalibrationData(OutputChannel(channelNumber));
            thisSignal.IsPercentage = true;
            thisSignal.MinValue = 0; //0% duty cycle
            thisSignal.MaxValue = 1; //100% duty cycle
            return thisSignal;
        }

        private void CreateOutputSignals()
        {
            _positionOutputSignal = CreatePositionOutputSignal();
            _digitalOutputs = CreateOutputSignalsForDigitalOutputChannels();
            _analogOutputs = CreateOutputSignalsForPWMOutputChannels();
        }

        private List<DigitalSignal> CreateOutputSignalsForDigitalOutputChannels()
        {
            return AllOutputChannels.Where(x => OutputChannelMode(x) == Henkie.Common.OutputChannelMode.Digital)
                .Select(x => CreateOutputSignalForOutputChannelConfiguredAsDigital(ChannelNumber(x)))
                .ToList();
        }

        private List<CalibratedAnalogSignal> CreateOutputSignalsForPWMOutputChannels()
        {
            return AllOutputChannels.Where(x => OutputChannelMode(x) == Henkie.Common.OutputChannelMode.PWM)
                .Select(x => CreateOutputSignalForOutputChannelConfiguredAsPWM(ChannelNumber(x)))
                .ToList();
        }

        private CalibratedAnalogSignal CreatePositionOutputSignal()
        {
            var thisSignal = new CalibratedAnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Synchro Control",
                FriendlyName = "Synchro Position (0-1023)",
                Id = $"HenkSDI[{"0x" + _deviceAddress.ToString("X").PadLeft(2, '0')}]__Synchro_Position",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = DeviceFunction == "PITCH"
                    ? 424
                    : DeviceFunction == "ROLL"
                        ? 512
                        : 0,
                MinValue = DeviceFunction == "PITCH"
                    ? 140
                    : 0,
                MaxValue = DeviceFunction == "PITCH"
                    ? 700
                    : 1023
            };
            return thisSignal;
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    UnregisterForDataChangeEvents();
                    Common.Util.DisposeObject(_sdiDevice);
                }
            }
            _isDisposed = true;
        }

        private void OutputSignalForDigitalOutputChannel_SignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            if (_sdiDevice == null) return;
            var signal = (DigitalSignal) sender;
            var channelNumber = signal.Index;
            var outputChannel = OutputChannel(channelNumber);
            try
            {
                _sdiDevice.SetOutputChannelValue(outputChannel, args.CurrentState ? (byte) 0x01 : (byte) 0x00);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private void OutputSignalForPWMOutputChannel_SignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            if (_sdiDevice == null) return;
            var signal = (AnalogSignal) sender;
            var channelNumber = signal.Index;
            var outputChannel = OutputChannel(channelNumber);
            try
            {
                _sdiDevice.SetOutputChannelValue(outputChannel, (byte) (args.CurrentState * byte.MaxValue));
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private void MoveIndicatorToPositionCoarse(int requestedPosition)
        {
            if (_sdiDevice == null) return;
            try
            {
                _sdiDevice.MoveIndicatorCoarse((byte) (requestedPosition / 4));
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private void MoveIndicatorToPositionFine(int requestedPosition)
        {
            if (_sdiDevice == null) return;
            try
            {
                if (requestedPosition >= 0 && requestedPosition <= 255)
                {
                    _sdiDevice.MoveIndicatorFine(Quadrant.One, (byte) requestedPosition);
                }
                else if (requestedPosition >= 256 && requestedPosition <= 511)
                {
                    _sdiDevice.MoveIndicatorFine(Quadrant.Two, (byte) (requestedPosition - 256));
                }
                else if (requestedPosition >= 512 && requestedPosition <= 767)
                {
                    _sdiDevice.MoveIndicatorFine(Quadrant.Three, (byte) (requestedPosition - 512));
                }
                else if (requestedPosition >= 768 && requestedPosition <= 1023)
                {
                    _sdiDevice.MoveIndicatorFine(Quadrant.Four, (byte) (requestedPosition - 768));
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private static SDIDriver.OutputChannels OutputChannel(int? channelNumber)
        {
            if (!channelNumber.HasValue) return SDIDriver.OutputChannels.Unknown;
            if (channelNumber.Value == 1) return SDIDriver.OutputChannels.DIG_PWM_1;
            if (channelNumber.Value == 2) return SDIDriver.OutputChannels.DIG_PWM_2;
            if (channelNumber.Value == 3) return SDIDriver.OutputChannels.DIG_PWM_3;
            if (channelNumber.Value == 4) return SDIDriver.OutputChannels.DIG_PWM_4;
            if (channelNumber.Value == 5) return SDIDriver.OutputChannels.DIG_PWM_5;
            if (channelNumber.Value == 6) return SDIDriver.OutputChannels.DIG_PWM_6;
            if (channelNumber.Value == 7) return SDIDriver.OutputChannels.DIG_PWM_7;
            if (channelNumber.Value == 8) return SDIDriver.OutputChannels.PWM_OUT;
            return SDIDriver.OutputChannels.Unknown;
        }

        private List<CalibrationPoint> OutputChannelCalibrationData(SDIDriver.OutputChannels outputChannel)
        {
            if (_deviceConfig?.OutputChannelsConfig == null) return Enumerable.Empty<CalibrationPoint>().ToList();
            CalibrationPoint[] calibrationData = null;
            switch (outputChannel)
            {
                case SDIDriver.OutputChannels.PWM_OUT:
                    calibrationData = _deviceConfig.OutputChannelsConfig.PWM_OUT?.CalibrationData;
                    break;
                case SDIDriver.OutputChannels.DIG_PWM_1:
                    calibrationData = _deviceConfig.OutputChannelsConfig.DIG_PWM_1?.CalibrationData;
                    break;
                case SDIDriver.OutputChannels.DIG_PWM_2:
                    calibrationData = _deviceConfig.OutputChannelsConfig.DIG_PWM_2?.CalibrationData;
                    break;
                case SDIDriver.OutputChannels.DIG_PWM_3:
                    calibrationData = _deviceConfig.OutputChannelsConfig.DIG_PWM_3?.CalibrationData;
                    break;
                case SDIDriver.OutputChannels.DIG_PWM_4:
                    calibrationData = _deviceConfig.OutputChannelsConfig.DIG_PWM_4?.CalibrationData;
                    break;
                case SDIDriver.OutputChannels.DIG_PWM_5:
                    calibrationData = _deviceConfig.OutputChannelsConfig.DIG_PWM_5?.CalibrationData;
                    break;
                case SDIDriver.OutputChannels.DIG_PWM_6:
                    calibrationData = _deviceConfig.OutputChannelsConfig.DIG_PWM_6?.CalibrationData;
                    break;
                case SDIDriver.OutputChannels.DIG_PWM_7:
                    calibrationData = _deviceConfig.OutputChannelsConfig.DIG_PWM_7?.CalibrationData;
                    break;
            }

            return (calibrationData ?? Enumerable.Empty<CalibrationPoint>())
                .Select(x => new CalibrationPoint(x.Input, x.Output / 255.000))
                .ToList();
        }

        private byte OutputChannelInitialValue(SDIDriver.OutputChannels outputChannel)
        {
            switch (outputChannel)
            {
                case SDIDriver.OutputChannels.DIG_PWM_1:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_PWM_1?.InitialValue ?? (DeviceFunction == "PITCH"
                               ? (byte) 1 //ENABLE PITCH/ROLL
                               : (byte) 0);
                case SDIDriver.OutputChannels.DIG_PWM_2:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_PWM_2?.InitialValue ?? (DeviceFunction == "PITCH"
                               ? (byte) 1 //GS POWER ON/OFF
                               : DeviceFunction == "ROLL"
                                   ? (byte) 1 //ROT & FLAGS POWER ON
                                   : (byte) 0);
                case SDIDriver.OutputChannels.DIG_PWM_3:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_PWM_3?.InitialValue ?? (DeviceFunction == "PITCH"
                               ? (byte) 128 //center horizontal command bar
                               : (byte) 0);
                case SDIDriver.OutputChannels.DIG_PWM_4:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_PWM_4?.InitialValue ?? (DeviceFunction == "PITCH"
                               ? (byte) 128 //center vertical command bar
                               : (byte) 0);
                case SDIDriver.OutputChannels.DIG_PWM_5:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_PWM_5?.InitialValue ?? (byte) 0;
                case SDIDriver.OutputChannels.DIG_PWM_6:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_PWM_6?.InitialValue ?? (byte) 0;
                case SDIDriver.OutputChannels.DIG_PWM_7:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_PWM_7?.InitialValue ?? (byte) 0;
                case SDIDriver.OutputChannels.PWM_OUT:
                    return _deviceConfig?.OutputChannelsConfig?.PWM_OUT?.InitialValue ?? (DeviceFunction == "ROLL"
                               ? (byte)
                               128 //center the Rate-of-Turn indicator by default if no user-supplied initial values are found in config file
                               : (byte) 0);
            }
            return 0;
        }

        private OutputChannelMode OutputChannelMode(SDIDriver.OutputChannels outputChannel)
        {
            switch (outputChannel)
            {
                case SDIDriver.OutputChannels.DIG_PWM_1:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_PWM_1?.Mode ?? Henkie.Common.OutputChannelMode.Digital;
                case SDIDriver.OutputChannels.DIG_PWM_2:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_PWM_2?.Mode ?? Henkie.Common.OutputChannelMode.Digital;
                case SDIDriver.OutputChannels.DIG_PWM_3:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_PWM_3?.Mode ??
                           (DeviceFunction == "PITCH" //HORIZONTAL ILS COMMAND BAR
                               ? Henkie.Common.OutputChannelMode.PWM
                               : Henkie.Common.OutputChannelMode.Digital);
                case SDIDriver.OutputChannels.DIG_PWM_4:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_PWM_4?.Mode ??
                           (DeviceFunction == "PITCH" //VERTICAL ILS COMMAND BAR
                               ? Henkie.Common.OutputChannelMode.PWM
                               : Henkie.Common.OutputChannelMode.Digital);
                case SDIDriver.OutputChannels.DIG_PWM_5:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_PWM_5?.Mode ?? Henkie.Common.OutputChannelMode.Digital;
                case SDIDriver.OutputChannels.DIG_PWM_6:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_PWM_6?.Mode ?? Henkie.Common.OutputChannelMode.Digital;
                case SDIDriver.OutputChannels.DIG_PWM_7:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_PWM_7?.Mode ?? Henkie.Common.OutputChannelMode.Digital;
                case SDIDriver.OutputChannels.PWM_OUT:
                    return Henkie.Common.OutputChannelMode.PWM;
            }
            return Henkie.Common.OutputChannelMode.Digital;
        }

        private void PositionOutputSignal_SignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            if (_positionOutputSignal != null)
            {
                var requestedPosition = _positionOutputSignal.State;
                MoveIndicatorToPositionFine((int) requestedPosition);
            }
        }

        private void RegisterForDataChangeEvents()
        {
            if (_positionOutputSignal != null)
            {
                _positionOutputSignal.SignalChanged += PositionOutputSignal_SignalChanged;
            }
            foreach (var digitalSignal in _digitalOutputs)
                digitalSignal.SignalChanged += OutputSignalForDigitalOutputChannel_SignalChanged;
            foreach (var analogSignal in _analogOutputs)
                analogSignal.SignalChanged += OutputSignalForPWMOutputChannel_SignalChanged;
        }

        private void UnregisterForDataChangeEvents()
        {
            if (_positionOutputSignal != null)
            {
                try
                {
                    _positionOutputSignal.SignalChanged -= PositionOutputSignal_SignalChanged;
                }
                catch (RemotingException)
                {
                }
            }
            foreach (var digitalSignal in _digitalOutputs)
                try
                {
                    digitalSignal.SignalChanged -= OutputSignalForDigitalOutputChannel_SignalChanged;
                }
                catch (RemotingException)
                {
                }
            foreach (var analogSignal in _analogOutputs)
                try
                {
                    analogSignal.SignalChanged -= OutputSignalForPWMOutputChannel_SignalChanged;
                }
                catch (RemotingException)
                {
                }
        }
    }
}