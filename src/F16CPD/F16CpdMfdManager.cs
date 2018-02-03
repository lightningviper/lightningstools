using System;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Common.Imaging;
using Common.PDF;
using F16CPD.FlightInstruments;
using F16CPD.FlightInstruments.Pfd;
using F16CPD.Mfd;
using F16CPD.Mfd.Controls;
using F16CPD.Mfd.Menus;
using F16CPD.Mfd.Menus.InstrumentsDisplay;
using F16CPD.Networking;
using F16CPD.Properties;
using F16CPD.SimSupport;
using Message = F16CPD.Networking.Message;
using log4net;

namespace F16CPD
{
    //TODO: fix nautical miles scale on map screen (mostly fixed, need to test bounds -- how high should this go?)
    //TODO: add map centering options on map screen
    //TODO: add track options on map screen (track up, desired track up, etc.) 
    //TODO: implement built-in test mode
    //TODO: implement other MFD pages
    //TODO: PRIO create way to save input assignments
    //TODO: PRIO prevent output window resizing below a certain threshold
    //TODO: re-enable "recover output window" button on main form
    //TODO: convert from SingleInstanceApplication??
    //TODO: retest networking now that output rotation is enabled.
    public sealed class F16CpdMfdManager : MfdManager, IDisposable
    {
        private const int MAX_BRIGHTNESS = 255;
        private const int NUM_BRIGHTNESS_STEPS = 30;
        private static readonly ILog _log = LogManager.GetLogger(typeof(F16CpdMfdManager));
        private readonly IBrightnessDecreaseButtonFactory _brightnessDecreaseButtonFactory;
        private readonly IBrightnessIncreaseButtonFactory _brightnessIncreaseButtonFactory;
        private readonly IChartsMenuPageFactory _chartsMenuPageFactory;
        private readonly IChecklistMenuPageFactory _checklistMenuPageFactory;
        private readonly IDayModeButtonFactory _dayModeButtonFactory;
        private readonly IExtFuelTransSwitchFactory _extFuelTransSwitchFactory;
        private readonly IFuelSelectSwitchFactory _fuelSelectSwitchFactory;
        private readonly IHsiModeSlectorSwitchFactory _hsiModeSelectorSwitchFactory;
        private readonly IInstrumentsDisplayMenuPageFactory _instrumentsDisplayMenuPageFactory;
        private readonly object _mapImageLock = new object();
        private readonly IMfdInputControlFinder _mfdInputControlFinder;
        private readonly INightModeButtonFactory _nightModeButtonFactory;
        private readonly IParamSelectKnobFactory _paramSelectKnobFactory;
        private readonly IPrimaryMenuPageFactory _primaryMenuPageFactory;
        private readonly ITADMenuPageFactory _tadMenuPageFactory;
        private readonly ITargetingPodMenuPageFactory _targetingPodMenuPageFactory;
        private int _airspeedIndexInKnots;
        private int _altitudeIndexInFeet;
        private int _brightness = 255;
        private F16CPDClient _client;
        private FileInfo _currentChartFile;
        private int _currentChartPageNum = 1;
        private int _currentChartPagesTotal;
        private FileInfo _currentChecklistFile;
        private int _currentChecklistPageNum = 1;
        private int _currentChecklistPagesTotal;
        private bool _isDisposed;

        private FileInfo _lastRenderedChartFile;
        private int _lastRenderedChartPageNum = 1;
        private Bitmap _lastRenderedChartPdfPage;
        private FileInfo _lastRenderedChecklistFile;
        private int _lastRenderedChecklistPageNum = 1;
        private Bitmap _lastRenderedChecklistPdfPage;
        private int _mapRangeRingsRadiusInNauticalMiles = 30;
        private float _mapZoom = 25000.0f;
        private bool _nightMode;
        private ISimSupportModule _simSupportModule;
        internal F16CpdMfdManager(Size screenBoundsPixels,
            IPrimaryMenuPageFactory primaryMenuPageFactory = null,
            IInstrumentsDisplayMenuPageFactory instrumentsDisplayMenuPageFactory = null,
            ITargetingPodMenuPageFactory targetingPodMenuPageFactory = null,
            ITADMenuPageFactory tadMenuPageFactory = null,
            IChecklistMenuPageFactory checklistMenuPageFactory = null,
            IChartsMenuPageFactory chartsMenuPageFactory = null,
            IMfdInputControlFinder mfdInputControlFinder = null,
            IHsiModeSlectorSwitchFactory hsiModeSelectorSwitchFactory = null,
            IFuelSelectSwitchFactory fuelSelectSwitchFactory = null,
            IExtFuelTransSwitchFactory extFuelTransSwitchFactory = null,
            IParamSelectKnobFactory paramSelectKnobFactory = null,
            INightModeButtonFactory nightModeButtonFactory = null,
            IDayModeButtonFactory dayModeButtonFactory = null,
            BrightnessIncreaseButtonFactory brightnessIncreaseButtonFactory = null,
            IBrightnessDecreaseButtonFactory brightnessDecreaseButtonFactory = null
            ) : base(screenBoundsPixels)
        {
            _primaryMenuPageFactory = primaryMenuPageFactory ?? new PrimaryMenuPageFactory(this);
            _instrumentsDisplayMenuPageFactory = instrumentsDisplayMenuPageFactory ??
                                                 new InstrumentsDisplayMenuPageFactory(this);
            _targetingPodMenuPageFactory = targetingPodMenuPageFactory ?? new TargetingPodMenuPageFactory(this);
            _tadMenuPageFactory = tadMenuPageFactory ?? new TADMenuPageFactory(this);
            _checklistMenuPageFactory = checklistMenuPageFactory ?? new ChecklistMenuPageFactory(this);
            _chartsMenuPageFactory = chartsMenuPageFactory ?? new ChartsMenuPageFactory(this);
            _mfdInputControlFinder = mfdInputControlFinder ?? new MfdInputControlFinder(this);
            _hsiModeSelectorSwitchFactory = hsiModeSelectorSwitchFactory ?? new HsiModeSelectorSwitchFactory(this);
            _fuelSelectSwitchFactory = fuelSelectSwitchFactory ?? new FuelSelectSwitchFactory(this);
            _extFuelTransSwitchFactory = extFuelTransSwitchFactory ?? new ExtFuelTransSwitchFactory(this);
            _paramSelectKnobFactory = paramSelectKnobFactory ?? new ParamSelectKnobFactory(this);
            _nightModeButtonFactory = nightModeButtonFactory ?? new NightModeButtonFactory(this);
            _dayModeButtonFactory = dayModeButtonFactory ?? new DayModeButtonFactory(this);
            _brightnessDecreaseButtonFactory = brightnessDecreaseButtonFactory ??
                                               new BrightnessDecreaseButtonFactory(this);
            _brightnessIncreaseButtonFactory = brightnessIncreaseButtonFactory ??
                                               new BrightnessIncreaseButtonFactory(this);
            SetupNetworking();
            BuildMfdPages();
            BuildNonOsbInputControls();
            InitializeFlightInstruments();
            _brightness = Settings.Default.Brightness;
            MapRotationMode = MapRotationMode.HeadingUp;
        }

        public int AltitudeIndexInFeet
        {
            get { return _altitudeIndexInFeet; }
            set
            {
                if (value < 0) value = 0;
                if (value > 99980) value = 99980;
                _altitudeIndexInFeet = value;
            }
        }

        public int AirspeedIndexInKnots
        {
            get { return _airspeedIndexInKnots; }
            set
            {
                if (value < 0) value = 0;
                if (value > 1500) value = 1500;
                _airspeedIndexInKnots = value;
            }
        }

        public int MapRangeRingsRadiusInNauticalMiles
        {
            get { return _mapRangeRingsRadiusInNauticalMiles; }
            set { _mapRangeRingsRadiusInNauticalMiles = value; }
        }

        public IF16CPDClient Client
        {
            get { return _client; }
        }

        public ISimSupportModule SimSupportModule
        {
            get { return _simSupportModule; }
            set { _simSupportModule = value; }
        }

        public Pfd Pfd { get; set; }
        public Hsi Hsi { get; set; }
        public FlightData FlightData { get; set; }

        public int Brightness
        {
            get { return _brightness; }
        }

        public int MaxBrightness
        {
            get { return MAX_BRIGHTNESS; }
        }

        public bool NightMode
        {
            get { return _nightMode; }
            set { _nightMode = value; }
        }

        public float MapZoom
        {
            get { return _mapZoom; }
        }
        public MapRotationMode MapRotationMode { get; set; }

        internal ToggleSwitchMfdInputControl HsiModeSelectorSwitch { get; private set; }

        internal RotaryEncoderMfdInputControl ParamAdjustKnob { get; private set; }

        internal ToggleSwitchMfdInputControl FuelSelectSwitch { get; private set; }

        internal ToggleSwitchMfdInputControl ExtFuelTransSwitch { get; private set; }

        public void DecreaseBrightness()
        {
            unchecked
            {
                const int brightnessStep = (int) (MAX_BRIGHTNESS/(float) NUM_BRIGHTNESS_STEPS);
                _brightness -= brightnessStep;
                if (_brightness < 0) _brightness = 0;
                Settings.Default.Brightness = _brightness;
                Util.SaveCurrentProperties();
            }
        }

        public void IncreaseBrightness()
        {
            unchecked
            {
                const int brightnessStep = (int) (MAX_BRIGHTNESS/(float) NUM_BRIGHTNESS_STEPS);
                _brightness += brightnessStep;
                if (_brightness > MAX_BRIGHTNESS) _brightness = MAX_BRIGHTNESS;
                Settings.Default.Brightness = _brightness;
                Util.SaveCurrentProperties();
            }
        }


        private static void TeardownService()
        {
            if (!Settings.Default.RunAsServer) return;
            var portNumber = Settings.Default.ServerPortNum;
            int port;
            Int32.TryParse(portNumber, out port);
            try
            {
                F16CPDServer.TearDownService(port);
            }
            catch { }
        }

        private void SetupNetworking()
        {
            if (Settings.Default.RunAsServer)
            {
                var portNumber = Settings.Default.ServerPortNum;
                int port;
                Int32.TryParse(portNumber, out port);
                try
                {
                    F16CPDServer.CreateService("F16CPDService", port);
                }
                catch (Exception e)
                {
                    _log.Error(e.Message, e);
                }
            }
            else if (Settings.Default.RunAsClient)
            {
                var serverIPAddress = Settings.Default.ServerIPAddress;
                var portNumber = Settings.Default.ServerPortNum;
                int port;
                Int32.TryParse(portNumber, out port);
                IPAddress ipAddress;
                IPAddress.TryParse(serverIPAddress, out ipAddress);
                var endpoint = new IPEndPoint(ipAddress, port);
                _client = new F16CPDClient(endpoint, "F16CPDService");
                _client.ClearPendingClientMessages();
            }
        }

        private void ProcessPendingMessagesToServerFromClient()
        {
            if (!Settings.Default.RunAsServer) return;
            var pendingMessage = F16CPDServer.GetNextPendingServerMessage();
            if (pendingMessage == null) return;
            var processed = false;
            if (_simSupportModule != null)
            {
                processed = _simSupportModule.ProcessPendingMessageToServerFromClient(pendingMessage);
            }
            if (processed) return;
            switch (pendingMessage.MessageType)
            {
                case "ToggleSplitMapDisplay":
                    ToggleSplitMapDisplay();
                    break;
                case "SetLMFDActiveOnTGP":
                    SetLMFDActiveOnTGP();
                    break;
                case "SetRMFDActiveOnTGP":
                    SetRMFDActiveOnTGP();
                    break;
            }
        }

        private void ProcessPendingMessagesToClientFromServer()
        {
            if (!Settings.Default.RunAsClient) return;
            var pendingMessage = _client.GetNextPendingClientMessage();
            if (pendingMessage == null) return;
            var processed = false;
            if (_simSupportModule != null)
            {
                processed = _simSupportModule.ProcessPendingMessageToClientFromServer(pendingMessage);
            }
            if (processed) return;
            switch (pendingMessage.MessageType)
            {
                case "CpdInputControlChangedEvent":
                    var controlThatChanged = (CpdInputControls) pendingMessage.Payload;
                    FireHandler(controlThatChanged);
                    break;
            }
        }

        private void BuildNonOsbInputControls()
        {
            HsiModeSelectorSwitch = _hsiModeSelectorSwitchFactory.CreateHsiModeSelectorSwitch();
            FuelSelectSwitch = _fuelSelectSwitchFactory.BuildFuelSelectSwitch();
            ExtFuelTransSwitch = _extFuelTransSwitchFactory.BuildExtFuelTransSwitch();
            ParamAdjustKnob = _paramSelectKnobFactory.BuildParamSelectKnob();
        }

        private void InitializeFlightInstruments()
        {
            Pfd = new Pfd();
            Hsi = new Hsi();
            FlightData = new FlightData();
        }

        private void BuildMfdPages()
        {
            var primaryPage = _primaryMenuPageFactory.CreatePrimaryMenuPage();
            var instrumentsDisplayPage = _instrumentsDisplayMenuPageFactory.BuildInstrumentsDisplayMenuPage();
            var tgpPage = _targetingPodMenuPageFactory.BuildTargetingPodMenuPage();
            var tadPage = _tadMenuPageFactory.BuildTADMenuPage();
            var checklistsPage = _checklistMenuPageFactory.BuildChecklistMenuPage();
            var chartsPage = _chartsMenuPageFactory.BuildChartsMenuPage();
            MenuPages = new[]
            {
                primaryPage, instrumentsDisplayPage, tgpPage, tadPage,
                checklistsPage, chartsPage
            };
            foreach (var thisPage in MenuPages)
            {
                thisPage.OptionSelectButtons.Add(_nightModeButtonFactory.CreateNightModeButton(thisPage));
                thisPage.OptionSelectButtons.Add(_dayModeButtonFactory.BuildDayModeButton(thisPage));
                thisPage.OptionSelectButtons.Add(_brightnessIncreaseButtonFactory.BuildBrightnessIncreaseButton(thisPage));
                thisPage.OptionSelectButtons.Add(_brightnessDecreaseButtonFactory.BuildBrightnessDecreaseButton(thisPage));
            }
            ActiveMenuPage = instrumentsDisplayPage;
        }


        internal MfdMenuPage FindMenuPageByName(string name)
        {
            return
                MenuPages.FirstOrDefault(page => name.ToLowerInvariant().Trim() == page.Name.ToLowerInvariant().Trim());
        }


        internal void PrevChartPage()
        {
            if (_currentChartPageNum > 1) _currentChartPageNum--;
        }

        internal void NextChartPage()
        {
            if (_currentChartPageNum != _currentChartPagesTotal && _currentChartPagesTotal > 0)
            {
                _currentChartPageNum++;
            }
        }

        internal void PrevChecklistPage()
        {
            if (_currentChecklistPageNum > 1) _currentChecklistPageNum--;
        }

        internal void NextChecklistPage()
        {
            if (_currentChecklistPageNum != _currentChecklistPagesTotal && _currentChecklistPagesTotal > 0)
            {
                _currentChecklistPageNum++;
            }
        }

        public void SwitchToTADPage()
        {
            if (ActiveMenuPage.Name == "TAD Page")
            {
                ToggleSplitMapDisplay();
            }
            else
            {
                SetPage("TAD Page");
            }
        }

        public void SwitchToChecklistsPage()
        {
            SetPage("Checklists Page");
        }

        public void SwitchToInstrumentsPage()
        {
            SetPage("Instruments Display Page");
        }

        public void SwitchToTargetingPodPage()
        {
            SetPage("Targeting Pod Page");
        }

        public void SwitchToChartsPage()
        {
            SetPage("Charts Page");
        }
        public void SetLMFDActiveOnTGP()
        {
            if (!Settings.Default.RunAsClient)
            {
                FlightData.ActiveMFD = "LMFD";
            }
            else
            {
                var message = new Message("SetLMFDActiveOnTGP", null);
                Client.SendMessageToServer(message);
            }
        }
        public void SetRMFDActiveOnTGP()
        {
            if (!Settings.Default.RunAsClient)
            {
                FlightData.ActiveMFD = "RMFD";
            }
            else
            {
                var message = new Message("SetRMFDActiveOnTGP", null);
                Client.SendMessageToServer(message);
            }
        }

        private void ToggleSplitMapDisplay()
        {
            if (!Settings.Default.RunAsClient)
            {
                FlightData.SplitMapDisplay = !FlightData.SplitMapDisplay;
            }
            else
            {
                var message = new Message("ToggleSplitMapDisplay", null);
                Client.SendMessageToServer(message);
            }
        }
        private void UpdateCurrentChecklistPageCount()
        {
            if (_currentChecklistFile != null)
            {
                var numPages = PdfRenderEngine.NumPagesInPdf(_currentChecklistFile.FullName);
                _currentChecklistPagesTotal = numPages;
                _currentChecklistPageNum = 1;
            }
            else
            {
                _currentChecklistPagesTotal = 0;
                _currentChecklistPageNum = 0;
            }
        }

        internal void NextChecklistFile()
        {
            var files = GetChecklistsFiles();
            _currentChecklistFile = GetNextFile(_currentChecklistFile, files);
            UpdateCurrentChecklistPageCount();
        }

        internal void PrevChecklistFile()
        {
            FileInfo[] files = GetChecklistsFiles();
            _currentChecklistFile = GetPrevFile(_currentChecklistFile, files);
            UpdateCurrentChecklistPageCount();
        }


        private void UpdateCurrentChartPageCount()
        {
            if (_currentChartFile != null)
            {
                var numPages = PdfRenderEngine.NumPagesInPdf(_currentChartFile.FullName);
                _currentChartPagesTotal = numPages;
                _currentChartPageNum = 1;
            }
            else
            {
                _currentChartPagesTotal = 0;
                _currentChartPageNum = 0;
            }
        }

        internal void NextChartFile()
        {
            var files = GetChartFiles();
            _currentChartFile = GetNextFile(_currentChartFile, files);
            UpdateCurrentChartPageCount();
        }

        internal void PrevChartFile()
        {
            var files = GetChartFiles();
            _currentChartFile = GetPrevFile(_currentChartFile, files);
            UpdateCurrentChartPageCount();
        }


        private static FileInfo GetPrevFile(FileInfo currentFile, FileInfo[] files)
        {
            if (files == null || files.Length == 0) return null;
            if (currentFile == null) return files[0];
            for (var i = 0; i < files.Length; i++)
            {
                if (files[i].FullName == currentFile.FullName)
                {
                    return i > 0 ? files[i - 1] : files[files.Length - 1];
                }
            }
            return files[files.Length - 1];
        }

        private static FileInfo GetNextFile(FileInfo currentFile, FileInfo[] files)
        {
            if (files == null || files.Length == 0) return null;
            if (currentFile == null) return files[0];
            for (var i = 0; i < files.Length; i++)
            {
                if (files[i].FullName == currentFile.FullName)
                {
                    return files.Length - 1 > i ? files[i + 1] : files[0];
                }
            }
            return files[0];
        }

        private static FileInfo[] GetChecklistsFiles()
        {
            const string searchPattern = "*.pdf";
            var di = new DirectoryInfo(Application.ExecutablePath);
            if (di.Parent == null) return null;
            var folderToSearch = di.Parent.FullName + Path.DirectorySeparatorChar + "checklists";
            return GetFilesOfType(searchPattern, folderToSearch);
        }

        private static FileInfo[] GetChartFiles()
        {
            const string searchPattern = "*.pdf";
            var di = new DirectoryInfo(Application.ExecutablePath);
            if (di.Parent == null) return null;
            var folderToSearch = di.Parent.FullName + Path.DirectorySeparatorChar + "charts";
            return GetFilesOfType(searchPattern, folderToSearch);
        }

        private static FileInfo[] GetFilesOfType(string searchPattern, string folderToSearch)
        {
            if (String.IsNullOrEmpty(searchPattern) || String.IsNullOrEmpty(folderToSearch))
            {
                return null;
            }
            var di = new DirectoryInfo(folderToSearch);
            FileInfo[] files = null;
            if (di.Exists)
            {
                files = di.GetFiles(searchPattern);
            }
            return files;
        }

        internal void DecreaseMapRange()
        {
            if (_mapRangeRingsRadiusInNauticalMiles >= 25)
            {
                _mapRangeRingsRadiusInNauticalMiles -= 5;
            }
            else
            {
                _mapRangeRingsRadiusInNauticalMiles -= 1;
            }
            if (_mapRangeRingsRadiusInNauticalMiles < 1) _mapRangeRingsRadiusInNauticalMiles = 1;
        }

        internal void IncreaseMapRange()
        {
            if (_mapRangeRingsRadiusInNauticalMiles >= 20)
            {
                _mapRangeRingsRadiusInNauticalMiles += 5;
            }
            else
            {
                _mapRangeRingsRadiusInNauticalMiles += 1;
            }
            if (_mapRangeRingsRadiusInNauticalMiles > 5000) _mapRangeRingsRadiusInNauticalMiles = 5000;
        }
        internal string GetMapRotationModeText(MapRotationMode mapRotationMode)
        {
            switch (MapRotationMode)
            {
                case F16CPD.MapRotationMode.HeadingUp:
                    return "HDG UP";
                case F16CPD.MapRotationMode.NorthUp:
                    return "NORTH UP";
            }
            return GetMapRotationModeText(MapRotationMode.NorthUp);
        }
        internal void MapZoomOut()
        {
            _mapZoom = _mapZoom * 1.1f;
        }

        internal void MapZoomIn()
        {
            _mapZoom = _mapZoom * 0.9f;
        }

        private void SetPage(string pageName)
        {
            var newPage = FindMenuPageByName(pageName);
            if (newPage != null)
            {
                ActiveMenuPage = newPage;
            }
        }


        public void ProcessPendingMessages()
        {
            if (Settings.Default.RunAsServer)
            {
                ProcessPendingMessagesToServerFromClient();
            }
            else if (Settings.Default.RunAsClient)
            {
                ProcessPendingMessagesToClientFromServer();
            }
        }
        private int LabelWidth { get { return (int)(35 * (ScreenBoundsPixels.Width / Constants.F_NATIVE_RES_WIDTH)); } }
        private int LabelHeight { get { return (int)(20 * (ScreenBoundsPixels.Height / Constants.F_NATIVE_RES_HEIGHT)); } }
        public override void Render(Graphics g)
        {
            var greenBrush = new SolidBrush(Color.FromArgb(0, 255, 0));

            var overallRenderRectangle = new Rectangle(0, 0, ScreenBoundsPixels.Width, ScreenBoundsPixels.Height);
            OptionSelectButton button;
            var origTransform = g.Transform;

            switch (ActiveMenuPage.Name)
            {
                case "Instruments Display Page":
                    button = ActiveMenuPage.FindOptionSelectButtonByFunctionName("AltitudeIndexIncrease");
                    button.Visible = !FlightData.PfdOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByLabelText("ALT");
                    button.Visible = !FlightData.PfdOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByFunctionName("AltitudeIndexDecrease");
                    button.Visible = !FlightData.PfdOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByFunctionName("BarometricPressureSettingIncrease");
                    button.Visible = !FlightData.PfdOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByLabelText("BARO");
                    button.Visible = !FlightData.PfdOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByFunctionName("BarometricPressureSettingDecrease");
                    button.Visible = !FlightData.PfdOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByFunctionName("ToggleAltimeterModeElecPneu");
                    button.Visible = !FlightData.PfdOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByFunctionName("LowAltitudeWarningThresholdIncrease");
                    button.Visible = !FlightData.PfdOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByLabelText("ALOW");
                    button.Visible = !FlightData.PfdOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByFunctionName("LowAltitudeWarningThresholdDecrease");
                    button.Visible = !FlightData.PfdOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByFunctionName("AirspeedIndexIncrease");
                    button.Visible = !FlightData.PfdOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByLabelText("ASPD");
                    button.Visible = !FlightData.PfdOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByFunctionName("AirspeedIndexDecrease");
                    button.Visible = !FlightData.PfdOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByFunctionName("CourseSelectIncrease");
                    button.Visible = !FlightData.HsiOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByLabelText("CRS");
                    button.Visible = !FlightData.HsiOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByFunctionName("CourseSelectDecrease");
                    button.Visible = !FlightData.HsiOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByFunctionName("HeadingSelectIncrease");
                    button.Visible = !FlightData.HsiOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByLabelText("HDG");
                    button.Visible = !FlightData.HsiOffFlag & FlightData.CpdPowerOnFlag;
                    button = ActiveMenuPage.FindOptionSelectButtonByFunctionName("HeadingSelectDecrease");
                    button.Visible = !FlightData.HsiOffFlag & FlightData.CpdPowerOnFlag;
                    break;
                case "TAD Page":
                {
                    var mapRangeLabel = ActiveMenuPage.FindOptionSelectButtonByFunctionName("MapRangeLabel");
                    var mapRangeString = _mapRangeRingsRadiusInNauticalMiles.ToString(CultureInfo.InvariantCulture);
                    mapRangeLabel.LabelText = string.Format("{0} NM", mapRangeString);

                    var mapRotationModeLabel = ActiveMenuPage.FindOptionSelectButtonByFunctionName("MapRotationModeLabel");
                    var mapRotationModeString = GetMapRotationModeText(MapRotationMode);
                    mapRotationModeLabel.LabelText = mapRotationModeString;

                }
                    break;
                case "Targeting Pod Page":
                    {
                        var lmfdSelectLabel = ActiveMenuPage.FindOptionSelectButtonByFunctionName("SetLMFDActiveOnTGP");
                        lmfdSelectLabel.InvertLabelText = FlightData.ActiveMFD == "LMFD";

                        var rmfdSelectLabel = ActiveMenuPage.FindOptionSelectButtonByFunctionName("SetRMFDActiveOnTGP");
                        rmfdSelectLabel.InvertLabelText = FlightData.ActiveMFD == "RMFD";
                    }
                    break;
                case "Checklists Page":
                {
                    if (_currentChecklistFile == null) NextChecklistFile();
                }
                    break;
                case "Charts Page":
                {
                    if (_currentChartFile == null) NextChartFile();
                    string shortName = null;
                    if (_currentChartFile != null)
                    {
                        shortName = Common.Win32.Paths.Util.GetShortPathName(_currentChartFile.FullName);
                        shortName = Common.Win32.Paths.Util.Compact(_currentChartFile.Name, 64);
                    }
                    if (shortName != null)
                    {
                        var labelText = shortName.ToUpperInvariant();
                        //currentChartFileLabel.LabelText = labelText;
                    }

                    //currentChartPageNumLabel.LabelText = _currentChartPageNum + "/" + _currentChartPagesTotal;
                }
                    break;
            }
            g.SetClip(overallRenderRectangle);
            g.TranslateTransform(overallRenderRectangle.X, overallRenderRectangle.Y);
            var originalSize = new Size(Constants.I_NATIVE_RES_WIDTH, Constants.I_NATIVE_RES_HEIGHT);
            g.ScaleTransform((overallRenderRectangle.Width/(float) originalSize.Width),
                (overallRenderRectangle.Height/(float) originalSize.Height));

            if (!FlightData.CpdPowerOnFlag)
            {
                const string toDisplay = "OFF";
                var path = new GraphicsPath();
                var sf = new StringFormat(StringFormatFlags.NoWrap)
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                var f = new Font(FontFamily.GenericMonospace, 20, FontStyle.Bold);
                var textSize = g.MeasureString(toDisplay, f, overallRenderRectangle.Size, sf);
                var leftX = (((Constants.I_NATIVE_RES_WIDTH - ((int) textSize.Width))/2));
                var topY = (((Constants.I_NATIVE_RES_HEIGHT - ((int) textSize.Height))/2));
                var target = new Rectangle(leftX, topY, (int) textSize.Width, (int) textSize.Height);
                path.AddString(toDisplay, f.FontFamily, (int) f.Style, f.SizeInPoints, target, sf);
                g.FillPathFast(greenBrush, path);
                return;
            }

            if (!FlightData.CpdPowerOnFlag) return;
            g.Transform = origTransform;

            switch (ActiveMenuPage.Name)
            {
                case "Instruments Display Page":
                {
                    RenderPfd(g);

                }
                    break;
                case "Targeting Pod Page":
                    var tgpRenderRectangle = new Rectangle(LabelWidth + 1, LabelHeight + 1,
                                                (ScreenBoundsPixels.Width - ((LabelWidth + 1) * 2)),
                                                ((ScreenBoundsPixels.Height - ((LabelHeight + 1) * 2))));

                    RenderTGPPage(g, tgpRenderRectangle);
                    break;
                case "TAD Page":
                    RenderTADPage(g, _mapZoom, _mapRangeRingsRadiusInNauticalMiles, FlightData.SplitMapDisplay);
                    break;
                case "Checklists Page":
                    if (_currentChecklistFile != null)
                    {
                        var checklistRenderRectangle = new Rectangle(LabelWidth + 1, LabelHeight + 1,
                            (ScreenBoundsPixels.Width - ((LabelWidth + 1)*2)),
                            ((ScreenBoundsPixels.Height - ((LabelHeight + 1)*2))));
                        RenderCurrentChecklist(g, checklistRenderRectangle);
                    }
                    break;
                case "Charts Page":
                    if (_currentChartFile != null)
                    {
                        var chartRenderRectangle = new Rectangle(LabelWidth + 1, LabelHeight + 1,
                            (ScreenBoundsPixels.Width - ((LabelWidth + 1)*2)),
                            ((ScreenBoundsPixels.Height - ((LabelHeight + 1)*2))));
                        RenderCurrentChart(g, chartRenderRectangle);
                    }
                    break;
            }

            g.Transform = origTransform;
            g.SetClip(overallRenderRectangle);
            g.TranslateTransform(overallRenderRectangle.X, overallRenderRectangle.Y);
            g.ScaleTransform((overallRenderRectangle.Width/(float) originalSize.Width),
                (overallRenderRectangle.Height/(float) originalSize.Height));

            foreach (var thisButton in ActiveMenuPage.OptionSelectButtons.Where(thisButton => thisButton.Visible))
            {
                thisButton.DrawLabel(g);
            }
            g.Transform = origTransform;
        }

        private void RenderPfd(Graphics g)
        {
            var origTransform = g.Transform;
            var pfd = Pfd;
            pfd.Manager = this;
            var pfdRenderRectangle = new Rectangle(LabelWidth + 5, LabelHeight + 1,
                (ScreenBoundsPixels.Width - ((LabelWidth + 5) * 2)),
                ((ScreenBoundsPixels.Height - ((LabelHeight + 1) * 2)) / 2) + 10);
            pfdRenderRectangle = new Rectangle(pfdRenderRectangle.Left, pfdRenderRectangle.Top,
                (pfdRenderRectangle.Width), (pfdRenderRectangle.Height));
            var pfdRenderSize = new Size(610, 495);
            g.SetClip(pfdRenderRectangle);
            g.TranslateTransform(pfdRenderRectangle.X, pfdRenderRectangle.Y);
            g.ScaleTransform((pfdRenderRectangle.Width / (float)pfdRenderSize.Width),
                (pfdRenderRectangle.Height / (float)pfdRenderSize.Height));
            pfd.Render(g, pfdRenderSize);
            g.Transform = origTransform;
            RenderHsi(g, pfdRenderRectangle);

        }

        private void RenderHsi(Graphics g, Rectangle pfdRenderRectangle)
        {
            var origTransform = g.Transform;
            var hsi = Hsi;
            hsi.Manager = this;
            var hsiRenderBounds = new Rectangle(pfdRenderRectangle.Left, pfdRenderRectangle.Bottom + 5,
                pfdRenderRectangle.Width, pfdRenderRectangle.Height - 40);
            var hsiRenderSize = new Size(596, 391);
            origTransform = g.Transform;
            g.SetClip(hsiRenderBounds);
            g.TranslateTransform(hsiRenderBounds.X, hsiRenderBounds.Y);
            g.ScaleTransform((hsiRenderBounds.Width / (float)hsiRenderSize.Width),
                (hsiRenderBounds.Height / (float)hsiRenderSize.Height));
            hsi.Render(g, hsiRenderSize);
            g.Transform = origTransform;
        }

        private static string BreakStringIntoLines(string toBreak, int maxLineLength)
        {
            if (toBreak == null) return null;
            if (maxLineLength <= 0) return "";
            var sb = new StringBuilder();
            for (var i = 0; i < toBreak.Length; i++)
            {
                sb.Append(toBreak[i]);
                if ((i + 1)%maxLineLength == 0)
                {
                    sb.Append("\n");
                }
            }
            return sb.ToString();
        }

        private void RenderCurrentChecklist(Graphics target, Rectangle fullSizeTargetRect)
        {
            var origCompositQuality = target.CompositingQuality;
            var origSmoothingMode = target.SmoothingMode;
            var origInterpolationMode = target.InterpolationMode;

            target.InterpolationMode = InterpolationMode.HighQualityBicubic;
            target.SmoothingMode = SmoothingMode.HighQuality;
            target.CompositingQuality = CompositingQuality.HighQuality;
            var targetRect = new Rectangle(fullSizeTargetRect.X, fullSizeTargetRect.Y, fullSizeTargetRect.Width, (int)(fullSizeTargetRect.Height * 0.95f));
            var labelRect = new Rectangle(targetRect.X, targetRect.Y + targetRect.Height, targetRect.Width, fullSizeTargetRect.Height - targetRect.Height);
            if (_currentChecklistFile != null)
            {
                if (_lastRenderedChecklistFile == null ||
                    _currentChecklistFile.FullName != _lastRenderedChecklistFile.FullName ||
                    _lastRenderedChecklistPageNum != _currentChecklistPageNum)
                {
                    Common.Util.DisposeObject(_lastRenderedChecklistPdfPage);
                    _lastRenderedChecklistPdfPage = PdfRenderEngine.GeneratePageBitmap(_currentChecklistFile.FullName,
                        _currentChecklistPageNum,
                        new Size(150, 150));
                    _lastRenderedChecklistPageNum = _currentChecklistPageNum;
                    _lastRenderedChecklistFile = _currentChecklistFile;
                }
                if (_lastRenderedChecklistPdfPage != null)
                {
                    if (_nightMode)
                    {
                        using (var copy = (Bitmap) Common.Imaging.Util.CopyBitmap(_lastRenderedChecklistPdfPage))
                        using (var reverseVideo = (Bitmap) Common.Imaging.Util.GetDimmerImage(copy, 0.4f))
                        {
                            target.DrawImage(reverseVideo, targetRect, 0, 0, reverseVideo.Width, reverseVideo.Height,
                                GraphicsUnit.Pixel);
                        }
                    }
                    else
                    {
                        target.DrawImage(_lastRenderedChecklistPdfPage, targetRect, 0, 0,
                            _lastRenderedChecklistPdfPage.Width, _lastRenderedChecklistPdfPage.Height,
                            GraphicsUnit.Pixel);
                    }

                    string shortName = string.Empty;
                    string labelText = string.Empty ;
                    if (_currentChecklistFile != null)
                    {
                        shortName = Common.Win32.Paths.Util.GetShortPathName(_currentChecklistFile.FullName);
                        shortName = Common.Win32.Paths.Util.Compact(_currentChecklistFile.Name, 60);
                        labelText = shortName.ToUpperInvariant();
                    }
                    labelText = labelText.PadRight(60, ' ');
                    var pageNumString = _currentChecklistPageNum + "/" + _currentChecklistPagesTotal;
                    labelText = labelText.Substring(0, labelText.Length - pageNumString.Length) + pageNumString;
                    DrawString(target, labelText, labelRect);
                }
            }
            target.InterpolationMode = origInterpolationMode;
            target.SmoothingMode = origSmoothingMode;
            target.CompositingQuality = origCompositQuality;
        }
        private void DrawString(Graphics g, string aString, Rectangle targetRectangle)
        {
            var greenBrush = new SolidBrush(Color.FromArgb(0, 255, 0));

            var path = new GraphicsPath();
            var sf = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };
            var f = new Font(FontFamily.GenericMonospace, 14, FontStyle.Bold);
            path.AddString(aString, f.FontFamily, (int)f.Style, f.SizeInPoints, targetRectangle, sf);
            g.FillPathFast(greenBrush, path);

        }

        private void RenderCurrentChart(Graphics target, Rectangle fullSizeTargetRect)
        {
            var origCompositQuality = target.CompositingQuality;
            var origSmoothingMode = target.SmoothingMode;
            var origInterpolationMode = target.InterpolationMode;

            target.InterpolationMode = InterpolationMode.HighQualityBicubic;
            target.SmoothingMode = SmoothingMode.HighQuality;
            target.CompositingQuality = CompositingQuality.HighQuality;
            var targetRect = new Rectangle(fullSizeTargetRect.X, fullSizeTargetRect.Y, fullSizeTargetRect.Width, (int)(fullSizeTargetRect.Height * 0.95f));
            var labelRect = new Rectangle(targetRect.X, targetRect.Y + targetRect.Height, targetRect.Width, fullSizeTargetRect.Height - targetRect.Height);

            if (_currentChartFile != null)
            {
                if (_lastRenderedChartFile == null || _currentChartFile.FullName != _lastRenderedChartFile.FullName ||
                    _lastRenderedChartPageNum != _currentChartPageNum)
                {
                    Common.Util.DisposeObject(_lastRenderedChartPdfPage);
                    _lastRenderedChartPdfPage = PdfRenderEngine.GeneratePageBitmap(_currentChartFile.FullName,
                        _currentChartPageNum,
                        new Size(150, 150));
                    _lastRenderedChartPageNum = _currentChartPageNum;
                    _lastRenderedChartFile = _currentChartFile;
                }
                if (_lastRenderedChartPdfPage != null)
                {
                    if (_nightMode)
                    {
                        using (var copy = (Bitmap) Common.Imaging.Util.CopyBitmap(_lastRenderedChartPdfPage))
                        using (var reverseVideo = (Bitmap) Common.Imaging.Util.GetDimmerImage(copy, 0.4f))
                        {
                            target.DrawImage(reverseVideo, targetRect, 0, 0, reverseVideo.Width, reverseVideo.Height,
                                GraphicsUnit.Pixel);
                        }
                    }
                    else
                    {
                        target.DrawImage(_lastRenderedChartPdfPage, targetRect, 0, 0, _lastRenderedChartPdfPage.Width,
                            _lastRenderedChartPdfPage.Height, GraphicsUnit.Pixel);
                    }
                    string shortName = string.Empty;
                    string labelText = string.Empty;
                    if (_currentChartFile != null)
                    {
                        shortName = Common.Win32.Paths.Util.GetShortPathName(_currentChartFile.FullName);
                        shortName = Common.Win32.Paths.Util.Compact(_currentChartFile.Name, 60);
                        labelText = shortName.ToUpperInvariant();
                    }
                    labelText = labelText.PadRight(60, ' ');
                    var pageNumString = _currentChartPageNum + "/" + _currentChartPagesTotal;
                    labelText = labelText.Substring(0, labelText.Length - pageNumString.Length) + pageNumString;
                    DrawString(target, labelText, labelRect);
                }
            }
            target.InterpolationMode = origInterpolationMode;
            target.SmoothingMode = origSmoothingMode;
            target.CompositingQuality = origCompositQuality;
        }

        private Bitmap RenderMapOnBehalfOfRemoteClient(Size renderSize, float mapZoom,
            int rangeRingRadiusInNauticalMiles)
        {
            var rendered = new Bitmap(renderSize.Width, renderSize.Height, PixelFormat.Format16bppRgb565);
            using (var g = Graphics.FromImage(rendered))
            {
                var renderRectangle = new Rectangle(new Point(0, 0), renderSize);
                RenderMapLocally(g, renderRectangle, mapZoom, rangeRingRadiusInNauticalMiles);
            }
            return rendered;
        }
        private void RenderTGPPage(Graphics g, Rectangle tgpRenderRectangle)
        {
            byte[] activeMFDImageBytes = null;
            if (FlightData.ActiveMFD == "LMFD") 
            {
                activeMFDImageBytes = FlightData.LMFDImage;
            }
            else if (FlightData.ActiveMFD == "RMFD") 
            {
                activeMFDImageBytes = FlightData.RMFDImage;
            }
            Image activeMFDImage=null;
            if (activeMFDImageBytes !=null)
            {
                activeMFDImage = Common.Imaging.Util.BitmapFromBytes(activeMFDImageBytes);
            }

            if (activeMFDImage != null)
            {
                var mfdRenderRectangleHeight = tgpRenderRectangle.Height - ((int)(tgpRenderRectangle.Height * 0.395));
                var mfdRenderRectangleWidth = Math.Min(mfdRenderRectangleHeight, tgpRenderRectangle.Width);
                if (mfdRenderRectangleWidth < mfdRenderRectangleHeight)
                {
                    mfdRenderRectangleHeight = mfdRenderRectangleWidth;
                }
                var mfdRenderRectangle = new Rectangle(
                    tgpRenderRectangle.X + ((tgpRenderRectangle.Width - mfdRenderRectangleWidth) / 2), 
                    tgpRenderRectangle.Y,
                    mfdRenderRectangleWidth, 
                    mfdRenderRectangleHeight);
                g.DrawImage(activeMFDImage, mfdRenderRectangle, 0, 0, activeMFDImage.Width, activeMFDImage.Height, GraphicsUnit.Pixel);

                var fullScreenRectangle = new Rectangle(0, 0, (ScreenBoundsPixels.Width), (ScreenBoundsPixels.Height));
                var splitTGPPaneHeightDifferenceFromFullScreen = (int)(fullScreenRectangle.Height * 0.395);
                RenderSplitDisplayPfd(g, fullScreenRectangle, splitTGPPaneHeightDifferenceFromFullScreen);
                RenderSplitDisplayHsi(g, fullScreenRectangle, splitTGPPaneHeightDifferenceFromFullScreen);
            }
        }
        private void RenderTADPage(Graphics g, float mapZoom, int rangeRingRadiusInNauticalMiles, bool splitDisplay)
        {
            var overallRenderRectangle = new Rectangle(0, 0, (ScreenBoundsPixels.Width), (ScreenBoundsPixels.Height));
            var mapRenderRectangle = overallRenderRectangle;
            var mapHeightDifferenceFromFullScreen = (int)(overallRenderRectangle.Height * 0.395);
            if (splitDisplay)
            {
                mapRenderRectangle = new Rectangle(overallRenderRectangle.X, overallRenderRectangle.Y, overallRenderRectangle.Width, overallRenderRectangle.Height - mapHeightDifferenceFromFullScreen);
                using (var smallMapRenderTarget = new Bitmap(mapRenderRectangle.Width, mapRenderRectangle.Height, PixelFormat.Format16bppRgb555))
                using (var smallG = Graphics.FromImage(smallMapRenderTarget))
                {
                    RenderMapLocally(smallG, mapRenderRectangle, mapZoom, rangeRingRadiusInNauticalMiles);
                    g.DrawImage(smallMapRenderTarget, new Point(0,0));
                }
            }
            else
            {
                RenderMapLocally(g, mapRenderRectangle, mapZoom, rangeRingRadiusInNauticalMiles);
            }

            if (splitDisplay)
            {
                RenderSplitDisplayPfd(g, overallRenderRectangle, mapHeightDifferenceFromFullScreen);
            }

            if (splitDisplay)
            {
                RenderSplitDisplayHsi(g, overallRenderRectangle, mapHeightDifferenceFromFullScreen);
            }
        }

        private void RenderSplitDisplayPfd(Graphics g, Rectangle overallRenderRectangle, int heightDifferenceFromFullScreen)
        {
            var bigPfdRenderSize = new Size(610, 495);

            var pfdRenderRectangle = new Rectangle(
                overallRenderRectangle.X,
                overallRenderRectangle.Y + (int)(overallRenderRectangle.Height - heightDifferenceFromFullScreen + (overallRenderRectangle.Height*.025)),
                (int)(overallRenderRectangle.Width / 2.0),
                0);

            pfdRenderRectangle.Height = (int)(heightDifferenceFromFullScreen - ((overallRenderRectangle.Height * .025)*3));
            using (var smallPfdRenderTarget = new Bitmap(pfdRenderRectangle.Width, pfdRenderRectangle.Height, PixelFormat.Format16bppRgb555))
            using (var smallG = Graphics.FromImage(smallPfdRenderTarget))
            {
                var pfd = Pfd;
                pfd.Manager = this;
                smallG.ScaleTransform((pfdRenderRectangle.Width / (float)bigPfdRenderSize.Width),
                    (pfdRenderRectangle.Height / (float)bigPfdRenderSize.Height));
                pfd.Render(smallG, bigPfdRenderSize);
                g.DrawImage(smallPfdRenderTarget, new Point(0, pfdRenderRectangle.Y));
            }
        }

        private void RenderSplitDisplayHsi(Graphics g, Rectangle overallRenderRectangle, int heightDifferenceFromFullScreen)
        {
            var bigHsiRenderSize = new Size(596, 391);

            var hsiRenderRectangle = new Rectangle(
                (int)(overallRenderRectangle.Width / 2),
                overallRenderRectangle.Y + (overallRenderRectangle.Height - heightDifferenceFromFullScreen + 20),
                (int)(overallRenderRectangle.Width / 2),
                0);
            hsiRenderRectangle.Height = (int)(bigHsiRenderSize.Height * ((float)hsiRenderRectangle.Width / (float)bigHsiRenderSize.Width));

            using (var smallHsiRenderTarget = new Bitmap(hsiRenderRectangle.Width, hsiRenderRectangle.Height, PixelFormat.Format16bppRgb555))
            using (var smallG = Graphics.FromImage(smallHsiRenderTarget))
            {
                var hsi = Hsi;
                hsi.Manager = this;
                smallG.ScaleTransform((hsiRenderRectangle.Width / (float)bigHsiRenderSize.Width),
                    (hsiRenderRectangle.Height / (float)bigHsiRenderSize.Height));
                hsi.Render(smallG, bigHsiRenderSize);
                g.DrawImage(smallHsiRenderTarget, new Point((int)(overallRenderRectangle.Width / 2), hsiRenderRectangle.Y));
            }
        }

        private void RenderMapLocally(Graphics g, Rectangle renderRectangle, float mapZoom,
            int rangeRingRadiusInNauticalMiles)
        {
            var greenBrush = new SolidBrush(Color.FromArgb(0, 255, 0));
            
            var tadRenderRectangle = renderRectangle;
            g.SetClip(tadRenderRectangle);
            SimSupportModule.RenderMap(g, tadRenderRectangle, mapZoom, rangeRingRadiusInNauticalMiles,
                MapRotationMode);

            var scaleX = (tadRenderRectangle.Width)/Constants.F_NATIVE_RES_WIDTH;
            var scaleY = (tadRenderRectangle.Height)/Constants.F_NATIVE_RES_HEIGHT;

            g.ScaleTransform(scaleX, scaleY);

            var latLongRect = new Rectangle(192, 734, (406 - 192), (768 - 734));
            g.FillRectangle(Brushes.Black, latLongRect);
            var latitudeDecDeg = FlightData.LatitudeInDecimalDegrees;
            var longitudeDecDeg = FlightData.LongitudeInDecimalDegrees;

            var latitudeWholeDeg = (int) (latitudeDecDeg);
            var longitudeWholeDeg = (int) (longitudeDecDeg);

            var latitudeMinutes = (latitudeDecDeg - latitudeWholeDeg)*60.0f;
            var longitudeMinutes = (longitudeDecDeg - longitudeWholeDeg)*60.0f;

            var latitudeQualifier = latitudeDecDeg >= 0.0f ? "N" : "S";
            var longitudeQualifier = longitudeDecDeg >= 0.0f ? "E" : "W";

            var latString = latitudeQualifier + latitudeWholeDeg + " " + string.Format("{0:00.000}", latitudeMinutes);
            var longString = longitudeQualifier + longitudeWholeDeg + " " +
                                string.Format("{0:00.000}", longitudeMinutes);
            var latLongString = latString + "  " + longString;
            var latLongFont = new Font(FontFamily.GenericMonospace, 10, FontStyle.Bold);

            g.DrawStringFast(latLongString, latLongFont, greenBrush, latLongRect);
        }

        private static void InformClientOfCpdInputControlChangedEvent(CpdInputControls control)
        {
            if (!Settings.Default.RunAsServer) return;
            var message = new Message("CpdInputControlChangedEvent", control);
            F16CPDServer.SubmitMessageToClient(message);
        }

        public void FireHandler(CpdInputControls control)
        {
            if (Settings.Default.RunAsServer)
            {
                InformClientOfCpdInputControlChangedEvent(control);
            }
            else
            {
                MfdInputControl inputControl = _mfdInputControlFinder.GetControl(ActiveMenuPage, control);
                if (inputControl != null)
                {
                    if (inputControl is MomentaryButtonMfdInputControl)
                    {
                        ((MomentaryButtonMfdInputControl) inputControl).Press(DateTime.UtcNow);
                    }
                    else if (inputControl is ToggleSwitchMfdInputControl)
                    {
                        ((ToggleSwitchMfdInputControl) inputControl).Toggle();
                    }
                    else if (inputControl is ToggleSwitchPositionMfdInputControl)
                    {
                        ((ToggleSwitchPositionMfdInputControl) inputControl).Activate();
                    }
                    else if (inputControl is RotaryEncoderMfdInputControl)
                    {
                        ((RotaryEncoderMfdInputControl) inputControl).RotateClockwise();
                    }
                }
            }
        }

        #region Destructors

        /// <summary>
        ///     Public implementation of IDisposable.Dispose().  Cleans up managed
        ///     and unmanaged resources used by this object before allowing garbage collection
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Standard finalizer, which will call Dispose() if this object is not
        ///     manually disposed.  Ordinarily called only by the garbage collector.
        /// </summary>
        ~F16CpdMfdManager()
        {
            Dispose();
        }

        /// <summary>
        ///     Private implementation of Dispose()
        /// </summary>
        /// <param name="disposing">
        ///     flag to indicate if we should actually perform disposal.  Distinguishes the private method
        ///     signature from the public signature.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    //dispose of managed resources here
                    Common.Util.DisposeObject(Pfd);
                    Common.Util.DisposeObject(Hsi);
                    Common.Util.DisposeObject(_client);
                    TeardownService();
                }
            }
            // Code to dispose the un-managed resources of the class
            _isDisposed = true;
        }

        #endregion
    }
}