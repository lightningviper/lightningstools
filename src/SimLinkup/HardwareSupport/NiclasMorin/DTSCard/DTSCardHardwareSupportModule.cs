﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.HardwareSupport;
using Common.MacroProgramming;
using DTSCard;
using log4net;
namespace SimLinkup.HardwareSupport.NiclasMorin.DTSCard
{
    public class DTSCardHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DTSCardHardwareSupportModule));
        private AnalogSignal _angleOutputSignal;
        private AnalogSignal _inputSignalFromSim;
        private bool _isDisposed;
        private double _lastAngle = double.MinValue;
        private global::DTSCard.DTSCardManaged _dtsCardManaged = new global::DTSCard.DTSCardManaged();
        private DeviceConfig _dtsCardDeviceConfig = null;
        private DateTime _lastUpdateTime = DateTime.MinValue;
        private DTSCardHardwareSupportModule(DeviceConfig deviceConfig)
        {
            _dtsCardDeviceConfig = deviceConfig;
            if (_dtsCardDeviceConfig != null)
            {
                CreateInputSignals();
                CreateOutputSignals();
                InitializeDTSCard();
            }
        }

        public override AnalogSignal[] AnalogInputs => new[] { _inputSignalFromSim };

        public override AnalogSignal[] AnalogOutputs => new[] { _angleOutputSignal };

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => $"Niclas Morin Digital-to-Synchro (DTS) Card [\"{_dtsCardDeviceConfig?.Serial }\"]";

        public override void Synchronize()
        {
            base.Synchronize();
            var angleToSet = GetOutputAngleForLinearInputValue(_inputSignalFromSim.State); 
            if (angleToSet != _lastAngle || DateTime.Now.Subtract(_lastUpdateTime).TotalMilliseconds > 1000)
            {
                SafeSetAngle(angleToSet);
            }
            SafeUpdate();
        }
        private double GetOutputAngleForLinearInputValue(double inputValue)
        {
            var mappingTableLowSideEntry = _dtsCardDeviceConfig.CalibrationData
                                                .Where(x => (x.Input <= inputValue))
                                                .OrderBy(x => x.Input)
                                                .LastOrDefault();

            var mappingTableHighSideEntry = _dtsCardDeviceConfig.CalibrationData
                                                .Where(x => (x.Input >= inputValue))
                                                .OrderBy(x => x.Input)
                                                .FirstOrDefault();

            if (mappingTableLowSideEntry.Input == mappingTableHighSideEntry.Input)
            {
                return mappingTableLowSideEntry.Output % 360;
            }
            else
            {
                var inputRangeWidth = Math.Abs(
                    Math.Abs(mappingTableHighSideEntry.Input)
                    - Math.Abs(mappingTableLowSideEntry.Input)
                    );

                var inputAsPctOfInputRange = (inputValue - mappingTableLowSideEntry.Input) / inputRangeWidth;

                var outputRangeWidth = Math.Abs(
                    Math.Abs(mappingTableHighSideEntry.Output % 360)
                    - Math.Abs(mappingTableLowSideEntry.Output % 360)
                    );
                
                var output = ((mappingTableLowSideEntry.Output % 360) + (outputRangeWidth * inputAsPctOfInputRange)) % 360;

                return output;
            }
        }
        private void SafeUpdate()
        {
            if (_dtsCardManaged == null) return;
            try
            {
                _dtsCardManaged.Update();
                _lastUpdateTime = DateTime.Now;
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }
        private void SafeSetAngle(double angle)
        {
            try
            {
                if (_dtsCardDeviceConfig.DeadZone != null 
                    && (angle > _dtsCardDeviceConfig.DeadZone.FromDegrees 
                    && angle < _dtsCardDeviceConfig.DeadZone.ToDegrees))
                {
                    return;
                }

                _angleOutputSignal.State = angle;
                var result = _dtsCardManaged?.SetAngle(angle);
                _lastAngle = angle;
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>();

            try
            {
                var hsmConfigFilePath = Path.Combine(Util.CurrentMappingProfileDirectory, $"{nameof(DTSCardHardwareSupportModule)}.config");
                var hsmConfig = DTSCardHardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    foreach (var deviceConfiguration in hsmConfig.Devices)
                    {
                        var hsmInstance = new DTSCardHardwareSupportModule(deviceConfiguration);
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
        private void InitializeDTSCard()
        {
            _dtsCardManaged.SetSerial(_dtsCardDeviceConfig?.Serial);
            SafeInit();
        }
        private void SafeInit()
        {
            try
            {
                var result = _dtsCardManaged.Init();
                if (result != (int)DTSCardOperationStatus.Success)
                {
                    Log.Error($"Could not initialize DTS card with serial:{ _dtsCardDeviceConfig.Serial}");
                    _dtsCardManaged = null;
                }

            }
            catch (Exception e)
            {
                _dtsCardManaged = null;
                Log.Error(e.Message, e);
            }
        }
        private void CreateInputSignals()
        {
            _inputSignalFromSim = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Input from Sim",
                Id = $"Niclas_Morin_DTS_Card[\"{_dtsCardDeviceConfig?.Serial }\"]_Input_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
            };
        }

        private void CreateOutputSignals()
        {
            _angleOutputSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Angle",
                Id = $"Niclas_Morin_DTS_Card[\"{_dtsCardDeviceConfig?.Serial }\"]_Angle",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = 0,
                MaxValue = 360
            };
        }

        ~DTSCardHardwareSupportModule()
        {
            Dispose(false);
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
                    if (_dtsCardManaged != null)
                    {
                        try
                        {
                            _dtsCardManaged.Clean();
                            _dtsCardManaged.Dispose();
                        }
                        catch { }
                        _dtsCardManaged = null;
                    }
                }
            }
            _isDisposed = true;
        }


    }
}