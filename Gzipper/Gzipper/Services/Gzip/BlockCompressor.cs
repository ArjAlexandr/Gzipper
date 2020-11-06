using System.IO;
using System.IO.Compression;
using Gzipper.Models;

namespace Gzipper.Services.Gzip
{
    public class BlockCompressor : IArchiver
    {
        public void Process(ByteBlock byteBlock)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    gzipStream.Write(byteBlock.Bytes, 0, byteBlock.Bytes.Length);
                }
                byteBlock.Bytes = outputStream.ToArray();
            }
        }
    }
}