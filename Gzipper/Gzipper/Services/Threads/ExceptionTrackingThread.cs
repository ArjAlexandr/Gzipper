using System;
using System.Threading;

namespace Gzipper.Services.Threads
{
    public abstract class ExceptionTrackingThread
    {
        private readonly CancellationTokenSource _tokenSource;
        private readonly Thread _thread;

        protected ExceptionTrackingThread(CancellationTokenSource tokenSource)
        {
            _thread = new Thread(Process);
            _tokenSource = tokenSource;
        }

        public Exception Exception { get; private set; }
        
        public void Start()
        {
            _thread.Start();
        }

        public void Join()
        {
            _thread.Join();
        }

        private void Process()
        {
            try
            {
                InternalProcess(_tokenSource.Token);
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                _tokenSource.Cancel();
                Exception = e;
            }
        }
        
        protected abstract void InternalProcess(CancellationToken cancellationToken);
    }
}