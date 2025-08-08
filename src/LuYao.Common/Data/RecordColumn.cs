using System;

namespace LuYao.Data;

/// <summary>
/// 表示一个数据列。
/// </summary>
public abstract class RecordColumn
{
    /// <summary>
    /// 获取关联的记录实例。
    /// </summary>
    /// <value>包含此列的 <see cref="Record"/> 对象。</value>
    public Record Record { get; }

    /// <summary>
    /// 创建一个数据列实例。
    /// </summary>
    /// <param name="record">关联的记录实例。</param>
    /// <param name="name">列的名称。</param>
    /// <param name="code">列的数据类型代码。</param>
    /// <param name="type">列的实际数据类型。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="record"/>、<paramref name="name"/> 或 <paramref name="type"/> 为 null 时抛出。</exception>
    internal RecordColumn(Record record, string name, RecordDataCode code, Type type)
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
    /// <value>列的名称字符串。</value>
    public string Name { get; }

    /// <summary>
    /// 获取列的数据类型代码。
    /// </summary>
    /// <value>表示列数据类型的 <see cref="RecordDataCode"/> 枚举值。</value>
    public RecordDataCode Code { get; }

    /// <summary>
    /// 获取列的实际数据类型。
    /// </summary>
    /// <value>表示列实际数据类型的 <see cref="Type"/> 对象。</value>
    public Type Type => this._type;

    /// <summary>
    /// 在指定行设置列的值。
    /// </summary>
    /// <param name="value">要设置的值，可以为 null。</param>
    /// <param name="row">目标行索引，从 0 开始。</param>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="row"/> 超出有效范围时抛出。</exception>
    public abstract void SetValue(object? value, int row);

    /// <summary>
    /// 获取指定行的列值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>指定行的列值，可能为 null。</returns>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="row"/> 超出有效范围时抛出。</exception>
    public abstract object? GetValue(int row);

    /// <summary>
    /// 删除指定行的列值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    public abstract void Delete(int row);

    /// <summary>
    /// 清空列中的所有数据。
    /// </summary>
    public abstract void Clear();

    /// <summary>
    /// 获取列的当前容量。
    /// </summary>
    /// <value>列能够容纳的最大行数。</value>
    public abstract int Capacity { get; }
    internal abstract void Extend(int length);

    ///  <inheritdoc/>
    public override string ToString()
    {
        if (this.Code != RecordDataCode.Object)
        {
            return $"{this.Name},{this.Code}";
        }
        else
        {
            return $"{this.Name},{this.Type.FullName}";
        }
    }

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
    /// <summary>
    /// 在指定行设置布尔值。
    /// </summary>
    /// <param name="value">要设置的布尔值。</param>
    /// <param name="row">目标行索引。</param>
    public virtual void Set(Boolean value, int row) => this.SetValue(value, row);

    /// <summary>
    /// 在指定行设置字节值。
    /// </summary>
    /// <param name="value">要设置的字节值。</param>
    /// <param name="row">目标行索引。</param>
    public virtual void Set(Byte value, int row) => this.SetValue(value, row);

    /// <summary>
    /// 在指定行设置字符值。
    /// </summary>
    /// <param name="value">要设置的字符值。</param>
    /// <param name="row">目标行索引。</param>
    public virtual void Set(Char value, int row) => this.SetValue(value, row);

    /// <summary>
    /// 在指定行设置日期时间值。
    /// </summary>
    /// <param name="value">要设置的日期时间值。</param>
    /// <param name="row">目标行索引。</param>
    public virtual void Set(DateTime value, int row) => this.SetValue(value, row);

    /// <summary>
    /// 在指定行设置十进制数值。
    /// </summary>
    /// <param name="value">要设置的十进制数值。</param>
    /// <param name="row">目标行索引。</param>
    public virtual void Set(Decimal value, int row) => this.SetValue(value, row);

    /// <summary>
    /// 在指定行设置双精度浮点数值。
    /// </summary>
    /// <param name="value">要设置的双精度浮点数值。</param>
    /// <param name="row">目标行索引。</param>
    public virtual void Set(Double value, int row) => this.SetValue(value, row);

    /// <summary>
    /// 在指定行设置16位有符号整数值。
    /// </summary>
    /// <param name="value">要设置的16位有符号整数值。</param>
    /// <param name="row">目标行索引。</param>
    public virtual void Set(Int16 value, int row) => this.SetValue(value, row);

    /// <summary>
    /// 在指定行设置32位有符号整数值。
    /// </summary>
    /// <param name="value">要设置的32位有符号整数值。</param>
    /// <param name="row">目标行索引。</param>
    public virtual void Set(Int32 value, int row) => this.SetValue(value, row);

    /// <summary>
    /// 在指定行设置64位有符号整数值。
    /// </summary>
    /// <param name="value">要设置的64位有符号整数值。</param>
    /// <param name="row">目标行索引。</param>
    public virtual void Set(Int64 value, int row) => this.SetValue(value, row);

    /// <summary>
    /// 在指定行设置8位有符号整数值。
    /// </summary>
    /// <param name="value">要设置的8位有符号整数值。</param>
    /// <param name="row">目标行索引。</param>
    public virtual void Set(SByte value, int row) => this.SetValue(value, row);

    /// <summary>
    /// 在指定行设置单精度浮点数值。
    /// </summary>
    /// <param name="value">要设置的单精度浮点数值。</param>
    /// <param name="row">目标行索引。</param>
    public virtual void Set(Single value, int row) => this.SetValue(value, row);

    /// <summary>
    /// 在指定行设置字符串值。
    /// </summary>
    /// <param name="value">要设置的字符串值。</param>
    /// <param name="row">目标行索引。</param>
    public virtual void Set(String value, int row) => this.SetValue(value, row);

    /// <summary>
    /// 在指定行设置16位无符号整数值。
    /// </summary>
    /// <param name="value">要设置的16位无符号整数值。</param>
    /// <param name="row">目标行索引。</param>
    public virtual void Set(UInt16 value, int row) => this.SetValue(value, row);

    /// <summary>
    /// 在指定行设置32位无符号整数值。
    /// </summary>
    /// <param name="value">要设置的32位无符号整数值。</param>
    /// <param name="row">目标行索引。</param>
    public virtual void Set(UInt32 value, int row) => this.SetValue(value, row);

    /// <summary>
    /// 在指定行设置64位无符号整数值。
    /// </summary>
    /// <param name="value">要设置的64位无符号整数值。</param>
    /// <param name="row">目标行索引。</param>
    public virtual void Set(UInt64 value, int row) => this.SetValue(value, row);

    /// <summary>
    /// 在当前游标位置设置布尔值。
    /// </summary>
    /// <param name="value">要设置的布尔值。</param>
    public void Set(Boolean value) => this.Set(value, this.Record.Cursor);

    /// <summary>
    /// 在当前游标位置设置字节值。
    /// </summary>
    /// <param name="value">要设置的字节值。</param>
    public void Set(Byte value) => this.Set(value, this.Record.Cursor);

    /// <summary>
    /// 在当前游标位置设置字符值。
    /// </summary>
    /// <param name="value">要设置的字符值。</param>
    public void Set(Char value) => this.Set(value, this.Record.Cursor);

    /// <summary>
    /// 在当前游标位置设置日期时间值。
    /// </summary>
    /// <param name="value">要设置的日期时间值。</param>
    public void Set(DateTime value) => this.Set(value, this.Record.Cursor);

    /// <summary>
    /// 在当前游标位置设置十进制数值。
    /// </summary>
    /// <param name="value">要设置的十进制数值。</param>
    public void Set(Decimal value) => this.Set(value, this.Record.Cursor);

    /// <summary>
    /// 在当前游标位置设置双精度浮点数值。
    /// </summary>
    /// <param name="value">要设置的双精度浮点数值。</param>
    public void Set(Double value) => this.Set(value, this.Record.Cursor);

    /// <summary>
    /// 在当前游标位置设置16位有符号整数值。
    /// </summary>
    /// <param name="value">要设置的16位有符号整数值。</param>
    public void Set(Int16 value) => this.Set(value, this.Record.Cursor);

    /// <summary>
    /// 在当前游标位置设置32位有符号整数值。
    /// </summary>
    /// <param name="value">要设置的32位有符号整数值。</param>
    public void Set(Int32 value) => this.Set(value, this.Record.Cursor);

    /// <summary>
    /// 在当前游标位置设置64位有符号整数值。
    /// </summary>
    /// <param name="value">要设置的64位有符号整数值。</param>
    public void Set(Int64 value) => this.Set(value, this.Record.Cursor);

    /// <summary>
    /// 在当前游标位置设置8位有符号整数值。
    /// </summary>
    /// <param name="value">要设置的8位有符号整数值。</param>
    public void Set(SByte value) => this.Set(value, this.Record.Cursor);

    /// <summary>
    /// 在当前游标位置设置单精度浮点数值。
    /// </summary>
    /// <param name="value">要设置的单精度浮点数值。</param>
    public void Set(Single value) => this.Set(value, this.Record.Cursor);

    /// <summary>
    /// 在当前游标位置设置字符串值。
    /// </summary>
    /// <param name="value">要设置的字符串值。</param>
    public void Set(String value) => this.Set(value, this.Record.Cursor);

    /// <summary>
    /// 在当前游标位置设置16位无符号整数值。
    /// </summary>
    /// <param name="value">要设置的16位无符号整数值。</param>
    public void Set(UInt16 value) => this.Set(value, this.Record.Cursor);

    /// <summary>
    /// 在当前游标位置设置32位无符号整数值。
    /// </summary>
    /// <param name="value">要设置的32位无符号整数值。</param>
    public void Set(UInt32 value) => this.Set(value, this.Record.Cursor);

    /// <summary>
    /// 在当前游标位置设置64位无符号整数值。
    /// </summary>
    /// <param name="value">要设置的64位无符号整数值。</param>
    public void Set(UInt64 value) => this.Set(value, this.Record.Cursor);

    #endregion

    #region Get

    /// <summary>
    /// 获取指定行的值并转换为指定的泛型类型。
    /// </summary>
    /// <typeparam name="T">要转换到的目标类型。</typeparam>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的值，如果原值为 null 则返回类型 T 的默认值。</returns>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="row"/> 超出有效范围时抛出。</exception>
    /// <exception cref="InvalidCastException">当值无法转换为目标类型时抛出。</exception>
    public virtual T? Get<T>(int row)
    {
        OnGet(row);
        object? value = this.GetValue(row);
        if (value is null) return default;
        if (value is T direct) return direct;
        return (T)Valid.To(value, typeof(T));
    }

    /// <summary>
    /// 获取当前游标位置的值并转换为指定的泛型类型。
    /// </summary>
    /// <typeparam name="T">要转换到的目标类型。</typeparam>
    /// <returns>转换后的值，如果原值为 null 则返回类型 T 的默认值。</returns>
    /// <exception cref="InvalidCastException">当值无法转换为目标类型时抛出。</exception>
    public T? Get<T>() => this.Get<T>(this.Record.Cursor);

    /// <summary>
    /// 获取指定行的值并转换为布尔值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的布尔值。</returns>
    public virtual Boolean GetBoolean(int row) => Valid.ToBoolean(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为字节值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的字节值。</returns>
    public virtual Byte GetByte(int row) => Valid.ToByte(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为字符值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的字符值。</returns>
    public virtual Char GetChar(int row) => Valid.ToChar(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为日期时间值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的日期时间值。</returns>
    public virtual DateTime GetDateTime(int row) => Valid.ToDateTime(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为十进制数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的十进制数值。</returns>
    public virtual Decimal GetDecimal(int row) => Valid.ToDecimal(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为双精度浮点数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的双精度浮点数值。</returns>
    public virtual Double GetDouble(int row) => Valid.ToDouble(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为16位有符号整数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的16位有符号整数值。</returns>
    public virtual Int16 GetInt16(int row) => Valid.ToInt16(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为32位有符号整数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的32位有符号整数值。</returns>
    public virtual Int32 GetInt32(int row) => Valid.ToInt32(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为64位有符号整数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的64位有符号整数值。</returns>
    public virtual Int64 GetInt64(int row) => Valid.ToInt64(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为8位有符号整数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的8位有符号整数值。</returns>
    public virtual SByte GetSByte(int row) => Valid.ToSByte(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为单精度浮点数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的单精度浮点数值。</returns>
    public virtual Single GetSingle(int row) => Valid.ToSingle(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为字符串。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的字符串值。</returns>
    public virtual String GetString(int row) => Valid.ToString(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为16位无符号整数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的16位无符号整数值。</returns>
    public virtual UInt16 GetUInt16(int row) => Valid.ToUInt16(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为32位无符号整数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的32位无符号整数值。</returns>
    public virtual UInt32 GetUInt32(int row) => Valid.ToUInt32(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为64位无符号整数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的64位无符号整数值。</returns>
    public virtual UInt64 GetUInt64(int row) => Valid.ToUInt64(this.GetValue(row));

    /// <summary>
    /// 获取当前游标位置的值并转换为布尔值。
    /// </summary>
    /// <returns>转换后的布尔值。</returns>
    public Boolean GetBoolean() => this.GetBoolean(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为字节值。
    /// </summary>
    /// <returns>转换后的字节值。</returns>
    public Byte GetByte() => this.GetByte(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为字符值。
    /// </summary>
    /// <returns>转换后的字符值。</returns>
    public Char GetChar() => this.GetChar(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为日期时间值。
    /// </summary>
    /// <returns>转换后的日期时间值。</returns>
    public DateTime GetDateTime() => this.GetDateTime(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为十进制数值。
    /// </summary>
    /// <returns>转换后的十进制数值。</returns>
    public Decimal GetDecimal() => this.GetDecimal(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为双精度浮点数值。
    /// </summary>
    /// <returns>转换后的双精度浮点数值。</returns>
    public Double GetDouble() => this.GetDouble(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为16位有符号整数值。
    /// </summary>
    /// <returns>转换后的16位有符号整数值。</returns>
    public Int16 GetInt16() => this.GetInt16(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为32位有符号整数值。
    /// </summary>
    /// <returns>转换后的32位有符号整数值。</returns>
    public Int32 GetInt32() => this.GetInt32(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为64位有符号整数值。
    /// </summary>
    /// <returns>转换后的64位有符号整数值。</returns>
    public Int64 GetInt64() => this.GetInt64(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为8位有符号整数值。
    /// </summary>
    /// <returns>转换后的8位有符号整数值。</returns>
    public SByte GetSByte() => this.GetSByte(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为单精度浮点数值。
    /// </summary>
    /// <returns>转换后的单精度浮点数值。</returns>
    public Single GetSingle() => this.GetSingle(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为字符串。
    /// </summary>
    /// <returns>转换后的字符串值。</returns>
    public String GetString() => this.GetString(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为16位无符号整数值。
    /// </summary>
    /// <returns>转换后的16位无符号整数值。</returns>
    public UInt16 GetUInt16() => this.GetUInt16(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为32位无符号整数值。
    /// </summary>
    /// <returns>转换后的32位无符号整数值。</returns>
    public UInt32 GetUInt32() => this.GetUInt32(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为64位无符号整数值。
    /// </summary>
    /// <returns>转换后的64位无符号整数值。</returns>
    public UInt64 GetUInt64() => this.GetUInt64(this.Record.Cursor);
    #endregion
}