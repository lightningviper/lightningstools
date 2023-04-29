using System.Collections.Generic;
using System;
using System.Xml;
using F4SharedMem.Headers;

namespace F4SharedMemTester
{
    internal class TestFile
    {
        internal class Moment
        {
            public TimeSpan? StartTime { get; set; } = TimeSpan.Zero;
            public BMS4FlightData? FlightData { get; set; }
            public FlightData2? FlightData2 { get; set; }
            public OSBData? OSBData { get; set; }
            public StringData StringData { get; set; }
            public IntellivibeData? IntellivibeData { get; set; }
            public DrawingData DrawingData { get; set; }
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
                var success = TimeSpan.TryParse(thisMomentStartTimeAttribute.Value, out thisMomentStartTime);
                if (success)
                {
                    thisMoment.StartTime = thisMomentStartTime;
                }
                var thisMomentFlightDataXmlNodes = thisMoment.MomentXmlElement.SelectNodes("FlightData");
                if (thisMomentFlightDataXmlNodes.Count > 0)
                {
                    var thisMomentFlightDataXmlElement = thisMomentFlightDataXmlNodes[0] as XmlElement;
                    thisMoment.FlightData = BMS4FlightDataXmlDeserializer.Deserialize(thisMomentFlightDataXmlElement);
                }

                var thisMomentFlightData2XmlNodes = thisMoment.MomentXmlElement.SelectNodes("FlightData2");
                if (thisMomentFlightData2XmlNodes.Count > 0)
                {
                    var thisMomentFlightData2XmlElement = thisMomentFlightData2XmlNodes[0] as XmlElement;
                    thisMoment.FlightData2 = FlightData2XmlDeserializer.Deserialize(thisMomentFlightData2XmlElement);
                }

                var thisMomentOsbDataXmlNodes = thisMoment.MomentXmlElement.SelectNodes("OSBData");
                if (thisMomentOsbDataXmlNodes.Count > 0)
                {
                    var thisMomentOsbDataXmlElement = thisMomentOsbDataXmlNodes[0] as XmlElement;
                    thisMoment.OSBData = OsbDataXmlDeserializer.Deserialize(thisMomentOsbDataXmlElement);
                }

                var thisMomentStringDataXmlNodes = thisMoment.MomentXmlElement.SelectNodes("StringData");
                if (thisMomentStringDataXmlNodes.Count > 0)
                {
                    var thisMomentStringDataXmlElement = thisMomentStringDataXmlNodes[0] as XmlElement;
                    thisMoment.StringData = StringDataXmlDeserializer.Deserialize(thisMomentStringDataXmlElement);
                }

                var thisMomentIntellivibeDataXmlNodes = thisMoment.MomentXmlElement.SelectNodes("IntellivibeData");
                if (thisMomentIntellivibeDataXmlNodes.Count > 0)
                {
                    var thisMomentIntellivibeDataXmlElement = thisMomentIntellivibeDataXmlNodes[0] as XmlElement;
                    thisMoment.IntellivibeData = IntellivibeDataXmlDeserializer.Deserialize(thisMomentIntellivibeDataXmlElement);
                }

                var thisMomentDrawingDataXmlNodes = thisMoment.MomentXmlElement.SelectNodes("DrawingData");
                if (thisMomentDrawingDataXmlNodes.Count > 0)
                {
                    var thisMomentDrawingDataXmlElement = thisMomentDrawingDataXmlNodes[0] as XmlElement;
                    thisMoment.DrawingData = DrawingDataXmlDeserializer.Deserialize(thisMomentDrawingDataXmlElement);
                }

                testFile.Moments.Add(thisMoment);
            }

            return testFile;
        }
    }
}
