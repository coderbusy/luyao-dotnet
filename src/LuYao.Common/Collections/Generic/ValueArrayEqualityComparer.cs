using System;
using System.Collections.Generic;
using System.Linq;

namespace LuYao.Collections.Generic;

/// <summary>
/// 提供用于比较值类型数组相等性的泛型比较器。
/// </summary>
/// <typeparam name="T">数组元素的类型。</typeparam>
public class ValueArrayEqualityComparer<T> : IEqualityComparer<T[]>
{
    /// <summary>
    /// 确定两个数组是否相等。
    /// </summary>
    /// <param name="x">要比较的第一个数组。</param>
    /// <param name="y">要比较的第二个数组。</param>
    /// <returns>如果指定的数组相等，则为 true；否则为 false。</returns>
    public bool Equals(T[]? x, T[]? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        if (x.Length != y.Length) return false;
        return x.SequenceEqual(y);
    }

    /// <summary>
    /// 返回指定数组的哈希代码。
    /// </summary>
    /// <param name="obj">要计算哈希代码的数组。</param>
    /// <returns>指定数组的哈希代码。</returns>
    public int GetHashCode(T[]? obj)
    {
        if (obj == null) return 0;

        unchecked
        {
            int hash = 17;
            foreach (T item in obj)
            {
                hash = hash * 31 + (item?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }
}
