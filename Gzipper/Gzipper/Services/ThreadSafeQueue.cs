using System;
using System.Collections.Generic;
using System.Threading;

namespace Gzipper.Services
{
    public class ThreadSafeQueue<T> : IDisposable
    {
        private readonly Queue<T> _queue = new Queue<T>();
        private readonly SemaphoreSlim _takeSemaphore = new SemaphoreSlim(0);
        private readonly CancellationTokenSource _takeCancellationTokenSource = new CancellationTokenSource();
        private readonly object _blockObject = new object();

        private bool _isDisposed;
        private const string ErrorMessage = "Enqueuing to the queue is completed";
        
        public bool IsCompleted
        {
            get
            {
                CheckDisposed();

                return _takeSemaphore.CurrentCount == 0 && _takeCancellationTokenSource.IsCancellationRequested;
            }
        }

        public void Enqueue(T item)
        {
            CheckDisposed();

            Monitor.Enter(_blockObject);

            try
            {
                _queue.Enqueue(item);
            }
            finally
            {
                Monitor.Exit(_blockObject);
                _takeSemaphore.Release();
            }
        }

        public T Dequeue(CancellationToken cancellationToken)
        {
            CheckDisposed();

            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _takeCancellationTokenSource.Token).Token;

            if (IsCompleted)
            {
                throw new InvalidOperationException(ErrorMessage);
            }

            try
            {
                var waitSuccessful = _takeSemaphore.Wait(0);
                if (!waitSuccessful)
                {
                    _takeSemaphore.Wait(cancellationTokenSource);
                }
            }
            catch (OperationCanceledException)
            {
                throw new InvalidOperationException(ErrorMessage);
            }

            Monitor.Enter(_blockObject);

            try
            {
               return _queue.Dequeue();
            }
            finally
            {
                Monitor.Exit(_blockObject);
            }
        }

        public void CompleteAdding()
        {
            CheckDisposed();

            _takeCancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _takeSemaphore.Dispose();
            _takeCancellationTokenSource.Dispose();

            _isDisposed = true;
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}