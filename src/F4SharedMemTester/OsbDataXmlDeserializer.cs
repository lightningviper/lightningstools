using F4SharedMem.Headers;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Xml;

namespace F4SharedMemTester
{
    internal class OsbDataXmlDeserializer
    {
        public static OSBData Deserialize(XmlElement osbDataXmlElement)
        {
            OSBData osbDataStruct = CreateNewEmptyOsbDataStruct();
            if (osbDataXmlElement == null) return osbDataStruct;

            var leftMfdXmlElements = osbDataXmlElement.SelectNodes("leftMFD");
            if (leftMfdXmlElements.Count > 0)
            {
                var leftMfdXmlElement = leftMfdXmlElements[0];
                var leftMfdOsbLabelChildElements = leftMfdXmlElement.SelectNodes("OSBLabel");
                for (var i = 0; i < leftMfdOsbLabelChildElements.Count && i<osbDataStruct.leftMFD.Length; i++)
                {
                    var thisOsbLabelXmlElement = leftMfdOsbLabelChildElements[i];
                    var line1Attribute = thisOsbLabelXmlElement.Attributes["Line1"];
                    if (line1Attribute !=null)
                    {
                        PopulateOSBLabelLineBytes(osbDataStruct.leftMFD[i].Line1, line1Attribute.Value);

                    }
                    var line2Attribute = thisOsbLabelXmlElement.Attributes["Line2"];
                    if (line2Attribute != null)
                    {
                        PopulateOSBLabelLineBytes(osbDataStruct.leftMFD[i].Line2, line2Attribute.Value);
                    }
                    var invertedAttribute = thisOsbLabelXmlElement.Attributes["Inverted"];
                    if (invertedAttribute != null)
                    {
                        bool.TryParse(invertedAttribute.Value, out osbDataStruct.leftMFD[i].Inverted); 
                    }

                }
            }

            var rightMfdXmlElements = osbDataXmlElement.SelectNodes("rightMFD");
            if (rightMfdXmlElements.Count > 0)
            {
                var rightMfdXmlElement = rightMfdXmlElements[0];
                var rightMfdOsbLabelChildElements = rightMfdXmlElement.SelectNodes("OSBLabel");
                for (var i = 0; i < rightMfdOsbLabelChildElements.Count && i < osbDataStruct.rightMFD.Length; i++)
                {
                    var thisOsbLabelXmlElement = rightMfdOsbLabelChildElements[i];
                    var line1Attribute = thisOsbLabelXmlElement.Attributes["Line1"];
                    if (line1Attribute != null)
                    {
                        PopulateOSBLabelLineBytes(osbDataStruct.rightMFD[i].Line1, line1Attribute.Value);

                    }
                    var line2Attribute = thisOsbLabelXmlElement.Attributes["Line2"];
                    if (line2Attribute != null)
                    {
                        PopulateOSBLabelLineBytes(osbDataStruct.rightMFD[i].Line2, line2Attribute.Value);
                    }
                    var invertedAttribute = thisOsbLabelXmlElement.Attributes["Inverted"];
                    if (invertedAttribute != null)
                    {
                        bool.TryParse(invertedAttribute.Value, out osbDataStruct.rightMFD[i].Inverted);
                    }
                }
            }

            return osbDataStruct;
        }

        private static OSBData CreateNewEmptyOsbDataStruct()
        {
            var osbDataStruct = new OSBData();
            osbDataStruct.leftMFD = new OSBLabel[20];
            osbDataStruct.rightMFD = new OSBLabel[20];
            for (var i = 0; i < 20; i++)
            {
                osbDataStruct.leftMFD[i] = new OSBLabel() { Inverted = false, Line1 = new byte[8], Line2 = new byte[8] };
                osbDataStruct.rightMFD[i] = new OSBLabel() { Inverted = false, Line1 = new byte[8], Line2 = new byte[8] };
            }
            return osbDataStruct;
        }


        private static void PopulateOSBLabelLineBytes(byte[] osbLabelLine, string toParse)
        {
            var thisLineChars = JsonConvert.DeserializeObject<char[]>(toParse);
            var thisLineBytes = Encoding.ASCII.GetBytes(thisLineChars);
            Array.Copy(
                sourceArray: thisLineBytes,
                destinationArray: osbLabelLine,
                length: Math.Min(thisLineBytes.Length, osbLabelLine.Length)
            );
        }
    }
}
