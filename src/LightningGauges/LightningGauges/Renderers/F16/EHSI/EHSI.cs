using System;
using System.IO;
using Common.Drawing;
using Common.Drawing.Imaging;
using Common.Drawing.Text;
using Common.Imaging;
using Common.SimSupport;
using log4net;

namespace LightningGauges.Renderers.F16.EHSI
{
    public interface IEHSI : IInstrumentRenderer
    {
        InstrumentState InstrumentState { get; set; }
        Options Options { get; set; }
    }

    public class EHSI : InstrumentRendererBase, IEHSI
    {
        private static readonly string IMAGES_FOLDER_NAME = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName + Path.DirectorySeparatorChar + "images";

        private const string EHSI_NO_DATA_IMAGE_FILENAME = "ehsioff.bmp";
        private const string EHSI_NO_DATA_MASK_FILENAME = "ehsioff_mask.bmp";
        private static readonly ILog _log = LogManager.GetLogger(typeof(EHSI));
        private static readonly object _imagesLock = new object();
        private static bool _imagesLoaded;
        private static ImageMaskPair _noData;
        private readonly PrivateFontCollection _fonts = new PrivateFontCollection();

        public EHSI()
        {
            _fonts.AddFontFile("isisdigits.ttf");
            _fonts.AddFontFile("ehsidigits.ttf");
            InstrumentState = new InstrumentState();
            Options = new Options();
        }

        private static void LoadImageResources()
        {
            if (_noData == null)
            {
                _noData = ImageMaskPair.CreateFromFiles(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + EHSI_NO_DATA_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + EHSI_NO_DATA_MASK_FILENAME);
            }
            _imagesLoaded = true;
        }

        public InstrumentState InstrumentState { get; set; }
        public Options Options { get; set; }

        public override void Render(Graphics destinationGraphics, Rectangle destinationRectangle)
        {
            if (!_imagesLoaded) LoadImageResources();
            var gfx = destinationGraphics;
            Bitmap fullBright = null;
            if (InstrumentState.Brightness != InstrumentState.MaxBrightness)
            {
                fullBright = new Bitmap(destinationRectangle.Size.Width, destinationRectangle.Size.Height, PixelFormat.Format32bppPArgb);
                gfx = Graphics.FromImage(fullBright);
            }
            lock (_imagesLock)
            {
                //store the canvas's transform and clip settings so we can restore them later
                var initialState = gfx.Save();

                //set up the canvas scale and clipping region
                const int width = 512;
                const int height = 512;

                var scaleWidth = destinationRectangle.Width;
                var scaleHeight = destinationRectangle.Height;

                if (scaleHeight > scaleWidth) scaleHeight = scaleWidth;
                if (scaleWidth > scaleHeight) scaleWidth = scaleHeight;

                gfx.ResetTransform(); //clear any existing transforms
                gfx.SetClip(destinationRectangle);
                //set the clipping region on the graphics object to our render rectangle's boundaries
                gfx.FillRectangle(Brushes.Black, destinationRectangle);

                float translateX = 0;
                float translateY = 0;
                if (scaleWidth < destinationRectangle.Width) translateX = (destinationRectangle.Width - scaleWidth) / 2.0f;
                if (scaleHeight < destinationRectangle.Height) translateY = (destinationRectangle.Height - scaleHeight) / 2.0f;
                gfx.TranslateTransform(translateX, translateY);
                gfx.ScaleTransform(scaleWidth / (float) width, scaleHeight / (float) height);
                //set the initial scale transformation 

                //save the basic canvas transform and clip settings so we can revert to them later, as needed
                gfx.Save();

                var outerRect = new RectangleF(0, 0, width, height);

                if (InstrumentState.NoPowerFlag) { }
                else if (InstrumentState.NoDataFlag) { DrawNoDataFlag(gfx, outerRect); }
                else
                {
                    //draw the compass
                    CompassRoseRenderer.DrawCompassRose(gfx, outerRect, _fonts, InstrumentState, Options);

                    //draw the desired heading bug
                    DesiredHeadingBugRenderer.DrawDesiredHeadingBug(gfx, outerRect, InstrumentState);

                    //draw course deviation needles
                    CourseDeviationNeedlesRenderer.DrawCourseDeviationNeedles(gfx, outerRect, InstrumentState);

                    //draw distance to beacon digits
                    DistanceToBeaconRenderer.DrawDistanceToBeacon(gfx, _fonts, InstrumentState, Options);

                    //draw desired course digits
                    DesiredCourseRenderer.DrawDesiredCourse(gfx, outerRect, _fonts, InstrumentState, Options);

                    //draw heading and course select knob labels
                    HeadingAndCourseAdjustLabelsRenderer.DrawHeadingAndCourseAdjustLabels(gfx, outerRect, _fonts);

                    //draw instrument mode label
                    InstrumentModeRenderer.DrawInstrumentMode(gfx, outerRect, _fonts, InstrumentState);

                    //draw bearing to beacon indicator
                    BearingToBeaconIndicatorRenderer.DrawBearingToBeaconIndicator(gfx, outerRect, InstrumentState);

                    //draw INU invalid flag if needed
                    if (InstrumentState.INUInvalidFlag)
                    {
                        var redFlagColor = Color.FromArgb(224, 43, 48);
                        TextWarningFlagRenderer.DrawTextWarningFlag(gfx, "I\nN\nU", redFlagColor, Color.White, _fonts);
                    }

                    //draw ATT invalid flag
                    if (InstrumentState.AttitudeFailureFlag)
                    {
                        var yellowFlagColor = Color.FromArgb(244, 240, 55);
                        TextWarningFlagRenderer.DrawTextWarningFlag(gfx, "A\nT\nT", yellowFlagColor, Color.Black, _fonts);
                    }

                    //draw airplane symbol
                    AirplaneSymbolRenderer.DrawAirplaneSymbol(gfx, outerRect);
                }
                if (InstrumentState.ShowBrightnessLabel) DrawBrightnessLabel(gfx, outerRect);

                //restore the canvas's transform and clip settings to what they were when we entered this method
                gfx.Restore(initialState);
            }
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

        private void DrawBrightnessLabel(Graphics g, RectangleF outerBounds) { CenterLabelRenderer.DrawCenterLabel(g, outerBounds, "BRT", _fonts); }

        private void DrawNoDataFlag(Graphics g, RectangleF outerBounds)
        {
            if (InstrumentState.NoDataFlag)
            {
                g.FillRectangle(Brushes.Black, outerBounds);
                if (_noData.MaskedImage != null) { g.DrawImageFast(_noData.MaskedImage, outerBounds, new RectangleF(new Point(0, 0), _noData.MaskedImage.Size), GraphicsUnit.Pixel); }
                else { _log.Debug("_noData.MaskedImage was null in DrawNoDataFlag"); }
            }
        }
    }
}