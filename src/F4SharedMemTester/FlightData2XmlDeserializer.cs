using F4SharedMem;
using F4SharedMem.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml;

namespace F4SharedMemTester
{
    internal class FlightData2XmlDeserializer
    {
        public static FlightData2 Deserialize(XmlElement flightData2XmlElement, FlightData2? originalData = null)
        {
            FlightData2 flightData2Struct = originalData.HasValue ? Common.Serialization.Util.DeepClone(originalData.Value): CreateNewEmptyFlightData2Struct();
            if (flightData2XmlElement == null) return flightData2Struct;

            for (var i = 0; i < flightData2XmlElement.Attributes.Count; i++)
            {
                var attribute = flightData2XmlElement.Attributes[i];
                switch (attribute.Name.Trim())
                {
                    case "nozzlePos2": float.TryParse(attribute.Value, out flightData2Struct.nozzlePos2); break;
                    case "rpm2": float.TryParse(attribute.Value, out flightData2Struct.rpm2); break;
                    case "ftit2": float.TryParse(attribute.Value, out flightData2Struct.ftit2); break;
                    case "oilPressure2": float.TryParse(attribute.Value, out flightData2Struct.oilPressure2); break;
                    case "navMode": Enum.TryParse<NavModes>(attribute.Value, out var navMode); flightData2Struct.navMode = (byte)navMode; break;
                    case "aauz": float.TryParse(attribute.Value, out flightData2Struct.aauz); break;
                    case "tacanInfo": PopulateEnumArray<TacanBits>(flightData2Struct.tacanInfo, attribute.Value); break;
                    case "AltCalReading": int.TryParse(attribute.Value, out flightData2Struct.AltCalReading); break;
                    case "altBits":Enum.TryParse<AltBits>(attribute.Value, out var altBits); flightData2Struct.altBits = (uint)altBits; break;
                    case "powerBits": Enum.TryParse<PowerBits>(attribute.Value, out var powerBits); flightData2Struct.powerBits = (uint)powerBits; break;
                    case "blinkBits": Enum.TryParse<BlinkBits>(attribute.Value, out var blinkBits); flightData2Struct.blinkBits = (uint)blinkBits; break;
                    case "cmdsMode": Enum.TryParse<CmdsModes>(attribute.Value, out var cmdsMode); flightData2Struct.cmdsMode = (int)cmdsMode; break;
                    case "BupUhfPreset": int.TryParse(attribute.Value, out flightData2Struct.BupUhfPreset); break;
                    case "BupUhfFreq": int.TryParse(attribute.Value, out flightData2Struct.BupUhfFreq); break;
                    case "cabinAlt": float.TryParse(attribute.Value, out flightData2Struct.cabinAlt); break;
                    case "hydPressureA": float.TryParse(attribute.Value, out flightData2Struct.hydPressureA); break;
                    case "hydPressureB": float.TryParse(attribute.Value, out flightData2Struct.hydPressureB); break;
                    case "currentTime": uint.TryParse(attribute.Value, out flightData2Struct.currentTime); break;
                    case "vehicleACD": short.TryParse(attribute.Value, out flightData2Struct.vehicleACD); break;
                    case "VersionNum": int.TryParse(attribute.Value, out flightData2Struct.VersionNum2); break;
                    case "fuelFlow2": float.TryParse(attribute.Value, out flightData2Struct.fuelFlow2); break;
                    case "lefPos":float.TryParse(attribute.Value, out flightData2Struct.lefPos); break;
                    case "tefPos":float.TryParse(attribute.Value, out flightData2Struct.tefPos); break;
                    case "vtolPos":float.TryParse(attribute.Value, out flightData2Struct.vtolPos); break;
                    case "pilotsOnline":byte.TryParse(attribute.Value, out flightData2Struct.pilotsOnline); break;
                    case "bumpIntensity":float.TryParse(attribute.Value, out flightData2Struct.bumpIntensity); break;
                    case "latitude":float.TryParse(attribute.Value, out flightData2Struct.latitude); break;
                    case "longitude":float.TryParse(attribute.Value, out flightData2Struct.longitude); break;
                    case "iffBackupMode1Digit1":byte.TryParse(attribute.Value, out flightData2Struct.iffBackupMode1Digit1); break;
                    case "iffBackupMode1Digit2":byte.TryParse(attribute.Value, out flightData2Struct.iffBackupMode1Digit2); break;
                    case "iffBackupMode3ADigit1": byte.TryParse(attribute.Value, out flightData2Struct.iffBackupMode3ADigit1); break;
                    case "iffBackupMode3ADigit2":byte.TryParse(attribute.Value, out flightData2Struct.iffBackupMode3ADigit2); break;
                    case "instrLight":Enum.TryParse<InstrLight>(attribute.Value, out var instrLight); flightData2Struct.instrLight = (byte)instrLight; break;
                    case "bettyBits":Enum.TryParse<BettyBits>(attribute.Value, out var bettyBits); flightData2Struct.bettyBits = (uint)bettyBits; break;
                    case "miscBits":Enum.TryParse<MiscBits>(attribute.Value, out var miscBits); flightData2Struct.miscBits = (uint)miscBits; break;
                    case "RALT":float.TryParse(attribute.Value, out flightData2Struct.RALT); break;
                    case "bingoFuel":float.TryParse(attribute.Value, out flightData2Struct.bingoFuel); break;
                    case "caraAlow":float.TryParse(attribute.Value, out flightData2Struct.caraAlow); break;
                    case "bullseyeX":float.TryParse(attribute.Value, out flightData2Struct.bullseyeX); break;
                    case "bullseyeY":float.TryParse(attribute.Value, out flightData2Struct.bullseyeY); break;
                    case "BMSVersionMajor":int.TryParse(attribute.Value, out flightData2Struct.BMSVersionMajor); break;
                    case "BMSVersionMinor":int.TryParse(attribute.Value, out flightData2Struct.BMSVersionMinor); break;
                    case "BMSVersionMicro":int.TryParse(attribute.Value, out flightData2Struct.BMSVersionMicro); break;
                    case "BMSBuildNumber":int.TryParse(attribute.Value, out flightData2Struct.BMSBuildNumber); break;
                    case "StringAreaSize":uint.TryParse(attribute.Value, out flightData2Struct.StringAreaSize); break;
                    case "StringAreaTime":uint.TryParse(attribute.Value, out flightData2Struct.StringAreaTime); break;
                    case "DrawingAreaSize":uint.TryParse(attribute.Value, out flightData2Struct.DrawingAreaSize); break;
                    case "turnRate":float.TryParse(attribute.Value, out flightData2Struct.turnRate); break;
                    case "floodConsole":Enum.TryParse<FloodConsole>(attribute.Value, out var floodConsole); flightData2Struct.floodConsole = floodConsole; break;
                    case "magDeviationSystem":float.TryParse(attribute.Value, out flightData2Struct.magDeviationSystem); break;
                    case "magDeviationReal":float.TryParse(attribute.Value, out flightData2Struct.magDeviationReal); break;
                    case "ecmOper":Enum.TryParse<EcmOperStates>(attribute.Value, out var ecmOper); flightData2Struct.ecmOper = ecmOper; break;
                    case "RWRInfo":PopulateArray(flightData2Struct.RwrInfo, attribute.Value); break;
                    case "pilotsCallsign":PopulatePilotsCallsignsLineOfTextArray(flightData2Struct.pilotsCallsign, attribute.Value);break;
                    case "pilotsStatus":PopulateEnumArray<FlyStates>(flightData2Struct.pilotsStatus, attribute.Value); break;
                    case "RTT_size":PopulateArray(flightData2Struct.RTT_size, attribute.Value); break;
                    case "RTT_area":PopulateArray(flightData2Struct.RTT_area, attribute.Value); break;
                    case "ecmBits":PopulateEnumArray<EcmBits>(flightData2Struct.ecmBits, attribute.Value); break;
                    case "RWRjammingStatus":PopulateEnumArray<JammingStates>(flightData2Struct.RWRjammingStatus, attribute.Value); break;
                    default:
                        break;

                }
            }
            return flightData2Struct;
        }

        private static FlightData2 CreateNewEmptyFlightData2Struct()
        {
            var flightData2Struct = new FlightData2();
            flightData2Struct.tacanInfo = new byte[(int)TacanSources.NUMBER_OF_SOURCES];
            flightData2Struct.RwrInfo = new byte[FlightData2.RWRINFO_SIZE];
            flightData2Struct.pilotsCallsign = CreateNewEmptyCallsignLineOfTextStructArray();
            flightData2Struct.pilotsStatus = new byte[FlightData2.MAX_CALLSIGNS];
            flightData2Struct.RTT_size = new ushort[2];
            flightData2Struct.RTT_area = new ushort[(int)RTT_areas.RTT_noOfAreas * 4];
            flightData2Struct.ecmBits = new uint[FlightData2.MAX_ECM_PROGRAMS];
            flightData2Struct.RWRjammingStatus = new JammingStates[FlightData.MAX_RWR_OBJECTS];
            return flightData2Struct;
        }

        private static void PopulatePilotsCallsignsLineOfTextArray(Callsign_LineOfText[] callsignLineOfTextArray, string toParse)
        {
            var callsigns = JsonConvert.DeserializeObject<string[]>(toParse);
            for (var i = 0; i < callsigns.Length && i < callsignLineOfTextArray.Length; i++)
            {
                var thisCallsignBytes = System.Text.Encoding.ASCII.GetBytes(callsigns[i].ToCharArray(), 0, callsigns[i].Length);
                Array.Copy(
                    sourceArray: thisCallsignBytes,
                    destinationArray: callsignLineOfTextArray[i].chars,
                    length: Math.Min(callsigns[i].Length, callsignLineOfTextArray[i].chars.Length)
                );
            }
        }
        private static Callsign_LineOfText[] CreateNewEmptyCallsignLineOfTextStructArray()
        {
            var callsignLinesOfText = new List<Callsign_LineOfText>();
            for (var i = 0; i< FlightData2.MAX_CALLSIGNS; i++)
            {
                var callsignLineOfText = new Callsign_LineOfText();
                callsignLineOfText.chars = new byte[Callsign_LineOfText.CALLSIGN_LEN];
                callsignLinesOfText.Add(callsignLineOfText);
            }
            return callsignLinesOfText.ToArray();
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
        private static void PopulateEnumArray<T>(Array destinationArray, string toParse) where T : struct
        {
            var valuesArray = (string[])JsonConvert.DeserializeObject(toParse, typeof(string[]));
            for (var i= 0; i < Math.Min(destinationArray.Length, valuesArray.Length); i++)
            {
                Enum.TryParse<T>(valuesArray[i], out var parsedVal); 
                destinationArray.SetValue(parsedVal,i);
            }
        }
    }
}
