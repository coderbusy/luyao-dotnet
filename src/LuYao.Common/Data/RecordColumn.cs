using System;
using System.Collections.Generic;
using LuYao.Data.Meta;

namespace LuYao.Data;

/// <summary>
/// 表示一个数据列。
/// </summary>
public abstract class RecordColumn : IXProp
{
    /// <summary>
    /// 获取关联的记录实例。
    /// </summary>
    /// <value>包含此列的 <see cref="RecordTable"/> 对象。</value>
    public RecordTable Table { get; }

    /// <summary>
    /// 创建一个数据列实例。
    /// </summary>
    /// <param name="table">关联的记录实例。</param>
    /// <param name="name">列的名称。</param>
    /// <param name="type">列的实际数据类型。</param>
    /// 
    /// <exception cref="ArgumentNullException">当 <paramref name="table"/>、<paramref name="name"/> 或 <paramref name="type"/> 为 null 时抛出。</exception>
    internal RecordColumn(RecordTable table, string name, Type type)
    {
        this.Table = table ?? throw new ArgumentNullException(nameof(table));
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
        this._type = type ?? throw new ArgumentNullException(nameof(type));
        this.ArrayRank = DetermineArrayRank(type);
        this.ColumnType = Helpers.GetColumnType(type);
        this.IsNullable = Helpers.IsNullableType(type);
    }
    private Type _type;
    /// <summary>
    /// 获取列的名称。
    /// </summary>
    /// <value>列的名称字符串。</value>
    public string Name { get; internal set; }

    /// <summary>
    /// 获取列的实际数据类型。
    /// </summary>
    /// <value>表示列实际数据类型的 <see cref="Type"/> 对象。</value>
    public Type Type => this._type;

    /// <summary>
    /// 获取列的基础枚举类型标识（不含可空信息和数组维度）。
    /// </summary>
    public RecordColumnType ColumnType { get; }

    /// <summary>
    /// 获取列是否为可空类型。
    /// </summary>
    public bool IsNullable { get; }

    /// <summary>
    /// 获取数组维度。0 表示非数组，1 表示一维数组（如 int[]），2 表示二维数组（如 int[,]），依此类推。
    /// </summary>
    public int ArrayRank { get; }

    /// <summary>
    /// 确定类型的数组维度。
    /// </summary>
    private static int DetermineArrayRank(Type type)
    {
        var effectiveType = Nullable.GetUnderlyingType(type) ?? type;
        return effectiveType.IsArray ? effectiveType.GetArrayRank() : 0;
    }

    /// <summary>
    /// 在指定行设置列的值。
    /// </summary>
    /// <param name="row">目标行索引，从 0 开始。</param>
    /// <param name="value">要设置的值，可以为 null。</param>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="row"/> 超出有效范围时抛出。</exception>
    public abstract void Set(int row, object? value);

    /// <summary>
    /// 获取指定行的列值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>指定行的列值，可能为 null。</returns>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="row"/> 超出有效范围时抛出。</exception>
    public abstract object? Get(int row);

    /// <summary>
    /// 删除指定行的列值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    public abstract void Delete(int row);

    /// <summary>
    /// 清空列中的所有数据。
    /// </summary>
    public abstract void Clear();

    /// <summary>
    /// 获取列的当前容量。
    /// </summary>
    /// <value>列能够容纳的最大行数。</value>
    public abstract int Capacity { get; }
    internal abstract void Extend(int length);

    /// <summary>
    /// 获取底层数据数组（截断到实际行数），用于序列化。
    /// </summary>
    internal abstract Array GetDataArray(int count);

    /// <summary>
    /// 从序列化数据数组还原列数据。
    /// </summary>
    internal abstract void SetDataArray(Array data, int count);

    /// <summary>
    /// 按给定的行索引置换重排列内部数据，实现原地排序。
    /// <paramref name="indices"/> 的长度等于当前记录行数，<c>indices[newRow] = oldRow</c>。
    /// </summary>
    internal abstract void Reorder(int[] indices);

    ///  <inheritdoc/>
    public override string ToString()
    {
        return $"{this.Name},{this.Type.FullName}";
    }

    internal void OnSet(int row)
    {
        if (row < 0 || row >= this.Table.Count) throw new ArgumentOutOfRangeException(nameof(row), $"行索引 {row} 超出有效范围 [0, {Table.Count - 1}]");
    }
    internal void OnGet(int row)
    {
        if (row < 0 || row >= this.Table.Count) throw new ArgumentOutOfRangeException(nameof(row), $"行索引 {row} 超出有效范围 [0, {Table.Count - 1}]");
    }

    #region To 
    /// <summary>
    /// 获取指定行的值并转换为指定的泛型类型。
    /// </summary>
    /// <typeparam name="T">要转换到的目标类型。</typeparam>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的值，如果原值为 null 则返回类型 T 的默认值。</returns>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="row"/> 超出有效范围时抛出。</exception>
    /// <exception cref="InvalidCastException">当值无法转换为目标类型时抛出。</exception>
    public virtual T? To<T>(int row)
    {
        OnGet(row);
        object? value = this.Get(row);
        if (value is null) return default;
        if (value is T direct) return direct;
        return (T)Valid.To(value, typeof(T));
    }

    /// <summary>
    /// 将指定行的列值转换为字符串表示。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>列值的字符串表示。</returns>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="row"/> 超出有效范围时抛出。</exception>
    public abstract String ToString(int row);

    /// <summary>
    /// 将整列数据转换为 <see cref="List{T}"/>，顺序与行索引一致。
    /// 原值为 null 时对应元素为 <typeparamref name="T"/> 的默认值。
    /// </summary>
    /// <typeparam name="T">目标类型。</typeparam>
    /// <returns>包含整列数据的列表，长度等于 <see cref="RecordTable.Count"/>。</returns>
    /// <exception cref="InvalidCastException">当某行值无法转换为目标类型时抛出。</exception>
    public List<T?> ToList<T>()
    {
        int count = this.Table.Count;
        var list = new List<T?>(count);
        for (int i = 0; i < count; i++)
            list.Add(To<T>(i));
        return list;
    }
    #endregion

    #region IXProp 实现

    bool IXProp.CanRead => true;
    bool IXProp.CanWrite => true;

    object? IXProp.GetValue(object instance)
    {
        if (instance is RecordRow row) return Get(row.Row);
        throw new InvalidCastException();
    }

    void IXProp.SetValue(object instance, object? value)
    {
        if (instance is RecordRow row)
            Set(row.Row, value);
        else
            throw new InvalidCastException();
    }

    #endregion
}