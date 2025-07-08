using System;
using System.Collections.Generic;

namespace LuYao.Collections;

/// <summary>
/// 提供对 <see cref="IEnumerable{T}"/> 的扩展方法。
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// 使用指定的比较器，将 <see cref="IEnumerable{T}"/> 集合转换为有序且去重的 <see cref="List{T}"/>。
    /// </summary>
    /// <typeparam name="T">集合中元素的类型。</typeparam>
    /// <param name="items">要转换的 <see cref="IEnumerable{T}"/> 集合。</param>
    /// <param name="comparer">用于元素排序的比较器。</param>
    /// <returns>包含原集合元素的有序且去重的 <see cref="List{T}"/>。</returns>
    public static List<T> ToSortedDistinctList<T>(IEnumerable<T> items, IComparer<T> comparer)
    {
        var list = new List<T>();
        foreach (var value in items)
        {
            var i = list.BinarySearch(value, comparer);
            if (i < 0) list.Insert(-i - 1, value);
        }
        return list;
    }

    /// <summary>
    /// 使用默认比较器，将 <see cref="IEnumerable{T}"/> 集合转换为有序且去重的 <see cref="List{T}"/>。
    /// </summary>
    /// <typeparam name="T">集合中元素的类型。</typeparam>
    /// <param name="items">要转换的 <see cref="IEnumerable{T}"/> 集合。</param>
    /// <returns>包含原集合元素的有序且去重的 <see cref="List{T}"/>。</returns>
    public static List<T> ToSortedDistinctList<T>(IEnumerable<T> items) => ToSortedDistinctList(items, Comparer<T>.Default);

    /// <summary>
    /// 将一个可枚举对象按指定批次大小分组。
    /// </summary>
    /// <typeparam name="T">元素类型。</typeparam>
    /// <param name="source">要分组的源可枚举对象。</param>
    /// <param name="batchSize">每个批次的元素数量。</param>
    /// <returns>包含元素列表的批次的可枚举对象。</returns>
    public static IEnumerable<IReadOnlyList<T>> SplitToBatch<T>(this IEnumerable<T> source, int batchSize)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));

        var buffer = new List<T>(batchSize);

        foreach (var item in source)
        {
            buffer.Add(item);
            if (buffer.Count >= batchSize)
            {
                yield return buffer.ToArray();
                buffer.Clear();
            }
        }

        if (buffer.Count > 0)
        {
            yield return buffer.ToArray();
            buffer.Clear();
        }
    }

}
