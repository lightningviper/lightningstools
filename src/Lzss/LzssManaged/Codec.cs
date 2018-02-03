using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Lzss
{
    public static class Codec
    {
        private const int DEFAULT_OUTPUT_BUFFER_SIZE = 10*1024*1024;

        /// <summary>
        ///   Compresses an array of bytes.
        /// </summary>
        /// <param name = "toCompress">an array of bytes containing the data to compress</param>
        /// <returns>an array of bytes containing the compressed data</returns>
        public static byte[] Compress(byte[] toCompress)
        {
            var outputBufferSize = toCompress.Length > DEFAULT_OUTPUT_BUFFER_SIZE
                                       ? toCompress.Length + 1024
                                       : DEFAULT_OUTPUT_BUFFER_SIZE;
            var compressedDataBuffer = new byte[outputBufferSize];

            var compressedDataSize = Compress(toCompress, 0, toCompress.Length, compressedDataBuffer, 0);
            var toReturn = new byte[compressedDataSize];
            Array.Copy(compressedDataBuffer, toReturn, compressedDataSize);
            return toReturn;
        }


        /// <summary>
        ///   Decompresses an array of bytes.
        /// </summary>
        /// <param name = "toDecompress">an array of bytes containing the data to decompress</param>
        /// <returns>an array of bytes containing the decompressed data</returns>
        public static byte[] Decompress(byte[] toDecompress, int uncompressedDataSize)
        {
            var toReturn = new byte[uncompressedDataSize];
            Decompress(toDecompress, 0, toReturn, 0, uncompressedDataSize);
            return toReturn;
        }

        /// <summary>
        ///   Compresses data from one stream into another.
        /// </summary>
        /// <param name = "nonCompressedDataStream">a Stream containing non-compressed data to compress</param>
        /// <param name =  param name = "nonCompressedDataLength">the length of the non-compressed data to compress</param>
        /// <param param name = "outputStream">a Stream to write the compressed data to</param>
        /// <returns>the number of bytes of compressed data written to the output stream</returns>
        public static int Compress(Stream nonCompressedDataStream, int nonCompressedDataLength, Stream outputStream)
        {
            var nonCompressedData = new byte[nonCompressedDataLength];
            nonCompressedDataStream.Read(nonCompressedData, 0, nonCompressedDataLength);
            var compressedData = Compress(nonCompressedData);
            outputStream.Write(compressedData, 0, compressedData.Length);
            outputStream.Flush();
            return compressedData.Length;
        }

        /// <summary>
        ///   Decompresses data from one stream into another.
        /// </summary>
        /// <param name = "compressedDataStream">a Stream containing compressed data to de-compress.</param>
        /// <param name = "compressedDataLength">the length of the data to de-compress.</param>
        /// <param name = "outputStream">a Stream to write the de-compressed data to.</param>
        /// <param name = "uncompressedDataLength">the length of the original uncompressed data (this must already be known)</param>
        public static void Decompress(Stream compressedDataStream, int compressedDataLength, Stream outputStream,
                                      int uncompressedDataLength)
        {
            var compressedData = new byte[compressedDataLength];
            compressedDataStream.Read(compressedData, 0, compressedDataLength);
            var decompressedData = Decompress(compressedData, uncompressedDataLength);
            outputStream.Write(decompressedData, 0, decompressedData.Length);
            outputStream.Flush();
        }

        /// <summary>
        ///   Compresses data within a buffer and writes the results to another buffer.
        /// </summary>
        /// <param name = "nonCompressedDataBuffer">A byte array containing data to compress</param>
        /// <param name = "nonCompressedDataStartingOffset">The zero-based index into <paramref name = "nonCompressedDataBuffer" /> where the data begins</param>
        /// <param name = "nonCompressedDataLength">The length of the data in <paramref name = "nonCompressedDataBuffer" />, starting at <paramref name = "nonCompressedDataStartingOffset" />, that should be compressed. </param>
        /// <param name = "compressedDataBuffer">A byte array (buffer) that will hold the results of data compression (WARNING: this buffer must be at least as large as the uncompressed data, and probably far larger, to avoid buffer overruns, in case the data is uncompressible)</param>
        /// <param name = "compressedDataBufferStartingOffset">The zero-based index into <paramref name = "compressedDataBuffer" /> where the compressed data should be written.</param>
        /// <returns>the length of the compressed data, in bytes</returns>
        public static int Compress(byte[] nonCompressedDataBuffer, int nonCompressedDataStartingOffset,
                                   int nonCompressedDataLength, byte[] compressedDataBuffer,
                                   int compressedDataBufferStartingOffset)
        {
            var gchNonCompressedDataBuffer = GCHandle.Alloc(nonCompressedDataBuffer);
            var gchCompressedDataBuffer = GCHandle.Alloc(compressedDataBuffer);
            var nonCompressedDataBufferStartAddr = Marshal.UnsafeAddrOfPinnedArrayElement(nonCompressedDataBuffer,
                                                                                          nonCompressedDataStartingOffset);
            var compressedDataBufferStartAddr = Marshal.UnsafeAddrOfPinnedArrayElement(compressedDataBuffer,
                                                                                       compressedDataBufferStartingOffset);
            var compressedDataLength = NativeMethods.LZSS_Compress(nonCompressedDataBufferStartAddr,
                                                                   compressedDataBufferStartAddr,
                                                                   nonCompressedDataLength);
            gchNonCompressedDataBuffer.Free();
            gchCompressedDataBuffer.Free();
            return compressedDataLength;
        }

        /// <summary>
        ///   Decompresses data in a buffer, and writes the results to another buffer.
        /// </summary>
        /// <param name = "compressedDataBuffer">A byte array containing data to decompress.</param>
        /// <param name = "compressedDataBufferStartingOffset">The zero-based index into <paramref name = "compressedDatabuffer" /> where the compressed data begins.</param>
        /// <param name = "decompressedDataBuffer">A byte array (buffer) that will hold the results of data decompression.  WARNING: this buffer must be at least as large as the uncompressed data.</param>
        /// <param name = "decompressedDataStartingOffset">The zero=-based index into <paramref name = "decompressedDataBuffer" />, where the decompressed data should be written.</param>
        /// <param name = "decompressedDataLength">The length of the original uncompressed data </param>
        /// <returns>the number of bytes of the compressed data that were processed in order to produce the decompressed data results</returns>
        public static int Decompress(byte[] compressedDataBuffer, int compressedDataBufferStartingOffset,
                                     byte[] decompressedDataBuffer, int decompressedDataStartingOffset,
                                     int decompressedDataLength)
        {
            var gchDecompressedDataBuffer = GCHandle.Alloc(decompressedDataBuffer);
            var gchCompressedDataBuffer = GCHandle.Alloc(compressedDataBuffer);
            var decompressedDataBufferStartAddr = Marshal.UnsafeAddrOfPinnedArrayElement(decompressedDataBuffer,
                                                                                         decompressedDataStartingOffset);
            var compressedDataBufferStartAddr = Marshal.UnsafeAddrOfPinnedArrayElement(compressedDataBuffer,
                                                                                       compressedDataBufferStartingOffset);
            var bytesProcessed = NativeMethods.LZSS_Expand(compressedDataBufferStartAddr,
                                                           decompressedDataBufferStartAddr, decompressedDataLength);
            gchDecompressedDataBuffer.Free();
            gchCompressedDataBuffer.Free();
            return bytesProcessed;
        }
    }
}