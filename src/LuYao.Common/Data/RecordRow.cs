using System;
using System.Collections.Generic;
using System.Dynamic;
using LuYao.Data.Meta;

namespace LuYao.Data;

/// <summary>
/// 代表一行数据，提供对列存储数据集合中特定行数据的访问。
/// </summary>
public partial struct RecordRow : IDynamicMetaObjectProvider
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

    #region 数据读取

    /// <summary>
    /// 根据列对象获取当前行指定列的泛型类型值。
    /// </summary>
    /// <typeparam name="T">要获取的值的类型。</typeparam>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回转换后的泛型类型值，否则按列名在当前记录中查找。</returns>
    public T? Field<T>(RecordColumn col) => col.Record == this.Record ? col.Get<T>(this.Row) : this.Field<T>(col.Name);

    /// <summary>
    /// 根据列名获取当前行指定列的泛型类型值。列不存在时返回 <typeparamref name="T"/> 的默认值。
    /// </summary>
    /// <typeparam name="T">要获取的值的类型。</typeparam>
    /// <param name="name">列的名称。</param>
    public T? Field<T>(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.Get<T>(this.Row) : default;
    }

    #endregion

    /// <summary>
    /// 按列名读取值；列不存在时返回 <see langword="null"/>。
    /// 供 Mapping、dynamic 读取等内部路径使用。
    /// </summary>
    internal object? GetValueOrDefault(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col?.GetValue(this);
    }

    /// <summary>
    /// 按列名写入值；列不存在时静默跳过。
    /// 供 Mapping 写入路径使用，<b>不会自动建列</b>。
    /// </summary>
    internal void TrySetValue(string name, object? value)
    {
        var col = this.Record.Columns.Find(name);
        if (col != null) col.SetValue(this.Row, value);
    }

    /// <summary>
    /// 按列名写入值；列不存在时按 <paramref name="value"/> 的运行时类型自动建列。
    /// 当 <paramref name="value"/> 为 <see langword="null"/> 且列不存在时跳过该次写入（无法推断列类型）。
    /// 供 dynamic 写入路径（成员/索引器赋值）使用。
    /// </summary>
    internal void SetAndEnsureColumn(string name, object? value)
    {
        var existing = this.Record.Columns.Find(name);
        if (existing != null)
        {
            existing.SetValue(this.Row, value);
            return;
        }
        if (value is null) return;
        var col = this.Record.Columns.Add(name, value.GetType());
        col.SetValue(this.Row, value);
    }

    /// <summary>
    /// 返回 <see cref="DynamicMetaObject"/>，支持动态成员访问。
    /// </summary>
    public DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
        => new RecordRowMetaObject(parameter, this);

    /// <summary>
    /// 根据列名设置当前行指定列的泛型类型值。
    /// 列不存在时按 <typeparamref name="T"/> 自动建列；列已存在但类型不匹配时抛 <see cref="InvalidOperationException"/>。
    /// </summary>
    /// <typeparam name="T">要设置的值的类型。</typeparam>
    /// <param name="name">列的名称。</param>
    /// <param name="value">要设置的值。</param>
    /// <exception cref="InvalidOperationException">当同名列已存在且类型与 <typeparamref name="T"/> 不一致时抛出。</exception>
    public void Set<T>(string name, T value)
    {
        var col = this.Record.Columns.Add<T>(name);
        col.Set(value, this.Row);
    }
}