using F4SharedMem;
using LightningGauges.Renderers.F16.RWR;
using System.Windows.Media;


namespace SimLinkup.HardwareSupport.TeensyRWR
{
    internal class BMSRWRRenderer
    {
        private readonly IRWRRendererFactory _rwrRendererFactory;
        private Reader _sharedmemReader = new Reader();
        private IRWRRenderer _rwrRenderer;
        private string rwrType = string.Empty;

        public BMSRWRRenderer(IRWRRendererFactory rwrRendererFactory=null)
        {
            _rwrRendererFactory = rwrRendererFactory ?? new RWRRendererFactory();
        }
        public void Render(DrawingContext drawingContext)
        {
            if (!_sharedmemReader.IsFalconRunning) { return; }
            var flightData = _sharedmemReader.GetCurrentData();
            if (_rwrRenderer == null)
            {
                CreateRWRRenderer(flightData);
            }
            if (_rwrRenderer != null)
            {
                _rwrRenderer.InstrumentState = GetInstrumentState(flightData);
                _rwrRenderer.Render(drawingContext);
            }
        }
        private InstrumentState GetInstrumentState(FlightData flightData)
        {
            var instrumentState = new InstrumentState
            {
                bearing = flightData.bearing,
                ChaffCount = flightData.ChaffCount,
                FlareCount = flightData.FlareCount,
                lethality = flightData.lethality,
                missileActivity = flightData.missileActivity,
                missileLaunch = flightData.missileLaunch,
                newDetection = flightData.newDetection,
                RwrInfo = flightData.RwrInfo,
                RwrObjectCount = flightData.RwrObjectCount,
                RWRsymbol = flightData.RWRsymbol,
                selected = flightData.selected,
                yaw = flightData.yaw
            };
            return instrumentState;
        }
        private void CreateRWRRenderer(FlightData flightData)
        {
            string[] rwrInfo = null;
            var rwrInfoBuffer = string.Empty;

            for (var i = 0; i < flightData.RwrInfo.Length; i++)
            {
                rwrInfoBuffer += (char)flightData.RwrInfo[i];
            }
            rwrInfo = rwrInfoBuffer.Split('<');

            if (RwrInfoContains(rwrInfo, "type") && RwrInfoGetKeyContent(rwrInfo, "type") != rwrType)
            {
                rwrType = RwrInfoGetKeyContent(rwrInfo, "type");
            }

            if ((rwrType == "0" || rwrType == "1"))
            {
                _rwrRenderer = _rwrRendererFactory.CreateRenderer(RWRType.ALR56);
            }
            else if (rwrType == "2")
            {
                _rwrRenderer = _rwrRendererFactory.CreateRenderer(RWRType.ALR69);
            }
            else if (rwrType == "3")
            {
                _rwrRenderer = _rwrRendererFactory.CreateRenderer(RWRType.ALR93);
            }
            else if (rwrType == "4")
            {
                _rwrRenderer = _rwrRendererFactory.CreateRenderer(RWRType.SPS1000);
            }
            else if (rwrType == "5")
            {
                _rwrRenderer = _rwrRendererFactory.CreateRenderer(RWRType.ALR67);
            }
            else if (rwrType == "6")
            {
                _rwrRenderer = _rwrRendererFactory.CreateRenderer(RWRType.CARAPACE);
            }
            else
            {
                _rwrRenderer = null;
            }
        }
        private bool RwrInfoContains(string[] rwrInfo, string key)
        {
            for (int i = 0; i < rwrInfo.Length; i++)
            {
                if (rwrInfo[i].StartsWith(key + ">")) return true;
            }
            return false;
        }

        private string RwrInfoGetKeyContent(string[] rwrInfo, string key)
        {
            for (int i = 0; i < rwrInfo.Length; i++)
            {
                if (rwrInfo[i].StartsWith(key + ">")) return rwrInfo[i].Replace(key + ">", string.Empty).Replace("\0", string.Empty);
            }
            return string.Empty;
        }
    }
}
