using Gzipper.Services.Gzip;
using Gzipper.Services.IO;

namespace Gzipper.Services.Factory
{
    public interface IArchiveFactory
    {
        IArchiver GetArchiver();
        IFileReader GetFileReader();
        IFileWriter GetFileWriter();
    }
}