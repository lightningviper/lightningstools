using System;
using System.IO;
using Common.Drawing;
using Common.Imaging;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface ISpeedbrakeIndicator : IInstrumentRenderer
    {
        SpeedbrakeIndicator.SpeedbrakeIndicatorInstrumentState InstrumentState { get; set; }
    }

    public class SpeedbrakeIndicator : InstrumentRendererBase, ISpeedbrakeIndicator
    {
        private const string SB_BACKGROUND_IMAGE_FILENAME = "speedbrake.bmp";
        private const string SB_BACKGROUND_MASK_FILENAME = "speedbrake_mask.bmp";
        private const string SB_CLOSED_IMAGE_FILENAME = "sbclosed2.bmp";
        private const string SB_POWER_OFF_IMAGE_FILENAME = "sbclosed.bmp";
        private const string SB_OPEN_IMAGE_FILENAME = "sbopen.bmp";

        private static readonly string IMAGES_FOLDER_NAME = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName + Path.DirectorySeparatorChar + "images";

        private static readonly object _imagesLock = new object();
        private static ImageMaskPair _background;
        private static Bitmap _closed;
        private static Bitmap _powerLoss;
        private static Bitmap _open;
        private static bool _imagesLoaded;

        public SpeedbrakeIndicator() { InstrumentState = new SpeedbrakeIndicatorInstrumentState(); }

        private static void LoadImageResources()
        {
            if (_background == null)
            {
                _background = ImageMaskPair.CreateFromFiles(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + SB_BACKGROUND_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + SB_BACKGROUND_MASK_FILENAME);
            }
            if (_closed == null) _closed = (Bitmap) Util.LoadBitmapFromFile(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + SB_CLOSED_IMAGE_FILENAME);
            if (_powerLoss == null) _powerLoss = (Bitmap) Util.LoadBitmapFromFile(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + SB_POWER_OFF_IMAGE_FILENAME);
            if (_open == null) _open = (Bitmap) Util.LoadBitmapFromFile(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + SB_OPEN_IMAGE_FILENAME);
            _imagesLoaded = true;
        }

        public SpeedbrakeIndicatorInstrumentState InstrumentState { get; set; }

        [Serializable]
        public class SpeedbrakeIndicatorInstrumentState : InstrumentStateBase
        {
            private float _percentOpen;

            public SpeedbrakeIndicatorInstrumentState()
            {
                PercentOpen = 0;
                PowerLoss = false;
            }


            public float PercentOpen
            {
                get => _percentOpen;
                set
                {
                    var pct = value;
                    if (float.IsInfinity(pct) || float.IsNaN(pct)) pct = 0;
                    if (pct < 0) pct = 0;
                    if (pct > 100) pct = 100;
                    _percentOpen = pct;
                }
            }


            public bool PowerLoss { get; set; }
        }

        public override void Render(Graphics destinationGraphics, Rectangle destinationRectangle)
        {
            if (!_imagesLoaded) LoadImageResources();
            lock (_imagesLock)
            {
                //store the canvas's transform and clip settings so we can restore them later
                var initialState = destinationGraphics.Save();

                //set up the canvas scale and clipping region
                var width = _background.MaskedImage.Width - 110;
                var height = _background.MaskedImage.Height - 110 - 4;
                width -= 59;
                destinationGraphics.ResetTransform(); //clear any existing transforms
                destinationGraphics.SetClip(destinationRectangle); //set the clipping region on the graphics object to our render rectangle's boundaries
                destinationGraphics.FillRectangle(Brushes.Black, destinationRectangle);
                destinationGraphics.ScaleTransform(destinationRectangle.Width / (float) width, destinationRectangle.Height / (float) height);
                //set the initial scale transformation 
                destinationGraphics.TranslateTransform(-55, -55);
                destinationGraphics.TranslateTransform(-29, 0);
                //save the basic canvas transform and clip settings so we can revert to them later, as needed
                var basicState = destinationGraphics.Save();

                var percentOpen = InstrumentState.PercentOpen;
                if (!InstrumentState.PowerLoss)
                {
                    if (percentOpen < 2.0f)
                    {
                        GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                        destinationGraphics.DrawImageFast(
                            _closed, new Rectangle(0, 0, _background.MaskedImage.Width, _background.MaskedImage.Height), new Rectangle(0, 0, _closed.Width, _closed.Height), GraphicsUnit.Pixel);
                        GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    }
                    else
                    {
                        GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                        destinationGraphics.DrawImageFast(
                            _open, new Rectangle(0, 0, _background.MaskedImage.Width, _background.MaskedImage.Height), new Rectangle(0, 0, _open.Width, _open.Height), GraphicsUnit.Pixel);
                        GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    }
                }
                else
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(
                        _powerLoss, new Rectangle(0, 0, _background.MaskedImage.Width, _background.MaskedImage.Height), new Rectangle(0, 0, _powerLoss.Width, _powerLoss.Height), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw the background image
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.DrawImageFast(
                    _background.MaskedImage, new Rectangle(0, 0, _background.MaskedImage.Width, _background.MaskedImage.Height),
                    new Rectangle(0, 0, _background.MaskedImage.Width, _background.MaskedImage.Height), GraphicsUnit.Pixel);
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //restore the canvas's transform and clip settings to what they were when we entered this method
                destinationGraphics.Restore(initialState);
            }
        }
    }
}