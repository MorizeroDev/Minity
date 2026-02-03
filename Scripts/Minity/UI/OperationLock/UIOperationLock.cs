using System;
using System.Threading;
using System.Threading.Tasks;

namespace Minity.UI.OperationLock
{
    public sealed class UIOperationLock
    {
        private readonly SemaphoreSlim _sem = new(1, 1);
        
        public IDisposable? TryEnter()
        {
            if (!_sem.Wait(0))
            {
                return null;
            }

            return new Releaser(_sem);
        }
        
        public async ValueTask<IAsyncDisposable> TryEnterAsync()
        {
            if (!await _sem.WaitAsync(0))
            {
                return null;
            }

            return new Releaser(_sem);
        }

        private readonly struct Releaser : IDisposable, IAsyncDisposable
        {
            private readonly SemaphoreSlim? _sem;

            internal Releaser(SemaphoreSlim sem)
            {
                _sem = sem;
            }

            public void Dispose()
            {
                _sem?.Release();
            }

            public ValueTask DisposeAsync()
            {
                _sem?.Release();
                return default;
            }
        }
    }
}
