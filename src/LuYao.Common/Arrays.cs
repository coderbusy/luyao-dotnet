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
}
