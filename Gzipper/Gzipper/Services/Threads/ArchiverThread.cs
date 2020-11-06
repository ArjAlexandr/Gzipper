using System.Threading;
using Gzipper.Models;
using Gzipper.Services.Gzip;

namespace Gzipper.Services.Threads
{
    public class ArchiverThread : ExceptionTrackingThread
    {
        private readonly IArchiver _archiver;
        private readonly ByteBlock _byteBlock;

        public ArchiverThread(IArchiver archiver, ByteBlock byteBlock, CancellationTokenSource tokenSource) : base(tokenSource)
        {
            _archiver = archiver;
            _byteBlock = byteBlock;
        }

        protected override void InternalProcess(CancellationToken cancellationToken)
        {
            _archiver.Process(_byteBlock);
        }
    }
}