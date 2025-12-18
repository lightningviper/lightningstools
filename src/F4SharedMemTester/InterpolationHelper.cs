using F4SharedMem;
using F4SharedMem.Headers;
using System;
using System.Linq;
using System.Reflection;
using static F4SharedMemTester.TestFile;

namespace F4SharedMemTester
{
    internal class InterpolationHelper
    {
        public static Moment Interpolate(Moment a, Moment b, TimeSpan momentTTime)
        {
            Moment toReturn = Common.Serialization.Util.DeepClone(a);
            if (!a.StartTime.HasValue || !b.StartTime.HasValue) return toReturn;
            if (a.FlightData.HasValue && b.FlightData.HasValue)
            {
                string[] nonInterpolatedFields = { 
                    "lightBits", "lightBits2", "lightBits3","hsiBits",
                    "courseState", "headingState", "totalStates", "totalValues",
                    "deviationLimit", "halfDeviationLimit", 
                    "airbaseX", "airbaseY", 
                    "DEDLines", "Invert", "PFLLines", "PFLInvert", 
                    "UFCTChan", "AUXTChan", 
                    "RWRObjectCount", "RWRsymbol", 
                    "bearing", "missileActivity", "missileLaunch", "selected", "lethality", "newDetection", 
                    "MainPower", "VersionNum"};

                string[] fieldsToIntegerize = { 
                    "ChaffCount", "FlareCount", 
                    "desiredCourse", "desiredHeading", "localizerCourse",
                    "fwd", "aft", "total" };

                foreach (var fieldInfo in typeof(BMS4FlightData).GetFields().Where(x => !nonInterpolatedFields.Contains(x.Name) && !x.IsStatic && !x.IsInitOnly && !x.IsLiteral && !x.IsPrivate).ToList())
                {
                    var interpolatedValue = Interpolate(
                        momentAValue: fieldInfo.GetValue(a.FlightData.Value),
                        momentATime: a.StartTime.Value,
                        momentBValue: fieldInfo.GetValue(b.FlightData.Value),
                        momentBTime: b.StartTime.Value,
                        momentTTime: momentTTime);
                    if (fieldsToIntegerize.Contains(fieldInfo.Name))
                    {
                        interpolatedValue = Convert.ChangeType(Convert.ToInt64(interpolatedValue), fieldInfo.GetValue(a.FlightData.Value).GetType());
                    }
                    var boxedStruct = (object)toReturn.FlightData.Value;
                    fieldInfo.SetValue(boxedStruct, interpolatedValue);
                    toReturn.FlightData = (BMS4FlightData)boxedStruct;
                }
            }

            if (a.FlightData2.HasValue && b.FlightData2.HasValue)
            {
                string[] nonInterpolatedFields = {
                    "navMode", "tacanInfo", "altBits", "powerBits", "blinkBits", "cmdsMode", "BupUhfPreset", 
                    "BupUhfFreq", "vehicleACD", "VersionNum2", "RwrInfo", 
                    "pilotsOnline", "pilotsCallsign", "pilotsStatus",
                    "RTT_size", "RTT_area",
                    "iffBackupMode1Digit1", "iffBackupMode1Digit2", "iffBackupMode3ADigit1", "iffBackupMode3ADigit2", 
                    "instrLight", "bettyBits", "miscBits", 
                    "BMSVersionMajor", "BMSVersionMinor", "BMSVersionMicro", "BMSBuildNumber", 
                    "StringAreaSize", "StringAreaTime", "DrawingAreaSize",
                    "floodConsole", "ecmBits", "ecmOper", "RWRjammingStatus"};

                string[] fieldsToIntegerize = { "currentTime", "bingoFuel", "caraAlow" };

                foreach (var fieldInfo in typeof(FlightData2).GetFields().Where(x => !nonInterpolatedFields.Contains(x.Name) && !x.IsStatic && !x.IsInitOnly && !x.IsLiteral && !x.IsPrivate).ToList())
                {
                    var interpolatedValue = Interpolate(
                        momentAValue: fieldInfo.GetValue(a.FlightData2.Value),
                        momentATime: a.StartTime.Value,
                        momentBValue: fieldInfo.GetValue(b.FlightData2.Value),
                        momentBTime: b.StartTime.Value,
                        momentTTime: momentTTime);
                    if (fieldsToIntegerize.Contains(fieldInfo.Name))
                    {
                        interpolatedValue = Convert.ChangeType(Convert.ToInt64(interpolatedValue), fieldInfo.GetValue(a.FlightData2.Value).GetType());
                    }
                    var boxedStruct = (object)toReturn.FlightData2.Value;
                    fieldInfo.SetValue(boxedStruct, interpolatedValue);
                    toReturn.FlightData2 = (FlightData2)boxedStruct;
                }
            }

            if (a.IntellivibeData.HasValue && b.IntellivibeData.HasValue)
            {
                string[] nonInterpolatedFields = { "lastDamage" };
                string[] fieldsToIntegerize = { };
                foreach (var fieldInfo in typeof(IntellivibeData).GetFields().Where(x => !nonInterpolatedFields.Contains(x.Name) && !x.IsStatic && !x.IsInitOnly && !x.IsLiteral && !x.IsPrivate).ToList())
                {
                    var interpolatedValue = Interpolate(
                        momentAValue: fieldInfo.GetValue(a.IntellivibeData.Value),
                        momentATime: a.StartTime.Value,
                        momentBValue: fieldInfo.GetValue(b.IntellivibeData.Value),
                        momentBTime: b.StartTime.Value,
                        momentTTime: momentTTime);
                    if (fieldsToIntegerize.Contains(fieldInfo.Name))
                    {
                        interpolatedValue = Convert.ChangeType(Convert.ToInt64(interpolatedValue), fieldInfo.GetValue(a.IntellivibeData.Value).GetType());
                    }
                    var boxedStruct = (object)toReturn.IntellivibeData.Value;
                    fieldInfo.SetValue(boxedStruct, interpolatedValue);
                    toReturn.IntellivibeData = (IntellivibeData)boxedStruct;
                }
            }
            return toReturn;
        }
        private static T Interpolate<T>(T momentAValue, TimeSpan momentATime, T momentBValue, TimeSpan momentBTime, TimeSpan momentTTime) 
        {
            switch(Type.GetTypeCode(momentAValue.GetType()))
            {
                case TypeCode.Byte: return (T)(object)(Byte)((Byte)(object)momentAValue + (((Byte)(object)momentBValue - (Byte)(object)momentAValue) * ((Double)(momentTTime.Ticks - momentATime.Ticks) / (momentBTime.Ticks - momentATime.Ticks))));
                case TypeCode.SByte: return (T)(object)(SByte)((SByte)(object)momentAValue + (((SByte)(object)momentBValue - (SByte)(object)momentAValue) * ((Double)(momentTTime.Ticks - momentATime.Ticks) / (momentBTime.Ticks - momentATime.Ticks))));
                case TypeCode.Single: return (T)(object)(Single)((Single)(object)momentAValue + (((Single)(object)momentBValue - (Single)(object)momentAValue) * ((Single)(momentTTime.Ticks - momentATime.Ticks) / (momentBTime.Ticks - momentATime.Ticks))));
                case TypeCode.Double: return (T)(object)(Double)((Double)(object)momentAValue + (((Double)(object)momentBValue - (Double)(object)momentAValue) * ((Double)(momentTTime.Ticks - momentATime.Ticks) / (momentBTime.Ticks - momentATime.Ticks))));
                case TypeCode.Int16: return (T)(object)(Int16)((Int16)(object)momentAValue + (((Int16)(object)momentBValue - (Int16)(object)momentAValue) * ((Double)(momentTTime.Ticks - momentATime.Ticks) / (momentBTime.Ticks - momentATime.Ticks))));
                case TypeCode.UInt16: return (T)(object)(UInt16)((UInt16)(object)momentAValue + (((UInt16)(object)momentBValue - (UInt16)(object)momentAValue) * ((Double)(momentTTime.Ticks - momentATime.Ticks) / (momentBTime.Ticks - momentATime.Ticks))));
                case TypeCode.Int32: return (T)(object)(Int32)((Int32)(object)momentAValue + (((Int32)(object)momentBValue - (Int32)(object)momentAValue) * ((Double)(momentTTime.Ticks - momentATime.Ticks) / (momentBTime.Ticks - momentATime.Ticks))));
                case TypeCode.UInt32: return (T)(object)(UInt32)((UInt32)(object)momentAValue + (((UInt32)(object)momentBValue - (UInt32)(object)momentAValue) * ((Double)(momentTTime.Ticks - momentATime.Ticks) / (momentBTime.Ticks - momentATime.Ticks))));
                case TypeCode.Int64: return (T)(object)(Int64)((Int64)(object)momentAValue + (((Int64)(object)momentBValue - (Int64)(object)momentAValue) * ((Double)(momentTTime.Ticks - momentATime.Ticks) / (momentBTime.Ticks - momentATime.Ticks))));                
                case TypeCode.UInt64: return (T)(object)(UInt64)((UInt64)(object)momentAValue + (((UInt64)(object)momentBValue - (UInt64)(object)momentAValue) * ((Double)(momentTTime.Ticks - momentATime.Ticks) / (momentBTime.Ticks - momentATime.Ticks))));
                case TypeCode.Decimal: return (T)(object)(Decimal)((Decimal)(object)momentAValue + (((Decimal)(object)momentBValue - (Decimal)(object)momentAValue) * ((Decimal)(momentTTime.Ticks - momentATime.Ticks) / (momentBTime.Ticks - momentATime.Ticks))));
                default: return momentAValue;
            }
        }

    }
}
