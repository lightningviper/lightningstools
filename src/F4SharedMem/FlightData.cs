using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using F4SharedMem.Headers;
using System.Collections.Generic;

namespace F4SharedMem
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed class FlightData
    {
        [ComVisible(true)]
        [Serializable]
        public struct OptionSelectButtonLabel
        {
            public string Line1;
            public string Line2;
            public bool Inverted;
        }
        public FlightData()
        {
        }
        internal FlightData(BMS4FlightData data)
        {
            PopulateFromStruct(data);
        }
        private static Dictionary<Type, FieldInfo[]> _headerFields =new Dictionary<Type, FieldInfo[]>();
        private static Dictionary<string, FieldInfo> _flightDataFields = new Dictionary<string, FieldInfo>();
        internal void PopulateFromStruct<T>(T data)
        {
            FieldInfo[] relevantHeaderFields = null;
            var foundRelevantHeaderFields = _headerFields.TryGetValue(data.GetType(), out relevantHeaderFields);
            if (!foundRelevantHeaderFields)
            {
                relevantHeaderFields = data.GetType().GetFields(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (relevantHeaderFields == null)
                {
                    return;
                }
                _headerFields[data.GetType()]= relevantHeaderFields;
            }
            foreach (var currentField in relevantHeaderFields)
            {
                FieldInfo thisFlightDataField = null;
                var fieldInfoFoundInCache=_flightDataFields.TryGetValue(currentField.Name, out thisFlightDataField);
                if (!fieldInfoFoundInCache)
                {
                    thisFlightDataField = typeof(FlightData).GetField(currentField.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (thisFlightDataField !=null)
                    {
                        _flightDataFields[currentField.Name]=thisFlightDataField;
                    }
                }
                    
                    
                if (thisFlightDataField == null) continue;
                var currentFieldType = currentField.FieldType;
                if (currentFieldType.IsArray)
                {
                    if (currentFieldType == typeof(DED_PFL_LineOfText[]))
                    {
                        PopulateDedPflLineOfText(data, currentField, thisFlightDataField);
                    }
                    else if (currentFieldType == typeof(OSBLabel[]))
                    {
                        PopulateOSBLabel(data, currentField, thisFlightDataField);
                    }
                    else if (currentFieldType == typeof(Callsign_LineOfText[]))
                    {
                        PopulateCallsignLineOfText(data, currentField, thisFlightDataField);
                    }
                    else
                    {
                        var currentValue = (Array)currentField.GetValue(data);
                        thisFlightDataField.SetValue(this, currentValue);
                    }
                }
                else if (currentFieldType.Name.ToLowerInvariant().Contains("uint"))
                {
                    thisFlightDataField.SetValue(this, BitConverter.ToInt32(BitConverter.GetBytes((uint)currentField.GetValue(data)), 0));
                }
                else
                {

                    thisFlightDataField.SetValue(this, currentField.GetValue(data));
                }
            }
        }

        private void PopulateDedPflLineOfText(object data, FieldInfo currentField, FieldInfo thisField)
        {
            var currentValue = (DED_PFL_LineOfText[]) currentField.GetValue(data);
            var valuesToAssign = new string[currentValue.Length];
            for (var j = 0; j < currentValue.Length; j++)
            {
                var currentItem = currentValue[j];
                var sb = new StringBuilder(currentItem.chars.Length);
                var invert = currentField.Name.ToLowerInvariant().Contains("invert");
                foreach (var chr in currentItem.chars)
                {
                    if (invert)
                    {
                        if (chr == 0x02)
                        {
                            sb.Append((char) chr);
                        }
                        else
                        {
                            sb.Append(' ');
                        }
                    }
                    else
                    {
                        if (chr != 0)
                        {
                            sb.Append((char) chr);
                        }
                    }
                }
                valuesToAssign[j] = sb.ToString();
            }
            thisField.SetValue(this, valuesToAssign);
        }

        private void PopulateCallsignLineOfText(object data, FieldInfo currentField, FieldInfo thisField)
        {
            var currentValue = (Callsign_LineOfText[]) currentField.GetValue(data);
            var valuesToAssign = new string[currentValue.Length];
            for (var j = 0; j < currentValue.Length; j++)
            {
                var currentItem = currentValue[j];
                var sb = new StringBuilder(currentItem.chars.Length);
                foreach (var chr in currentItem.chars.Where(chr => chr != 0))
                {
                    sb.Append((char) chr);
                }
                valuesToAssign[j] = sb.ToString();
            }
            thisField.SetValue(this, valuesToAssign);
        }

        private void PopulateOSBLabel(object data, FieldInfo currentField, FieldInfo thisField)
        {
            var currentValue = (OSBLabel[]) currentField.GetValue(data);
            var valuesToAssign = new OptionSelectButtonLabel[currentValue.Length];
            for (var j = 0; j < currentValue.Length; j++)
            {
                var currentItem = currentValue[j];
                var label = new OptionSelectButtonLabel();
                var lineBuilder = new StringBuilder(currentItem.Line1.Length);

                foreach (var chr in currentItem.Line1)
                {
                    if (chr == 0)
                    {
                        lineBuilder.Append(" ");
                    }
                    else
                    {
                        lineBuilder.Append((char) chr);
                    }
                }
                label.Line1 = lineBuilder.ToString();
                lineBuilder = new StringBuilder(currentItem.Line2.Length);
                foreach (var chr in currentItem.Line2)
                {
                    if (chr == 0)
                    {
                        lineBuilder.Append(" ");
                    }
                    else
                    {
                        lineBuilder.Append((char) chr);
                    }
                }
                label.Line2 = lineBuilder.ToString();
                label.Inverted = currentItem.Inverted;
                valuesToAssign[j] = label;
            }
            thisField.SetValue(this, valuesToAssign);
        }

        public float x;            // Ownship North (Ft)
        public float y;            // Ownship East (Ft)
        public float z;            // Ownship Down (Ft)
        public float xDot;         // Ownship North Rate (ft/sec)
        public float yDot;         // Ownship East Rate (ft/sec)
        public float zDot;         // Ownship Down Rate (ft/sec)
        public float alpha;        // Ownship AOA (Degrees)
        public float beta;         // Ownship Beta (Degrees)
        public float gamma;        // Ownship Gamma (Radians)
        public float pitch;        // Ownship Pitch (Radians)
        public float roll;         // Ownship Roll (Radians)
        public float yaw;          // Ownship Yaw (Radians)
        public float mach;         // Ownship Mach number
        public float kias;         // Ownship Indicated Airspeed (Knots)
        public float vt;           // Ownship True Airspeed (Ft/Sec)
        public float gs;           // Ownship Normal Gs
        public float windOffset;   // Wind delta to FPM (Radians)
        public float nozzlePos;    // Ownship engine nozzle percent open (0-100)
        public float nozzlePos2;   // Ownship engine nozzle2 percent open (0-100)
        public float internalFuel; // Ownship internal fuel (Lbs)
        public float externalFuel; // Ownship external fuel (Lbs)
        public float fuelFlow;     // Ownship fuel flow (Lbs/Hour)
        public float rpm;          // Ownship engine rpm (Percent 0-103)
        public float rpm2;         // Ownship engine rpm2 (Percent 0-103)
        public float ftit;         // Ownship Forward Turbine Inlet Temp (Degrees C)
        public float ftit2;        // Ownship Forward Turbine Inlet Temp2 (Degrees C)
        public float gearPos;      // Ownship Gear position 0 = up, 1 = down;
        public float speedBrake;   // Ownship speed brake position 0 = closed, 1 = 60 Degrees open
        public float epuFuel;      // Ownship EPU fuel (Percent 0-100)
        public float oilPressure;  // Ownship Oil Pressure (Percent 0-100)
        public float oilPressure2; // Ownship Oil Pressure2 (Percent 0-100)
        public float hydPressureA;  // Ownship Hydraulic Pressure A
        public float hydPressureB;  // Ownship Hydraulic Pressure B
        public int lightBits;    // Cockpit Indicator Lights, one bit per bulb. See enum

        // These are inputs. Use them carefully
        // NB: these do not work when TrackIR device is enabled
        public float headPitch;    // Head pitch offset from design eye (radians)
        public float headRoll;     // Head roll offset from design eye (radians)
        public float headYaw;      // Head yaw offset from design eye (radians)

        // new lights
        public int lightBits2;   // Cockpit Indicator Lights, one bit per bulb. See enum
        public int lightBits3;   // Cockpit Indicator Lights, one bit per bulb. See enum

        // chaff/flare
        public float ChaffCount;   // Number of Chaff left
        public float FlareCount;   // Number of Flare left

        // landing gear
        public float NoseGearPos;  // Position of the nose landinggear; caution: full down values defined in dat files
        public float LeftGearPos;  // Position of the left landinggear; caution: full down values defined in dat files
        public float RightGearPos; // Position of the right landinggear; caution: full down values defined in dat files

        // ADI values
        public float AdiIlsHorPos; // Position of horizontal ILS bar
        public float AdiIlsVerPos; // Position of vertical ILS bar

        // HSI states
        public int courseState;    // HSI_STA_CRS_STATE
        public int headingState;   // HSI_STA_HDG_STATE
        public int totalStates;    // HSI_STA_TOTAL_STATES; never set

        // HSI values
        public float courseDeviation;  // HSI_VAL_CRS_DEVIATION
        public float desiredCourse;    // HSI_VAL_DESIRED_CRS
        public float distanceToBeacon;    // HSI_VAL_DISTANCE_TO_BEACON
        public float bearingToBeacon;  // HSI_VAL_BEARING_TO_BEACON
        public float currentHeading;      // HSI_VAL_CURRENT_HEADING
        public float desiredHeading;   // HSI_VAL_DESIRED_HEADING
        public float deviationLimit;      // HSI_VAL_DEV_LIMIT
        public float halfDeviationLimit;  // HSI_VAL_HALF_DEV_LIMIT
        public float localizerCourse;     // HSI_VAL_LOCALIZER_CRS
        public float airbaseX;            // HSI_VAL_AIRBASE_X
        public float airbaseY;            // HSI_VAL_AIRBASE_Y
        public float totalValues;         // HSI_VAL_TOTAL_VALUES; never set

        public float TrimPitch;  // Value of trim in pitch axis, -0.5 to +0.5
        public float TrimRoll;   // Value of trim in roll axis, -0.5 to +0.5
        public float TrimYaw;    // Value of trim in yaw axis, -0.5 to +0.5

        // HSI flags
        public int hsiBits;      // HSI flags

        //DED Lines
        public string[] DEDLines;  //25 usable chars
        public string[] Invert;    //25 usable chars

        //PFL Lines
        public string[] PFLLines;  //25 usable chars
        public string[] PFLInvert; //25 usable chars

        //TacanChannel
        public int UFCTChan;
        public int AUXTChan;

        //RWR
        public int RwrObjectCount;
        public int[] RWRsymbol;
        public float[] bearing;
        public int[] missileActivity;
        public int[] missileLaunch;
        public int[] selected;
        public float[] lethality;
        public int[] newDetection;

        //fuel values
        public float fwd;
        public float aft;
        public float total;

        public int VersionNum;    //Version of Mem area
        public int VersionNum2;     // Version of Mem area
        public float headX;        // Head X offset from design eye (feet)
        public float headY;        // Head Y offset from design eye (feet)
        public float headZ;        // Head Z offset from design eye (feet)
        public int MainPower;

        public byte navMode; //HSI nav mode (new in BMS4)
        public float aauz; //AAU altimeter indicated altitude (new in BMS4)
        public int AltCalReading;	// barometric altitude calibration (depends on CalType)
        public int altBits;		// various altimeter bits, see AltBits enum for details
        public float cabinAlt;		// Ownship cabin altitude

        public int BupUhfPreset;	// BUP UHF channel preset
        public int BupUhfFreq;		// BUP UHF channel frequency

        public int powerBits;		// Ownship power bus / generator states, see PowerBits enum for details
        public int blinkBits;		// Cockpit indicator lights blink status, see BlinkBits enum for details
        // NOTE: these bits indicate only *if* a lamp is blinking, in addition to the
        // existing on/off bits. It's up to the external program to implement the
        // *actual* blinking.
        public int cmdsMode;		// Ownship CMDS mode state, see CmdsModes enum for details
        public int currentTime;	    // Current time in seconds (max 60 * 60 * 24)
        public short vehicleACD;	// Ownship ACD index number, i.e. which aircraft type are we flying.
        public byte[] tacanInfo;    //TACAN info (new in BMS4)
        public float fuelFlow2;     // Ownship fuel flow2 (Lbs/Hour)
        public byte[] RwrInfo;      //[512] New RWR Info
        public float lefPos;       // Ownship LEF position
        public float tefPos;       // Ownship TEF position
        public float vtolPos;      // Ownship VTOL exhaust angle

        public byte pilotsOnline;       // Number of pilots in an MP session

        // RWRINFO_SIZE  512
        // CALLSIGN_LEN  12
        // MAX_CALLSIGNS  32
        public string[] pilotsCallsign;   // [MAX_CALLSIGNS][CALLSIGN_LEN] List of pilots callsign connected to an MP session
        public byte[] pilotsStatus;     // [MAX_CALLSIGNS] Status of the MP pilots, see enum FlyStates

        public OptionSelectButtonLabel[] leftMFD;
        public OptionSelectButtonLabel[] rightMFD;
        public object ExtensionData;

        public bool UfcTacanIsAA { get { return (tacanInfo !=null && ((tacanInfo[(int)TacanSources.UFC] & (byte)TacanBits.mode) != 0)); } }
        public bool AuxTacanIsAA { get { return (tacanInfo != null && ((tacanInfo[(int)TacanSources.AUX] & (byte)TacanBits.mode) != 0)); } }
        public bool UfcTacanIsX { get { return (tacanInfo != null && ((tacanInfo[(int)TacanSources.UFC] & (byte)TacanBits.band) != 0)); } }
        public bool AuxTacanIsX { get { return (tacanInfo != null && ((tacanInfo[(int)TacanSources.AUX] & (byte)TacanBits.band) != 0)); } }

        public IntellivibeData IntellivibeData { get; set; }

        public RadioClientControl RadioClientControlData { get; set; }
        public RadioClientStatus RadioClientStatus { get; set; }
        
    }
}