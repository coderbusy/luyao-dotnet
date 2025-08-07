using System;

namespace LuYao.Data;

/// <summary>
/// 表示一个数据列。
/// </summary>
public abstract class RecordColumn
{
    /// <summary>
    /// 关联的记录
    /// </summary>
    public Record Record { get; }
    /// <summary>
    /// 创建一个数据列
    /// </summary>
    internal RecordColumn(Record record, string name, RecordDataType code, Type type)
    {
        this.Record = record ?? throw new ArgumentNullException(nameof(record));
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
        this.Code = code;
        this._type = type ?? throw new ArgumentNullException(nameof(type));
    }
    private Type _type;
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
    public abstract void SetValue(object? value, int row);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public abstract object? GetValue(int row);
    /// <summary>
    /// 清空列中的所有数据。
    /// </summary>
    public abstract void Clear();

    /// <summary>
    /// 容量
    /// </summary>
    public abstract int Capacity { get; }
    internal abstract void Extend(int length);

    internal void OnSet(int row)
    {
        if (row == 0 && this.Record.Count == 0)
        {
            this.Record.AddRow();
            return;
        }
        if (row < 0 || row >= this.Record.Count) throw new ArgumentOutOfRangeException(nameof(row), $"行索引 {row} 超出有效范围 [0, {Record.Count - 1}]");
    }
    internal void OnGet(int row)
    {
        if (row < 0 || row >= this.Record.Count) throw new ArgumentOutOfRangeException(nameof(row), $"行索引 {row} 超出有效范围 [0, {Record.Count - 1}]");
    }

    #region Set
    public virtual void Set(Boolean value, int row) => this.SetValue(value, row);
    public virtual void Set(Byte value, int row) => this.SetValue(value, row);
    public virtual void Set(Char value, int row) => this.SetValue(value, row);
    public virtual void Set(DateTime value, int row) => this.SetValue(value, row);
    public virtual void Set(Decimal value, int row) => this.SetValue(value, row);
    public virtual void Set(Double value, int row) => this.SetValue(value, row);
    public virtual void Set(Int16 value, int row) => this.SetValue(value, row);
    public virtual void Set(Int32 value, int row) => this.SetValue(value, row);
    public virtual void Set(Int64 value, int row) => this.SetValue(value, row);
    public virtual void Set(SByte value, int row) => this.SetValue(value, row);
    public virtual void Set(Single value, int row) => this.SetValue(value, row);
    public virtual void Set(String value, int row) => this.SetValue(value, row);
    public virtual void Set(UInt16 value, int row) => this.SetValue(value, row);
    public virtual void Set(UInt32 value, int row) => this.SetValue(value, row);
    public virtual void Set(UInt64 value, int row) => this.SetValue(value, row);

    public void Set(Boolean value) => this.Set(value, this.Record.Cursor);
    public void Set(Byte value) => this.Set(value, this.Record.Cursor);
    public void Set(Char value) => this.Set(value, this.Record.Cursor);
    public void Set(DateTime value) => this.Set(value, this.Record.Cursor);
    public void Set(Decimal value) => this.Set(value, this.Record.Cursor);
    public void Set(Double value) => this.Set(value, this.Record.Cursor);
    public void Set(Int16 value) => this.Set(value, this.Record.Cursor);
    public void Set(Int32 value) => this.Set(value, this.Record.Cursor);
    public void Set(Int64 value) => this.Set(value, this.Record.Cursor);
    public void Set(SByte value) => this.Set(value, this.Record.Cursor);
    public void Set(Single value) => this.Set(value, this.Record.Cursor);
    public void Set(String value) => this.Set(value, this.Record.Cursor);
    public void Set(UInt16 value) => this.Set(value, this.Record.Cursor);
    public void Set(UInt32 value) => this.Set(value, this.Record.Cursor);
    public void Set(UInt64 value) => this.Set(value, this.Record.Cursor);

    #endregion

    #region To

    public virtual T? To<T>(int row)
    {
        OnGet(row);
        object? value = this.GetValue(row);
        if (value is null) return default;
        if (value is T direct) return direct;
        return (T)Valid.To(value, typeof(T));
    }

    public T? To<T>() => this.To<T>(this.Record.Cursor);

    public virtual Boolean ToBoolean(int row) => Valid.ToBoolean(this.GetValue(row));
    public virtual Byte ToByte(int row) => Valid.ToByte(this.GetValue(row));
    public virtual Char ToChar(int row) => Valid.ToChar(this.GetValue(row));
    public virtual DateTime ToDateTime(int row) => Valid.ToDateTime(this.GetValue(row));
    public virtual Decimal ToDecimal(int row) => Valid.ToDecimal(this.GetValue(row));
    public virtual Double ToDouble(int row) => Valid.ToDouble(this.GetValue(row));
    public virtual Int16 ToInt16(int row) => Valid.ToInt16(this.GetValue(row));
    public virtual Int32 ToInt32(int row) => Valid.ToInt32(this.GetValue(row));
    public virtual Int64 ToInt64(int row) => Valid.ToInt64(this.GetValue(row));
    public virtual SByte ToSByte(int row) => Valid.ToSByte(this.GetValue(row));
    public virtual Single ToSingle(int row) => Valid.ToSingle(this.GetValue(row));
    public virtual String ToString(int row) => Valid.ToString(this.GetValue(row));
    public virtual UInt16 ToUInt16(int row) => Valid.ToUInt16(this.GetValue(row));
    public virtual UInt32 ToUInt32(int row) => Valid.ToUInt32(this.GetValue(row));
    public virtual UInt64 ToUInt64(int row) => Valid.ToUInt64(this.GetValue(row));

    public Boolean ToBoolean() => this.ToBoolean(this.Record.Cursor);
    public Byte ToByte() => this.ToByte(this.Record.Cursor);
    public Char ToChar() => this.ToChar(this.Record.Cursor);
    public DateTime ToDateTime() => this.ToDateTime(this.Record.Cursor);
    public Decimal ToDecimal() => this.ToDecimal(this.Record.Cursor);
    public Double ToDouble() => this.ToDouble(this.Record.Cursor);
    public Int16 ToInt16() => this.ToInt16(this.Record.Cursor);
    public Int32 ToInt32() => this.ToInt32(this.Record.Cursor);
    public Int64 ToInt64() => this.ToInt64(this.Record.Cursor);
    public SByte ToSByte() => this.ToSByte(this.Record.Cursor);
    public Single ToSingle() => this.ToSingle(this.Record.Cursor);
    public override String ToString() => this.ToString(this.Record.Cursor);
    public UInt16 ToUInt16() => this.ToUInt16(this.Record.Cursor);
    public UInt32 ToUInt32() => this.ToUInt32(this.Record.Cursor);
    public UInt64 ToUInt64() => this.ToUInt64(this.Record.Cursor);
    #endregion
}