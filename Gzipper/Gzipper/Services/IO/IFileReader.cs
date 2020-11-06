using System;
using Gzipper.Models;

namespace Gzipper.Services.IO
{
    public interface IFileReader : IDisposable
    {
        bool CanRead { get; }
        ByteBlock ReadNext();
    }
}