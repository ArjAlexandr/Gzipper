using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using Gzipper.Services.Threads;

namespace Gzipper
{
    public static class ExceptionTrackingThreadExtension
    {
        public static void JoinAll(this ICollection<ExceptionTrackingThread> threads)
        {
            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
        
        public static void StartAll(this ICollection<ExceptionTrackingThread> threads)
        {
            foreach (var thread in threads)
            {
                thread.Start();
            }
        }
        
        public static void TryThrowFirstException(this ICollection<ExceptionTrackingThread> threads)
        {
            var thread = threads.FirstOrDefault(t => t.Exception != null);
            if (thread != null)
            {
                ExceptionDispatchInfo.Throw(thread.Exception);
            }
        }
    }
}