using System.Collections.Generic;
using System;
using System.Xml;
using System.Linq;
using F4SharedMem.Headers;
using System.Globalization;

namespace F4SharedMemTester
{
    internal class TestFile
    {
        [Serializable]
        internal class Moment
        {
            public TimeSpan? StartTime { get; set; } = TimeSpan.Zero;
            public BMS4FlightData? FlightData { get; set; }
            public FlightData2? FlightData2 { get; set; }
            public OSBData? OSBData { get; set; }
            public StringData StringData { get; set; }
            public IntellivibeData? IntellivibeData { get; set; }
            public DrawingData DrawingData { get; set; }
            [NonSerialized]
            public XmlElement MomentXmlElement;
        }

        public IList<Moment> Moments { get; set; } = new List<Moment>();

        public static TestFile Load(string filePath)
        {
            var testFile = new TestFile();
            var testFileXmlDoc = new XmlDocument(); 
            testFileXmlDoc.Load(filePath);
            var momentElements = testFileXmlDoc.GetElementsByTagName(nameof(Moment));
            for (var thisMomentElementIndex = 0; thisMomentElementIndex < momentElements.Count; thisMomentElementIndex++)
            {
                var thisMoment = new Moment();
                thisMoment.MomentXmlElement = momentElements[thisMomentElementIndex] as XmlElement;
                var thisMomentStartTimeAttribute = thisMoment.MomentXmlElement.Attributes.GetNamedItem("t") as XmlAttribute;
                var thisMomentStartTime = TimeSpan.MinValue;
                var success = TimeSpan.TryParseExact(thisMomentStartTimeAttribute.Value, @"hh\:mm\:ss\:fff", CultureInfo.InvariantCulture, out thisMomentStartTime);
                if (success)
                {
                    thisMoment.StartTime = thisMomentStartTime;
                }
                testFile.Moments.Add(thisMoment);
            }

            Moment previousMoment = null;
            foreach (var thisMoment in testFile.Moments.OrderBy(x=>x.StartTime))
            { 
                var thisMomentFlightDataXmlNodes = thisMoment.MomentXmlElement.SelectNodes("FlightData");
                if (thisMomentFlightDataXmlNodes.Count > 0)
                {
                    var thisMomentFlightDataXmlElement = thisMomentFlightDataXmlNodes[0] as XmlElement;
                    thisMoment.FlightData = BMS4FlightDataXmlDeserializer.Deserialize(thisMomentFlightDataXmlElement, previousMoment?.FlightData);
                }

                var thisMomentFlightData2XmlNodes = thisMoment.MomentXmlElement.SelectNodes("FlightData2");
                if (thisMomentFlightData2XmlNodes.Count > 0)
                {
                    var thisMomentFlightData2XmlElement = thisMomentFlightData2XmlNodes[0] as XmlElement;
                    thisMoment.FlightData2 = FlightData2XmlDeserializer.Deserialize(thisMomentFlightData2XmlElement, previousMoment?.FlightData2);
                }

                var thisMomentOsbDataXmlNodes = thisMoment.MomentXmlElement.SelectNodes("OSBData");
                if (thisMomentOsbDataXmlNodes.Count > 0)
                {
                    var thisMomentOsbDataXmlElement = thisMomentOsbDataXmlNodes[0] as XmlElement;
                    thisMoment.OSBData = OsbDataXmlDeserializer.Deserialize(thisMomentOsbDataXmlElement, previousMoment?.OSBData);
                }

                var thisMomentStringDataXmlNodes = thisMoment.MomentXmlElement.SelectNodes("StringData");
                if (thisMomentStringDataXmlNodes.Count > 0)
                {
                    var thisMomentStringDataXmlElement = thisMomentStringDataXmlNodes[0] as XmlElement;
                    thisMoment.StringData = StringDataXmlDeserializer.Deserialize(thisMomentStringDataXmlElement, previousMoment?.StringData);
                }

                var thisMomentIntellivibeDataXmlNodes = thisMoment.MomentXmlElement.SelectNodes("IntellivibeData");
                if (thisMomentIntellivibeDataXmlNodes.Count > 0)
                {
                    var thisMomentIntellivibeDataXmlElement = thisMomentIntellivibeDataXmlNodes[0] as XmlElement;
                    thisMoment.IntellivibeData = IntellivibeDataXmlDeserializer.Deserialize(thisMomentIntellivibeDataXmlElement, previousMoment?.IntellivibeData);
                }

                var thisMomentDrawingDataXmlNodes = thisMoment.MomentXmlElement.SelectNodes("DrawingData");
                if (thisMomentDrawingDataXmlNodes.Count > 0)
                {
                    var thisMomentDrawingDataXmlElement = thisMomentDrawingDataXmlNodes[0] as XmlElement;
                    thisMoment.DrawingData = DrawingDataXmlDeserializer.Deserialize(thisMomentDrawingDataXmlElement, previousMoment?.DrawingData);
                }
                previousMoment = thisMoment;
            }

            return testFile;
        }
    }
}
