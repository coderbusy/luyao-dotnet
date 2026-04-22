using System;
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
    /// <value>包含此列的 <see cref="Record"/> 对象。</value>
    public Record Record { get; }

    /// <summary>
    /// 创建一个数据列实例。
    /// </summary>
    /// <param name="record">关联的记录实例。</param>
    /// <param name="name">列的名称。</param>
    /// <param name="type">列的实际数据类型。</param>
    /// 
    /// <exception cref="ArgumentNullException">当 <paramref name="record"/>、<paramref name="name"/> 或 <paramref name="type"/> 为 null 时抛出。</exception>
    internal RecordColumn(Record record, string name, Type type)
    {
        this.Record = record ?? throw new ArgumentNullException(nameof(record));
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
        this._type = type ?? throw new ArgumentNullException(nameof(type));
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
    /// 获取列的基础枚举类型标识（不含可空信息）。
    /// </summary>
    public RecordColumnType ColumnType { get; }

    /// <summary>
    /// 获取列是否为可空类型。
    /// </summary>
    public bool IsNullable { get; }

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

    ///  <inheritdoc/>
    public override string ToString()
    {
        return $"{this.Name},{this.Type.FullName}";
    }

    internal void OnSet(int row)
    {
        if (row < 0 || row >= this.Record.Count) throw new ArgumentOutOfRangeException(nameof(row), $"行索引 {row} 超出有效范围 [0, {Record.Count - 1}]");
    }
    internal void OnGet(int row)
    {
        if (row < 0 || row >= this.Record.Count) throw new ArgumentOutOfRangeException(nameof(row), $"行索引 {row} 超出有效范围 [0, {Record.Count - 1}]");
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

    #endregion

    #region IXProp 实现

    bool IXProp.CanRead => true;
    bool IXProp.CanWrite => true;

    object? IXProp.GetValue(object instance) => Get(((RecordRow)instance).Row);
    void IXProp.SetValue(object instance, object? value) => Set(((RecordRow)instance).Row, value);

    #endregion
}