using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.HardwareSupport.Calibration;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16;
using log4net;

namespace SimLinkup.HardwareSupport.Simtek
{
    //Simtek 10-0581-02 F-16 VVI Indicator (piecewise: -6000..+6000 FPM → ±10 V; 0 FPM at +1.83 V)
    public class Simtek10058102HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek10058102HardwareSupportModule));
        private readonly IVerticalVelocityIndicatorUSA _renderer = new VerticalVelocityIndicatorUSA();

        // Editor-authored calibration. Default ships the 8 spec-sheet
        // test points from the gauge's drawing (Table 1 on sheet 4). Same
        // shape as Simtek10058201 AoA: digital POWER-OFF input overrides
        // to -10 V regardless of the VVI value when true.
        private Simtek10058102HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _vviCalibration;

        private ConfigFileReloadWatcher _configWatcher;

        private bool _isDisposed;

        private DigitalSignal _offFlagInputSignal;
        private DigitalSignal.SignalChangedEventHandler _offFlagInputSignalChangedEventHandler;
        private AnalogSignal _verticalVelocityInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _verticalVelocityInputSignalChangedEventHandler;
        private AnalogSignal _verticalVelocityOutputSignal;

        public Simtek10058102HardwareSupportModule(Simtek10058102HardwareSupportModuleConfig config)
        {
            _config = config;
            _vviCalibration = ResolvePiecewiseChannel(config, "10058102_Vertical_Velocity_To_Instrument");
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            StartConfigWatcher();
        }

        // Same helper as the other piecewise gauges.
        private static GaugeChannelConfig ResolvePiecewiseChannel(GaugeCalibrationConfig config, string channelId)
        {
            if (config == null) return null;
            var ch = config.FindChannel(channelId);
            if (ch != null
                && ch.Transform != null
                && ch.Transform.Kind == "piecewise"
                && ch.Transform.Breakpoints != null
                && ch.Transform.Breakpoints.Length >= 2)
            {
                return ch;
            }
            return null;
        }

        private void StartConfigWatcher()
        {
            if (_config == null || string.IsNullOrEmpty(_config.FilePath)) return;
            try
            {
                _configWatcher = new ConfigFileReloadWatcher(_config.FilePath, ReloadConfig);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private void ReloadConfig()
        {
            try
            {
                var configFile = _config != null ? _config.FilePath : null;
                if (string.IsNullOrEmpty(configFile)) return;
                var reloaded = Simtek10058102HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                _vviCalibration = ResolvePiecewiseChannel(reloaded, "10058102_Vertical_Velocity_To_Instrument");
                // Re-evaluate every output with the cached input values so the
                // user sees the new calibration immediately. Without this,
                // SimLinkup's event-driven update loop won't fire until the
                // simulator next pushes a new input value.
                UpdateOutputValues();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public override AnalogSignal[] AnalogInputs => new[] {_verticalVelocityInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_verticalVelocityOutputSignal};

        public override DigitalSignal[] DigitalInputs => new[] {_offFlagInputSignal};

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-0581-02 - Indicator - Simulated Vertical Velocity";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek10058102HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek10058102HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek10058102HardwareSupportModule.config");
                hsmConfig = Simtek10058102HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek10058102HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            g.Clear(Color.Black);

            _renderer.InstrumentState.OffFlag = _offFlagInputSignal.State;
            _renderer.InstrumentState.VerticalVelocityFeet = (float) _verticalVelocityInputSignal.State;

            var vviWidth = (int) (destinationRectangle.Height * (102f / 227f));
            var vviHeight = destinationRectangle.Height;

            using (var vviBmp = new Bitmap(vviWidth, vviHeight))
            using (var vviBmpGraphics = Graphics.FromImage(vviBmp))
            {
                _renderer.Render(vviBmpGraphics, new Rectangle(0, 0, vviWidth, vviHeight));
                var targetRectangle = new Rectangle(
                    destinationRectangle.X + (int) ((destinationRectangle.Width - vviWidth) / 2.0),
                    destinationRectangle.Y, vviWidth, destinationRectangle.Height);
                g.DrawImage(vviBmp, targetRectangle);
            }
        }

        private void AbandonInputEventHandlers()
        {
            _verticalVelocityInputSignalChangedEventHandler = null;
            _offFlagInputSignalChangedEventHandler = null;
        }

        private void CreateInputEventHandlers()
        {
            _verticalVelocityInputSignalChangedEventHandler = vvi_InputSignalChanged;
            _offFlagInputSignalChangedEventHandler =
                vviPower_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _verticalVelocityInputSignal = CreateVerticalVelocityInputSignal();
            _offFlagInputSignal = CreateOffFlagInputSignal();
        }

        private DigitalSignal CreateOffFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "OFF Flag",
                Id = "10058102_VVI_Power_Off_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };
            return thisSignal;
        }

        private void CreateOutputSignals()
        {
            _verticalVelocityOutputSignal = CreateVerticalVelocityOutputSignal();
        }

        private AnalogSignal CreateVerticalVelocityInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Vertical Velocity (feet per minute)",
                Id = "10058102_Vertical_Velocity_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = -6000,
                MaxValue = 6000
            };
            return thisSignal;
        }

        private AnalogSignal CreateVerticalVelocityOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Vertical Velocity",
                Id = "10058102_Vertical_Velocity_To_Instrument",
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

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    UnregisterForInputEvents();
                    AbandonInputEventHandlers();
                    Common.Util.DisposeObject(_renderer);
                    if (_configWatcher != null)
                    {
                        try { _configWatcher.Dispose(); } catch { }
                        _configWatcher = null;
                    }
                }
            }
            _isDisposed = true;
        }

        private void RegisterForInputEvents()
        {
            if (_verticalVelocityInputSignal != null)
            {
                _verticalVelocityInputSignal.SignalChanged += _verticalVelocityInputSignalChangedEventHandler;
            }
            if (_offFlagInputSignal != null)
            {
                _offFlagInputSignal.SignalChanged += _offFlagInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_verticalVelocityInputSignalChangedEventHandler != null && _verticalVelocityInputSignal != null)
            {
                try
                {
                    _verticalVelocityInputSignal.SignalChanged -= _verticalVelocityInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_offFlagInputSignalChangedEventHandler != null && _offFlagInputSignal != null)
            {
                try
                {
                    _offFlagInputSignal.SignalChanged -= _offFlagInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateOutputValues()
        {
            var vviPowerOff = false;
            if (_offFlagInputSignal != null)
            {
                vviPowerOff = _offFlagInputSignal.State;
            }

            if (_verticalVelocityInputSignal != null)
            {
                var vviInput = _verticalVelocityInputSignal.State;
                double vviOutputValue = 0;

                if (_verticalVelocityOutputSignal != null)
                {
                    // Editor-authored override: when a .config file declared
                    // a piecewise table for this channel, evaluate via the
                    // generic helper and per-channel trim. Power-off check
                    // runs first regardless of config — gauge mechanism,
                    // not calibratable. Falls through to the hardcoded
                    // formula below when no config is present.
                    if (_vviCalibration != null)
                    {
                        double v;
                        if (vviPowerOff)
                        {
                            v = -10;
                        }
                        else
                        {
                            v = GaugeTransform.EvaluatePiecewise(vviInput, _vviCalibration.Transform.Breakpoints);
                        }
                        _verticalVelocityOutputSignal.State = _vviCalibration.ApplyTrim(v, _verticalVelocityOutputSignal.MinValue, _verticalVelocityOutputSignal.MaxValue);
                        return;
                    }

                    if (vviPowerOff)
                    {
                        vviOutputValue = -10;
                    }
                    else
                    {
                        if (vviInput < -6000)
                        {
                            vviOutputValue = -6.37;
                        }
                        else if (vviInput >= -6000 && vviInput < -3000)
                        {
                            vviOutputValue = -6.37 + (vviInput - -6000) / 3000 * 1.66;
                        }
                        else if (vviInput >= -3000 && vviInput < -1000)
                        {
                            vviOutputValue = -4.71 + (vviInput - -3000) / 2000 * 2.90;
                        }
                        else if (vviInput >= -1000 && vviInput < -400)
                        {
                            vviOutputValue = -1.81 + (vviInput - -1000) / 600 * 1.81;
                        }
                        else if (vviInput >= -400 && vviInput < 0)
                        {
                            vviOutputValue = 0 + (vviInput - -400) / 400 * 1.83;
                        }
                        else if (vviInput >= 0 && vviInput < 1000)
                        {
                            vviOutputValue = 1.83 + vviInput / 1000 * 3.65;
                        }
                        else if (vviInput >= 1000 && vviInput < 3000)
                        {
                            vviOutputValue = 5.48 + (vviInput - 1000) / 2000 * 2.9;
                        }
                        else if (vviInput >= 3000 && vviInput < 6000)
                        {
                            vviOutputValue = 8.38 + (vviInput - 3000) / 3000 * 1.62;
                        }
                        else if (vviInput >= 6000)
                        {
                            vviOutputValue = 10;
                        }
                    }

                    if (vviOutputValue < -10)
                    {
                        vviOutputValue = -10;
                    }
                    else if (vviOutputValue > 10)
                    {
                        vviOutputValue = 10;
                    }

                    _verticalVelocityOutputSignal.State = vviOutputValue;
                }
            }
        }

        private void vvi_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void vviPower_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }
    }
}