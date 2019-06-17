using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using Common.Math;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.Malwin
{
    //Malwin 19581 F-16 HYDRAULIC PRESSURE IND
    public class Malwin19581HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly IHydraulicPressureGauge _hydARenderer = new HydraulicPressureGauge();
        private readonly IHydraulicPressureGauge _hydBRenderer = new HydraulicPressureGauge();
        private AnalogSignal _hydPressureACOSOutputSignal;
        private AnalogSignal _hydPressureAInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _hydPressureAInputSignalChangedEventHandler;
        private AnalogSignal _hydPressureASINOutputSignal;
        private AnalogSignal _hydPressureBCOSOutputSignal;
        private AnalogSignal _hydPressureBInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _hydPressureBInputSignalChangedEventHandler;
        private AnalogSignal _hydPressureBSINOutputSignal;

        private bool _isDisposed;

        private Malwin19581HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] {_hydPressureAInputSignal, _hydPressureBInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[]
        {
            _hydPressureASINOutputSignal, _hydPressureACOSOutputSignal, _hydPressureBSINOutputSignal,
            _hydPressureBCOSOutputSignal
        };

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Malwin P/N 19581 - Hydraulic Pressure Ind";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Malwin19581HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Malwin19581HardwareSupportModule()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            g.Clear(Color.Black);

            _hydARenderer.InstrumentState.HydraulicPressurePoundsPerSquareInch = (float) _hydPressureAInputSignal.State;
            var hydARectangle = new Rectangle(destinationRectangle.X, destinationRectangle.Y,
                destinationRectangle.Width / 2, destinationRectangle.Height / 2);
            using (var hydABitmap = new Bitmap(hydARectangle.Width, hydARectangle.Height))
            using (var hydAGraphics = Graphics.FromImage(hydABitmap))
            {
                _hydARenderer.Render(hydAGraphics, new Rectangle(0, 0, hydARectangle.Width, hydARectangle.Height));
                g.DrawImage(hydABitmap, hydARectangle);
            }

            _hydBRenderer.InstrumentState.HydraulicPressurePoundsPerSquareInch = (float) _hydPressureBInputSignal.State;
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

        private AnalogSignal CreateHydPressureACOSOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Hydraulic Pressure A (COS)",
                Id = "19581_Hydraulic_Pressure_A_COS_To_Instrument",
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

        private AnalogSignal CreateHydPressureASINOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Hydraulic Pressure A (SIN)",
                Id = "19581_Hydraulic_Pressure_A_SIN_To_Instrument",
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

        private AnalogSignal CreateHydPressureBCOSOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Hydraulic Pressure B (COS)",
                Id = "19581_Hydraulic_Pressure_B_COS_To_Instrument",
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

        private AnalogSignal CreateHydPressureBSINOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Hydraulic Pressure B (SIN)",
                Id = "19581_Hydraulic_Pressure_B_SIN_To_Instrument",
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

        private AnalogSignal CreateHydraulicPressureAInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Hydraulic Pressure A",
                Id = "19581_Hydraulic_Pressure_A_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsSine = true,
                MinValue = 0,
                MaxValue = 4000
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
                Id = "19581_Hydraulic_Pressure_B_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 4000
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

        private void CreateOutputSignals()
        {
            _hydPressureASINOutputSignal = CreateHydPressureASINOutputSignal();
            _hydPressureACOSOutputSignal = CreateHydPressureACOSOutputSignal();

            _hydPressureBSINOutputSignal = CreateHydPressureBSINOutputSignal();
            _hydPressureBCOSOutputSignal = CreateHydPressureBCOSOutputSignal();
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
            if (_hydPressureASINOutputSignal != null)
            {
                var hydPressureASINOutputValue = hydPressureAInput < 0
                    ? 0
                    : (hydPressureAInput > 4000
                        ? 10.0000 * Math.Sin(320.0000 * Constants.RADIANS_PER_DEGREE)
                        : 10.0000 *
                          Math.Sin(hydPressureAInput / 4000 * 320.0000 *
                                   Constants.RADIANS_PER_DEGREE));

                if (hydPressureASINOutputValue < -10)
                {
                    hydPressureASINOutputValue = -10;
                }
                else if (hydPressureASINOutputValue > 10)
                {
                    hydPressureASINOutputValue = 10;
                }

                _hydPressureASINOutputSignal.State = hydPressureASINOutputValue;
            }

            if (_hydPressureACOSOutputSignal == null) return;
            var hydPressureACOSOutputValue = hydPressureAInput < 0
                ? 0
                : (hydPressureAInput > 4000
                    ? 10.0000 * Math.Cos(320.0000 * Constants.RADIANS_PER_DEGREE)
                    : 10.0000 *
                      Math.Cos(hydPressureAInput / 4000.0000 * 320.0000 *
                               Constants.RADIANS_PER_DEGREE));

            if (hydPressureACOSOutputValue < -10)
            {
                hydPressureACOSOutputValue = -10;
            }
            else if (hydPressureACOSOutputValue > 10)
            {
                hydPressureACOSOutputValue = 10;
            }

            _hydPressureACOSOutputSignal.State = hydPressureACOSOutputValue;
        }

        private void UpdateHydBOutputValues()
        {
            if (_hydPressureBInputSignal == null) return;
            var hydPressureBInput = _hydPressureBInputSignal.State;
            if (_hydPressureBSINOutputSignal != null)
            {
                var hydPressureBSINOutputValue = hydPressureBInput < 0
                    ? 0
                    : (hydPressureBInput > 4000
                        ? 10.0000 * Math.Sin(320.0000 * Constants.RADIANS_PER_DEGREE)
                        : 10.0000 *
                          Math.Sin(hydPressureBInput / 4000.0000 * 320.0000 *
                                   Constants.RADIANS_PER_DEGREE));

                if (hydPressureBSINOutputValue < -10)
                {
                    hydPressureBSINOutputValue = -10;
                }
                else if (hydPressureBSINOutputValue > 10)
                {
                    hydPressureBSINOutputValue = 10;
                }

                _hydPressureBSINOutputSignal.State = hydPressureBSINOutputValue;
            }

            if (_hydPressureBCOSOutputSignal == null) return;
            var hydPressureBCOSOutputValue = hydPressureBInput < 0
                ? 0
                : (hydPressureBInput > 4000
                    ? 10.0000 * Math.Cos(320.0000 * Constants.RADIANS_PER_DEGREE)
                    : 10.0000 *
                      Math.Cos(hydPressureBInput / 4000.0000 * 320.0000 *
                               Constants.RADIANS_PER_DEGREE));

            if (hydPressureBCOSOutputValue < -10)
            {
                hydPressureBCOSOutputValue = -10;
            }
            else if (hydPressureBCOSOutputValue > 10)
            {
                hydPressureBCOSOutputValue = 10;
            }

            _hydPressureBCOSOutputSignal.State = hydPressureBCOSOutputValue;
        }
    }
}