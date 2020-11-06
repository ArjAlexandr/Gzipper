namespace Gzipper.Services.Factory
{
    public class ArchiveSettings
    {
        public ArchiveSettings(bool isCompressionOperation, string sourcePath, string destinationPath, int blockSize, int processorCount)
        {
            IsCompressionOperation = isCompressionOperation;
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            BlockSize = blockSize;
            ProcessorCount = processorCount;
        }

        public bool IsCompressionOperation { get; }
        public string SourcePath { get; }
        public string DestinationPath { get; }
        public int BlockSize { get; }
        public int ProcessorCount { get; }
    }
}