using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Linq;

namespace LuYao.Threading
{
    [TestClass]
    public class AsyncLockTests
    {
        [TestMethod]
        public async Task LockAsync_WithNullKey_ThrowsArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                async () => await AsyncLock.LockAsync(null));
        }

        [TestMethod]
        public async Task LockAsync_ShouldAllowReentry_AfterDispose()
        {
            const string key = "test";
            using (await AsyncLock.LockAsync(key))
            {
                // 首次获取锁成功
            }

            using (await AsyncLock.LockAsync(key))
            {
                // 释放后可以再次获取锁
            }
        }

        [TestMethod]
        public async Task LockAsync_DifferentKeys_ShouldNotBlock()
        {
            var task1 = Task.Run(async () =>
            {
                using (await AsyncLock.LockAsync("key1"))
                {
                    await Task.Delay(100);
                }
            });

            var task2 = Task.Run(async () =>
            {
                using (await AsyncLock.LockAsync("key2"))
                {
                    await Task.Delay(100);
                }
            });

            var sw = Stopwatch.StartNew();
            await Task.WhenAll(task1, task2);
            sw.Stop();

            // 两个任务应该并行执行，总时间应该接近100ms而不是200ms
            Assert.IsTrue(sw.ElapsedMilliseconds < 150);
        }

        [TestMethod]
        public async Task LockAsync_SameKey_ShouldBlock()
        {
            const string key = "test";
            var results = new ConcurrentQueue<int>();

            var task1 = Task.Run(async () =>
            {
                using (await AsyncLock.LockAsync(key))
                {
                    results.Enqueue(1);
                    await Task.Delay(100);
                    results.Enqueue(2);
                }
            });

            var task2 = Task.Run(async () =>
            {
                using (await AsyncLock.LockAsync(key))
                {
                    results.Enqueue(3);
                    await Task.Delay(100);
                    results.Enqueue(4);
                }
            });

            await Task.WhenAll(task1, task2);

            var arr = results.ToArray();
            bool ok = (arr.SequenceEqual(new[] { 1, 2, 3, 4 }) || arr.SequenceEqual(new[] { 3, 4, 1, 2 }));
            Assert.IsTrue(ok, $"实际顺序: {string.Join(",", arr)}");
        }
    }
}