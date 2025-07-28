using System;
using System.Collections;
using System.Collections.Generic;

namespace LuYao.Data;

/// <summary>
/// 列集合
/// </summary>
public class RecordColumnCollection : IReadOnlyList<RecordColumn>
{
    private readonly List<RecordColumn> _columns = new List<RecordColumn>();

    #region IReadOnlyList

    /// <inheritdoc/>
    public int Count => _columns.Count;

    /// <inheritdoc/>
    public RecordColumn this[int index] => _columns[index];

    /// <inheritdoc/>
    public IEnumerator<RecordColumn> GetEnumerator() => _columns.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion

    private Record _re;
    private int _capacity;
    private int _count;
    /// <summary>
    /// 容量
    /// </summary>
    public int Capacity => _capacity;
    internal void SetCapacity(int capacity)
    {
        if (this._columns.Count > 0) throw new InvalidOperationException("不能在已有列的情况下设置容量");
        if (capacity < 1) capacity = 1;
        this._capacity = capacity;
    }
    /// <summary>
    /// 数据行数
    /// </summary>
    public int Rows
    {
        get => _count;
        internal set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "行数不能小于0");
            _count = value;
        }
    }

    internal RecordColumnCollection(Record table, int capacity)
    {
        this._re = table ?? throw new ArgumentNullException(nameof(table), "表不能为空");
        if (capacity < 1) throw new ArgumentOutOfRangeException(nameof(capacity), "容量不能小于1");
        this._capacity = capacity;
    }

    /// <summary>
    /// 根据列名查找列
    /// </summary>
    public RecordColumn? Find(string name)
    {
        var idx = this.IndexOf(name);
        if (idx >= 0) return this[idx];
        return null;
    }

    #region Add

    internal RecordColumn AddInternal(string name, RecordDataType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "列名不能为空");

        if (this.Contains(name))
            throw new InvalidOperationException($"列 '{name}' 已存在");

        var col = new RecordColumn(this._re, name, type, this._capacity);
        this._columns.Add(col);
        return col;
    }

    internal RecordColumn AddInternal(string name, Type type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "列名不能为空");

        if (this.Contains(name))
            throw new InvalidOperationException($"列 '{name}' 已存在");

        var col = new RecordColumn(this._re, name, type, this._capacity);
        this._columns.Add(col);
        return col;
    }

    /// <summary>添加数据列</summary>
    public RecordColumn AddBoolean(string name) => this.AddInternal(name, RecordDataType.Boolean);

    /// <summary>添加数据列</summary>
    public RecordColumn AddByte(string name) => this.AddInternal(name, RecordDataType.Byte);

    /// <summary>添加数据列</summary>
    public RecordColumn AddChar(string name) => this.AddInternal(name, RecordDataType.Char);

    /// <summary>添加数据列</summary>
    public RecordColumn AddDateTime(string name) => this.AddInternal(name, RecordDataType.DateTime);

    /// <summary>添加数据列</summary>
    public RecordColumn AddDecimal(string name) => this.AddInternal(name, RecordDataType.Decimal);

    /// <summary>添加数据列</summary>
    public RecordColumn AddDouble(string name) => this.AddInternal(name, RecordDataType.Double);

    /// <summary>添加数据列</summary>
    public RecordColumn AddInt16(string name) => this.AddInternal(name, RecordDataType.Int16);

    /// <summary>添加数据列</summary>
    public RecordColumn AddInt32(string name) => this.AddInternal(name, RecordDataType.Int32);

    /// <summary>添加数据列</summary>
    public RecordColumn AddInt64(string name) => this.AddInternal(name, RecordDataType.Int64);

    /// <summary>添加数据列</summary>
    public RecordColumn AddSByte(string name) => this.AddInternal(name, RecordDataType.SByte);

    /// <summary>添加数据列</summary>
    public RecordColumn AddSingle(string name) => this.AddInternal(name, RecordDataType.Single);

    /// <summary>添加数据列</summary>
    public RecordColumn AddString(string name) => this.AddInternal(name, RecordDataType.String);

    /// <summary>添加数据列</summary>
    public RecordColumn AddUInt16(string name) => this.AddInternal(name, RecordDataType.UInt16);

    /// <summary>添加数据列</summary>
    public RecordColumn AddUInt32(string name) => this.AddInternal(name, RecordDataType.UInt32);

    /// <summary>添加数据列</summary>
    public RecordColumn AddUInt64(string name) => this.AddInternal(name, RecordDataType.UInt64);

    /// <summary>添加数据列</summary>
    public RecordColumn Add<T>(string name) => Add(name, typeof(T));

    /// <summary>添加数据列</summary>
    public RecordColumn Add(string name, Type type)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Boolean: return this.AddBoolean(name);
            case TypeCode.Byte: return this.AddByte(name);
            case TypeCode.Char: return this.AddChar(name);
            case TypeCode.DateTime: return this.AddDateTime(name);
            case TypeCode.Decimal: return this.AddDecimal(name);
            case TypeCode.Double: return this.AddDouble(name);
            case TypeCode.Int16: return this.AddInt16(name);
            case TypeCode.Int32: return this.AddInt32(name);
            case TypeCode.Int64: return this.AddInt64(name);
            case TypeCode.SByte: return this.AddSByte(name);
            case TypeCode.Single: return this.AddSingle(name);
            case TypeCode.String: return this.AddString(name);
            case TypeCode.UInt16: return this.AddUInt16(name);
            case TypeCode.UInt32: return this.AddUInt32(name);
            case TypeCode.UInt64: return this.AddUInt64(name);
        }
        return AddInternal(name, type);
    }
    #endregion

    /// <summary>
    /// 添加一行
    /// </summary>
    /// <returns>行号</returns>
    internal int AddRow()
    {
        this._count++;
        if (this._capacity < this._count)
        {
            this._capacity *= 2;
            foreach (RecordColumn col in this)
            {
                col.Extend(this._capacity);
            }
        }
        var idx = this._count - 1;
        return idx;
    }

    /// <summary>
    /// 查找指定列名的索引
    /// </summary>
    public int IndexOf(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "列名不能为空");
        for (int i = 0; i < this.Count; i++)
        {
            RecordColumn col = this[i];
            if (col.Name == name) return i;
        }
        return -1;
    }

    /// <summary>
    /// 判断指定的列名是否存在
    /// </summary>
    public bool Contains(string name) => this.IndexOf(name) >= 0;

    /// <summary>
    /// 删除一个列
    /// </summary>
    public bool Remove(RecordColumn column) => this._columns.Remove(column);

    /// <summary>
    /// 清理所有列
    /// </summary>
    public void Clear() => this._columns.Clear();
}
