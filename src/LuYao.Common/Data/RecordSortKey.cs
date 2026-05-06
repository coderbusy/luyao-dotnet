namespace LuYao.Data;

/// <summary>
/// 表示一个排序键，包含列名和排序方向。
/// </summary>
public readonly struct RecordSortKey
{
    /// <summary>
    /// 列名（区分大小写）。
    /// </summary>
    public string Column { get; }

    /// <summary>
    /// 是否降序排列。<see langword="false"/> 表示升序（ASC），<see langword="true"/> 表示降序（DESC）。
    /// </summary>
    public bool Descending { get; }

    /// <summary>
    /// 初始化 <see cref="RecordSortKey"/> 实例。
    /// </summary>
    /// <param name="column">列名。</param>
    /// <param name="descending">是否降序。</param>
    public RecordSortKey(string column, bool descending = false)
    {
        Column = column;
        Descending = descending;
    }

#if !NET45 && !NET461
    /// <summary>
    /// 从元组 <c>(string column, bool descending)</c> 隐式转换。
    /// </summary>
    public static implicit operator RecordSortKey((string column, bool descending) tuple)
        => new RecordSortKey(tuple.column, tuple.descending);
#endif

    ///<inheritdoc/>
    public override string ToString() => Descending ? $"{Column} DESC" : $"{Column} ASC";
}
