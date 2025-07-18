using System.Collections;
using System.Text;

namespace LuYao;

/// <summary>
/// 提供数组相关的实用方法。
/// </summary>
public static class Arrays
{
    private static class EmptyArray<T>
    {
        internal static readonly T[] Value = new T[0];
    }

    /// <summary>
    /// 获取指定类型的空数组。
    /// </summary>
    /// <typeparam name="T">数组的元素类型。</typeparam>
    /// <returns>指定类型的空数组。</returns>
    public static T[] Empty<T>() => EmptyArray<T>.Value;

    /// <summary>
    /// 将 <see cref="IEnumerable"/> 的元素从指定的起始索引到结束索引（可选）连接为字符串，并使用指定的分隔符分隔。
    /// </summary>
    /// <param name="ie">要连接的集合。</param>
    /// <param name="spacer">用于分隔元素的字符串。</param>
    /// <param name="start">开始连接的元素索引（从 0 开始）。</param>
    /// <param name="end">结束连接的元素索引（包含，默认为 0 表示连接到末尾）。</param>
    /// <returns>连接后的字符串。</returns>
    public static string Join(IEnumerable ie, string spacer, int start, int end = 0)
    {
        if (ie == null) return string.Empty;
        StringBuilder sb = new StringBuilder();
        IEnumerator ienum = ie.GetEnumerator();
        int idx = 0, count = 0;
        while (ienum.MoveNext())
        {
            if (end > 0 && idx > end) break;
            if (idx++ < start) continue;
            object v = ienum.Current;
            if (count > 0) sb.Append(spacer);
            sb.Append(Valid.ToString(v));
            count++;
        }
        return sb.ToString();
    }

    /// <summary>
    /// 连接为字符串
    ///</summary>
    public static string Join(params string[] ls) => Join(ls, ",", 0);
}
