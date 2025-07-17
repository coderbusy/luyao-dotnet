using System;

namespace LuYao.Data;

/// <summary>
/// 表示一个数据列。
/// </summary>
public sealed partial class Column
{
    private Array _data;
    private int _count;
    private bool _isArray = false;
    private Type _type;

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
    /// <param name="code">列的数据类型。</param>
    /// <param name="isArray">列的维度，默认为 0。</param>
    /// <param name="capacity">列的初始容量，默认为 60。</param>
    /// <param name="cursor"></param>
    /// <exception cref="ArgumentNullException">当 <paramref name="name"/> 为 null 时抛出。</exception>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="isArray"/> 小于 0 时抛出。</exception>
    public Column(string name, TypeCode code, bool isArray, int capacity, int cursor)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "列的名称不能为空或空白");
        this.Name = name;
        this.Code = code;
        if (capacity < 1) throw new ArgumentOutOfRangeException(nameof(capacity), "容量不能小于1");
        this._isArray = isArray;
        this._type = Helpers.MakeType(code, isArray);
        this._data = Array.CreateInstance(this._type, capacity);
        this._count = 0;
        this._cursor = cursor;
    }

    internal void Extend(int length)
    {
        if (this._data.Length >= length) return;
        Array tmp = Array.CreateInstance(this._type, length);
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
    public TypeCode Code { get; }

    /// <summary>
    /// 获是否数组
    /// </summary>
    public bool IsArray => this._isArray;

    /// <summary>
    /// 获取列的实际数据类型。
    /// </summary>
    public Type Type => this._type;

    /// <summary>
    ///  
    /// </summary>
    /// <param name="value"></param>
    /// <param name="row"></param>
    public void Set(object? value, int row) => _data.SetValue(Cast(value), row);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void Set(object? value) => _data.SetValue(Cast(value), _cursor);

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

    private object? Cast(object? value)
    {
        if (value == null) return null;
        if (Convert.IsDBNull(value)) return null;
        if (_type.IsInstanceOfType(value)) return value;
        return Convert.ChangeType(value, _type);
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