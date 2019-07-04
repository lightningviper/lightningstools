using System;
using System.IO;
using Common.Drawing;
using Common.Imaging;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface INosewheelSteeringIndexer : IInstrumentRenderer
    {
        NosewheelSteeringIndexer.NosewheelSteeringIndexerInstrumentState InstrumentState { get; set; }
    }

    public class NosewheelSteeringIndexer : InstrumentRendererBase, INosewheelSteeringIndexer
    {
        private const string NWSI_BACKGROUND_IMAGE_FILENAME = "index2.bmp";
        private const string NWSI_BACKGROUND_MASK_FILENAME = "index2_mask.bmp";
        private const string NWSI_DISC_IMAGE_FILENAME = "ind2disc.bmp";
        private const string NWSI_DISC_MASK_FILENAME = "ind2disc_mask.bmp";
        private const string NWSI_NWS_IMAGE_FILENAME = "ind2nws.bmp";
        private const string NWSI_NWS_MASK_FILENAME = "ind2nws_mask.bmp";
        private const string NWSI_RDY_IMAGE_FILENAME = "ind2ready.bmp";
        private const string NWSI_RDY_MASK_FILENAME = "ind2ready_mask.bmp";

        private static readonly string IMAGES_FOLDER_NAME =  "images";

        private static readonly object _imagesLock = new object();
        private static ImageMaskPair _background;
        private static ImageMaskPair _disc;
        private static ImageMaskPair _nws;
        private static ImageMaskPair _rdy;
        private static bool _imagesLoaded;

        public NosewheelSteeringIndexer() { InstrumentState = new NosewheelSteeringIndexerInstrumentState(); }

        private static void LoadImageResources()
        {
            if (_background == null)
            {
                _background = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + NWSI_BACKGROUND_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + NWSI_BACKGROUND_MASK_FILENAME);
                _background.Use1BitAlpha = true;
            }
            if (_disc == null)
            {
                _disc = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + NWSI_DISC_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + NWSI_DISC_MASK_FILENAME);
            }
            if (_nws == null)
            {
                _nws = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + NWSI_NWS_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + NWSI_NWS_MASK_FILENAME);
            }
            if (_rdy == null)
            {
                _rdy = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + NWSI_RDY_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + NWSI_RDY_MASK_FILENAME);
            }
            _imagesLoaded = true;
        }

        public NosewheelSteeringIndexerInstrumentState InstrumentState { get; set; }

        [Serializable]
        public class NosewheelSteeringIndexerInstrumentState : InstrumentStateBase
        {
            public bool AR_NWS { get; set; }
            public bool RDY { get; set; }
            public bool DISC { get; set; }
        }

        public override void Render(Graphics destinationGraphics, Rectangle destinationRectangle)
        {
            if (!_imagesLoaded) LoadImageResources();
            lock (_imagesLock)
            {
                //store the canvas's transform and clip settings so we can restore them later
                var initialState = destinationGraphics.Save();

                //set up the canvas scale and clipping region
                var width = _background.Image.Width - 92;
                var height = _background.Image.Height - 99;
                width -= 117;
                height -= 2;
                destinationGraphics.ResetTransform(); //clear any existing transforms
                destinationGraphics.SetClip(destinationRectangle); //set the clipping region on the graphics object to our render rectangle's boundaries
                destinationGraphics.FillRectangle(Brushes.Black, destinationRectangle);
                destinationGraphics.ScaleTransform(destinationRectangle.Width / (float) width, destinationRectangle.Height / (float) height);
                //set the initial scale transformation 

                destinationGraphics.TranslateTransform(-46, -46);
                destinationGraphics.TranslateTransform(-50, -2);

                //save the basic canvas transform and clip settings so we can revert to them later, as needed
                var basicState = destinationGraphics.Save();

                //draw the background image
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.DrawImageFast(_background.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                if (InstrumentState.DISC)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_disc.MaskedImage, new Point(0, 0));
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }
                if (InstrumentState.RDY)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_rdy.MaskedImage, new Point(0, 0));
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }
                if (InstrumentState.AR_NWS)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_nws.MaskedImage, new Point(0, 0));
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //restore the canvas's transform and clip settings to what they were when we entered this method
                destinationGraphics.Restore(initialState);
            }
        }
    }
}