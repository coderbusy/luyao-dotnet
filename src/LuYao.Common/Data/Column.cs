using System;

namespace LuYao.Data;

/// <summary>
/// 表示一个数据列。
/// </summary>
public sealed class Column
{
    private Array _data;
    private int _count;
    private int _dimension = 0;
    private Type _dataType;

    private int _cursor = 0;

    /// <summary>
    /// 访问游标位置
    /// </summary>
    public int Cursor
    {
        get => _cursor;
        set => _cursor = value;
    }

    /// <summary>
    /// 数据
    /// </summary>
    internal Array Data => _data;

    /// <summary>
    /// 初始化 <see cref="Column"/> 类的新实例。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <param name="type">列的数据类型。</param>
    /// <param name="dimension">列的维度，默认为 0。</param>
    /// <param name="capacity">列的初始容量，默认为 60。</param>
    /// <param name="cursor"></param>
    /// <exception cref="ArgumentNullException">当 <paramref name="name"/> 为 null 时抛出。</exception>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="dimension"/> 小于 0 时抛出。</exception>
    public Column(string name, TypeCode type, int dimension, int capacity, int cursor)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "列的名称不能为空或空白");
        this.Name = name;
        this.Type = type;
        if (dimension < 0) throw new ArgumentOutOfRangeException(nameof(dimension), "维度不能小于0");
        if (capacity < 1) throw new ArgumentOutOfRangeException(nameof(capacity), "容量不能小于1");
        this._dimension = dimension;
        this._dataType = Helpers.MakeType(type, dimension);
        this._data = Array.CreateInstance(this._dataType, capacity);
        this._count = 0;
        this._cursor = cursor;
    }

    internal void Extend(int length)
    {
        if (this._data.Length >= length) return;
        Array tmp = Array.CreateInstance(this._dataType, length);
        this._data.CopyTo(tmp, 0);
        this._data = tmp;
    }

    /// <summary>
    /// 获取列的名称。
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 获取列的数据类型。
    /// </summary>
    public TypeCode Type { get; }

    /// <summary>
    /// 获取列的维度。
    /// </summary>
    public int Dimension => this._dimension;

    /// <summary>
    /// 获取列的实际数据类型。
    /// </summary>
    public Type DataType => this._dataType;

    /// <summary>
    ///  
    /// </summary>
    /// <param name="value"></param>
    /// <param name="row"></param>
    public void Set(object? value, int row) => _data.SetValue(From(value), row);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void Set(object? value) => _data.SetValue(From(value), _cursor);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public object? Get(int row) => _data.GetValue(row);
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public object? Get() => _data.GetValue(_cursor);

    private object? From(object? value)
    {
        if (value == null) return null;
        if (Convert.IsDBNull(value)) return null;
        if (_dataType.IsInstanceOfType(value)) return value;
        return Convert.ChangeType(value, _dataType);
    }

    /// <summary>
    /// 清空列中的所有数据。
    /// </summary>
    public void Clear()
    {
        Array.Clear(_data, 0, _data.Length);
        _count = 0;
    }
    /// <summary>
    /// 获取指定游标位置的列。
    /// </summary>
    public Column this[int index] { get { this._cursor = index; return this; } }
}