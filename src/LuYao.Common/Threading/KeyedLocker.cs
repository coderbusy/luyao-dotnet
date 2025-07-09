using System;

namespace LuYao.Threading;

/// <summary>
/// 提供基于键的锁机制，允许通过键获取对应的锁对象。
/// </summary>
public static class KeyedLocker<T>
{
    private static object[] Lockers;

    static KeyedLocker()
    {
        int Length = Math.Max(8, Environment.ProcessorCount * 4);
        var temp = new object[Length];
        for (int i = 0; i < Length; i++) temp[i] = new object();
        Lockers = temp;
    }

    /// <summary>
    /// 根据指定的键获取锁对象。
    /// </summary>
    /// <param name="key">用于获取锁的键。</param>
    /// <returns>与键关联的锁对象。</returns>
    /// <exception cref="ArgumentNullException">当键为 null 时抛出此异常。</exception>
    public static object GetLock(string key)
    {
        if (key is null) throw new ArgumentNullException(nameof(key));
        var code = key.GetHashCode();
        return Lockers[Math.Abs(code % Lockers.Length)];
    }
}
