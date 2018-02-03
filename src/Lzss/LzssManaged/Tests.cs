using System;
using System.IO;

namespace Lzss
{
    public static class Tests
    {

        public static void Main(string[] args)
        {
            
            for (var i = 0; i < 5; i++)
            {
                TestCompressionLowLevelInterface();
                TestCompressionStreamInterface();
            }
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        public static void TestCompressionLowLevelInterface()
        {
            Console.WriteLine("Testing low-level compression interface...");
            //create a 5MB buffer of random data to attempt compression with (it will actually get bigger after "compression", since random data is not compressible
            var nonCompressedDataSize = 500*1024*1024;
            var nonCompressedDataBuffer = new byte[nonCompressedDataSize];
            var rnd = new Random();
            Console.WriteLine("Generating " + nonCompressedDataSize + " bytes of random data.");
            rnd.NextBytes(nonCompressedDataBuffer);

            //create a buffer 2x as big as our uncompressed data, to hold the results of LZSS data compression
            var compressedDataBuffer = new byte[nonCompressedDataBuffer.Length*2];
            //compress the original data
            Console.WriteLine("Compressing generated data...");
            var compressedSize = Codec.Compress(nonCompressedDataBuffer, 0, nonCompressedDataBuffer.Length,
                                                compressedDataBuffer, 0);
            Console.WriteLine("Size after compression: " + compressedSize);

            //create another buffer to hold the results of LSZZ decompression, to compare against our original non-compressed data
            var decompressedDataBuffer = new byte[nonCompressedDataSize];
            //decompress the compressed data
            Console.WriteLine("Decompressing compressed data...");
            var bytesProcessed = Codec.Decompress(compressedDataBuffer, 0, decompressedDataBuffer, 0,
                                                  nonCompressedDataSize);
            Console.WriteLine("Bytes processed in decompression: " + bytesProcessed);

            Console.WriteLine("Comparing decompressed data to original data...");
            //compare our decompressed buffer with the original noncompressed data to ensure it matches
            var everythingMatched = true;
            for (var i = 0; i < nonCompressedDataSize; i++)
            {
                var originalNonCompressedData = nonCompressedDataBuffer[i];
                var decompressedData = decompressedDataBuffer[i];
                if (originalNonCompressedData != decompressedData)
                {
                    everythingMatched = false;
                    Console.WriteLine("Data does not match at offset: " + i);
                    Console.Beep();
                    break;
                }
            }
            if (everythingMatched)
            {
                Console.WriteLine("All data matched after compression and decompression.");
            }
            Console.WriteLine("Finished testing low-level compression interface.");
        }

        public static void TestCompressionStreamInterface()
        {
            Console.WriteLine("Testing stream-based compression interface...");
            //create a 5MB buffer of random data to attempt compression with (it will actually get bigger after "compression", since random data is not compressible
            var nonCompressedDataSize = 500*1024*1024;
            var nonCompressedDataBuffer = new byte[nonCompressedDataSize];
            var rnd = new Random();
            Console.WriteLine("Generating " + nonCompressedDataSize + " bytes of random data.");
            rnd.NextBytes(nonCompressedDataBuffer);
            Stream nonCompressedDataStream = new MemoryStream(nonCompressedDataBuffer, 0, nonCompressedDataSize, false,
                                                              false);

            //compress the data
            Stream compressedDataStream = new MemoryStream();
            var compressedSize = Codec.Compress(nonCompressedDataStream, nonCompressedDataSize, compressedDataStream);
            Console.WriteLine("Size after compression: " + compressedSize);

            //decompress the compressed data
            compressedDataStream.Seek(0, SeekOrigin.Begin);
            Stream decompressedDataStream = new MemoryStream();
            Console.WriteLine("Decompressing compressed data...");
            Codec.Decompress(compressedDataStream, compressedSize, decompressedDataStream, nonCompressedDataSize);
            decompressedDataStream.Seek(0, SeekOrigin.Begin);

            Console.WriteLine("Comparing decompressed data to original data...");
            //compare our decompressed buffer with the original noncompressed data to ensure it matches
            var everythingMatched = true;
            for (var i = 0; i < nonCompressedDataSize; i++)
            {
                var originalNonCompressedData = nonCompressedDataBuffer[i];
                var nextDecompressedByte = decompressedDataStream.ReadByte();
                if (nextDecompressedByte == -1)
                {
                    everythingMatched = false;
                    Console.WriteLine("Premature end of decompressed data stream at offset: " + i);
                    Console.Beep();
                    break;
                }
                var decompressedData = (byte) nextDecompressedByte;
                if (originalNonCompressedData != decompressedData)
                {
                    everythingMatched = false;
                    Console.WriteLine("Data does not match at offset: " + i);
                    Console.Beep();
                    break;
                }
            }
            if (everythingMatched)
            {
                Console.WriteLine("All data matched after compression and decompression.");
            }
            Console.WriteLine("Finished testing stream-based compression interface.");
        }
    }
}