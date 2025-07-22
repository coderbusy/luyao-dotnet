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
    public int Rows => _count;

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

    /// <summary>添加数据列</summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public RecordColumn Add(string name, TypeCode type)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "列名不能为空");
        RecordColumn? col = this.Find(name);
        if (col != null) return col;
        col = new RecordColumn(this._re, name, type, this._capacity);
        this._columns.Add(col);
        return col;
    }

    /// <summary>添加数据列</summary>
    public RecordColumn AddBoolean(string name) => this.Add(name, TypeCode.Boolean);

    /// <summary>添加数据列</summary>
    public RecordColumn AddByte(string name) => this.Add(name, TypeCode.Byte);

    /// <summary>添加数据列</summary>
    public RecordColumn AddChar(string name) => this.Add(name, TypeCode.Char);

    /// <summary>添加数据列</summary>
    public RecordColumn AddDateTime(string name) => this.Add(name, TypeCode.DateTime);

    /// <summary>添加数据列</summary>
    public RecordColumn AddDecimal(string name) => this.Add(name, TypeCode.Decimal);

    /// <summary>添加数据列</summary>
    public RecordColumn AddDouble(string name) => this.Add(name, TypeCode.Double);

    /// <summary>添加数据列</summary>
    public RecordColumn AddInt16(string name) => this.Add(name, TypeCode.Int16);

    /// <summary>添加数据列</summary>
    public RecordColumn AddInt32(string name) => this.Add(name, TypeCode.Int32);

    /// <summary>添加数据列</summary>
    public RecordColumn AddInt64(string name) => this.Add(name, TypeCode.Int64);

    /// <summary>添加数据列</summary>
    public RecordColumn AddSByte(string name) => this.Add(name, TypeCode.SByte);

    /// <summary>添加数据列</summary>
    public RecordColumn AddSingle(string name) => this.Add(name, TypeCode.Single);

    /// <summary>添加数据列</summary>
    public RecordColumn AddString(string name) => this.Add(name, TypeCode.String);

    /// <summary>添加数据列</summary>
    public RecordColumn AddUInt16(string name) => this.Add(name, TypeCode.UInt16);

    /// <summary>添加数据列</summary>
    public RecordColumn AddUInt32(string name) => this.Add(name, TypeCode.UInt32);

    /// <summary>添加数据列</summary>
    public RecordColumn AddUInt64(string name) => this.Add(name, TypeCode.UInt64);
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
