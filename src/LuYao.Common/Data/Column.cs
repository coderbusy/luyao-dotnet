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
    /// <summary>
    /// 数据
    /// </summary>
    public Array Data => _data;

    /// <summary>
    /// 初始化 <see cref="Column"/> 类的新实例。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <param name="type">列的数据类型。</param>
    /// <param name="dimension">列的维度，默认为 0。</param>
    /// <param name="capacity">列的初始容量，默认为 10。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="name"/> 为 null 时抛出。</exception>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="dimension"/> 小于 0 时抛出。</exception>
    public Column(string name, DataType type, int dimension = 0, int capacity = 60)
    {
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
        this.Type = type;
        if (dimension < 0) throw new ArgumentOutOfRangeException(nameof(dimension), "维度不能小于0");
        this._dimension = dimension;
        this._dataType = Helpers.MakeType(type, dimension);
        this._data = Array.CreateInstance(this._dataType, capacity);
        this._count = 0;
    }

    /// <summary>
    /// 获取列的名称。
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 获取列的数据类型。
    /// </summary>
    public DataType Type { get; }

    /// <summary>
    /// 获取列的维度。
    /// </summary>
    public int Dimension => this._dimension;

    /// <summary>
    /// 获取列的实际数据类型。
    /// </summary>
    public Type DataType => this._dataType;

    /// <summary>
    /// 向列中添加一个值。
    /// </summary>
    /// <param name="value">要添加的值。</param>
    /// <exception cref="ArgumentException">当值的类型与列的数据类型不匹配时抛出。</exception>
    public void Add(object value)
    {
        object? s = From(value);
        if (_count == _data.Length)
        {
            var newData = Array.CreateInstance(_dataType, _data.Length * 2);
            Array.Copy(_data, newData, _data.Length);
            _data = newData;
        }
        _data.SetValue(s, _count);
        _count++;
    }

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
}