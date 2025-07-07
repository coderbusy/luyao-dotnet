using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LuYao.Threading;

/// <summary>
/// 提供基于字符串键的异步锁定功能
/// </summary>
public static class AsyncLock
{
    /// <summary>
    /// 存储字符串键与对应信号量的并发字典
    /// </summary>
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks =
        new ConcurrentDictionary<string, SemaphoreSlim>();

    /// <summary>
    /// 异步获取指定键的锁
    /// </summary>
    /// <param name="key">锁定的键</param>
    /// <returns>一个 IDisposable 对象，在释放时会解除锁定</returns>
    /// <exception cref="ArgumentNullException">当 key 为 null 时抛出</exception>
    public static async Task<IDisposable> LockAsync(string key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        var semaphore = _locks.GetOrAdd(key, static _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();
        return new ReleaseSemaphoreOnDispose(semaphore);
    }

    /// <summary>
    /// 用于自动释放信号量的辅助类
    /// </summary>
    private class ReleaseSemaphoreOnDispose : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;

        /// <summary>
        /// 初始化 ReleaseSemaphoreOnDispose 的新实例
        /// </summary>
        /// <param name="semaphore">需要被释放的信号量</param>
        public ReleaseSemaphoreOnDispose(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        /// <summary>
        /// 释放信号量
        /// </summary>
        public void Dispose()
        {
            _semaphore.Release();
        }
    }
}
