using System;
using System.IO;
using Gzipper.Models;

namespace Gzipper.Services.IO
{
    public class CompressedFileReader : IFileReader
    {
        private const int HeaderLength = 16;
        private const int ExtraFieldPresent = 4;
        
        private readonly FileStream _fileStream;

        private bool _isDisposed;

        public CompressedFileReader(string path)
        {
            _fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        public bool CanRead
        {
            get
            {
                CheckDisposed();
                
                return _fileStream.Position != _fileStream.Length;
            }
        }

        public ByteBlock ReadNext()
        {
            try
            {
                var buffer = new byte[HeaderLength];
                _fileStream.Read(buffer, 0, buffer.Length);

                if (buffer[3] != ExtraFieldPresent)
                {
                    throw new FormatException();
                }

                var extraFieldLength = BitConverter.ToInt16(new ReadOnlySpan<byte>(buffer, 10, 2));
                var blockLength = BitConverter.ToInt32(new ReadOnlySpan<byte>(buffer, 12, extraFieldLength));

                Array.Resize(ref buffer, buffer.Length + blockLength);

                _fileStream.Read(buffer, HeaderLength, blockLength);

                return new ByteBlock {Bytes = buffer};
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Unsupported file format", e);
            }
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