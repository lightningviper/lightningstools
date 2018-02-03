using System;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Drawing.Imaging;
using Common.Drawing.Text;
using System.Runtime.Remoting.Contexts;
using System.Windows.Forms;
using Common.Collections;
using Common.Imaging;
using Common.InputSupport.DirectInput;
using F16CPD.Mfd.Controls;
using F16CPD.Properties;
using F16CPD.SimSupport;
using F16CPD.SimSupport.Falcon4;
using F16CPD.UI.Forms;
using log4net;

namespace F16CPD
{
    [Synchronization]
    internal partial class F16CpdEngine : MfdForm, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (F16CpdEngine));
        private readonly Bitmap _bezel = Resources.cpdbezel;
        private readonly IControlBindingsLoader _controlBindingsLoader = new ControlBindingsLoader();
        private SerializableDictionary<CpdInputControls, ControlBinding> _controlBindings =
            new SerializableDictionary<CpdInputControls, ControlBinding>();
        private IDirectInputEventHandler _directInputEventHandler;
        private IKeyboardWatcher _keyboardWatcher = new KeyboardWatcher();
        private IMouseClickHandler _mouseClickHandler = new MouseClickHandler();

        private bool _disposing;
        private bool _isDisposed;

        private bool _keepRunning;
        private F16CpdMfdManager _manager;
        private Mediator _mediator;
        private Mediator.PhysicalControlStateChangedEventHandler _mediatorHandler;
        private Bitmap _renderTarget;
        private ISimSupportModule _simSupportModule;

        public F16CpdEngine()
        {
            InitializeComponent();
        }

        public bool TestMode { get; set; }

        private Bitmap CreateRenderTarget()
        {
            var rotation = Settings.Default.Rotation;
            if (rotation == RotateFlipType.Rotate180FlipNone || rotation == RotateFlipType.RotateNoneFlipNone)
            {
                return new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format16bppRgb565);
            }
            return new Bitmap(ClientRectangle.Height, ClientRectangle.Width, PixelFormat.Format16bppRgb565);
        }

        private void InitializeInternal()
        {
            if (_mediator != null)
            {
                _mediator.PhysicalControlStateChanged -= _mediatorHandler;
                Common.Util.DisposeObject(_mediator);
            }
            _mediator = new Mediator(this) {RaiseEvents = true};
            _controlBindings = _controlBindingsLoader.LoadControlBindings(_mediator);
            UpdateMfdManagerSize();
            LoadSimSupportModule();
            _mediatorHandler =
                new Mediator.PhysicalControlStateChangedEventHandler(_directInputEventHandler.MediatorPhysicalControlStateChanged);
            _mediator.PhysicalControlStateChanged += _mediatorHandler;
            foreach (var deviceMonitor in _mediator.DeviceMonitors.Values)
            {
                deviceMonitor.Poll();
            }
            _keyboardWatcher.Start(new KeyDownEventHandler(_controlBindings, _manager));
            _mouseClickHandler.Start(_manager, this, delegate() { 
                RenderOnce(Settings.Default.PollingFrequencyMillis);
                Application.DoEvents();
            });

        }

        public void Start()
        {
            InitializeInternal();
            _renderTarget = CreateRenderTarget();
            _keepRunning = true;
            if (Settings.Default.RunAsClient)
            {
                if (_manager != null && _manager.Client != null)
                {
                    _manager.Client.ClearPendingClientMessages();
                }
            }

            if (!Settings.Default.RunAsServer)
            {
                //if in client or standalone mode
                Show();
                var size = new Size(Settings.Default.CpdWindowWidth, Settings.Default.CpdWindowHeight);
                var location = new Point(Settings.Default.CpdWindowX, Settings.Default.CpdWindowY);
                SetClientSizeCore(size.Width, size.Height);
                Draggable = true;
                BackColor = Color.Green;
                WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.None;
                TopMost = true;
                Location = location;
                SetClientSizeCore(size.Width, size.Height);
                UpdateMfdManagerSize();
                UpdateMfdManagerSize();
                TransparencyKey = Color.AntiqueWhite;
            }
            else
            {
                Hide();
            }
            var pollingFrequencyMillis = Settings.Default.PollingFrequencyMillis;
            if (pollingFrequencyMillis < 15) pollingFrequencyMillis = 15;
            while (_keepRunning)
            {
                try
                {
                    RenderOnce(pollingFrequencyMillis);
                    Application.DoEvents();
                }
                catch (Exception e)
                {
                    _log.Error(e.Message, e);
                }
            }
        }

        private void RenderOnce(int pollingFrequencyMillis)
        {
            if (_isDisposed || !_keepRunning) return;
            UpdateMfdManagerSize();
            Render();
            var loopEndTime = DateTime.UtcNow;
        }

        protected void UpdateMfdManagerSize()
        {
            var rotation = Settings.Default.Rotation;
            var oldWidth = DesktopBounds.Width;
            var oldHeight = DesktopBounds.Height;

            int newWidth;
            int newHeight;
            if (rotation == RotateFlipType.RotateNoneFlipNone || rotation == RotateFlipType.Rotate180FlipNone)
            {
                newWidth = Math.Min(oldHeight, oldWidth);
                newHeight = Math.Max(oldHeight, oldWidth);
            }
            else
            {
                newWidth = Math.Max(oldHeight, oldWidth);
                newHeight = Math.Min(oldHeight, oldWidth);
            }
            Width = newWidth;
            Height = newHeight;

            if (_manager == null)
            {
                CreateMfdManager(rotation);
            }
            else
            {
                UpdateMfdManagerScreenBounds(rotation);
            }

            return;
        }

        private void LoadSimSupportModule()
        {
            if (_simSupportModule == null)
            {
                _simSupportModule = new Falcon4Support(_manager);
                _manager.SimSupportModule = _simSupportModule;
            }
        }

        private void UpdateMfdManagerScreenBounds(RotateFlipType rotation)
        {
            if (rotation == RotateFlipType.Rotate180FlipNone || rotation == RotateFlipType.RotateNoneFlipNone)
            {
                _manager.ScreenBoundsPixels = new Size(DesktopBounds.Width, DesktopBounds.Height);
            }
            else
            {
                _manager.ScreenBoundsPixels = new Size(DesktopBounds.Height, DesktopBounds.Width);
            }
        }

        private void CreateMfdManager(RotateFlipType rotation)
        {
            if (rotation == RotateFlipType.Rotate180FlipNone || rotation == RotateFlipType.RotateNoneFlipNone)
            {
                _manager = new F16CpdMfdManager(new Size(DesktopBounds.Width, DesktopBounds.Height));
            }
            else
            {
                _manager = new F16CpdMfdManager(new Size(DesktopBounds.Height, DesktopBounds.Width));
            }
            _directInputEventHandler = new DirectInputEventHandler(_controlBindings, _manager);
            _keyboardWatcher.Start(new KeyDownEventHandler(_controlBindings, _manager));
        }

        public void Stop()
        {
            _keepRunning = false;
            SavePositionAndSize();
            Application.DoEvents();
            _keyboardWatcher.Stop();
            Close();
            Dispose();
        }
        private void SavePositionAndSize()
        {
            Settings.Default.CpdWindowWidth = DesktopBounds.Width;
            Settings.Default.CpdWindowHeight = DesktopBounds.Height;
            Settings.Default.CpdWindowX = Location.X;
            Settings.Default.CpdWindowY = Location.Y;
            Util.SaveCurrentProperties();
        }
        protected void Render()
        {
            if (_isDisposed || !_keepRunning || _disposing) return;

            try
            {
                _manager.ProcessPendingMessages();
                _simSupportModule.UpdateManagerFlightData();
                _manager
                    .FindMenuPageByName("Instruments Display Page")
                    .FindOptionSelectButtonByFunctionName("ToggleAltimeterModeElecPneu")
                    .LabelText = _manager.FlightData.AltimeterMode == AltimeterMode.Electronic ? "ELEC" : "PNEU";

                if (Settings.Default.RunAsServer)
                {
                    return; //no rendering needed in server mode
                }
                if (_disposing) return;
                using (var h = Graphics.FromImage(_renderTarget))
                {
                    h.Clear(Color.Black);
                    //h.CompositingQuality = CompositingQuality.HighQuality;
                    h.SmoothingMode = SmoothingMode.AntiAlias;
                    h.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    h.TextContrast = 10;
                    var origTransform = h.Transform;
                    //h.SetClip(this.DisplayRectangle);
                    _manager.Render(h);
                    h.Transform = origTransform;

                    var outerBounds = DesktopBounds;
                    var innerBounds = DesktopBounds;
                    innerBounds.Inflate(-7, -7);
                    var curPos = Cursor.Position;

                    if ((outerBounds.Contains(curPos) && !innerBounds.Contains(curPos)) ||
                        (Cursor == Cursors.SizeAll && Drag))
                    {
                        var greenPen = new Pen(Color.Green) {Width = 7};
                        var renderTargetBounds = new Rectangle(0, 0, _renderTarget.Width, _renderTarget.Height);
                        h.DrawRectangleFast(greenPen, renderTargetBounds);
                    }


                    var attrs = new ImageAttributes();
                    ColorMatrix cm;
                    if (_manager.NightMode)
                    {
                        cm = new ColorMatrix
                            (
                            new[]
                                {
                                    new float[] {0, 0, 0, 0, 0}, //red %
                                    new[]
                                        {
                                            0,
                                            (_manager.Brightness/(float) _manager.MaxBrightness),
                                            0, 0, 0
                                        }, //green
                                    new float[] {0, 0, 0, 0, 0}, //blue %
                                    new float[] {0, 0, 0, 1, 0}, //alpha %
                                    new float[] {-1, 0, -1, 0, 1}, //add
                                }
                            );
                    }
                    else
                    {
                        cm = new ColorMatrix
                            (
                            new[]
                                {
                                    new[] {_manager.Brightness/(float) _manager.MaxBrightness, 0, 0, 0, 0}, //red %
                                    new[] {0, _manager.Brightness/(float) _manager.MaxBrightness, 0, 0, 0}, //green %
                                    new[] {0, 0, _manager.Brightness/(float) _manager.MaxBrightness, 0, 0}, //blue %
                                    new float[] {0, 0, 0, 1, 0}, //alpha %
                                    new float[] {0, 0, 0, 0, 1}, //add
                                }
                            );
                    }
                    attrs.SetColorMatrix(cm, ColorMatrixFlag.Default);

                    using (var formGraphics = (Graphics)CreateGraphics())
                    {
                        formGraphics.CompositingQuality = CompositingQuality.HighQuality;
                        formGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        formGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        formGraphics.SmoothingMode = SmoothingMode.HighQuality;
                        formGraphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                        var rotation = Settings.Default.Rotation;
                        if (rotation == RotateFlipType.RotateNoneFlipNone)
                        {
                            formGraphics.DrawImageFast(_renderTarget, DisplayRectangle, 0, 0, _renderTarget.Width,
                                                   _renderTarget.Height, GraphicsUnit.Pixel, attrs);
                        }
                        else
                        {
                            using (var rotated = (Bitmap) _renderTarget.Clone())
                            {
                                rotated.RotateFlip(rotation);
                                formGraphics.DrawImageFast(rotated, DisplayRectangle, 0, 0, rotated.Width, rotated.Height,
                                                       GraphicsUnit.Pixel, attrs);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }


        

        private void F16CpdEngine_SizeChanged(object sender, EventArgs e)
        {
            UpdateMfdManagerSize();

            Common.Util.DisposeObject(_renderTarget);
            _renderTarget = CreateRenderTarget();
        }

        #region Destructors

        public new void Dispose()
        {
            Dispose(true);
            try
            {
                base.Dispose(true);
            }
            catch (ObjectDisposedException e)
            {
                _log.Debug(e.Message, e);
            }
            GC.SuppressFinalize(this);
        }

        ~F16CpdEngine()
        {
            Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _disposing = true;
                    if (components != null) components.Dispose();
                    try
                    {
                        if (_mediator != null)
                        {
                            _mediator.PhysicalControlStateChanged -= _mediatorHandler;
                        }
                        _mouseClickHandler.Stop();
                    }
                    catch (Exception e)
                    {
                        _log.Debug(e.Message, e);
                    }
                    //dispose of managed resources here
                    Common.Util.DisposeObject(_simSupportModule);
                    Common.Util.DisposeObject(_bezel);
                    Common.Util.DisposeObject(_mediator);
                    Common.Util.DisposeObject(_manager);
                    Common.Util.DisposeObject(_controlBindings);
                    Common.Util.DisposeObject(_renderTarget);
                    _keyboardWatcher.Stop();
                }
                base.Dispose(disposing);
            }
            // Code to dispose the un-managed resources of the class
            _isDisposed = true;
        }

        #endregion

        #region Nested type: RECT

        public struct RECT
        {
            public int Bottom { get; set; }
            public int Left { get; set; }
            public int Right { get; set; }
            public int Top { get; set; }
        }

        #endregion
    }
}