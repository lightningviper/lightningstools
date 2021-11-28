using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using Common.Math;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.NiclasMorin.DTSCard
{
    //F-16 HYDRAULIC PRESSURE IND
    public class HydraulicPressureSynchroHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly IHydraulicPressureGauge _hydARenderer = new HydraulicPressureGauge();
        private readonly IHydraulicPressureGauge _hydBRenderer = new HydraulicPressureGauge();
        private AnalogSignal _hydPressureAInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _hydPressureAInputSignalChangedEventHandler;
        private AnalogSignal _hydPressureBInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _hydPressureBInputSignalChangedEventHandler;
        private bool _isDisposed;
        private global::DTSCard.DTSCardManaged _hydA_DTSCard = new global::DTSCard.DTSCardManaged();
        private global::DTSCard.DTSCardManaged _hydB_DTSCard = new global::DTSCard.DTSCardManaged();

        private HydraulicPressureSynchroHardwareSupportModule()
        {
            CreateInputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            InitializeDigitalToSynchroCards();
        }

        public override AnalogSignal[] AnalogInputs => new[] { _hydPressureAInputSignal, _hydPressureBInputSignal };

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Niclas Morin DTS Card - Hydraulic Pressure Synchros";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HydraulicPressureSynchroHardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new HydraulicPressureSynchroHardwareSupportModule()
            };

            return toReturn.ToArray();
        }
        private void InitializeDigitalToSynchroCards()
        {
            _hydA_DTSCard.SetSerial("HYDA");
            _hydA_DTSCard.Init();
            _hydA_DTSCard.SetAngle(0);
            _hydA_DTSCard.Update();

            _hydB_DTSCard.SetSerial("HYDB");
            _hydB_DTSCard.Init();
            _hydB_DTSCard.SetAngle(0);
            _hydB_DTSCard.Update();
        }
        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            g.Clear(Color.Black);

            _hydARenderer.InstrumentState.HydraulicPressurePoundsPerSquareInch = (float)_hydPressureAInputSignal.State;
            var hydARectangle = new Rectangle(destinationRectangle.X, destinationRectangle.Y,
                destinationRectangle.Width / 2, destinationRectangle.Height / 2);
            using (var hydABitmap = new Bitmap(hydARectangle.Width, hydARectangle.Height))
            using (var hydAGraphics = Graphics.FromImage(hydABitmap))
            {
                _hydARenderer.Render(hydAGraphics, new Rectangle(0, 0, hydARectangle.Width, hydARectangle.Height));
                g.DrawImage(hydABitmap, hydARectangle);
            }

            _hydBRenderer.InstrumentState.HydraulicPressurePoundsPerSquareInch = (float)_hydPressureBInputSignal.State;
            var hydBRectangle = new Rectangle(destinationRectangle.X + destinationRectangle.Width / 2,
                destinationRectangle.Y + destinationRectangle.Height / 2, destinationRectangle.Width / 2,
                destinationRectangle.Height / 2);
            using (var hydBBitmap = new Bitmap(hydBRectangle.Width, hydBRectangle.Height))
            using (var hydBGraphics = Graphics.FromImage(hydBBitmap))
            {
                _hydBRenderer.Render(hydBGraphics, new Rectangle(0, 0, hydBRectangle.Width, hydBRectangle.Height));
                g.DrawImage(hydBBitmap, hydBRectangle);
            }
        }

        private void AbandonInputEventHandlers()
        {
            _hydPressureAInputSignalChangedEventHandler = null;
            _hydPressureBInputSignalChangedEventHandler = null;
        }

        private AnalogSignal CreateHydraulicPressureAInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Hydraulic Pressure A",
                Id = "Niclas_Morin_DTS_Card_Hydraulic_Pressure_A_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsSine = true,
                MinValue = 0,
                MaxValue = 5000
            };

            return thisSignal;
        }

        private AnalogSignal CreateHydraulicPressureBInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Hydraulic Pressure B",
                Id = "niclas_Morin_DTS_Card_Hydraulic_Pressure_B_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 5000
            };

            return thisSignal;
        }

        private void CreateInputEventHandlers()
        {
            _hydPressureAInputSignalChangedEventHandler =
                hydPressureA_InputSignalChanged;

            _hydPressureBInputSignalChangedEventHandler =
                hydPressureB_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _hydPressureAInputSignal = CreateHydraulicPressureAInputSignal();
            _hydPressureBInputSignal = CreateHydraulicPressureBInputSignal();
        }


        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    UnregisterForInputEvents();
                    AbandonInputEventHandlers();
                    Common.Util.DisposeObject(_hydARenderer);
                    Common.Util.DisposeObject(_hydBRenderer);
                    if (_hydA_DTSCard !=null)
                    {
                        _hydA_DTSCard.Clean();
                        _hydA_DTSCard.Dispose();
                        _hydA_DTSCard = null;
                    }
                    if (_hydB_DTSCard != null)
                    {
                        _hydB_DTSCard.Clean();
                        _hydB_DTSCard.Dispose();
                        _hydB_DTSCard = null;
                    }
                }
            }
            _isDisposed = true;
        }

        private void hydPressureA_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateHydAOutputValues();
        }

        private void hydPressureB_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateHydBOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_hydPressureAInputSignal != null)
            {
                _hydPressureAInputSignal.SignalChanged += _hydPressureAInputSignalChangedEventHandler;
            }
            if (_hydPressureBInputSignal != null)
            {
                _hydPressureBInputSignal.SignalChanged += _hydPressureBInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_hydPressureAInputSignalChangedEventHandler != null && _hydPressureAInputSignal != null)
            {
                try
                {
                    _hydPressureAInputSignal.SignalChanged -= _hydPressureAInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }

            if (_hydPressureBInputSignalChangedEventHandler != null && _hydPressureBInputSignal != null)
            {
                try
                {
                    _hydPressureBInputSignal.SignalChanged -= _hydPressureBInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateHydAOutputValues()
        {
            if (_hydPressureAInputSignal == null) return;
            var hydPressureAInput = _hydPressureAInputSignal.State;
            var hydAAngle = hydPressureAInput < 0
                    ? 0
                    : (hydPressureAInput > 5000
                        ? 320.0000 
                        : (hydPressureAInput / 5000) * 360.0000);

            if (hydAAngle < 0)
            {
                hydAAngle = 0;
            }
            else if (hydAAngle > 360)
            {
                hydAAngle = 360;
            }
            _hydA_DTSCard.SetAngle(hydAAngle);
            _hydA_DTSCard.Update();
        }
        private void UpdateHydBOutputValues()
        {
            if (_hydPressureBInputSignal == null) return;
            var hydPressureBInput = _hydPressureBInputSignal.State;
            var hydBAngle = hydPressureBInput < 0
                    ? 0
                    : (hydPressureBInput > 5000
                        ? 360.0000
                        : (hydPressureBInput / 5000) * 360.0000);

            if (hydBAngle < 0)
            {
                hydBAngle = 0;
            }
            else if (hydBAngle > 360)
            {
                hydBAngle = 360;
            }
            _hydB_DTSCard.SetAngle(hydBAngle);
            _hydB_DTSCard.Update();

        }
    }
}