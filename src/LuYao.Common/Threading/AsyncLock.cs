using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LuYao.Threading;

public static class AsyncLock
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks =
        new ConcurrentDictionary<string, SemaphoreSlim>();

    public static async Task<IDisposable> LockAsync(string key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        var semaphore = _locks.GetOrAdd(key, static _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();
        return new ReleaseSemaphoreOnDispose(semaphore);
    }

    private class ReleaseSemaphoreOnDispose : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;

        public ReleaseSemaphoreOnDispose(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        public void Dispose()
        {
            _semaphore.Release();
        }
    }
}
