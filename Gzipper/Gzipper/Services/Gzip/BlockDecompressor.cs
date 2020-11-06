using System;
using System.IO;
using System.IO.Compression;
using Gzipper.Models;

namespace Gzipper.Services.Gzip
{
    public class BlockDecompressor : IArchiver
    {
        public void Process(ByteBlock byteBlock)
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(byteBlock.Bytes, 0, byteBlock.Bytes.Length);
                memoryStream.Position = 0;

                using (var decompressStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    var blockLength = BitConverter.ToInt32(byteBlock.Bytes, byteBlock.Bytes.Length - 4);
                    var buffer = new byte[blockLength];
                    decompressStream.Read(buffer, 0, buffer.Length);
                    byteBlock.Bytes = buffer;
                }
            }
        }
    }
}