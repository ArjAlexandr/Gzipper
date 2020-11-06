using Gzipper.Models;

namespace Gzipper.Services.Gzip
{
    public interface IArchiver
    {
        void Process(ByteBlock data);
    }
}