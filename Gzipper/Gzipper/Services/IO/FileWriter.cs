using System;
using System.IO;

namespace Gzipper.Services.IO
{
    public class FileWriter : IFileWriter
    {
        private readonly FileStream _fileStream;

        private bool _isDisposed;
        
        public FileWriter(string path)
        {
            _fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
        }

        public void WriteNext(byte[] data)
        {
            CheckDisposed();
            
            _fileStream.Write(data, 0, data.Length);
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