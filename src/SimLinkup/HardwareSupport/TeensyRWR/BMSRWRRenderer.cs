using F4SharedMem;
using LightningGauges.Renderers.F16.RWR;
using System.Drawing;
using System.Windows.Media;


namespace SimLinkup.HardwareSupport.TeensyRWR
{
    internal class BMSRWRRenderer
    {
        private readonly IRWRRendererFactory _rwrRendererFactory;
        private IRWRRenderer _rwrRenderer;
        private string _lastRwrType = string.Empty;
        public BMSRWRRenderer(IRWRRendererFactory rwrRendererFactory=null)
        {
            _rwrRendererFactory = rwrRendererFactory ?? new RWRRendererFactory();
        }
        public void Render(DrawingContext drawingContext, InstrumentState instrumentState)
        {
            CreateRWRRenderer(instrumentState.RwrInfo);
            if (_rwrRenderer != null)
            {
                _rwrRenderer.InstrumentState = instrumentState;
                _rwrRenderer.Render(drawingContext);
            }
        }
        public void Render(Graphics destinationGraphics, Rectangle destinationRectangle, InstrumentState instrumentState)
        {
            CreateRWRRenderer(instrumentState.RwrInfo);
            if (_rwrRenderer != null)
            {
                _rwrRenderer.InstrumentState = instrumentState;
                _rwrRenderer.Render(destinationGraphics, destinationRectangle);
            }
        }
        private void CreateRWRRenderer(byte[] rwrInfoArray)
        {
            var rwrType = GetRWRType(rwrInfoArray);
            if (_rwrRenderer == null || rwrType != _lastRwrType)
            {

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
                _lastRwrType = rwrType;
            }
        }
        private string GetRWRType(byte[] rwrInfoArray)
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
