using System.Threading;
using Gzipper.Models;
using Gzipper.Services.Factory;
using Gzipper.Services.Threads;

namespace Gzipper.Services
{
    public class GzipManager
    {
        private readonly ArchiveSettings _settings;
        private readonly IArchiveFactory _archiveFactory;

        public GzipManager(ArchiveSettings settings)
        {
            _settings = settings;

            if (settings.IsCompressionOperation)
                _archiveFactory = new CompressFactory(settings);
            else
                _archiveFactory = new DecompressFactory(settings);
        }
        
        public void Process()
        {
            using var writer = _archiveFactory.GetFileWriter();
            using var reader = _archiveFactory.GetFileReader();
            using var blocksQueue = new ThreadSafeQueue<ProcessingBlock>();
            using var cancellationTokenSource = new CancellationTokenSource();
            using var semaphore = new SemaphoreSlim(_settings.ProcessorCount, _settings.ProcessorCount);

            var archiver = _archiveFactory.GetArchiver();
            
            var threads = new ExceptionTrackingThread[]
            {
                new ReaderThread(archiver, reader, blocksQueue, semaphore, cancellationTokenSource),
                new WriterThread(writer, blocksQueue, semaphore, cancellationTokenSource)
            };

            threads.StartAll();
            threads.JoinAll();
            threads.TryThrowFirstException();
        }
    }
}