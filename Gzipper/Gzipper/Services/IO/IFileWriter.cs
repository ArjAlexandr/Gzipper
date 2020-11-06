using System;

namespace Gzipper.Services.IO
{
    public interface IFileWriter : IDisposable
    {
        void WriteNext(byte[] data);
    }
}