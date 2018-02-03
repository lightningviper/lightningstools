using System;
using System.Threading;
using Common.Drawing;
using Common.Drawing.Imaging;
using Common.Drawing.Text;
using Common.Imaging;
using Common.SimSupport;

//TODO: add options to the options screen for setting pressure units, metric/standard velocity units, etc.
//TODO: test this instro with Falcon
//TODO: baro adjust?

namespace LightningGauges.Renderers.F16.ISIS
{
    public interface IISIS : IInstrumentRenderer
    {
        Options Options { get; set; }
        InstrumentState InstrumentState { get; set; }
    }

    public class ISIS : InstrumentRendererBase, IISIS
    {
        private static readonly ThreadLocal<PrivateFontCollection> Fonts = new ThreadLocal<PrivateFontCollection>(
            () =>
            {
                var pfc = new PrivateFontCollection();
                pfc.AddFontFile("ISISDigits.ttf");
                return pfc;
            });

        public ISIS()
        {
            InstrumentState = new InstrumentState();
            Options = new Options();
        }

        public Options Options { get; set; }
        public InstrumentState InstrumentState { get; set; }

        public override void Render(Graphics destinationGraphics, Rectangle destinationRectangle)
        {
            var gfx = destinationGraphics;
            Bitmap fullBright = null;
            if (InstrumentState.Brightness != InstrumentState.MaxBrightness)
            {
                fullBright = new Bitmap(destinationRectangle.Size.Width, destinationRectangle.Size.Height, PixelFormat.Format16bppRgb565);
                gfx = Graphics.FromImage(fullBright);
            }
            //store the canvas's transform and clip settings so we can restore them later
            var initialState = gfx.Save();
            //set up the canvas scale and clipping region
            const int width = 256;
            const int height = 256;
            gfx.ResetTransform(); //clear any existing transforms
            gfx.SetClip(destinationRectangle);
            //set the clipping region on the graphics object to our render rectangle's boundaries
            gfx.FillRectangle(Brushes.Black, destinationRectangle);
            gfx.ScaleTransform(destinationRectangle.Width / (float) width, destinationRectangle.Height / (float) height);
            //set the initial scale transformation 

            //save the basic canvas transform and clip settings so we can revert to them later, as needed
            var basicState = gfx.Save();

            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);
            gfx.TranslateTransform(-7, 0);
            AttitudeRenderer.DrawAttitude(gfx, width, height, InstrumentState, Fonts.Value);
            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);

            //draw heading tape
            var headingTapeSize = HeadingTapeRenderer.DrawHeadingTape(gfx, width, height, ref basicState, InstrumentState, Fonts.Value);

            //draw heading triangle
            HeadingTriangleRenderer.DrawHeadingTriangle(gfx, ref basicState, width, height, headingTapeSize);

            //draw airspeed tape
            AirspeedTapeRenderer.DrawAirspeedTape(gfx, ref basicState, height, InstrumentState, Fonts.Value);

            //draw altitude tape
            AltitudeTapeRenderer.DrawAltitudeTape(gfx, ref basicState, width, height, InstrumentState, Fonts.Value);

            //draw top rectangle
            var topRectangle = TopRectangleRenderer.DrawTopRectangle(gfx, width, ref basicState);

            //draw mach rectangle
            MachRectangleRenderer.DrawMachRectangle(gfx, ref basicState, topRectangle, InstrumentState, Fonts.Value);

            //draw the radar altimeter area
            RadarAltimeterAreaRenderer.DrawRadarAltimeterArea(gfx, ref basicState, topRectangle, InstrumentState, Options, Fonts.Value);

            //draw the barometric pressure area
            BarometricPressureAreaRenderer.DrawBarometricPressureArea(gfx, ref basicState, topRectangle, InstrumentState, Options, Fonts.Value);

            //draw the ILS bars
            ILSBarsRenderer.DrawIlsBars(gfx, width, height, ref basicState, InstrumentState);

            //restore the canvas's transform and clip settings to what they were when we entered this method
            gfx.Restore(initialState);

            if (fullBright != null)
            {
                var ia = new ImageAttributes();
                var dimmingMatrix = Util.GetDimmingColorMatrix(InstrumentState.Brightness / (float) InstrumentState.MaxBrightness);
                ia.SetColorMatrix(dimmingMatrix);
                destinationGraphics.DrawImageFast(fullBright, destinationRectangle, 0, 0, fullBright.Width, fullBright.Height, GraphicsUnit.Pixel, ia);
                Common.Util.DisposeObject(gfx);
                Common.Util.DisposeObject(fullBright);
            }
        }
    }
}