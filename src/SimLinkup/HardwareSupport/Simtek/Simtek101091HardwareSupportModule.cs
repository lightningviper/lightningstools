using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using Common.Math;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.Simtek
{
    //Simtek 10-1091 F-16 ENGINE OIL PRESSURE IND
    public class Simtek101091HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly IOilPressureGauge _renderer = new OilPressureGauge();

        private bool _isDisposed;
        private AnalogSignal _oilPressureCOSOutputSignal;
        private AnalogSignal _oilPressureInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _oilPressureInputSignalChangedEventHandler;
        private AnalogSignal _oilPressureSINOutputSignal;

        private Simtek101091HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] {_oilPressureInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[]
            {_oilPressureSINOutputSignal, _oilPressureCOSOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-1091 - Engine Oil Pressure Ind";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek101091HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Simtek101091HardwareSupportModule()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.OilPressurePercent = (float) _oilPressureInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _oilPressureInputSignalChangedEventHandler = null;
        }

        private void CreateInputEventHandlers()
        {
            _oilPressureInputSignalChangedEventHandler =
                oil_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _oilPressureInputSignal = CreateOilInputSignal();
        }

        private AnalogSignal CreateOilCOSOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Oil  Pressure (COS)",
                Id = "101091_Oil_Pressure_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 10.00, //volts
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateOilInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Oil Pressure (0-100%)",
                Id = "101091_Oil_Pressure_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsPercentage = true,
                MinValue = 0,
                MaxValue = 100
            };

            return thisSignal;
        }

        private AnalogSignal CreateOilSINOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Oil  Pressure (SIN)",
                Id = "101091_Oil_Pressure_SIN_To_Instrument",
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

        private void CreateOutputSignals()
        {
            _oilPressureSINOutputSignal = CreateOilSINOutputSignal();
            _oilPressureCOSOutputSignal = CreateOilCOSOutputSignal();
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

        private void oil_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_oilPressureInputSignal != null)
            {
                _oilPressureInputSignal.SignalChanged += _oilPressureInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_oilPressureInputSignalChangedEventHandler == null || _oilPressureInputSignal == null) return;
            try
            {
                _oilPressureInputSignal.SignalChanged -= _oilPressureInputSignalChangedEventHandler;
            }
            catch (RemotingException)
            {
            }
        }

        private void UpdateOutputValues()
        {
            if (_oilPressureInputSignal == null) return;
            var oilPressureInput = _oilPressureInputSignal.State;
            if (_oilPressureSINOutputSignal != null)
            {
                var oilPressureSINOutputValue = oilPressureInput < 0
                    ? 0
                    : (oilPressureInput > 100
                        ? 10.0000 * Math.Sin(320.0000 * Constants.RADIANS_PER_DEGREE)
                        : 10.0000 *
                          Math.Sin(oilPressureInput / 100.0000 * 320.0000 *
                                   Constants.RADIANS_PER_DEGREE));

                if (oilPressureSINOutputValue < -10)
                {
                    oilPressureSINOutputValue = -10;
                }
                else if (oilPressureSINOutputValue > 10)
                {
                    oilPressureSINOutputValue = 10;
                }

                _oilPressureSINOutputSignal.State = oilPressureSINOutputValue;
            }

            if (_oilPressureCOSOutputSignal == null) return;
            var oilPressureCOSOutputValue = oilPressureInput < 0
                ? 0
                : (oilPressureInput > 100
                    ? 10.0000 * Math.Cos(320.0000 * Constants.RADIANS_PER_DEGREE)
                    : 10.0000 *
                      Math.Cos(oilPressureInput / 100.0000 * 320.0000 *
                               Constants.RADIANS_PER_DEGREE));

            if (oilPressureCOSOutputValue < -10)
            {
                oilPressureCOSOutputValue = -10;
            }
            else if (oilPressureCOSOutputValue > 10)
            {
                oilPressureCOSOutputValue = 10;
            }

            _oilPressureCOSOutputSignal.State = oilPressureCOSOutputValue;
        }
    }
}