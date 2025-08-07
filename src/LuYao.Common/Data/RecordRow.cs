using System;

namespace LuYao.Data;

/// <summary>
/// 代表一行数据
/// </summary>
public struct RecordRow : IRecordCursor
{
    internal RecordRow(Record record, int row)
    {
        if (row < 0 || row >= record.Count) throw new ArgumentOutOfRangeException(nameof(row));
        this.Record = record ?? throw new ArgumentNullException(nameof(record));
        this.Row = row;
    }
    /// <summary>
    /// 集合
    /// </summary>
    public Record Record { get; }

    /// <summary>
    /// 行号
    /// </summary>
    public int Row { get; }

    /// <summary>
    /// 隐式转换为行索引（int）。
    /// </summary>
    /// <param name="rowRef">要转换的 <see cref="RecordRow"/> 实例。</param>
    /// <returns>该行的索引。</returns>
    public static implicit operator int(RecordRow rowRef) => rowRef.Row;

    #region IRecordCursor
    public Boolean GetBoolean(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.ToBoolean() : default;
    }

    public Boolean GetBoolean(RecordColumn col) => col.Record == this.Record ? col.ToBoolean() : GetBoolean(col.Name);

    public Byte GetByte(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.ToByte() : default;
    }

    public Byte GetByte(RecordColumn col) => col.Record == this.Record ? col.ToByte() : GetByte(col.Name);

    public Char GetChar(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.ToChar() : default;
    }

    public Char GetChar(RecordColumn col) => col.Record == this.Record ? col.ToChar() : GetChar(col.Name);

    public DateTime GetDateTime(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.ToDateTime() : default;
    }

    public DateTime GetDateTime(RecordColumn col) => col.Record == this.Record ? col.ToDateTime() : GetDateTime(col.Name);

    public Decimal GetDecimal(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.ToDecimal() : default;
    }

    public Decimal GetDecimal(RecordColumn col) => col.Record == this.Record ? col.ToDecimal() : GetDecimal(col.Name);

    public Double GetDouble(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.ToDouble() : default;
    }

    public Double GetDouble(RecordColumn col) => col.Record == this.Record ? col.ToDouble() : GetDouble(col.Name);

    public Int16 GetInt16(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.ToInt16() : default;
    }

    public Int16 GetInt16(RecordColumn col) => col.Record == this.Record ? col.ToInt16() : GetInt16(col.Name);

    public Int32 GetInt32(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.ToInt32() : default;
    }

    public Int32 GetInt32(RecordColumn col) => col.Record == this.Record ? col.ToInt32() : GetInt32(col.Name);

    public Int64 GetInt64(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.ToInt64() : default;
    }

    public Int64 GetInt64(RecordColumn col) => col.Record == this.Record ? col.ToInt64() : GetInt64(col.Name);

    public SByte GetSByte(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.ToSByte() : default;
    }

    public SByte GetSByte(RecordColumn col) => col.Record == this.Record ? col.ToSByte() : GetSByte(col.Name);

    public Single GetSingle(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.ToSingle() : default;
    }

    public Single GetSingle(RecordColumn col) => col.Record == this.Record ? col.ToSingle() : GetSingle(col.Name);

    public String? GetString(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.ToString() : default;
    }

    public String? GetString(RecordColumn col) => col.Record == this.Record ? col.ToString() : GetString(col.Name);

    public UInt16 GetUInt16(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.ToUInt16() : default;
    }

    public UInt16 GetUInt16(RecordColumn col) => col.Record == this.Record ? col.ToUInt16() : GetUInt16(col.Name);

    public UInt32 GetUInt32(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.ToUInt32() : default;
    }

    public UInt32 GetUInt32(RecordColumn col) => col.Record == this.Record ? col.ToUInt32() : GetUInt32(col.Name);

    public UInt64 GetUInt64(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.ToUInt64() : default;
    }

    public UInt64 GetUInt64(RecordColumn col) => col.Record == this.Record ? col.ToUInt64() : GetUInt64(col.Name);

    public T? Get<T>(RecordColumn col) => col.Record == this.Record ? col.To<T>() : default;
    public T? Get<T>(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.To<T>() : default;
    }

    #endregion
}