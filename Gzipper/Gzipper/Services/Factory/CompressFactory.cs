using Gzipper.Services.Gzip;
using Gzipper.Services.IO;

namespace Gzipper.Services.Factory
{
    public class CompressFactory : IArchiveFactory
    {
        private readonly ArchiveSettings _settings;

        public CompressFactory(ArchiveSettings settings) =>
            _settings = settings;

        public IArchiver GetArchiver() => 
            new BlockCompressor();

        public IFileReader GetFileReader() =>
            new FileReader(_settings.SourcePath, _settings.BlockSize);

        public IFileWriter GetFileWriter() =>
            new CompressedFileWriter(_settings.DestinationPath);
    }
}