using F4SharedMem.Headers;
using System.Xml;

namespace F4SharedMemTester
{
    internal class DrawingDataXmlDeserializer
    {
        public static DrawingData Deserialize(XmlElement drawingDataXmlElement)
        {
            DrawingData drawingDataStruct = new DrawingData();
            if (drawingDataXmlElement == null) return drawingDataStruct;
            uint.TryParse(drawingDataXmlElement.GetAttribute("VersionNum"), out drawingDataStruct.VersionNum);
            uint.TryParse(drawingDataXmlElement.GetAttribute("HUD_length"), out drawingDataStruct.HUD_length);
            drawingDataStruct.HUD_commands = drawingDataXmlElement.GetAttribute("HUD_commands");
            uint.TryParse(drawingDataXmlElement.GetAttribute("RWR_length"), out drawingDataStruct.RWR_length);
            drawingDataStruct.RWR_commands = drawingDataXmlElement.GetAttribute("RWR_commands");
            uint.TryParse(drawingDataXmlElement.GetAttribute("HMS_length"), out drawingDataStruct.HMS_length);
            drawingDataStruct.HMS_commands = drawingDataXmlElement.GetAttribute("HMS_commands");
            return drawingDataStruct;
        }
    }
}

