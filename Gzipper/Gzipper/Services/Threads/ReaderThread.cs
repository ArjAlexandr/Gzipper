using System.Threading;
using Gzipper.Models;
using Gzipper.Services.Gzip;
using Gzipper.Services.IO;

namespace Gzipper.Services.Threads
{
    public class ReaderThread : ExceptionTrackingThread
    {
        private readonly IArchiver _archiver;
        private readonly IFileReader _reader;
        private readonly ThreadSafeQueue<ProcessingBlock> _blocksQueue;
        private readonly SemaphoreSlim _semaphore;
        private readonly CancellationTokenSource _tokenSource;

        public ReaderThread(
            IArchiver archiver,
            IFileReader reader,
            ThreadSafeQueue<ProcessingBlock> blocksQueue,
            SemaphoreSlim semaphore,
            CancellationTokenSource tokenSource) : base(tokenSource)
        {
            _archiver = archiver;
            _reader = reader;
            _blocksQueue = blocksQueue;
            _semaphore = semaphore;
            _tokenSource = tokenSource;
        }

        protected override void InternalProcess(CancellationToken cancellationToken)
        {
            while (_reader.CanRead)
            {
                _semaphore.Wait(cancellationToken);

                var byteBlock = _reader.ReadNext();

                var archiverThread = new ArchiverThread(_archiver, byteBlock, _tokenSource);
                archiverThread.Start();
                
                var processingBlock = new ProcessingBlock(archiverThread, byteBlock);

                _blocksQueue.Enqueue(processingBlock);
            }
            _blocksQueue.CompleteAdding();
        }
    }
}