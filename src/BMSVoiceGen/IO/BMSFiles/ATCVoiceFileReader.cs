using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace BMSVoiceGen.IO.BMSFiles
{
    internal static class ATCVoiceFileReader
    {
        internal static IEnumerable<(ushort FragNum, string[] SpeakerText)> ReadBMSVoiceStringCSVFile(string filePath)
        {
            var toReturn = new List<(ushort FragNum, string[] SpeakerText)> ();
            ushort lineNum = 0;
            using (var stream = File.OpenRead(filePath))
            using (var parser = new TextFieldParser(stream))
            {
                parser.SetDelimiters( new[] { "," });
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TrimWhiteSpace = true;
                while (!parser.EndOfData)
                {
                    lineNum++;
                    var fields= parser.ReadFields();
                    if (lineNum == 1)
                    {
                        continue;
                    }
                    ushort.TryParse(fields[0], out ushort fragNum);
                    ushort.TryParse(fields[1], out ushort numVoices);
                    var textStrings = fields.Skip(2).Take(14).ToArray();
                    toReturn.Add((FragNum: fragNum, SpeakerText: textStrings));
                }
            }
            return toReturn;
        }
    }
}
