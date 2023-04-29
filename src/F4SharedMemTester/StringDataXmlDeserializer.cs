using F4SharedMem.Headers;
using System;
using System.Collections;
using System.Text;
using System.Xml;

namespace F4SharedMemTester
{
    internal class StringDataXmlDeserializer
    {
        public static StringData Deserialize(XmlElement stringDataXmlElement)
        {
            StringData stringDataStruct = new StringData();
            if (stringDataXmlElement == null) return stringDataStruct;
            uint.TryParse(stringDataXmlElement.GetAttribute("VersionNum"), out stringDataStruct.VersionNum);
            uint.TryParse(stringDataXmlElement.GetAttribute("NoOfStrings"), out stringDataStruct.NoOfStrings);
            uint.TryParse(stringDataXmlElement.GetAttribute("dataSize"), out stringDataStruct.dataSize);
            var dataElementNodes = stringDataXmlElement.GetElementsByTagName("data");
            if (dataElementNodes.Count >0)
            {
                var dataElement = dataElementNodes[0] as XmlElement;
                var stringStructElements = dataElement.GetElementsByTagName("StringStruct");
                for (var i = 0; i< stringStructElements.Count; i++)
                {
                    var thisStringStructElement = stringStructElements[i] as XmlElement;
                    var thisStringStruct = new StringStruct();

                    Enum.TryParse(thisStringStructElement.GetAttribute("strId"), out StringIdentifier stringIdentifier); thisStringStruct.strId = (uint)stringIdentifier;
                    uint.TryParse(thisStringStructElement.GetAttribute("strLength"), out thisStringStruct.strLength);
                    thisStringStruct.strData = Encoding.Default.GetBytes(thisStringStructElement.GetAttribute("strData"));
                    (stringDataStruct.data as IList).Add(thisStringStruct);
                }
            }

            return stringDataStruct;
        }

    }
}

