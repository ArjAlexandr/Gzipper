using System;
using System.IO;

namespace Gzipper.Services.IO
{
    public class CompressedFileWriter : IFileWriter
    {
        private const int HeaderLength = 10;
        private const int ExtraFieldPresent = 4;

        private readonly FileStream _fileStream;
        
        private bool _isDisposed;
        
        public CompressedFileWriter(string path)
        {
            _fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
        }
        
        public void WriteNext(byte[] data)
        {
            CheckDisposed();

            using var bufferStream = new MemoryStream();
            var dataLength = data.Length - HeaderLength;
            var dataLengthAsBytes = BitConverter.GetBytes(dataLength);

            bufferStream.Write(data, 0, 3);
            bufferStream.Write(new byte[] {ExtraFieldPresent});
            bufferStream.Write(data, 3, 6);
            bufferStream.Write(new byte[] {4, 0});
            bufferStream.Write(dataLengthAsBytes);
            bufferStream.Write(data, HeaderLength, dataLength);

            bufferStream.Seek(0, SeekOrigin.Begin);

            bufferStream.CopyTo(_fileStream);
        }
        
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            
            _fileStream?.Dispose();
            _isDisposed = true;
        }
        
        private void CheckDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}