using System;
using System.IO;
using Gzipper.Models;

namespace Gzipper.Services.IO
{
    public class FileReader : IFileReader
    {
        private readonly int _blockSize;
        private readonly FileStream _fileStream;
        
        private bool _isDisposed;

        public FileReader(string path, int blockSize)
        {
            _blockSize = blockSize;
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
            CheckDisposed();

            var bufferLength = GetBufferSize();
            var buffer = new byte[bufferLength];

            _fileStream.Read(buffer, 0, buffer.Length);

            return new ByteBlock {Bytes = buffer};
        }

        private int GetBufferSize()
        {
            return _fileStream.Length - _fileStream.Position < _blockSize ?
                (int) (_fileStream.Length - _fileStream.Position) :
                _blockSize;
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