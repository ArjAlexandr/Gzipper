using System;
using System.Collections.Generic;

namespace Gzipper.Services.Factory
{
    public static class SettingsProvider
    {
        private const int DefBlockSize = 1024 * 1024;

        private static readonly Dictionary<string, bool> OperationValues = new Dictionary<string, bool>
        {
            {"compress", true},
            {"decompress", false}
        };
        
        public static ArchiveSettings GetSettings(string[] args)
        {
            ValidateArguments(args);

            return new ArchiveSettings(OperationValues[args[0]], args[1], args[2], DefBlockSize, Environment.ProcessorCount);
        }
        
        private static void ValidateArguments(string[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentException("compress/decompress [full path to source file] [full path to destination file]");
            }

            if (!OperationValues.ContainsKey(args[0]))
            {
                throw new ArgumentException("first argument should be: compress/decompress");
            }
            if (string.IsNullOrWhiteSpace(args[1]))
            {
                throw new ArgumentException("second argument should be a path to source file");
            }
            if (string.IsNullOrWhiteSpace(args[2]))
            {
                throw new ArgumentException("third argument should be a path to destination file");
            }
        }
    }
}