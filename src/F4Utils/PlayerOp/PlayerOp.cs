using System.IO;
using System.Text;

namespace F4Utils.PlayerOp
{
    public class PlayerOp
    {
        public const int PL_FNAME_LEN = 32;
        public int ACMIFileSize;
        public int BldDeaggLevel;
        public int CampAirDefenseRatio;
        public int CampAirRatio;
        public int CampEnemyAirExperience;
        public int CampEnemyGroundExperience;
        public int CampEnemyStockpile;
        public int CampFriendlyStockpile;
        public int CampGroundRatio;
        public int CampNavalRatio;


        public int DispFlags;
        public int DispMaxTerrainLevel;
        public float DispTerrainDist;
        public int DynamicHeadSensitivity;
        public int GeneralFlags;
        public float GrassDensity;
        public int[] GroupVol = new int[(int) SoundGroups.NUM_SOUND_GROUPS];
        public float HDRBloom;
        public float HDRBlur;
        public int IVCvsAIBalance;
        public int[] InitVol = new int[(int)SoundGroups.NUM_SOUND_GROUPS];

        public int KeyboardPOVPanningSensitivity;
        public float MouseLookSensitivity;
        public int MouseWheelSensitivity;
        public int ObjDeaggLevel;
        public float ObjDetailLevel;
        public int ObjFlags;
        public float ObjMagnification;
        public float PlayerBubble;
        public bool PlayerRadioVoice;
        public float Realism;
        public int Season;
        public float SfxLevel;
        public RefuelModeType SimAirRefuelingMode;
        public AutopilotModeType SimAutopilotType;
        public AvionicsType SimAvionicsType;
        public int SimFlags;
        public FlightModelType SimFlightModel;
        public PadlockModeType SimPadlockMode;
        public VisualCueType SimVisualCueMode;
        public WeaponEffectType SimWeaponEffect;
        public int SoundExtAttenuation;
        public int SoundFlags;
        public int[] TempVol = new int[(int) SoundGroups.NUM_SOUND_GROUPS];
        public int TrackIRZAxisUse;
        public bool TrackIR_2d;
        public bool TrackIR_3d;
        public bool TrackIR_VE;
        public float TreeDensity;
        public bool UIComms;
        public byte[] VersionString = new byte[32];
        public int WeatherCondition;
        public bool clickablePitMode;
        public bool enableAxisShaping;
        public bool enableFFB;
        public bool enableMouseLook;
        public bool infoBar;
        public byte[] keyfile = new byte[PL_FNAME_LEN];
        public int pit3DPanMode;
        public bool rollLinkedNWS;
        public int RAMP_MINUTES;
        public byte skycol;
        public bool sticky3dPitSnapViews;
        public bool subTitles;
        public bool userMessages;

        public PlayerOp()
        {
        }

        public PlayerOp(Stream stream) : this()
        {
            using (var br = new BinaryReader(stream, Encoding.Default, leaveOpen:true))
            {
                DispFlags = br.ReadInt32();
                DispTerrainDist = br.ReadSingle();
                DispMaxTerrainLevel = br.ReadInt32();
                ObjFlags = br.ReadInt32();
                ObjDetailLevel = br.ReadSingle();
                ObjMagnification = br.ReadSingle();
                ObjDeaggLevel = br.ReadInt32();
                BldDeaggLevel = br.ReadInt32();
                ACMIFileSize = br.ReadInt32();
                SfxLevel = br.ReadSingle();
                PlayerBubble = br.ReadSingle();

                HDRBlur = br.ReadSingle();
                HDRBloom = br.ReadSingle();
                TreeDensity = br.ReadSingle();
                GrassDensity = br.ReadSingle();
                WeatherCondition = br.ReadInt32();
                Season = br.ReadInt32();
                SimFlags = br.ReadInt32();
                SimFlightModel = (FlightModelType) (br.ReadInt32());
                SimWeaponEffect = (WeaponEffectType) (br.ReadInt32());
                SimAvionicsType = (AvionicsType) (br.ReadInt32());
                SimAutopilotType = (AutopilotModeType) (br.ReadInt32());
                SimAirRefuelingMode = (RefuelModeType) (br.ReadInt32());
                SimPadlockMode = (PadlockModeType) (br.ReadInt32());
                SimVisualCueMode = (VisualCueType) (br.ReadInt32());
                GeneralFlags = br.ReadInt32();
                CampGroundRatio = br.ReadInt32();
                CampAirRatio = br.ReadInt32();
                CampAirDefenseRatio = br.ReadInt32();
                CampNavalRatio = br.ReadInt32();
                CampEnemyAirExperience = br.ReadInt32();
                CampEnemyGroundExperience = br.ReadInt32();
                CampEnemyStockpile = br.ReadInt32();
                CampFriendlyStockpile = br.ReadInt32();
                for (var i = 0; i < (int) SoundGroups.NUM_SOUND_GROUPS; i++)
                {
                    GroupVol[i] = br.ReadInt32();
                }
                for (var i = 0; i < (int)SoundGroups.NUM_SOUND_GROUPS; i++)
                {
                    TempVol[i] = br.ReadInt32();
                }
                for (var i = 0; i < (int)SoundGroups.NUM_SOUND_GROUPS; i++)
                {
                    InitVol[i] = br.ReadInt32();
                }
                IVCvsAIBalance = br.ReadInt32();
                Realism = br.ReadSingle();
                keyfile = br.ReadBytes(PL_FNAME_LEN);
                RAMP_MINUTES = br.ReadInt32();
                skycol = br.ReadByte();
                PlayerRadioVoice = br.ReadBoolean();
                UIComms = br.ReadBoolean();
                infoBar = br.ReadBoolean();
                subTitles = br.ReadBoolean();
                TrackIR_2d = br.ReadBoolean();
                TrackIR_3d = br.ReadBoolean();
                TrackIR_VE = br.ReadBoolean();
                TrackIRZAxisUse = br.ReadInt32();
                enableFFB = br.ReadBoolean();
                enableMouseLook = br.ReadBoolean();
                MouseLookSensitivity = br.ReadSingle();
                MouseWheelSensitivity = br.ReadInt32();
                KeyboardPOVPanningSensitivity = br.ReadInt32();
                DynamicHeadSensitivity = br.ReadInt32();
                clickablePitMode = br.ReadBoolean();
                enableAxisShaping = br.ReadBoolean();
                rollLinkedNWS = br.ReadBoolean();
                pit3DPanMode = br.ReadInt32();
                sticky3dPitSnapViews = br.ReadBoolean();
                userMessages = br.ReadBoolean();
                VersionString = br.ReadBytes(32);
                SoundFlags = br.ReadInt32();
                SoundExtAttenuation = br.ReadInt32();
            }
        }
    }
}