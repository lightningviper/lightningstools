using System;
using System.Globalization;
using System.IO;

namespace AnalogDevices
{
    public class IhxFile
    {
        public IhxFile(Stream stream)
        {
            var buf = new byte[255];
            var eof = false;

            for (var i = 0; i < IhxData.Length; i++)
                IhxData[i] = -1;

            while (!eof)
            {
                int b;
                do
                {
                    b = stream.ReadByte();
                    if (b < 0)
                    {
                        throw new InvalidDataException("Unexpected end of file");
                    }
                } while (b != (byte) ':');

                var len = ReadHexByte(stream);
                var checksum = len;

                b = ReadHexByte(stream); // address field
                checksum += b;
                var addr = b << 8;
                b = ReadHexByte(stream);
                checksum += b;
                addr |= b;

                b = ReadHexByte(stream); // record type field
                checksum += b;

                for (var i = 0; i < len; i++)
                {
                    // data
                    buf[i] = (byte) ReadHexByte(stream);
                    checksum += buf[i];
                }

                checksum += ReadHexByte(stream); // checksum
                if ((checksum & 0xff) != 0)
                {
                    throw new InvalidDataException("Checksum error");
                }

                switch (b)
                {
                    case 0:
                        for (var i = 0; i < len; i++)
                        {
                            if (IhxData[addr + i] >= 0)
                            {
                                Console.Error.WriteLine("Warning: Memory at position 0x" +
                                                        i.ToString("X8", CultureInfo.InvariantCulture) +
                                                        " overwritten");
                            }
                            IhxData[addr + i] = (short) (buf[i] & 0xFF);
                        }
                        break;
                    case 1:
                        eof = true;
                        break;
                    default:
                        throw new InvalidDataException("Invalid record type: " + b);
                }
            }
        }

        public short[] IhxData { get; } = new short[65536];

        private static int ReadHexDigit(Stream stream)
        {
            var b = stream.ReadByte();
            if (b >= (byte) '0' && b <= (byte) '9')
            {
                return b - (byte) '0';
            }
            if (b >= (byte) 'a' && b <= (byte) 'f')
            {
                return 10 + b - (byte) 'a';
            }
            if (b >= (byte) 'A' && b <= (byte) 'F')
            {
                return 10 + b - (byte) 'A';
            }
            if (b == -1)
            {
                throw new InvalidDataException("Unexpected end of file");
            }
            throw new InvalidDataException("Hex digit expected: " + (char) b);
        }

        private static int ReadHexByte(Stream stream)
        {
            return (ReadHexDigit(stream) << 4) | ReadHexDigit(stream);
        }
    }
}