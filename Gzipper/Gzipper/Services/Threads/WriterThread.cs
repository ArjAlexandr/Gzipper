using System.Threading;
using Gzipper.Models;
using Gzipper.Services.IO;

namespace Gzipper.Services.Threads
{
    public class WriterThread : ExceptionTrackingThread
    {
        private readonly IFileWriter _writer;
        private readonly ThreadSafeQueue<ProcessingBlock> _blocksQueue;
        private readonly SemaphoreSlim _semaphore;

        public WriterThread(
            IFileWriter writer,
            ThreadSafeQueue<ProcessingBlock> blocksQueue,
            SemaphoreSlim semaphore,
            CancellationTokenSource tokenSource) : base(tokenSource)
        {
            _writer = writer;
            _blocksQueue = blocksQueue;
            _semaphore = semaphore;
        }

        protected override void InternalProcess(CancellationToken cancellationToken)
        {
            while (!_blocksQueue.IsCompleted)
            {
                var processingBlock = _blocksQueue.Dequeue(cancellationToken);

                processingBlock.ProcessingThread.Join();

                _writer.WriteNext(processingBlock.ByteBlock.Bytes);
                
                _semaphore.Release();
            }
        }
    }
}