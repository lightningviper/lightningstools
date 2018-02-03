using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phcc
{
    internal static class CharTo7SegConverter
    {
        /// <summary>
        ///   Produces a 7-segment bitmask with the appropriate bits set, to represent a specific Latin character
        /// </summary>
        /// <param name = "charToConvert">a Latin character to produce a seven-segment display bitmask from</param>
        /// <returns>a byte whose bits are set appropriately for sending to a seven-segment display</returns>
        internal static byte ConvertCharTo7Seg(char charToConvert)
        {
            switch (charToConvert)
            {
                case '0':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF);
                case '1':
                    return (byte)(SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC);
                case '2':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentG |
                         SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentD);
                case '3':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentG);
                case '4':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG | SevenSegmentBits.SegmentB |
                         SevenSegmentBits.SegmentC);
                case '5':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG |
                         SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD);
                case '6':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentD |
                         SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentG);
                case '7':
                    return (byte)(SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC);
                case '8':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF |
                         SevenSegmentBits.SegmentG);
                case '9':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case '.':
                    return (byte)(SevenSegmentBits.SegmentDP);
                case '-':
                    return (byte)(SevenSegmentBits.SegmentG);
                case '+':
                    return (byte)(SevenSegmentBits.SegmentG | SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentE);
                case '!':
                    return (byte)(SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentDP);
                case '@':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentG);
                case '#':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentE |
                         SevenSegmentBits.SegmentF);
                case '$':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case '%':
                    return (byte)(SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentG);
                case '^':
                    return (byte)(SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentF);
                case '&':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF |
                         SevenSegmentBits.SegmentG);
                case '*':
                    return (byte)(SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case '(':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE |
                         SevenSegmentBits.SegmentF);
                case ')':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentD);
                case '_':
                    return (byte)(SevenSegmentBits.SegmentD);
                case '=':
                    return (byte)(SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentG);
                case '[':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE |
                         SevenSegmentBits.SegmentF);
                case ']':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentD);
                case '{':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE |
                         SevenSegmentBits.SegmentF);
                case '}':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentD);
                case '|':
                    return (byte)(SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF);
                case '\\':
                    return (byte)(SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case ':':
                    return (byte)(SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF);
                case ';':
                    return (byte)(SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentD);
                case '\'':
                    return (byte)(SevenSegmentBits.SegmentB);
                case '"':
                    return (byte)(SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentF);
                case '<':
                    return (byte)(SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentG);
                case '>':
                    return (byte)(SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentG);
                case ',':
                    return (byte)(SevenSegmentBits.SegmentE);
                case '/':
                    return (byte)(SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentG);
                case '?':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentE |
                         SevenSegmentBits.SegmentG | SevenSegmentBits.SegmentDP);
                case '`':
                    return (byte)(SevenSegmentBits.SegmentF);
                case '~':
                    return (byte)(SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentG);

                case 'A':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'a':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentG);
                case 'B':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF |
                         SevenSegmentBits.SegmentG);
                case 'b':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'C':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE |
                         SevenSegmentBits.SegmentF);
                case 'c':
                    return (byte)(SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentG);
                case 'D':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF);
                case 'd':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD |
                         SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentG);
                case 'E':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'e':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentD |
                         SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'F':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF |
                         SevenSegmentBits.SegmentG);
                case 'f':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF |
                         SevenSegmentBits.SegmentG);
                case 'G':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD |
                         SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF);
                case 'g':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'H':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentE |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'h':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF |
                         SevenSegmentBits.SegmentG);
                case 'I':
                    return (byte)(SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF);
                case 'i':
                    return (byte)(SevenSegmentBits.SegmentC);
                case 'J':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD |
                         SevenSegmentBits.SegmentE);
                case 'j':
                    return (byte)(SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD);
                case 'K':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'k':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'L':
                    return (byte)(SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF);
                case 'l':
                    return (byte)(SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE);
                case 'M':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentD |
                         SevenSegmentBits.SegmentF);
                case 'm':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentD |
                         SevenSegmentBits.SegmentF);
                case 'N':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF);
                case 'n':
                    return (byte)(SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentG);
                case 'O':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF);
                case 'o':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE |
                         SevenSegmentBits.SegmentG);
                case 'P':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentE |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'p':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentE |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'Q':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'q':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'R':
                    return (byte)(SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF);
                case 'r':
                    return (byte)(SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentG);
                case 'S':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 's':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'T':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF |
                         SevenSegmentBits.SegmentG);
                case 't':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF |
                         SevenSegmentBits.SegmentG);
                case 'U':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD |
                         SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentF);
                case 'u':
                    return (byte)(SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentE);
                case 'V':
                    return (byte)(SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentD | SevenSegmentBits.SegmentF);
                case 'v':
                    return (byte)(SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'W':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD |
                         SevenSegmentBits.SegmentE);
                case 'w':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentA | SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD |
                         SevenSegmentBits.SegmentE);
                case 'X':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentE |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'x':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentE |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'Y':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'y':
                    return
                        (byte)
                        (SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentC | SevenSegmentBits.SegmentD |
                         SevenSegmentBits.SegmentF | SevenSegmentBits.SegmentG);
                case 'Z':
                    return (byte)(SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentG);
                case 'z':
                    return (byte)(SevenSegmentBits.SegmentB | SevenSegmentBits.SegmentE | SevenSegmentBits.SegmentG);
                default:
                    return 0x00;
            }

        }
    }
}
