using System.Text;
using F4SharedMem;

namespace F4Utils.SimSupport
{
    public interface IDEDAlowReader
    {
        bool CheckDED_ALOW(FlightData fromFalcon, out int newAlow);
    }

    public class DEDAlowReader : IDEDAlowReader
    {
        public bool CheckDED_ALOW(FlightData fromFalcon, out int newAlow)
        {
            if (fromFalcon.DEDLines != null && fromFalcon.Invert != null)
            {
                for (var i = 0; i < fromFalcon.DEDLines.Length; i++)
                {
                    var alowString = fromFalcon.DEDLines[i] ?? "";
                    var alowInverseString = fromFalcon.Invert[i] ?? "";
                    var anyCharsHighlighted = false;
                    if (!alowString.Contains("CARA ALOW")) continue;

                    var newAlowString = "";
                    var builder = new StringBuilder();
                    builder.Append(newAlowString);
                    for (var j = 0; j < alowString.Length; j++)
                    {
                        var someChar = alowString[j];
                        var inverseChar = alowInverseString[j];
                        if (!int.TryParse(new string(someChar, 1), out var _)) continue;

                        if (inverseChar != ' ')
                        {
                            anyCharsHighlighted = true;
                            break;
                        }

                        builder.Append(someChar);
                    }

                    newAlowString = builder.ToString();
                    if (anyCharsHighlighted)
                    {
                        newAlow = -1;
                        return false;
                    }

                    var success = int.TryParse(newAlowString, out newAlow);
                    if (!success) { newAlow = -1; }
                    return success;
                }
            }

            newAlow = -1;
            return false;
        }
    }
}