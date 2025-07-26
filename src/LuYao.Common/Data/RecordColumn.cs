using System;

namespace LuYao.Data;

/// <summary>
/// 表示一个数据列。
/// </summary>
public sealed partial class RecordColumn
{
    private Record _table;
    private ColumnData _data;
    private Type _type;

    /// <summary>
    /// 数据
    /// </summary>
    internal ColumnData Data => _data;

    /// <summary>
    /// 初始化 <see cref="RecordColumn"/> 类的新实例。
    /// </summary>
    /// <param name="table"></param>
    /// <param name="name">列的名称。</param>
    /// <param name="code">列的数据类型。</param>
    /// <param name="capacity">列的初始容量，默认为 60。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="name"/> 为 null 时抛出。</exception>
    public RecordColumn(Record table, string name, RecordDataType code, int capacity)
    {
        if (table == null) throw new ArgumentNullException(nameof(table), "表不能为空");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "列的名称不能为空或空白");
        this._table = table;
        this.Name = name;
        this.Code = code;
        if (capacity < 1) throw new ArgumentOutOfRangeException(nameof(capacity), "容量不能小于1");
        this._type = Helpers.ToType(code);
        this._data = Helpers.MakeData(code, capacity);
    }

    internal void Extend(int length) => this._data.Extend(length);

    /// <summary>
    /// 获取列的名称。
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 获取列的数据类型。
    /// </summary>
    public RecordDataType Code { get; }

    /// <summary>
    /// 获取列的实际数据类型。
    /// </summary>
    public Type Type => this._type;

    /// <summary>
    ///  
    /// </summary>
    /// <param name="value"></param>
    /// <param name="row"></param>
    public void SetValue(object? value, int row) => _data.SetValue(value, row);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="row"></param>
    public void SetValue(object? value, RecordRow row) => SetValue(value, row.RowIndex);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public object? GetValue(int row) => _data.GetValue(row);

    ///<inheritdoc/>
    public object? GetValue(RecordRow row) => GetValue(row.RowIndex);

    /// <summary>
    /// 清空列中的所有数据。
    /// </summary>
    public void Clear()
    {
        _data.Clear();
    }

    ///<inheritdoc/>
    public override string ToString() => $"{this.Name},{this.Code}";
}