using System;
using System.Collections;
using System.Collections.Generic;

namespace LuYao.Data;

/// <summary>
/// 列存储数据集合
/// </summary>
public partial class Record : IEnumerable<RecordRow>, IRecordCursor
{
    /// <summary>
    /// 初始化 <see cref="Record"/> 类的新实例。
    /// </summary>
    public Record() : this(null, 0)
    {

    }

    /// <summary>
    /// 使用指定的表名和行数初始化 <see cref="Record"/> 类的新实例。
    /// </summary>
    /// <param name="name">表名称。</param>
    /// <param name="rows">初始行数。</param>
    public Record(string? name, int rows)
    {
        if (!string.IsNullOrWhiteSpace(name)) this.Name = name!;
        int c = rows;
        if (c < 20) c = 20;
        this.Capacity = c;
        _cols = new RecordColumnCollection(this);
    }

    /// <summary>
    /// 集合名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    private readonly RecordColumnCollection _cols;

    /// <summary>
    /// 表列集合
    /// </summary>
    public RecordColumnCollection Columns => _cols;
    /// <summary>
    /// 容量
    /// </summary>
    public int Capacity { get; internal set; }
    /// <summary>
    /// 数据条数
    /// </summary>
    public int Count { get; private set; } = 0;
    /// <summary>
    /// 游标位置
    /// </summary>
    public int Cursor { get; private set; } = 0;

    /// <summary>
    /// 添加一行数据。
    /// </summary>
    public RecordRow AddRow()
    {
        this.Cursor = this.Count;
        this.Count++;
        this.Columns.OnAddRow();
        return new RecordRow(this, this.Cursor);
    }

    /// <summary>
    /// 读取一行，成功返回 true，失败返回 false。
    /// 当游标位置已经到达最后一行时，重置游标到第一行并返回 false。
    /// </summary>
    public bool Read()
    {
        if (this.Cursor < this.Count)
        {
            this.Cursor++;
            return true;
        }
        this.Cursor = 0;
        return false;
    }

    /// <summary>
    /// 清楚所有数据。
    /// </summary>
    public void ClearRows()
    {
        this.OnClear();
        foreach (RecordColumn col in this.Columns)
        {
            col.Clear();
        }
    }
    internal void OnClear()
    {
        this.Count = 0;
        this.Cursor = 0;
    }
    #region IEnumerable

    ///<inheritdoc/> 
    public IEnumerator<RecordRow> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return new RecordRow(this, i);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    #endregion

    /// <summary>
    /// 获取数据是否为空
    /// </summary>
    public bool IsEmpty { get { return Count > 0 ? false : true; } }

    #region Get
    public T? Get<T>(RecordColumn col) => col.Record == this ? col.To<T>() : default;
    public T? Get<T>(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.To<T>() : default;
    }

    public Boolean GetBoolean(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToBoolean() : default;
    }

    public Boolean GetBoolean(RecordColumn col) => col.Record == this ? col.ToBoolean() : GetBoolean(col.Name);

    public Byte GetByte(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToByte() : default;
    }

    public Byte GetByte(RecordColumn col) => col.Record == this ? col.ToByte() : GetByte(col.Name);

    public Char GetChar(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToChar() : default;
    }

    public Char GetChar(RecordColumn col) => col.Record == this ? col.ToChar() : GetChar(col.Name);

    public DateTime GetDateTime(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToDateTime() : default;
    }

    public DateTime GetDateTime(RecordColumn col) => col.Record == this ? col.ToDateTime() : GetDateTime(col.Name);

    public Decimal GetDecimal(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToDecimal() : default;
    }

    public Decimal GetDecimal(RecordColumn col) => col.Record == this ? col.ToDecimal() : GetDecimal(col.Name);

    public Double GetDouble(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToDouble() : default;
    }

    public Double GetDouble(RecordColumn col) => col.Record == this ? col.ToDouble() : GetDouble(col.Name);

    public Int16 GetInt16(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToInt16() : default;
    }

    public Int16 GetInt16(RecordColumn col) => col.Record == this ? col.ToInt16() : GetInt16(col.Name);

    public Int32 GetInt32(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToInt32() : default;
    }

    public Int32 GetInt32(RecordColumn col) => col.Record == this ? col.ToInt32() : GetInt32(col.Name);

    public Int64 GetInt64(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToInt64() : default;
    }

    public Int64 GetInt64(RecordColumn col) => col.Record == this ? col.ToInt64() : GetInt64(col.Name);

    public SByte GetSByte(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToSByte() : default;
    }

    public SByte GetSByte(RecordColumn col) => col.Record == this ? col.ToSByte() : GetSByte(col.Name);

    public Single GetSingle(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToSingle() : default;
    }

    public Single GetSingle(RecordColumn col) => col.Record == this ? col.ToSingle() : GetSingle(col.Name);

    public String? GetString(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToString() : default;
    }

    public String? GetString(RecordColumn col) => col.Record == this ? col.ToString() : GetString(col.Name);

    public UInt16 GetUInt16(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToUInt16() : default;
    }

    public UInt16 GetUInt16(RecordColumn col) => col.Record == this ? col.ToUInt16() : GetUInt16(col.Name);

    public UInt32 GetUInt32(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToUInt32() : default;
    }

    public UInt32 GetUInt32(RecordColumn col) => col.Record == this ? col.ToUInt32() : GetUInt32(col.Name);

    public UInt64 GetUInt64(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToUInt64() : default;
    }

    public UInt64 GetUInt64(RecordColumn col) => col.Record == this ? col.ToUInt64() : GetUInt64(col.Name);
    #endregion
}
