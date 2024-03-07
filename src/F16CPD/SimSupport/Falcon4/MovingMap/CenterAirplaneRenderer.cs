using System;
using Common.Drawing;
using Common.Imaging;
using F16CPD.Properties;

namespace F16CPD.SimSupport.Falcon4.MovingMap
{
    internal interface ICenterAirplaneRenderer
    {
        void DrawCenterAirplaneSymbol(Graphics g, Rectangle renderRectangle);
    }

    internal class CenterAirplaneRenderer : ICenterAirplaneRenderer
    {
        private Bitmap _mapAirplaneBitmap;

        public void DrawCenterAirplaneSymbol(Graphics g, Rectangle renderRectangle)
        {
            if (_mapAirplaneBitmap == null)
            {
                _mapAirplaneBitmap = new Bitmap((System.Drawing.Image) Resources.F16Symbol.Clone());
                _mapAirplaneBitmap.MakeTransparent(Color.FromArgb(255, 0, 255));
                _mapAirplaneBitmap =
                    (Bitmap)
                        Common.Imaging.Util.ResizeBitmap(_mapAirplaneBitmap,
                            new Size(
                                (int) Math.Floor(((float) _mapAirplaneBitmap.Width)),
                                (int) Math.Floor(((float) _mapAirplaneBitmap.Height))));
            }
            g.DrawImageFast(_mapAirplaneBitmap, (((renderRectangle.Width - _mapAirplaneBitmap.Width) / 2)),
                (((renderRectangle.Height - _mapAirplaneBitmap.Height)/2)));
        }
    }
}