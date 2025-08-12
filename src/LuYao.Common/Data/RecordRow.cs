using LuYao.Text.Json;
using System;

namespace LuYao.Data;

/// <summary>
/// 代表一行数据，提供对列存储数据集合中特定行数据的访问。
/// 实现了 <see cref="IRecordCursor"/> 接口，支持类型安全的数据读取操作。
/// </summary>
public struct RecordRow : IRecordCursor, IDataModel
{
    /// <summary>
    /// 初始化 <see cref="RecordRow"/> 结构体的新实例。
    /// </summary>
    /// <param name="record">包含此行的数据集合。</param>
    /// <param name="row">行索引，必须在有效范围内。</param>
    /// <exception cref="ArgumentOutOfRangeException">当行索引超出记录范围时抛出。</exception>
    /// <exception cref="ArgumentNullException">当记录参数为 null 时抛出。</exception>
    internal RecordRow(Record record, int row)
    {
        this.Record = record ?? throw new ArgumentNullException(nameof(record));
        if (row < 0 || row >= record.Count) throw new ArgumentOutOfRangeException(nameof(row));
        this.Row = row;
    }

    /// <summary>
    /// 获取包含此行的数据集合。
    /// </summary>
    /// <value>关联的 <see cref="Record"/> 实例。</value>
    public Record Record { get; }

    /// <summary>
    /// 获取当前行在数据集合中的索引位置。
    /// </summary>
    /// <value>从零开始的行索引。</value>
    public int Row { get; }


    /// <summary>
    /// 定义从 <see cref="RecordRow"/> 到 <see cref="int"/> 的隐式转换。
    /// 返回行的索引位置。
    /// </summary>
    /// <param name="rowRef">要转换的 <see cref="RecordRow"/> 实例。</param>
    /// <returns>该行在数据集合中的索引位置。</returns>
    public static implicit operator int(RecordRow rowRef) => rowRef.Row;

    #region IRecordCursor 实现

    /// <summary>
    /// 根据列对象获取当前行指定列的泛型类型值。
    /// </summary>
    /// <typeparam name="T">要获取的值的类型。</typeparam>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回转换后的泛型类型值，否则返回同名值。</returns>
    public T? Get<T>(RecordColumn col) => col.Record == this.Record ? col.Get<T>(this.Row) : this.Get<T>(col.Name);

    /// <summary>
    /// 根据列名获取当前行指定列的泛型类型值。
    /// </summary>
    /// <typeparam name="T">要获取的值的类型。</typeparam>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的泛型类型值，否则返回该类型的默认值。</returns>
    public T? Get<T>(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.Get<T>(this.Row) : default;
    }

    #endregion

    #region IDataModel

    ///<inheritdoc/>
    public object? this[string key]
    {
        get => this.Record.Columns.Get(key).GetValue(this);
        set => this.Record.Columns.Get(key).SetValue(value, this);
    }
    #endregion
}