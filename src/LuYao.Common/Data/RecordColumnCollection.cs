using LuYao.Data.Columns;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LuYao.Data;

/// <summary>
/// 列集合
/// </summary>
public class RecordColumnCollection : IReadOnlyList<RecordColumn>
{
    private readonly List<RecordColumn> _list = new List<RecordColumn>();
    /// <summary>
    /// 关联的记录
    /// </summary>
    public Record Record { get; }

    /// <summary>
    /// 初始化 <see cref="RecordColumnCollection"/> 类的新实例
    /// </summary>
    /// <param name="record">关联的记录实例</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="record"/> 为 null 时抛出</exception>
    internal RecordColumnCollection(Record record)
    {
        this.Record = record ?? throw new ArgumentNullException(nameof(record));
    }

    #region IReadOnlyList

    ///<inheritdoc/>
    public RecordColumn this[int index] => _list[index];

    ///<inheritdoc/>
    public int Count => _list.Count;

    ///<inheritdoc/>
    public IEnumerator<RecordColumn> GetEnumerator() => _list.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    /// <summary>
    /// 当添加行时调用，用于扩展列的容量以适应新行
    /// </summary>
    internal void OnAddRow()
    {
        if (this.Count == 0) return;
        int num = this.Record.Count;
        foreach (RecordColumn col in this)
        {
            if (col.Capacity >= num) continue;
            col.Extend(num);
        }
        this.Record.Capacity = this.Min(f => f.Capacity);
    }

    /// <summary>
    /// 查找指定列名的索引
    /// </summary>
    /// <param name="name">要查找的列名</param>
    /// <returns>如果找到列则返回索引，否则返回 -1</returns>
    public int IndexOf(string name)
    {
        for (int i = 0; i < this.Count; i++)
        {
            if (this[i].Name == name) return i;
        }
        return -1;
    }


    /// <summary>
    /// 判断指定的列名是否存在
    /// </summary>
    /// <param name="name">要检查的列名</param>
    /// <returns>如果列名存在则返回 true，否则返回 false</returns>
    public bool Contains(string name) { return this.IndexOf(name) > -1; }

    /// <summary>
    /// 根据列名查找列
    /// </summary>
    /// <param name="name">要查找的列名</param>
    /// <returns>如果找到列则返回 <see cref="RecordColumn"/> 实例，否则返回 null</returns>
    public RecordColumn? Find(string name)
    {
        int idx = this.IndexOf(name);
        return idx > -1 ? this[idx] : null;
    }

    /// <summary>
    /// 删除一个列
    /// </summary>
    /// <param name="name">要删除的列名</param>
    /// <returns>如果成功删除则返回被删除的 <see cref="RecordColumn"/> 实例，否则返回 null</returns>
    public RecordColumn? Remove(string name)
    {
        RecordColumn? col = null;
        int idx = this.IndexOf(name);
        if (idx > -1)
        {
            col = this[idx];
            this._list.RemoveAt(idx);
        }
        return col;
    }

    /// <summary>
    /// 清理所有列
    /// </summary>
    public void Clear()
    {
        this._list.Clear();
        this.Record.OnClear();
    }

    #region Add

    /// <summary>
    /// 验证添加列时的列名有效性
    /// </summary>
    /// <param name="name">要验证的列名</param>
    /// <exception cref="ArgumentNullException">当列名为 null 或空白时抛出</exception>
    /// <exception cref="ArgumentException">当列名已存在时抛出</exception>
    private void OnAdd(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "列名不能为空");
        if (this.Contains(name)) throw new ArgumentException($"列名 '{name}' 已经存在", nameof(name));
    }

    /// <summary>
    /// 添加一个 Boolean 类型的列
    /// </summary>
    /// <param name="name">列名</param>
    /// <returns>BooleanRecordColumn 实例</returns>
    public BooleanRecordColumn AddBoolean(string name)
    {
        this.OnAdd(name);
        var col = new BooleanRecordColumn(this.Record, name);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加一个 Byte 类型的列
    /// </summary>
    /// <param name="name">列名</param>
    /// <returns>ByteRecordColumn 实例</returns>
    public ByteRecordColumn AddByte(string name)
    {
        this.OnAdd(name);
        var col = new ByteRecordColumn(this.Record, name);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加一个 Char 类型的列
    /// </summary>
    /// <param name="name">列名</param>
    /// <returns>CharRecordColumn 实例</returns>
    public CharRecordColumn AddChar(string name)
    {
        this.OnAdd(name);
        var col = new CharRecordColumn(this.Record, name);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加一个 DateTime 类型的列
    /// </summary>
    /// <param name="name">列名</param>
    /// <returns>DateTimeRecordColumn 实例</returns>
    public DateTimeRecordColumn AddDateTime(string name)
    {
        this.OnAdd(name);
        var col = new DateTimeRecordColumn(this.Record, name);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加一个 Decimal 类型的列
    /// </summary>
    /// <param name="name">列名</param>
    /// <returns>DecimalRecordColumn 实例</returns>
    public DecimalRecordColumn AddDecimal(string name)
    {
        this.OnAdd(name);
        var col = new DecimalRecordColumn(this.Record, name);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加一个 Double 类型的列
    /// </summary>
    /// <param name="name">列名</param>
    /// <returns>DoubleRecordColumn 实例</returns>
    public DoubleRecordColumn AddDouble(string name)
    {
        this.OnAdd(name);
        var col = new DoubleRecordColumn(this.Record, name);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加一个 Int16 类型的列
    /// </summary>
    /// <param name="name">列名</param>
    /// <returns>Int16RecordColumn 实例</returns>
    public Int16RecordColumn AddInt16(string name)
    {
        this.OnAdd(name);
        var col = new Int16RecordColumn(this.Record, name);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加一个 Int32 类型的列
    /// </summary>
    /// <param name="name">列名</param>
    /// <returns>Int32RecordColumn 实例</returns>
    public Int32RecordColumn AddInt32(string name)
    {
        this.OnAdd(name);
        var col = new Int32RecordColumn(this.Record, name);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加一个 Int64 类型的列
    /// </summary>
    /// <param name="name">列名</param>
    /// <returns>Int64RecordColumn 实例</returns>
    public Int64RecordColumn AddInt64(string name)
    {
        this.OnAdd(name);
        var col = new Int64RecordColumn(this.Record, name);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加一个 SByte 类型的列
    /// </summary>
    /// <param name="name">列名</param>
    /// <returns>SByteRecordColumn 实例</returns>
    public SByteRecordColumn AddSByte(string name)
    {
        this.OnAdd(name);
        var col = new SByteRecordColumn(this.Record, name);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加一个 Single 类型的列
    /// </summary>
    /// <param name="name">列名</param>
    /// <returns>SingleRecordColumn 实例</returns>
    public SingleRecordColumn AddSingle(string name)
    {
        this.OnAdd(name);
        var col = new SingleRecordColumn(this.Record, name);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加一个 String 类型的列
    /// </summary>
    /// <param name="name">列名</param>
    /// <returns>StringRecordColumn 实例</returns>
    public StringRecordColumn AddString(string name)
    {
        this.OnAdd(name);
        var col = new StringRecordColumn(this.Record, name);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加一个 UInt16 类型的列
    /// </summary>
    /// <param name="name">列名</param>
    /// <returns>UInt16RecordColumn 实例</returns>
    public UInt16RecordColumn AddUInt16(string name)
    {
        this.OnAdd(name);
        var col = new UInt16RecordColumn(this.Record, name);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加一个 UInt32 类型的列
    /// </summary>
    /// <param name="name">列名</param>
    /// <returns>UInt32RecordColumn 实例</returns>
    public UInt32RecordColumn AddUInt32(string name)
    {
        this.OnAdd(name);
        var col = new UInt32RecordColumn(this.Record, name);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加一个 UInt64 类型的列
    /// </summary>
    /// <param name="name">列名</param>
    /// <returns>UInt64RecordColumn 实例</returns>
    public UInt64RecordColumn AddUInt64(string name)
    {
        this.OnAdd(name);
        var col = new UInt64RecordColumn(this.Record, name);
        this._list.Add(col);
        return col;
    }


    ///<summary>
    /// 根据列名和类型添加一个列。
    /// </summary>
    /// <param name="name">列名</param>
    /// <param name="type">列的数据类型</param>
    /// <returns>添加的 <see cref="RecordColumn"/> 实例</returns>
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
        this.OnAdd(name);
        RecordColumn col = Helpers.MakeRecordColumn(this.Record, name, type);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加指定泛型类型的列
    /// </summary>
    /// <typeparam name="T">列的数据类型</typeparam>
    /// <param name="name">列名</param>
    /// <returns>添加的 <see cref="RecordColumn{T}"/> 实例</returns>
    public RecordColumn<T> Add<T>(string name) => (RecordColumn<T>)this.Add(name, typeof(T));
    #endregion
}