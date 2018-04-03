using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Common.HardwareSupport;
using Common.MacroProgramming;
using Henkie.Common;
using log4net;
using PHCCDevice=Phcc;
namespace SimLinkup.HardwareSupport.Henk.QuadSinCos
{
    //Henkie Quad SIN/COS Interface Board Hardware Support Module
    public class HenkieQuadSinCosBoardHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(HenkieQuadSinCosBoardHardwareSupportModule));
        private readonly DeviceConfig _deviceConfig;
        private byte _deviceAddress;

        private bool _isDisposed;
        private AnalogSignal _device0SinOutputSignal;
        private AnalogSignal _device0CosOutputSignal;
        private AnalogSignal _device1SinOutputSignal;
        private AnalogSignal _device1CosOutputSignal;
        private AnalogSignal _device2SinOutputSignal;
        private AnalogSignal _device2CosOutputSignal;
        private AnalogSignal _device3SinOutputSignal;
        private AnalogSignal _device3CosOutputSignal;
        private DigitalSignal _sadiOffFlagOutputSignal;
        private DigitalSignal _digitalOutput23OutputSignal;

        private Henkie.QuadSinCos.Device _sinCosInterfaceBoard;

        private HenkieQuadSinCosBoardHardwareSupportModule(DeviceConfig deviceConfig)
        {
            _deviceConfig = deviceConfig;
            if (_deviceConfig != null)
            {
                ConfigureDevice();
                CreateOutputSignals();
            }
        }

        public override AnalogSignal[] AnalogOutputs
        {
            get
            {
                return new[] 
                {
                     _device0CosOutputSignal, _device0SinOutputSignal,
                     _device1CosOutputSignal, _device1SinOutputSignal,
                     _device2CosOutputSignal, _device2SinOutputSignal,
                     _device3CosOutputSignal, _device3SinOutputSignal,
                }
                .OrderBy(x => x.FriendlyName)
                .ToArray();
            }
        }

        public override AnalogSignal[] AnalogInputs => null;

        public override DigitalSignal[] DigitalOutputs
        {
            get
            {
                return new[]
                {
                    _sadiOffFlagOutputSignal, 
                    _digitalOutput23OutputSignal
                }
                .OrderBy(x => x.FriendlyName)
                .ToArray();
            }
        }

        public override DigitalSignal[] DigitalInputs => null;

        public override string FriendlyName =>
            $"Henkie Quad SIN/COS Interface Board: 0x{_deviceAddress.ToString("X").PadLeft(2, '0')} on {_deviceConfig.ConnectionType?.ToString() ?? "UNKNOWN"} [ {_deviceConfig.COMPort ?? "<UNKNOWN>"} ]";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HenkieQuadSinCosBoardHardwareSupportModule()
        {
            Dispose();
        }
        public override void Synchronize()
        {
            if (_sinCosInterfaceBoard == null) return;

            try
            {
                _sinCosInterfaceBoard.SetSADIOffFlag(_sadiOffFlagOutputSignal.State);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }

            try
            {
                _sinCosInterfaceBoard.SetDigitalOutputDevice2And3Connector(_digitalOutput23OutputSignal.State);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }

            try
            {
                _sinCosInterfaceBoard.SetSineDeferred(_device0SinOutputSignal.State / 10.0 );
                _sinCosInterfaceBoard.SetCosineDeferred(_device0CosOutputSignal.State / 10.0);
                _sinCosInterfaceBoard.LoadDeferredSineAndCosine(0);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }

            try
            {
                _sinCosInterfaceBoard.SetSineDeferred(_device1SinOutputSignal.State / 10.0);
                _sinCosInterfaceBoard.SetCosineDeferred(_device1CosOutputSignal.State / 10.0);
                _sinCosInterfaceBoard.LoadDeferredSineAndCosine(1);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }

            try
            {
                _sinCosInterfaceBoard.SetSineDeferred(_device2SinOutputSignal.State / 10.0);
                _sinCosInterfaceBoard.SetCosineDeferred(_device2CosOutputSignal.State / 10.0);
                _sinCosInterfaceBoard.LoadDeferredSineAndCosine(2);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }

            try
            {
                _sinCosInterfaceBoard.SetSineDeferred(_device3SinOutputSignal.State / 10.0);
                _sinCosInterfaceBoard.SetCosineDeferred(_device3CosOutputSignal.State / 10.0);
                _sinCosInterfaceBoard.LoadDeferredSineAndCosine(3);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }

        }
        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>();

            try
            {
                var hsmConfigFilePath = Path.Combine(Util.CurrentMappingProfileDirectory, "HenkieQuadSinCosHardwareSupportModule.config");
                var hsmConfig = HenkieQuadSinCosBoardHardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    foreach (var deviceConfiguration in hsmConfig.Devices)
                    {
                        var hsmInstance = new HenkieQuadSinCosBoardHardwareSupportModule(deviceConfiguration);
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

        private void ConfigureDevice()
        {
            ConfigureDeviceConnection();
            ConfigureDiagnosticLEDBehavior();
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
            if (_sinCosInterfaceBoard == null) return;

            var diagnosticLEDBehavior = _deviceConfig?.DiagnosticLEDMode != null &&
                                        _deviceConfig.DiagnosticLEDMode.HasValue
                ? _deviceConfig.DiagnosticLEDMode.Value
                : DiagnosticLEDMode.Heartbeat;
            try
            {
                _sinCosInterfaceBoard.ConfigureDiagnosticLEDBehavior(diagnosticLEDBehavior);
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
                var phccDevice = new PHCCDevice.Device(comPort, false);
                _sinCosInterfaceBoard = new Henkie.QuadSinCos.Device(phccDevice, addressByte);
                _sinCosInterfaceBoard.DisableWatchdog();
            }
            catch (Exception e)
            {
                Common.Util.DisposeObject(_sinCosInterfaceBoard);
                _sinCosInterfaceBoard = null;
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
                _sinCosInterfaceBoard = new Henkie.QuadSinCos.Device(comPort);
                _sinCosInterfaceBoard.DisableWatchdog();
                _sinCosInterfaceBoard.ConfigureUsbDebug(false);
            }
            catch (Exception e)
            {
                Common.Util.DisposeObject(_sinCosInterfaceBoard);
                _sinCosInterfaceBoard = null;
                _log.Error(e.Message, e);
            }
        }

        private void CreateOutputSignals()
        {
            _device0SinOutputSignal = CreateSinCosOutputSignal(0, "SIN");
            _device0CosOutputSignal = CreateSinCosOutputSignal(0, "COS");
            _device1SinOutputSignal = CreateSinCosOutputSignal(1, "SIN");
            _device1CosOutputSignal = CreateSinCosOutputSignal(1, "COS");
            _device2SinOutputSignal = CreateSinCosOutputSignal(2, "SIN");
            _device2CosOutputSignal = CreateSinCosOutputSignal(2, "COS");
            _device3SinOutputSignal = CreateSinCosOutputSignal(3, "SIN");
            _device3CosOutputSignal = CreateSinCosOutputSignal(3, "COS");
            _sadiOffFlagOutputSignal = CreateSADIDigitalOutputSignal();
            _digitalOutput23OutputSignal = CreateDigitalOutput23Signal();
        }
        private DigitalSignal CreateSADIDigitalOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = $"SADI OFF Flag Hidden (0=Visible, 1=Hidden)",
                Id = $"HenkQuadSinCos[{"0x" + _deviceAddress.ToString("X").PadLeft(2, '0')}]__SADI_OFF_FLAG_HIDDEN",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }
        private DigitalSignal CreateDigitalOutput23Signal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = $"Digital Output On DVC2.3 Connector (0=OFF, 1=ON)",
                Id = $"HenkQuadSinCos[{"0x" + _deviceAddress.ToString("X").PadLeft(2, '0')}]__DIG_OUT_23",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private AnalogSignal CreateSinCosOutputSignal(byte deviceNum, string sinCosType)
        {
            var canonicalFunction = deviceNum == 0 ? "(notionally: PITCH)" : deviceNum == 1 ? "(notionally: ROLL)" : string.Empty;
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = string.Format("Device {0} {1} {2} (-10VDC to +10VDC)", deviceNum, canonicalFunction, sinCosType),
                Id = "HenkQuadSinCos[" + "0x" + _deviceAddress.ToString("X").PadLeft(2, '0') + "]__DEVICE_" + deviceNum + "_" + sinCosType,
                Index = deviceNum,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = sinCosType.Equals("COS", StringComparison.InvariantCultureIgnoreCase) ? 10 : 0,
                IsVoltage = true,
                IsCosine = sinCosType.Equals("COS", StringComparison.InvariantCultureIgnoreCase),
                IsSine = sinCosType.Equals("SIN", StringComparison.InvariantCultureIgnoreCase),
                Precision = 4,
                MinValue = -10.0,
                MaxValue = +10.0
            };
            return thisSignal;
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Common.Util.DisposeObject(_sinCosInterfaceBoard);
                }
            }
            _isDisposed = true;
        }

    }
}