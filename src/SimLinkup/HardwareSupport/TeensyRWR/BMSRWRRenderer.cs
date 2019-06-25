using LightningGauges.Renderers.F16.RWR;
using System.Drawing;
using System.Windows.Media;


namespace SimLinkup.HardwareSupport.TeensyRWR
{
    internal class BMSRWRRenderer
    {
        private readonly IRWRRendererFactory _rwrRendererFactory;
        private RWRRenderer _rwrRenderer;
        private string _lastRwrType = string.Empty;
        internal int ActualWidth { get; set; }
        internal int ActualHeight{ get; set; }
        public BMSRWRRenderer(IRWRRendererFactory rwrRendererFactory=null)
        {
            _rwrRendererFactory = rwrRendererFactory ?? new RWRRendererFactory();
        }
        public void Render(DrawingContext drawingContext, InstrumentState instrumentState, bool formatForVectorDisplay=false)
        {
            CreateRWRRenderer(instrumentState.RwrInfo, formatForVectorDisplay);
            if (_rwrRenderer != null)
            {
                _rwrRenderer.InstrumentState = instrumentState;
                _rwrRenderer.Render(drawingContext);
            }
        }
        public void Render(Graphics destinationGraphics, Rectangle destinationRectangle, InstrumentState instrumentState, bool formatForVectorDisplay=false)
        {
            CreateRWRRenderer(instrumentState.RwrInfo, formatForVectorDisplay);
            if (_rwrRenderer != null)
            {
                _rwrRenderer.InstrumentState = instrumentState;
                _rwrRenderer.Render(destinationGraphics, destinationRectangle);
            }
        }
        private void CreateRWRRenderer(byte[] rwrInfoArray, bool formatForVectorDisplay=false)
        {
            var rwrType = GetRWRType(rwrInfoArray);
            if (_rwrRenderer == null || rwrType != _lastRwrType)
            {

                if ((rwrType == "0" || rwrType == "1"))
                {
                    _rwrRenderer = _rwrRendererFactory.CreateRenderer(RWRType.ALR56, formatForVectorDisplay);
                }
                else if (rwrType == "2")
                {
                    _rwrRenderer = _rwrRendererFactory.CreateRenderer(RWRType.ALR69, formatForVectorDisplay);
                }
                else if (rwrType == "3")
                {
                    _rwrRenderer = _rwrRendererFactory.CreateRenderer(RWRType.ALR93, formatForVectorDisplay);
                }
                else if (rwrType == "4")
                {
                    _rwrRenderer = _rwrRendererFactory.CreateRenderer(RWRType.SPS1000, formatForVectorDisplay);
                }
                else if (rwrType == "5")
                {
                    _rwrRenderer = _rwrRendererFactory.CreateRenderer(RWRType.ALR67, formatForVectorDisplay);
                }
                else if (rwrType == "6")
                {
                    _rwrRenderer = _rwrRendererFactory.CreateRenderer(RWRType.CARAPACE, formatForVectorDisplay);
                }
                else
                {
                    _rwrRenderer = null;
                }
                _lastRwrType = rwrType;
            }
            if (_rwrRenderer !=null && ActualWidth !=0 && ActualHeight !=0)
            {
                (_rwrRenderer as RWRRenderer).ActualWidth = ActualWidth;
                (_rwrRenderer as RWRRenderer).ActualHeight = ActualHeight;
            }
        }
        private static string GetRWRType(byte[] rwrInfoArray)
        {
            string[] rwrInfo = null;
            var rwrInfoBuffer = string.Empty;

            for (var i = 0; i < rwrInfoArray.Length; i++)
            {
                rwrInfoBuffer += (char)rwrInfoArray[i];
            }
            rwrInfo = rwrInfoBuffer.Split('<');
            string rwrType = string.Empty;
            if (RwrInfoContains(rwrInfo, "type") && RwrInfoGetKeyContent(rwrInfo, "type") != rwrType)
            {
                rwrType = RwrInfoGetKeyContent(rwrInfo, "type");
            }
            return rwrType;
        }
        private static bool RwrInfoContains(string[] rwrInfo, string key)
        {
            for (int i = 0; i < rwrInfo.Length; i++)
            {
                if (rwrInfo[i].StartsWith(key + ">")) return true;
            }
            return false;
        }

        private static string RwrInfoGetKeyContent(string[] rwrInfo, string key)
        {
            for (int i = 0; i < rwrInfo.Length; i++)
            {
                if (rwrInfo[i].StartsWith(key + ">")) return rwrInfo[i].Replace(key + ">", string.Empty).Replace("\0", string.Empty);
            }
            return string.Empty;
        }
    }
}
