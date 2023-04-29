using F4SharedMem;
using F4SharedMem.Headers;
using Newtonsoft.Json;
using System;
using System.Xml;

namespace F4SharedMemTester
{
    internal class BMS4FlightDataXmlDeserializer
    {
        public static BMS4FlightData Deserialize(XmlElement flightDataXmlElement)
        {
            BMS4FlightData bms4FlightDataStruct = CreateNewEmptyBMS4FlightDataStruct();
            if (flightDataXmlElement == null) return bms4FlightDataStruct;

            for (var i=0; i< flightDataXmlElement.Attributes.Count;i++)
            {
                var attribute = flightDataXmlElement.Attributes[i];
                switch (attribute.Name.Trim())
                {
                    case "x": float.TryParse(attribute.Value, out bms4FlightDataStruct.x); break;
                    case "y": float.TryParse(attribute.Value, out bms4FlightDataStruct.y); break;
                    case "z": float.TryParse(attribute.Value, out bms4FlightDataStruct.z); break;
                    case "xDot": float.TryParse(attribute.Value, out bms4FlightDataStruct.xDot); break;
                    case "yDot": float.TryParse(attribute.Value, out bms4FlightDataStruct.yDot); break;
                    case "zDot": float.TryParse(attribute.Value, out bms4FlightDataStruct.zDot); break;
                    case "alpha": float.TryParse(attribute.Value, out bms4FlightDataStruct.alpha); break;
                    case "beta": float.TryParse(attribute.Value, out bms4FlightDataStruct.beta); break;
                    case "gamma": float.TryParse(attribute.Value, out bms4FlightDataStruct.gamma); break;
                    case "pitch": float.TryParse(attribute.Value, out bms4FlightDataStruct.pitch); break;
                    case "roll": float.TryParse(attribute.Value, out bms4FlightDataStruct.roll); break;
                    case "yaw": float.TryParse(attribute.Value, out bms4FlightDataStruct.yaw); break;
                    case "mach": float.TryParse(attribute.Value, out bms4FlightDataStruct.mach); break;
                    case "kias": float.TryParse(attribute.Value, out bms4FlightDataStruct.kias); break;
                    case "vt": float.TryParse(attribute.Value, out bms4FlightDataStruct.vt); break;
                    case "gs": float.TryParse(attribute.Value, out bms4FlightDataStruct.gs); break;
                    case "windOffset": float.TryParse(attribute.Value, out bms4FlightDataStruct.windOffset); break;
                    case "nozzlePos": float.TryParse(attribute.Value, out bms4FlightDataStruct.nozzlePos); break;
                    case "internalFuel": float.TryParse(attribute.Value, out bms4FlightDataStruct.internalFuel); break;
                    case "externalFuel": float.TryParse(attribute.Value, out bms4FlightDataStruct.externalFuel); break;
                    case "fuelFlow": float.TryParse(attribute.Value, out bms4FlightDataStruct.fuelFlow); break;
                    case "rpm": float.TryParse(attribute.Value, out bms4FlightDataStruct.rpm); break;
                    case "ftit": float.TryParse(attribute.Value, out bms4FlightDataStruct.ftit); break;
                    case "gearPos": float.TryParse(attribute.Value, out bms4FlightDataStruct.gearPos); break;
                    case "speedBrake": float.TryParse(attribute.Value, out bms4FlightDataStruct.speedBrake); break;
                    case "epuFuel": float.TryParse(attribute.Value, out bms4FlightDataStruct.epuFuel); break;
                    case "oilPressure": float.TryParse(attribute.Value, out bms4FlightDataStruct.oilPressure); break;
                    case "headPitch": float.TryParse(attribute.Value, out bms4FlightDataStruct.headPitch); break;
                    case "headRoll": float.TryParse(attribute.Value, out bms4FlightDataStruct.headRoll); break;
                    case "headYaw": float.TryParse(attribute.Value, out bms4FlightDataStruct.headYaw); break;
                    case "ChaffCount": float.TryParse(attribute.Value, out bms4FlightDataStruct.ChaffCount); break;
                    case "FlareCount": float.TryParse(attribute.Value, out bms4FlightDataStruct.FlareCount); break;
                    case "NoseGearPos": float.TryParse(attribute.Value, out bms4FlightDataStruct.NoseGearPos); break;
                    case "LeftGearPos": float.TryParse(attribute.Value, out bms4FlightDataStruct.LeftGearPos); break;
                    case "RightGearPos": float.TryParse(attribute.Value, out bms4FlightDataStruct.RightGearPos); break;
                    case "AdiIlsHorPos": float.TryParse(attribute.Value, out bms4FlightDataStruct.AdiIlsHorPos); break;
                    case "AdiIlsVerPos": float.TryParse(attribute.Value, out bms4FlightDataStruct.AdiIlsVerPos); break;
                    case "courseDeviation": float.TryParse(attribute.Value, out bms4FlightDataStruct.courseDeviation); break;
                    case "desiredCourse": float.TryParse(attribute.Value, out bms4FlightDataStruct.desiredCourse); break;
                    case "distanceToBeacon": float.TryParse(attribute.Value, out bms4FlightDataStruct.distanceToBeacon); break;
                    case "bearingToBeacon": float.TryParse(attribute.Value, out bms4FlightDataStruct.bearingToBeacon); break;
                    case "currentHeading": float.TryParse(attribute.Value, out bms4FlightDataStruct.currentHeading); break;
                    case "desiredHeading": float.TryParse(attribute.Value, out bms4FlightDataStruct.desiredHeading); break;
                    case "deviationLimit": float.TryParse(attribute.Value, out bms4FlightDataStruct.deviationLimit); break;
                    case "halfDeviationLimit": float.TryParse(attribute.Value, out bms4FlightDataStruct.halfDeviationLimit); break;
                    case "localizerCourse": float.TryParse(attribute.Value, out bms4FlightDataStruct.localizerCourse); break;
                    case "airbaseX": float.TryParse(attribute.Value, out bms4FlightDataStruct.airbaseX); break;
                    case "airbaseY": float.TryParse(attribute.Value, out bms4FlightDataStruct.airbaseY); break;
                    case "totalValues": float.TryParse(attribute.Value, out bms4FlightDataStruct.totalValues); break;
                    case "TrimPitch": float.TryParse(attribute.Value, out bms4FlightDataStruct.TrimPitch); break;
                    case "TrimRoll": float.TryParse(attribute.Value, out bms4FlightDataStruct.TrimRoll); break;
                    case "TrimYaw": float.TryParse(attribute.Value, out bms4FlightDataStruct.TrimYaw); break;
                    case "fwd": float.TryParse(attribute.Value, out bms4FlightDataStruct.fwd); break;
                    case "aft": float.TryParse(attribute.Value, out bms4FlightDataStruct.aft); break;
                    case "total": float.TryParse(attribute.Value, out bms4FlightDataStruct.total); break;
                    case "headX": float.TryParse(attribute.Value, out bms4FlightDataStruct.headX); break;
                    case "headY": float.TryParse(attribute.Value, out bms4FlightDataStruct.headY); break;
                    case "headZ": float.TryParse(attribute.Value, out bms4FlightDataStruct.headZ); break;
                    case "lightBits": Enum.TryParse<LightBits>(attribute.Value, out var lightBits); bms4FlightDataStruct.lightBits = (uint)lightBits; break;
                    case "lightBits2": Enum.TryParse<LightBits2>(attribute.Value, out var lightBits2); bms4FlightDataStruct.lightBits2 = (uint)lightBits2; break;
                    case "lightBits3": Enum.TryParse<LightBits3>(attribute.Value, out var lightBits3); bms4FlightDataStruct.lightBits3 = (uint)lightBits3; break;
                    case "hsiBits": Enum.TryParse<HsiBits>(attribute.Value, out var hsiBits); bms4FlightDataStruct.hsiBits = (uint)hsiBits; break;
                    case "courseState": int.TryParse(attribute.Value, out bms4FlightDataStruct.courseState); break;
                    case "headingState": int.TryParse(attribute.Value, out bms4FlightDataStruct.headingState); break;
                    case "totalStates": int.TryParse(attribute.Value, out bms4FlightDataStruct.totalStates); break;
                    case "UFCTChan": int.TryParse(attribute.Value, out bms4FlightDataStruct.UFCTChan); break;
                    case "AUXTChan": int.TryParse(attribute.Value, out bms4FlightDataStruct.AUXTChan); break;
                    case "VersionNum": int.TryParse(attribute.Value, out bms4FlightDataStruct.VersionNum); break;
                    case "MainPower": Enum.TryParse<TriStateSwitchStates>(attribute.Value, out var mainPower); bms4FlightDataStruct.MainPower = (int)mainPower; break;
                    case "RWRObjectCount": int.TryParse(attribute.Value, out bms4FlightDataStruct.RwrObjectCount); break;
                    case "DEDLines": PopulateDEDPFLLineOfTextArray(bms4FlightDataStruct.DEDLines, attribute.Value); break;
                    case "Invert": PopulateDEDPFLLineOfTextArray(bms4FlightDataStruct.Invert, attribute.Value); break;
                    case "PFLLines": PopulateDEDPFLLineOfTextArray(bms4FlightDataStruct.PFLLines, attribute.Value); break;
                    case "PFLInvert": PopulateDEDPFLLineOfTextArray(bms4FlightDataStruct.PFLInvert, attribute.Value); break;
                    case "RWRSymbol": PopulateArray(bms4FlightDataStruct.RWRsymbol, attribute.Value); break;
                    case "bearing": PopulateArray(bms4FlightDataStruct.bearing, attribute.Value); break;
                    case "missileActivity": PopulateArray(bms4FlightDataStruct.missileActivity, attribute.Value); break;
                    case "missileLaunch": PopulateArray(bms4FlightDataStruct.missileLaunch, attribute.Value); break;
                    case "selected": PopulateArray(bms4FlightDataStruct.selected, attribute.Value); break;
                    case "lethality": PopulateArray(bms4FlightDataStruct.lethality, attribute.Value); break;
                    case "newDetection": PopulateArray(bms4FlightDataStruct.newDetection, attribute.Value); break;
                    default:
                        break;

                }
            }
            return bms4FlightDataStruct;
        }

        private static BMS4FlightData CreateNewEmptyBMS4FlightDataStruct()
        {
            BMS4FlightData flightData = new BMS4FlightData();
            flightData.DEDLines = GetNewEmptyDEDPFLLineOfTextArray();
            flightData.Invert = GetNewEmptyDEDPFLLineOfTextArray();
            flightData.PFLLines = GetNewEmptyDEDPFLLineOfTextArray();
            flightData.PFLInvert = GetNewEmptyDEDPFLLineOfTextArray();
            flightData.RWRsymbol = new int[FlightData.MAX_RWR_OBJECTS];
            flightData.bearing = new float[FlightData.MAX_RWR_OBJECTS];
            flightData.missileActivity = new uint[FlightData.MAX_RWR_OBJECTS];
            flightData.missileLaunch = new uint[FlightData.MAX_RWR_OBJECTS];
            flightData.selected = new uint[FlightData.MAX_RWR_OBJECTS];
            flightData.lethality = new float[FlightData.MAX_RWR_OBJECTS];
            flightData.newDetection = new uint[FlightData.MAX_RWR_OBJECTS];
            return flightData;
        }

        private static DED_PFL_LineOfText[] GetNewEmptyDEDPFLLineOfTextArray()
        {
            return new DED_PFL_LineOfText[] {
                new DED_PFL_LineOfText { chars = new byte[26] },
                new DED_PFL_LineOfText { chars = new byte[26] },
                new DED_PFL_LineOfText { chars = new byte[26] },
                new DED_PFL_LineOfText { chars = new byte[26] },
                new DED_PFL_LineOfText { chars = new byte[26] }
            };
        }

        private static void PopulateArray(Array destinationArray, string toParse)
        {
            var valuesArray = (Array)JsonConvert.DeserializeObject(toParse, destinationArray.GetType());
            Array.Copy(
                sourceArray: valuesArray,
                destinationArray: destinationArray,
                length: Math.Min(valuesArray.Length, destinationArray.Length)
            );
        }

        private static void PopulateDEDPFLLineOfTextArray(DED_PFL_LineOfText[] dedPFLLineOfTextArray, string toParse)
        {
            var decoded = JsonConvert.DeserializeObject<char[][]>(toParse);
            for (var i = 0; i < decoded.Length && i < dedPFLLineOfTextArray.Length; i++)
            {
                var thisLineBytes = System.Text.Encoding.ASCII.GetBytes(decoded[i], 0, decoded[i].Length);
                Array.Copy(
                    sourceArray: thisLineBytes,
                    destinationArray: dedPFLLineOfTextArray[i].chars,
                    length: Math.Min(decoded[i].Length, dedPFLLineOfTextArray[i].chars.Length)
                );
            }
        }
    }
}
