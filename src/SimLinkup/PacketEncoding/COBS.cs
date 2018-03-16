using System;
using System.Collections.Generic;
using System.Linq;

namespace SimLinkup.PacketEncoding
{
    internal static class COBS
    {

        public static byte[] Encode(byte[] src)
        {
            List<byte[]> dest = new List<byte[]>(MaxEncodedSize(src.Length));
            Encode(src, 0, src.Length, dest);
            dest.TrimExcess();
            return dest.SelectMany(a => a).ToArray();
        }

        private static void Encode(byte[] src, int from, int to, List<byte[]> dest)
        {
            CheckRange(from, to, src);
            int code = 1; 
            int blockStart = -1;

            while (from < to)
            {
                if (src[from] == 0)
                {
                    FinishBlock(code, src, blockStart, dest, from - blockStart);
                    code = 1;
                    blockStart = -1;
                }
                else
                {
                    if (blockStart < 0)
                    {
                        blockStart = from;
                    }
                    code++;
                    if (code == 0xFF)
                    {
                        FinishBlock(code, src, blockStart, dest, from - blockStart + 1);
                        code = 1;
                        blockStart = -1;
                    }
                }
                from++;
            }

            FinishBlock(code, src, blockStart, dest, from - blockStart);
        }

        private static void FinishBlock(int code, byte[] src, int blockStart, List<byte[]> dest, int length)
        {
            byte[] codeByteArray = new byte[1];
            codeByteArray[0] = (byte)code;

            dest.Add(codeByteArray);  
            if (blockStart >= 0)
            {
                byte[] myByteArray = new byte[length];

                for (int i = 0; i < length; i++)
                {
                    myByteArray[i] = src[blockStart + i];
                }
                dest.Add(myByteArray);
            }
        }

        public static int MaxEncodedSize(int size)
        {
            return size + 1 + size / 254;
        }

        public static byte[] decode(byte[] src)
        {
            List<byte[]> dest = new List<byte[]>(src.Length);
            Decode(src, 0, src.Length, dest);
            dest.TrimExcess();
            return dest.SelectMany(a => a).ToArray();
        }

        private static void Decode(byte[] src, int from, int to, List<byte[]> dest) 
        {
            CheckRange(from, to, src);

            while (from < to)
            {
                int code = src[from++] & 0xFF;
                int len = code - 1;

                if (code == 0 || from + len > to) throw new Exception("Corrupt COBS encoded data");

                byte[] myByteArray = new byte[len];

                for (int i = 0; i < len; i++)
                {
                    myByteArray[i] = src[from + i];

                }
                dest.Add(myByteArray);


                from += len;
                if (code < 0xFF && from < to)
                { 
                    byte[] zero = new byte[1];
                    zero[0] = 0x00;
                    dest.Add(zero);
                }
            }
        }

        private static void CheckRange(int from, int to, byte[] arr)
        {
            if (from < 0 || from > to || to > arr.Length)
            {
                throw new Exception("from: " + from + ", to: " + to + ", size: " + arr.Length);
            }
        }
    }
}