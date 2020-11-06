using Gzipper.Services.Gzip;
using Gzipper.Services.IO;

namespace Gzipper.Services.Factory
{
    public class DecompressFactory : IArchiveFactory
    {
        private readonly ArchiveSettings _settings;

        public DecompressFactory(ArchiveSettings settings) => 
            _settings = settings;

        public IArchiver GetArchiver() => 
            new BlockDecompressor();

        public IFileReader GetFileReader() => 
            new CompressedFileReader(_settings.SourcePath);

        public IFileWriter GetFileWriter() => 
            new FileWriter(_settings.DestinationPath);
    }
}