using System;
using System.ComponentModel;
using Common.Drawing;
using Common.Drawing.Imaging;
using System.Windows.Forms;
using Common.Imaging;
using F4Utils.Terrain;
using log4net;
using Common.Drawing.Drawing2D;
using F4Utils.Resources;
using System.IO;
using F16CPD.FlightInstruments.Pfd;
using F16CPD.Networking;
namespace F16CPD.SimSupport.Falcon4.MovingMap
{
    internal interface IMovingMap 
    {

        bool RenderMap(Graphics g, Rectangle renderRectangle, float mapZoom,
            float mapCoordinateFeetEast, float mapCoordinateFeetNorth, float magneticHeadingInDecimalDegrees,
            int outerMapRingRadiusInNauticalMiles, MapRotationMode rotationMode );
    }

    internal class MovingMap : IMovingMap
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MovingMap));
        private readonly IMapRingRenderer _mapRingRenderer;
        private readonly ICenterAirplaneRenderer _centerAirplaneRenderer;
        private readonly ITheaterMapRetriever _theaterMapRetriever;
        private Bitmap _theaterMap;
        private float _mapWidthInFeet;
        public MovingMap( 
            TerrainDB terrainDB, 
            IF16CPDClient client,
            ITheaterMapRetriever theaterMapRetriever=null,
            IMapRingRenderer mapRingRenderer=null,
            ICenterAirplaneRenderer centerAirplaneRenderer=null
            )
        {
            _mapRingRenderer = mapRingRenderer ?? new MapRingRenderer();
            _centerAirplaneRenderer = centerAirplaneRenderer ?? new CenterAirplaneRenderer();
            _theaterMapRetriever = theaterMapRetriever ?? new TheaterMapRetriever(terrainDB, client);
        }

        public bool RenderMap(Graphics g, Rectangle renderRectangle, float mapZoom, float mapCoordinateFeetEast, float mapCoordinateFeetNorth, float magneticHeadingInDecimalDegrees,
            int outerMapRingRadiusInNauticalMiles, MapRotationMode rotationMode)
        {
            _theaterMap = _theaterMap ?? _theaterMapRetriever.GetTheaterMapImage(ref _mapWidthInFeet);
            if (_theaterMap == null) return false;
            
            var mapImageFeetPerPixel = _mapWidthInFeet / (float)_theaterMap.Width;
            var outerMapRingRadiusPixels = (outerMapRingRadiusInNauticalMiles * Common.Math.Constants.FEET_PER_NM) / mapImageFeetPerPixel ;
            var zoom = 1/(mapZoom / 50000.0f) ;
            var xOffset = (-(mapCoordinateFeetEast / mapImageFeetPerPixel) + (((float)_theaterMap.Width / 2.0f)));
            var yOffset = (((mapCoordinateFeetNorth / mapImageFeetPerPixel) - (((float)_theaterMap.Height / 2.0f))));
            try
            {
                using (var renderTarget = new Bitmap(Math.Max(renderRectangle.Width, renderRectangle.Height), Math.Max(renderRectangle.Width, renderRectangle.Height), PixelFormat.Format16bppRgb565))
                {
                    var scaleX = (float)renderTarget.Width / ((float)_theaterMap.Width);
                    var scaleY = (float)renderTarget.Height / ((float)_theaterMap.Height);
                    using (var h = Graphics.FromImage(renderTarget))
                    {
                        var backgroundColor = Color.FromArgb(181,186,222);
                        h.PixelOffsetMode = PixelOffsetMode.Half;
                        h.Clear(backgroundColor);
                        var origTransform = h.Transform;

                        h.TranslateTransform(renderTarget.Width / 2.0f, renderTarget.Height / 2.0f);
                        h.ScaleTransform(zoom, zoom);
                        h.TranslateTransform(-renderTarget.Width / 2.0f, -renderTarget.Height / 2.0f);
                        var zoomedTransform = h.Transform;

                        if (rotationMode == MapRotationMode.HeadingUp)
                        {
                            h.TranslateTransform(renderTarget.Width / 2.0f, renderTarget.Height / 2.0f);
                            h.RotateTransform(-(magneticHeadingInDecimalDegrees));
                            h.TranslateTransform(-renderTarget.Width / 2.0f, -renderTarget.Height / 2.0f);
                        }
                        h.ScaleTransform(scaleX, scaleY);
                        h.TranslateTransform(xOffset, yOffset);
                        h.DrawImage(_theaterMap,
                            0,
                            0,
                            new Rectangle(0, 0, _theaterMap.Width, _theaterMap.Height), 
                            GraphicsUnit.Pixel);

                        h.Transform = zoomedTransform;
                        if (rotationMode == MapRotationMode.NorthUp)
                        {
                            h.TranslateTransform(renderTarget.Width / 2.0f, renderTarget.Height / 2.0f);
                            h.RotateTransform(magneticHeadingInDecimalDegrees);
                            h.TranslateTransform(-renderTarget.Width / 2.0f, -renderTarget.Height / 2.0f);
                        }
                        h.ScaleTransform(scaleX, scaleY);
                        _mapRingRenderer.DrawMapRing(h,_theaterMap.Size, (int)outerMapRingRadiusPixels, magneticHeadingInDecimalDegrees);


                    }
                    
                    g.DrawImageFast(
                        renderTarget,
                        renderRectangle,
                        new Rectangle(
                            (renderTarget.Width - renderRectangle.Width) / 2, 
                            (renderTarget.Height - renderRectangle.Height) / 2,
                            renderRectangle.Width, renderRectangle.Height),
                            GraphicsUnit.Pixel
                        );


                    var originalTransform = g.Transform;
                    if (rotationMode == MapRotationMode.NorthUp)
                    {
                        g.TranslateTransform(renderRectangle.Width / 2.0f, renderRectangle.Height / 2.0f);
                        g.RotateTransform(magneticHeadingInDecimalDegrees);
                        g.TranslateTransform(-renderRectangle.Width / 2.0f, -renderRectangle.Height / 2.0f);
                    }
                    _centerAirplaneRenderer.DrawCenterAirplaneSymbol(g, new Rectangle(0, 0, renderRectangle.Width, renderRectangle.Height));
                    g.Transform = originalTransform;

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return false;
            }
            return true;
        }

    }
}