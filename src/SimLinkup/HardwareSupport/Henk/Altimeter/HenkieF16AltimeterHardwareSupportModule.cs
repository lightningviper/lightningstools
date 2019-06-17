using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16;
using System.IO;
using log4net;
using System.Linq;
using Henkie.Common;
using Henkie.Altimeter;
using System.Globalization;
using p = Phcc;
namespace SimLinkup.HardwareSupport.Henk.Altimeter
{
    //Henkie F-16 Altimeter interface board
    public class HenkieF16AltimeterHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HenkieF16AltimeterHardwareSupportModule));
        private readonly IAltimeter _renderer = new LightningGauges.Renderers.F16.Altimeter();

        private AnalogSignal _altitudeInputSignal;
        private AnalogSignal _barometricPressureInputSignal;
        private readonly DeviceConfig _deviceConfig;
        private byte _deviceAddress;
        private List<DigitalSignal> _digitalOutputs = new List<DigitalSignal>();

        private bool _isDisposed;
        private AnalogSignal _positionOutputSignal;
        private Device _altimeterDeviceInterface;


        private const double DEFAULT_MIN_BARO_PRESSURE = 28.09;
        private const double DEFAULT_MAX_BARO_PRESSURE = 31.025;
        private const double DEFAULT_DIFFERENCE_IN_INDICATED_ALTITUDE_FROM_MIN_BARO_TO_MAX_BARO_IN_FEET = 2800;
        private double _minBaroPressure = DEFAULT_MIN_BARO_PRESSURE;
        private double _maxBaroPressure = DEFAULT_MAX_BARO_PRESSURE;
        private double _differenceInIndicatedAltitudeFromMinBaroToMaxBaroInFeet = DEFAULT_DIFFERENCE_IN_INDICATED_ALTITUDE_FROM_MIN_BARO_TO_MAX_BARO_IN_FEET;
        private CalibrationPoint[] _calibrationData;
        private HenkieF16AltimeterHardwareSupportModule(DeviceConfig deviceConfig)
        {
            _deviceConfig = deviceConfig;
            if (_deviceConfig != null)
            {
                ConfigureDevice();
                CreateInputSignals();
                CreateOutputSignals();
                RegisterForEvents();
            }

        }

        public override AnalogSignal[] AnalogOutputs
        {
            get
            {
                return new[] { _positionOutputSignal }
                    .OrderBy(x => x.FriendlyName)
                    .ToArray();
            }
        }

        public override AnalogSignal[] AnalogInputs => new[] { _altitudeInputSignal, _barometricPressureInputSignal };

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
            $"Henkie Altimeter Drive Interface: 0x{_deviceAddress.ToString("X").PadLeft(2, '0')} on {_deviceConfig.ConnectionType?.ToString() ?? "UNKNOWN"} [ {_deviceConfig.COMPort ?? "<UNKNOWN>"} ]";

        private static OutputChannels[] DigitalOutputChannels => new[]
        {
            OutputChannels.DIG_OUT_1,
            OutputChannels.DIG_OUT_2,
            OutputChannels.DIG_OUT_3
        };

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HenkieF16AltimeterHardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>();

            try
            {
                var hsmConfigFilePath = Path.Combine(Util.CurrentMappingProfileDirectory, "HenkieF16Altimeter.config");
                var hsmConfig = HenkieF16AltimeterHardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    foreach (var deviceConfiguration in hsmConfig.Devices)
                    {
                        var hsmInstance = new HenkieF16AltimeterHardwareSupportModule(deviceConfiguration);
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

        private static int ChannelNumber(OutputChannels outputChannel)
        {
            var lastCharOfChannelName = outputChannel.ToString().Substring(outputChannel.ToString().Length - 1, 1);
            int.TryParse(lastCharOfChannelName, out var channelNumber);
            return channelNumber > 0 ? channelNumber : 8;
        }

        private void ConfigureDevice()
        {
            ConfigureDeviceConnection();
            ConfigureStatorBaseAngles();
            ConfigureDiagnosticLEDBehavior();
            ConfigureOutputChannels();
            ConfigureAltimeterCalibration();
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
                Log.Error(e.Message, e);
            }
        }

        private void ConfigureDiagnosticLEDBehavior()
        {
            if (_altimeterDeviceInterface == null) return;

            var diagnosticLEDBehavior = _deviceConfig?.DiagnosticLEDMode !=null && 
                _deviceConfig.DiagnosticLEDMode.HasValue
                ? _deviceConfig.DiagnosticLEDMode.Value
                : DiagnosticLEDMode.Heartbeat;
            try
            {
                _altimeterDeviceInterface.ConfigureDiagnosticLEDBehavior(diagnosticLEDBehavior);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private void ConfigureOutputChannels()
        {
            if (_altimeterDeviceInterface == null) return;

            try
            {
                _altimeterDeviceInterface.SetDigitalOutputChannelValue(OutputChannels.DIG_OUT_1,
                    OutputChannelInitialValue(OutputChannels.DIG_OUT_1));
                _altimeterDeviceInterface.SetDigitalOutputChannelValue(OutputChannels.DIG_OUT_2,
                    OutputChannelInitialValue(OutputChannels.DIG_OUT_2));
                _altimeterDeviceInterface.SetDigitalOutputChannelValue(OutputChannels.DIG_OUT_3,
                    OutputChannelInitialValue(OutputChannels.DIG_OUT_3));
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
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
                var phccDevice = new p.Device(comPort, false);
                _altimeterDeviceInterface = new Device(phccDevice, addressByte);
                _altimeterDeviceInterface.DisableWatchdog();
            }
            catch (Exception e)
            {
                Common.Util.DisposeObject(_altimeterDeviceInterface);
                _altimeterDeviceInterface = null;
                Log.Error(e.Message, e);
            }
        }

        private void ConfigureStatorBaseAngles()
        {
            if (_altimeterDeviceInterface == null) return;

            var s1StatorBaseAngle = _deviceConfig?.StatorBaseAnglesConfig?.S1BaseAngleDegrees / 360.000 *
                                    Device.STATOR_ANGLE_MAX_OFFSET ?? 0.0 / 360.000 * Device.STATOR_ANGLE_MAX_OFFSET;


            var s2StatorBaseAngle = _deviceConfig?.StatorBaseAnglesConfig?.S2BaseAngleDegrees / 360.000 *
                                    Device.STATOR_ANGLE_MAX_OFFSET ?? 120.0 / 360.000 * Device.STATOR_ANGLE_MAX_OFFSET;


            var s3StatorBaseAngle = _deviceConfig?.StatorBaseAnglesConfig?.S3BaseAngleDegrees / 360.000 *
                                    Device.STATOR_ANGLE_MAX_OFFSET ?? 240.0 / 360.000 * Device.STATOR_ANGLE_MAX_OFFSET;


            try
            {
                _altimeterDeviceInterface.SetStatorBaseAngle(StatorSignals.S1, (short)s1StatorBaseAngle);
                _altimeterDeviceInterface.SetStatorBaseAngle(StatorSignals.S2, (short)s2StatorBaseAngle);
                _altimeterDeviceInterface.SetStatorBaseAngle(StatorSignals.S3, (short)s3StatorBaseAngle);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private void ConfigureAltimeterRangesAndLimits()
        {
            if (_altimeterDeviceInterface == null) return;

            _minBaroPressure = _deviceConfig?.MinBaroPressureInHg ?? 28.10;
            _maxBaroPressure = _deviceConfig?.MaxBaroPressureInHg ?? 31.00;
            _differenceInIndicatedAltitudeFromMinBaroToMaxBaroInFeet = _deviceConfig?.IndicatedAltitudeDifferenceInFeetFromMinBaroToMaxBaro ?? DEFAULT_DIFFERENCE_IN_INDICATED_ALTITUDE_FROM_MIN_BARO_TO_MAX_BARO_IN_FEET;
        }
        private void ConfigureAltimeterCalibration()
        {
            if (_altimeterDeviceInterface == null || _deviceConfig?.CalibrationData == null) return;
            _calibrationData = _deviceConfig?.CalibrationData;
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
                _altimeterDeviceInterface = new Device(comPort);
                _altimeterDeviceInterface.DisableWatchdog();
                _altimeterDeviceInterface.ConfigureUsbDebug(false);
            }
            catch (Exception e)
            {
                Common.Util.DisposeObject(_altimeterDeviceInterface);
                _altimeterDeviceInterface = null;
                Log.Error(e.Message, e);
            }
        }

        private DigitalSignal CreateOutputSignalForOutputChannelConfiguredAsDigital(int channelNumber)
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = $"DIG_OUT_{channelNumber} (0=OFF, 1=ON)",
                Id = $"HenkieF16Altimeter[{"0x" + _deviceAddress.ToString("X").PadLeft(2, '0')}]__DIG_OUT_{channelNumber}",
                Index = channelNumber,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = OutputChannelInitialValue(OutputChannel(channelNumber)) 
            };
            return thisSignal;
        }

        private void CreateOutputSignals()
        {
            _positionOutputSignal = CreatePositionOutputSignal();
            _digitalOutputs = CreateOutputSignalsForDigitalOutputChannels();
        }

        private List<DigitalSignal> CreateOutputSignalsForDigitalOutputChannels()
        {
            return DigitalOutputChannels
                .Select(x => CreateOutputSignalForOutputChannelConfiguredAsDigital(ChannelNumber(x)))
                .ToList();
        }

        private AnalogSignal CreatePositionOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Indicator Position",
                FriendlyName = "Indicator Position (0-4095)",
                Id = $"HenkieF16Altimeter[{"0x" + _deviceAddress.ToString("X").PadLeft(2, '0')}]__Indicator_Position",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 4095
            };
            return thisSignal;
        }

        private void OutputSignalForDigitalOutputChannel_SignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            if (_altimeterDeviceInterface == null) return;
            var signal = (DigitalSignal)sender;
            var channelNumber = signal.Index;
            var outputChannel = OutputChannel(channelNumber);
            try
            {
                _altimeterDeviceInterface.SetDigitalOutputChannelValue(outputChannel, args.CurrentState);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private void SetPosition(ushort requestedPosition)
        {
            if (_altimeterDeviceInterface == null) return;
            try
            {
                _altimeterDeviceInterface.SetPosition((short)requestedPosition);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private static OutputChannels OutputChannel(int? channelNumber)
        {
            if (!channelNumber.HasValue) return OutputChannels.Unknown;
            if (channelNumber.Value == 1) return OutputChannels.DIG_OUT_1;
            if (channelNumber.Value == 2) return OutputChannels.DIG_OUT_2;
            if (channelNumber.Value == 3) return OutputChannels.DIG_OUT_3;
            return OutputChannels.Unknown;
        }

        private bool OutputChannelInitialValue(OutputChannels outputChannel)
        {
            switch (outputChannel)
            {
                case OutputChannels.DIG_OUT_1:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_OUT_1?.InitialValue ?? false;
                case OutputChannels.DIG_OUT_2:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_OUT_2?.InitialValue ?? false;
                case OutputChannels.DIG_OUT_3:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_OUT_3?.InitialValue ?? false;

            }
            return false;
        }

        private void PositionOutputSignal_SignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            if (_positionOutputSignal != null)
            {
                var requestedPosition = _positionOutputSignal.State;
                SetPosition((ushort)requestedPosition);
            }
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.BarometricPressure = (float)_barometricPressureInputSignal.State * 100;
            _renderer.InstrumentState.IndicatedAltitudeFeetMSL = (float)_altitudeInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void altitude_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateAltitudeOutputValues();
        }

        private void barometricPressure_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateAltitudeOutputValues();
        }

        private AnalogSignal CreateAltitudeInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Altitude (Indicated)",
                Id = "HenkieF16Altimeter_Altitude_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = -1000,
                MaxValue = 80000
            };
            return thisSignal;
        }

        private AnalogSignal CreateBarometricPressureInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Barometric Pressure (Indicated), In. Hg.",
                Id = "HenkieF16Altimeter_Barometric_Pressure_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 29.92,
                MinValue = 28.10,
                MaxValue = 31.00
            };
            return thisSignal;
        }

        private void CreateInputSignals()
        {
            _altitudeInputSignal = CreateAltitudeInputSignal();
            _barometricPressureInputSignal = CreateBarometricPressureInputSignal();
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    UnregisterForEvents();
                    Common.Util.DisposeObject(_altimeterDeviceInterface);
                    Common.Util.DisposeObject(_renderer);
                }
            }
            _isDisposed = true;
        }

        private void RegisterForEvents()
        {
            if (_altitudeInputSignal != null)
            {
                _altitudeInputSignal.SignalChanged += altitude_InputSignalChanged;
            }
            if (_barometricPressureInputSignal != null)
            {
                _barometricPressureInputSignal.SignalChanged += barometricPressure_InputSignalChanged;
            }
            if (_positionOutputSignal != null)
            {
                _positionOutputSignal.SignalChanged += PositionOutputSignal_SignalChanged;
            }
            foreach (var digitalSignal in _digitalOutputs)
            {
                digitalSignal.SignalChanged += OutputSignalForDigitalOutputChannel_SignalChanged;
            }

        }

        private void UnregisterForEvents()
        {

            if (_altitudeInputSignal != null)
            {
                try
                {
                    _altitudeInputSignal.SignalChanged -= altitude_InputSignalChanged;
                }
                catch (RemotingException)
                {
                }
            }
            if (_barometricPressureInputSignal != null)
            {
                try
                {
                    _barometricPressureInputSignal.SignalChanged -= barometricPressure_InputSignalChanged;
                }
                catch (RemotingException)
                {
                }
            }

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
        private ushort CalibratedPosition(double altitude)
        {
            var altMod10k = altitude % 10000.0;
            if (_calibrationData == null) return (ushort)((altMod10k /10000.0)*4095.0);


            var lowerPoint = _calibrationData.OrderBy(x => x.Input).LastOrDefault(x => x.Input <= altMod10k) ??
                             new CalibrationPoint(0, 0);
            var upperPoint =
                _calibrationData
                    .OrderBy(x => x.Input)
                    .FirstOrDefault(x => x != lowerPoint && x.Input >= lowerPoint.Input) ?? new CalibrationPoint(10000, 10000);
            var inputRange = Math.Abs(upperPoint.Input - lowerPoint.Input);
            var outputRange = Math.Abs(upperPoint.Output - lowerPoint.Output);
            var inputPct = inputRange != 0
                ? (altMod10k - lowerPoint.Input) / inputRange
                : 1.00;
            return (ushort)((inputPct * outputRange) + lowerPoint.Output);

        }
        private void UpdateAltitudeOutputValues()
        {
            if (_altitudeInputSignal != null)
            {
                var altitudeFromSim = _altitudeInputSignal.State;
                var baroSettingFromSimInchesOfMercury = _barometricPressureInputSignal.State;
                if (baroSettingFromSimInchesOfMercury == 0.00f) baroSettingFromSimInchesOfMercury = 29.92f;
                var baroDeltaFromStandardPressure = baroSettingFromSimInchesOfMercury - 29.92f;
                var altToAddForBaroComp = -(_differenceInIndicatedAltitudeFromMinBaroToMaxBaroInFeet / (_maxBaroPressure - _minBaroPressure)) * baroDeltaFromStandardPressure;
                var altitudeOutput = altitudeFromSim + altToAddForBaroComp;
                var positionOutput = CalibratedPosition(altitudeOutput);
                _positionOutputSignal.State=positionOutput;
            }
        }
    }
}