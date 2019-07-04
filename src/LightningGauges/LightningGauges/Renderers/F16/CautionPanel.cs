using System;
using System.IO;
using Common.Drawing;
using Common.Imaging;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface ICautionPanel : IInstrumentRenderer
    {
        CautionPanel.CautionPanelInstrumentState InstrumentState { get; set; }
    }

    public class CautionPanel : InstrumentRendererBase, ICautionPanel
    {
        private const string CAUTION_PANEL_BACKGROUND_IMAGE_FILENAME = "caution.bmp";
        private const string CAUTION_PANEL_BACKGROUND_MASK_FILENAME = "caution_mask.bmp";
        private const string CAUTION_PANEL_FLCS_FAULT_IMAGE_FILENAME = "cau1.bmp";
        private const string CAUTION_PANEL_ENGINE_FAULT_IMAGE_FILENAME = "cau2.bmp";
        private const string CAUTION_PANEL_AVIONICS_FAULT_IMAGE_FILENAME = "cau3.bmp";
        private const string CAUTION_PANEL_SEAT_NOT_ARMED_IMAGE_FILENAME = "cau4.bmp";
        private const string CAUTION_PANEL_ELEC_SYS_IMAGE_FILENAME = "cau5.bmp";
        private const string CAUTION_PANEL_SEC_IMAGE_FILENAME = "cau6.bmp";
        private const string CAUTION_PANEL_EQUIP_HOT_IMAGE_FILENAME = "cau7.bmp";
        private const string CAUTION_PANEL_NWS_FAIL_IMAGE_FILENAME = "cau8.bmp";
        private const string CAUTION_PANEL_PROBE_HEAT_IMAGE_FILENAME = "cau9.bmp";
        private const string CAUTION_PANEL_FUEL_OIL_HOT_IMAGE_FILENAME = "cau10.bmp";
        private const string CAUTION_PANEL_RADAR_ALT_IMAGE_FILENAME = "cau11.bmp";
        private const string CAUTION_PANEL_ANTI_SKID_IMAGE_FILENAME = "cau12.bmp";
        private const string CAUTION_PANEL_C_ADC_IMAGE_FILENAME = "cau13.bmp";
        private const string CAUTION_PANEL_INLET_ICING_IMAGE_FILENAME = "cau14.bmp";
        private const string CAUTION_PANEL_IFF_IMAGE_FILENAME = "cau15.bmp";
        private const string CAUTION_PANEL_HOOK_IMAGE_FILENAME = "cau16.bmp";
        private const string CAUTION_PANEL_STORES_CONFIG_IMAGE_FILENAME = "cau17.bmp";
        private const string CAUTION_PANEL_OVERHEAT_IMAGE_FILENAME = "cau18.bmp";
        private const string CAUTION_PANEL_NUCLEAR_IMAGE_FILENAME = "cau19.bmp";
        private const string CAUTION_PANEL_OXY_LOW_IMAGE_FILENAME = "cau20.bmp";
        private const string CAUTION_PANEL_ATF_NOT_ENGAGED_IMAGE_FILENAME = "cau21.bmp";
        private const string CAUTION_PANEL_EEC_IMAGE_FILENAME = "cau22.bmp";
        private const string CAUTION_PANEL_ECM_IMAGE_FILENAME = "cau23.bmp";
        private const string CAUTION_PANEL_CABIN_PRESS_IMAGE_FILENAME = "cau24.bmp";
        private const string CAUTION_PANEL_FWD_FUEL_LOW_IMAGE_FILENAME = "cau25.bmp";
        private const string CAUTION_PANEL_BUC_IMAGE_FILENAME = "cau26.bmp";
        private const string CAUTION_PANEL_SLOT27_IMAGE_FILENAME = "cau27.bmp";
        private const string CAUTION_PANEL_SLOT28_IMAGE_FILENAME = "cau27.bmp";
        private const string CAUTION_PANEL_AFT_FUEL_LOW_IMAGE_FILENAME = "cau29.bmp";
        private const string CAUTION_PANEL_SLOT30_IMAGE_FILENAME = "cau27.bmp";
        private const string CAUTION_PANEL_SLOT31_IMAGE_FILENAME = "cau27.bmp";
        private const string CAUTION_PANEL_SLOT32_IMAGE_FILENAME = "cau27.bmp";

        private static readonly string IMAGES_FOLDER_NAME =  "images";

        private static readonly object _imagesLock = new object();
        private static ImageMaskPair _background;
        private static Bitmap _flcsFault;
        private static Bitmap _engineFault;
        private static Bitmap _avionicsFault;
        private static Bitmap _seatNotArmed;
        private static Bitmap _elecSys;
        private static Bitmap _sec;
        private static Bitmap _equipHot;
        private static Bitmap _nwsFail;
        private static Bitmap _probeHeat;
        private static Bitmap _fuelOilHot;
        private static Bitmap _radarAlt;
        private static Bitmap _antiSkid;
        private static Bitmap _CADC;
        private static Bitmap _inletIcing;
        private static Bitmap _iff;
        private static Bitmap _hook;
        private static Bitmap _storesConfig;
        private static Bitmap _overheat;
        private static Bitmap _nuclear;
        private static Bitmap _oxyLow;
        private static Bitmap _atfNotEngaged;
        private static Bitmap _EEC;
        private static Bitmap _ECM;
        private static Bitmap _cabinPress;
        private static Bitmap _fwdFuelLow;
        private static Bitmap _BUC;
        private static Bitmap _slot27;
        private static Bitmap _slot28;
        private static Bitmap _aftFuelLow;
        private static Bitmap _slot30;
        private static Bitmap _slot31;
        private static Bitmap _slot32;
        private static bool _imagesLoaded;

        public CautionPanel() { InstrumentState = new CautionPanelInstrumentState(); }

        private static void LoadImageResources()
        {
            if (_background == null)
            {
                _background = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_BACKGROUND_IMAGE_FILENAME,
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_BACKGROUND_MASK_FILENAME);
            }

            if (_flcsFault == null) _flcsFault = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_FLCS_FAULT_IMAGE_FILENAME);
            if (_engineFault == null) _engineFault = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_ENGINE_FAULT_IMAGE_FILENAME);
            if (_avionicsFault == null) _avionicsFault = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_AVIONICS_FAULT_IMAGE_FILENAME);
            if (_seatNotArmed == null) _seatNotArmed = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_SEAT_NOT_ARMED_IMAGE_FILENAME);
            if (_elecSys == null) _elecSys = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_ELEC_SYS_IMAGE_FILENAME);
            if (_sec == null) _sec = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_SEC_IMAGE_FILENAME);
            if (_equipHot == null) _equipHot = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_EQUIP_HOT_IMAGE_FILENAME);
            if (_nwsFail == null) _nwsFail = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_NWS_FAIL_IMAGE_FILENAME);
            if (_probeHeat == null) _probeHeat = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_PROBE_HEAT_IMAGE_FILENAME);
            if (_fuelOilHot == null) _fuelOilHot = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_FUEL_OIL_HOT_IMAGE_FILENAME);
            if (_radarAlt == null) _radarAlt = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_RADAR_ALT_IMAGE_FILENAME);
            if (_antiSkid == null) _antiSkid = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_ANTI_SKID_IMAGE_FILENAME);
            if (_CADC == null) _CADC = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_C_ADC_IMAGE_FILENAME);
            if (_inletIcing == null) _inletIcing = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_INLET_ICING_IMAGE_FILENAME);
            if (_iff == null) _iff = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_IFF_IMAGE_FILENAME);
            if (_hook == null) _hook = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_HOOK_IMAGE_FILENAME);
            if (_storesConfig == null) _storesConfig = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_STORES_CONFIG_IMAGE_FILENAME);
            if (_overheat == null) _overheat = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_OVERHEAT_IMAGE_FILENAME);
            if (_nuclear == null) _nuclear = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_NUCLEAR_IMAGE_FILENAME);
            if (_oxyLow == null) _oxyLow = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_OXY_LOW_IMAGE_FILENAME);
            if (_atfNotEngaged == null) _atfNotEngaged = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_ATF_NOT_ENGAGED_IMAGE_FILENAME);
            if (_EEC == null) _EEC = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_EEC_IMAGE_FILENAME);
            if (_ECM == null) _ECM = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_ECM_IMAGE_FILENAME);
            if (_cabinPress == null) _cabinPress = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_CABIN_PRESS_IMAGE_FILENAME);
            if (_fwdFuelLow == null) _fwdFuelLow = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_FWD_FUEL_LOW_IMAGE_FILENAME);
            if (_BUC == null) _BUC = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_BUC_IMAGE_FILENAME);
            if (_slot27 == null) _slot27 = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_SLOT27_IMAGE_FILENAME);
            if (_slot28 == null) _slot28 = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_SLOT28_IMAGE_FILENAME);
            if (_aftFuelLow == null)
            {
                _aftFuelLow = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_AFT_FUEL_LOW_IMAGE_FILENAME);
            }
            if (_slot30 == null) _slot30 = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_SLOT30_IMAGE_FILENAME);
            if (_slot31 == null) _slot31 = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_SLOT31_IMAGE_FILENAME);
            if (_slot32 == null) _slot32 = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CAUTION_PANEL_SLOT32_IMAGE_FILENAME);
            _imagesLoaded = true;
        }

        public CautionPanelInstrumentState InstrumentState { get; set; }

        [Serializable]
        public class CautionPanelInstrumentState : InstrumentStateBase
        {
            public CautionPanelInstrumentState()
            {
                FLCSFault = false;
                EngineFault = false;
                AvionicsFault = false;
                SeatNotArmed = false;
                ElecSys = false;
                SEC = false;
                EquipHot = false;
                NWSFail = false;
                ProbeHeat = false;
                FuelOilHot = false;
                RadarAlt = false;
                AntiSkid = false;
                CADC = false;
                InletIcing = false;
                IFF = false;
                Hook = false;
                StoresConfig = false;
                Overheat = false;
                Nuclear = false;
                OxyLow = false;
                ATFNotEngaged = false;
                EEC = false;
                ECM = false;
                CabinPress = false;
                FwdFuelLow = false;
                Slot27 = false;
                Slot28 = false;
                AftFuelLow = false;
                Slot30 = false;
                Slot31 = false;
                Slot32 = false;
            }

            public bool FLCSFault { get; set; }
            public bool EngineFault { get; set; }
            public bool AvionicsFault { get; set; }
            public bool SeatNotArmed { get; set; }
            public bool ElecSys { get; set; }
            public bool SEC { get; set; }
            public bool EquipHot { get; set; }
            public bool NWSFail { get; set; }
            public bool ProbeHeat { get; set; }
            public bool FuelOilHot { get; set; }
            public bool RadarAlt { get; set; }
            public bool AntiSkid { get; set; }
            public bool CADC { get; set; }
            public bool InletIcing { get; set; }
            public bool IFF { get; set; }
            public bool Hook { get; set; }
            public bool StoresConfig { get; set; }
            public bool Overheat { get; set; }
            public bool Nuclear { get; set; }
            public bool OxyLow { get; set; }
            public bool ATFNotEngaged { get; set; }
            public bool EEC { get; set; }
            public bool ECM { get; set; }
            public bool CabinPress { get; set; }
            public bool FwdFuelLow { get; set; }
            public bool BUC { get; set; }
            public bool Slot27 { get; set; }
            public bool Slot28 { get; set; }
            public bool AftFuelLow { get; set; }
            public bool Slot30 { get; set; }
            public bool Slot31 { get; set; }
            public bool Slot32 { get; set; }
        }

        public override void Render(Graphics destinationGraphics, Rectangle destinationRectangle)
        {
            if (!_imagesLoaded) LoadImageResources();
            lock (_imagesLock)
            {
                //store the canvas's transform and clip settings so we can restore them later
                var initialState = destinationGraphics.Save();

                //set up the canvas scale and clipping region
                const int width = 283;
                const int height = 217;
                destinationGraphics.ResetTransform(); //clear any existing transforms
                destinationGraphics.SetClip(destinationRectangle); //set the clipping region on the graphics object to our render rectangle's boundaries
                destinationGraphics.FillRectangle(Brushes.Black, destinationRectangle);
                destinationGraphics.ScaleTransform(destinationRectangle.Width / (float) width, destinationRectangle.Height / (float) height);
                //set the initial scale transformation 
                destinationGraphics.TranslateTransform(-115, -147);
                //save the basic canvas transform and clip settings so we can revert to them later, as needed
                var basicState = destinationGraphics.Save();

                //draw the background image
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.DrawImageFast(_background.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw FLCS FAULT light
                if (InstrumentState.FLCSFault)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_flcsFault, new Rectangle(127, 154, 57, 19), new Rectangle(35, 54, 57, 17), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw ENGINE FAULT light
                if (InstrumentState.EngineFault)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_engineFault, new Rectangle(193, 154, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw AVIONICS FAULT light
                if (InstrumentState.AvionicsFault)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_avionicsFault, new Rectangle(262, 154, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw SEAT NOT ARMED light
                if (InstrumentState.SeatNotArmed)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_seatNotArmed, new Rectangle(331, 154, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw ELEC SYS light
                if (InstrumentState.ElecSys)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_elecSys, new Rectangle(124, 180, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw SEC light
                if (InstrumentState.SEC)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_sec, new Rectangle(193, 180, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw EQUIP HOT light
                if (InstrumentState.EquipHot)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_equipHot, new Rectangle(262, 180, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw NWS FAIL light
                if (InstrumentState.NWSFail)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_nwsFail, new Rectangle(331, 180, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw PROBE HEAT light
                if (InstrumentState.ProbeHeat)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_probeHeat, new Rectangle(124, 206, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw FUEL OIL HOT light
                if (InstrumentState.FuelOilHot)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_fuelOilHot, new Rectangle(193, 206, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw RADAR ALT light
                if (InstrumentState.RadarAlt)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_radarAlt, new Rectangle(262, 206, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw ANTI SKID light
                if (InstrumentState.AntiSkid)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_antiSkid, new Rectangle(331, 206, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw CADC light
                if (InstrumentState.CADC)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_CADC, new Rectangle(124, 232, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw INLET ICING light
                if (InstrumentState.InletIcing)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_inletIcing, new Rectangle(193, 232, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw IFF light
                if (InstrumentState.IFF)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_iff, new Rectangle(262, 232, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw HOOK light
                if (InstrumentState.Hook)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_hook, new Rectangle(331, 232, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw STORES CONFIG light
                if (InstrumentState.StoresConfig)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_storesConfig, new Rectangle(124, 258, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw OVERHEAT light
                if (InstrumentState.Overheat)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_overheat, new Rectangle(193, 258, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw NUCLEAR light
                if (InstrumentState.Nuclear)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_nuclear, new Rectangle(262, 258, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw OXY LOW light
                if (InstrumentState.OxyLow)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_oxyLow, new Rectangle(331, 258, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw ATF NOT ENGAGED light
                if (InstrumentState.ATFNotEngaged)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_atfNotEngaged, new Rectangle(124, 284, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw EEC light
                if (InstrumentState.EEC)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_EEC, new Rectangle(193, 284, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw ECM light
                if (InstrumentState.ECM)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_ECM, new Rectangle(262, 284, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw CABIN PRESS light
                if (InstrumentState.CabinPress)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_cabinPress, new Rectangle(331, 284, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw FWD FUEL LOW light
                if (InstrumentState.FwdFuelLow)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_fwdFuelLow, new Rectangle(124, 310, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw BUC light
                if (InstrumentState.BUC)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_BUC, new Rectangle(193, 310, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw slot #27 light
                if (InstrumentState.Slot27)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_slot27, new Rectangle(262, 310, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw slot #28 light
                if (InstrumentState.Slot28)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_slot28, new Rectangle(331, 310, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw AFT FUEL LOW light
                if (InstrumentState.AftFuelLow)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_aftFuelLow, new Rectangle(124, 336, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw slot #30 light
                if (InstrumentState.Slot30)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_slot30, new Rectangle(193, 336, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw slot #31 light
                if (InstrumentState.Slot31)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_slot31, new Rectangle(262, 336, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw slot #32 light
                if (InstrumentState.Slot32)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_slot32, new Rectangle(331, 336, 61, 19), new Rectangle(33, 54, 61, 19), GraphicsUnit.Pixel);
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }
                //restore the canvas's transform and clip settings to what they were when we entered this method
                destinationGraphics.Restore(initialState);
            }
        }
    }
}