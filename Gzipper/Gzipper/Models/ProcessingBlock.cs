using Gzipper.Services.Threads;

namespace Gzipper.Models
{
    public class ProcessingBlock
    {
        public ProcessingBlock(ArchiverThread processingThread, ByteBlock byteBlock)
        {
            ProcessingThread = processingThread;
            ByteBlock = byteBlock;
        }

        public ArchiverThread ProcessingThread { get; }
        public ByteBlock ByteBlock { get; }
    }
}