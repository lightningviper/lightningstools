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
    //Simtek 10-1088 F-16 NOZZLE POSITION IND
    public class Simtek101088HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly INozzlePositionIndicator _renderer = new NozzlePositionIndicator();

        private bool _isDisposed;
        private AnalogSignal _nozzlePositionCOSOutputSignal;
        private AnalogSignal _nozzlePositionInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _nozzlePositionInputSignalChangedEventHandler;
        private AnalogSignal _nozzlePositionSINOutputSignal;

        private Simtek101088HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] {_nozzlePositionInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[]
            {_nozzlePositionSINOutputSignal, _nozzlePositionCOSOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-1088 - Nozzle Position Ind";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek101088HardwareSupportModule()
        {
            Dispose();
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Simtek101088HardwareSupportModule()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.NozzlePositionPercent = (float) _nozzlePositionInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _nozzlePositionInputSignalChangedEventHandler = null;
        }

        private void CreateInputEventHandlers()
        {
            _nozzlePositionInputSignalChangedEventHandler =
                nozzlePosition_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _nozzlePositionInputSignal = CreateNozzlePositionInputSignal();
        }

        private AnalogSignal CreateNozzlePositionCOSOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Nozzle Position (COS)",
                Id = "101088_Nozzle_Position_COS_To_Instrument",
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

        private AnalogSignal CreateNozzlePositionInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Nozzle Position (0-100%)",
                Id = "101088_Nozzle_Position_From_Sim",
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

        private AnalogSignal CreateNozzlePositionSINOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Nozzle Position (SIN)",
                Id = "101088_Nozzle_Position_SIN_To_Instrument",
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
            _nozzlePositionSINOutputSignal = CreateNozzlePositionSINOutputSignal();
            _nozzlePositionCOSOutputSignal = CreateNozzlePositionCOSOutputSignal();
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

        private void nozzlePosition_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_nozzlePositionInputSignal != null)
            {
                _nozzlePositionInputSignal.SignalChanged += _nozzlePositionInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_nozzlePositionInputSignalChangedEventHandler == null || _nozzlePositionInputSignal == null) return;
            try
            {
                _nozzlePositionInputSignal.SignalChanged -= _nozzlePositionInputSignalChangedEventHandler;
            }
            catch (RemotingException)
            {
            }
        }

        private void UpdateOutputValues()
        {
            if (_nozzlePositionInputSignal == null) return;
            var nozzlePositionInput = _nozzlePositionInputSignal.State;

            if (_nozzlePositionSINOutputSignal != null)
            {
                var nozzlePositionSINOutputValue = nozzlePositionInput < 0
                    ? 0
                    : (nozzlePositionInput > 100
                        ? 10.0000 * Math.Sin(225.0000 * Constants.RADIANS_PER_DEGREE)
                        : 10.0000 *
                          Math.Sin(nozzlePositionInput / 100.0000 * 225.0000 *
                                   Constants.RADIANS_PER_DEGREE));

                if (nozzlePositionSINOutputValue < -10)
                {
                    nozzlePositionSINOutputValue = -10;
                }
                else if (nozzlePositionSINOutputValue > 10)
                {
                    nozzlePositionSINOutputValue = 10;
                }

                _nozzlePositionSINOutputSignal.State = nozzlePositionSINOutputValue;
            }

            if (_nozzlePositionCOSOutputSignal == null) return;
            var nozzlePositionCOSOutputValue = nozzlePositionInput < 0
                ? 0
                : (nozzlePositionInput > 100
                    ? 10.0000 * Math.Cos(225.0000 * Constants.RADIANS_PER_DEGREE)
                    : 10.0000 *
                      Math.Cos(nozzlePositionInput / 100.0000 * 225.0000 *
                               Constants.RADIANS_PER_DEGREE));

            if (nozzlePositionCOSOutputValue < -10)
            {
                nozzlePositionCOSOutputValue = -10;
            }
            else if (nozzlePositionCOSOutputValue > 10)
            {
                nozzlePositionCOSOutputValue = 10;
            }

            _nozzlePositionCOSOutputSignal.State = nozzlePositionCOSOutputValue;
        }
    }
}