using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.Simtek
{
    //Simtek 10-0582-01 F-16 AOA Indicator
    public class Simtek10058201HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly IAngleOfAttackIndicator _renderer = new AngleOfAttackIndicator();

        private AnalogSignal _aoaInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _aoaInputSignalChangedEventHandler;

        private AnalogSignal _aoaOutputSignal;
        private DigitalSignal.SignalChangedEventHandler _aoaPowerInputSignalChangedEventHandler;
        private DigitalSignal _aoaPowerOffInputSignal;

        private bool _isDisposed;

        private Simtek10058201HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] {_aoaInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_aoaOutputSignal};

        public override DigitalSignal[] DigitalInputs => new[] {_aoaPowerOffInputSignal};

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName =>
            "Simtek P/N 10-0582-01 - Indicator - Simulated Angle Of Attack Indicator";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek10058201HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Simtek10058201HardwareSupportModule()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            g.Clear(Color.Black);
            _renderer.InstrumentState.OffFlag = _aoaPowerOffInputSignal.State;
            _renderer.InstrumentState.AngleOfAttackDegrees = (float) _aoaInputSignal.State;

            var aoaWidth = (int) (destinationRectangle.Height * (102f / 227f));
            var aoaHeight = destinationRectangle.Height;

            using (var aoaBmp = new Bitmap(aoaWidth, aoaHeight))
            using (var aoaBmpGraphics = Graphics.FromImage(aoaBmp))
            {
                _renderer.Render(aoaBmpGraphics, new Rectangle(0, 0, aoaWidth, aoaHeight));
                var targetRectangle = new Rectangle(
                    destinationRectangle.X + (int) ((destinationRectangle.Width - aoaWidth) / 2.0),
                    destinationRectangle.Y, aoaWidth, destinationRectangle.Height);
                g.DrawImage(aoaBmp, targetRectangle);
            }
        }

        private void AbandonInputEventHandlers()
        {
            _aoaInputSignalChangedEventHandler = null;
            _aoaPowerInputSignalChangedEventHandler = null;
        }

        private void AOA_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void AOAPower_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private AnalogSignal CreateAOAInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Angle of Attack [alpha] (degrees)",
                Id = "10058201_AOA_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = -5,
                MaxValue = 40
            };
            return thisSignal;
        }

        private AnalogSignal CreateAOAOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "AOA",
                Id = "10058201_AOA_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = -10.00, //volts
                IsVoltage = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private DigitalSignal CreateAOAPowerInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "OFF Flag",
                Id = "10058201_OFF_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };
            return thisSignal;
        }

        private void CreateInputEventHandlers()
        {
            _aoaInputSignalChangedEventHandler = AOA_InputSignalChanged;
            _aoaPowerInputSignalChangedEventHandler =
                AOAPower_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _aoaInputSignal = CreateAOAInputSignal();
            _aoaPowerOffInputSignal = CreateAOAPowerInputSignal();
        }

        private void CreateOutputSignals()
        {
            _aoaOutputSignal = CreateAOAOutputSignal();
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

        private void RegisterForInputEvents()
        {
            if (_aoaInputSignal != null)
            {
                _aoaInputSignal.SignalChanged += _aoaInputSignalChangedEventHandler;
            }
            if (_aoaPowerOffInputSignal != null)
            {
                _aoaPowerOffInputSignal.SignalChanged += _aoaPowerInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_aoaInputSignalChangedEventHandler != null && _aoaInputSignal != null)
            {
                try
                {
                    _aoaInputSignal.SignalChanged -= _aoaInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_aoaPowerInputSignalChangedEventHandler == null || _aoaPowerOffInputSignal == null) return;
            try
            {
                _aoaPowerOffInputSignal.SignalChanged -= _aoaPowerInputSignalChangedEventHandler;
            }
            catch (RemotingException)
            {
            }
        }

        private void UpdateOutputValues()
        {
            var aoaPowerOff = false;
            if (_aoaPowerOffInputSignal != null)
            {
                aoaPowerOff = _aoaPowerOffInputSignal.State;
            }

            if (_aoaInputSignal != null)
            {
                var aoaInput = _aoaInputSignal.State;

                if (_aoaOutputSignal == null) return;
                var aoaOutputValue = aoaPowerOff
                    ? -10
                    : (aoaInput < -5
                        ? -6.37
                        : (aoaInput >= -5 && aoaInput <= 40 ? -6.37 + (aoaInput + 5) / 45 * 16.37 : 10));

                if (aoaOutputValue < -10)
                {
                    aoaOutputValue = -10;
                }
                else if (aoaOutputValue > 10)
                {
                    aoaOutputValue = 10;
                }

                _aoaOutputSignal.State = aoaOutputValue;
            }
        }
    }
}