using System;
using System.Diagnostics;
using Gzipper.Services;
using Gzipper.Services.Factory;

namespace Gzipper
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var archiveSettings = SettingsProvider.GetSettings(args);
                var gzipManager = new GzipManager(archiveSettings);

                Console.WriteLine("Processing started");
                
                var stopwatch = Stopwatch.StartNew();
                gzipManager.Process();
                stopwatch.Stop();

                Console.WriteLine($"Processed in: {stopwatch.Elapsed.ToString()}");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Console.WriteLine($"Exception occured: {e.Message}");
                return 1;
            }
            
            return 0;
        }
    }
}