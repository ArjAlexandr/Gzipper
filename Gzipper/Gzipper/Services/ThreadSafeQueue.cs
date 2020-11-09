using System;
using System.Collections.Generic;
using System.Threading;

namespace Gzipper.Services
{
    public class ThreadSafeQueue<T> : IDisposable
    {
        private readonly Queue<T> _queue = new Queue<T>();
        private readonly SemaphoreSlim _dequeueSemaphore = new SemaphoreSlim(0);
        private readonly CancellationTokenSource _dequeueCancellationTokenSource = new CancellationTokenSource();
        private readonly object _blockObject = new object();

        private bool _isDisposed;
        private const string ErrorMessage = "Enqueuing to the queue is completed";
        
        public bool IsCompleted
        {
            get
            {
                CheckDisposed();

                return _dequeueSemaphore.CurrentCount == 0 && _dequeueCancellationTokenSource.IsCancellationRequested;
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
                _dequeueSemaphore.Release();
            }
        }

        public T Dequeue(CancellationToken cancellationToken)
        {
            CheckDisposed();

            var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _dequeueCancellationTokenSource.Token).Token;

            if (IsCompleted)
            {
                throw new InvalidOperationException(ErrorMessage);
            }

            try
            {
                var waitSuccessful = _dequeueSemaphore.Wait(0);
                if (!waitSuccessful)
                {
                    _dequeueSemaphore.Wait(linkedCancellationToken);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
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

            _dequeueCancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _dequeueSemaphore.Dispose();
            _dequeueCancellationTokenSource.Dispose();

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