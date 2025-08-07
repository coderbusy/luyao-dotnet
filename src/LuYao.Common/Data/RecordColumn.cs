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
    /// 清空列中的所有数据。
    /// </summary>
    public abstract void Clear();

    /// <summary>
    /// 获取列的当前容量。
    /// </summary>
    /// <value>列能够容纳的最大行数。</value>
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

    #region To

    /// <summary>
    /// 获取指定行的值并转换为指定的泛型类型。
    /// </summary>
    /// <typeparam name="T">要转换到的目标类型。</typeparam>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的值，如果原值为 null 则返回类型 T 的默认值。</returns>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="row"/> 超出有效范围时抛出。</exception>
    /// <exception cref="InvalidCastException">当值无法转换为目标类型时抛出。</exception>
    public virtual T? To<T>(int row)
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
    public T? To<T>() => this.To<T>(this.Record.Cursor);

    /// <summary>
    /// 获取指定行的值并转换为布尔值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的布尔值。</returns>
    public virtual Boolean ToBoolean(int row) => Valid.ToBoolean(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为字节值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的字节值。</returns>
    public virtual Byte ToByte(int row) => Valid.ToByte(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为字符值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的字符值。</returns>
    public virtual Char ToChar(int row) => Valid.ToChar(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为日期时间值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的日期时间值。</returns>
    public virtual DateTime ToDateTime(int row) => Valid.ToDateTime(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为十进制数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的十进制数值。</returns>
    public virtual Decimal ToDecimal(int row) => Valid.ToDecimal(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为双精度浮点数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的双精度浮点数值。</returns>
    public virtual Double ToDouble(int row) => Valid.ToDouble(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为16位有符号整数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的16位有符号整数值。</returns>
    public virtual Int16 ToInt16(int row) => Valid.ToInt16(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为32位有符号整数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的32位有符号整数值。</returns>
    public virtual Int32 ToInt32(int row) => Valid.ToInt32(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为64位有符号整数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的64位有符号整数值。</returns>
    public virtual Int64 ToInt64(int row) => Valid.ToInt64(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为8位有符号整数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的8位有符号整数值。</returns>
    public virtual SByte ToSByte(int row) => Valid.ToSByte(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为单精度浮点数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的单精度浮点数值。</returns>
    public virtual Single ToSingle(int row) => Valid.ToSingle(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为字符串。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的字符串值。</returns>
    public virtual String ToString(int row) => Valid.ToString(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为16位无符号整数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的16位无符号整数值。</returns>
    public virtual UInt16 ToUInt16(int row) => Valid.ToUInt16(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为32位无符号整数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的32位无符号整数值。</returns>
    public virtual UInt32 ToUInt32(int row) => Valid.ToUInt32(this.GetValue(row));

    /// <summary>
    /// 获取指定行的值并转换为64位无符号整数值。
    /// </summary>
    /// <param name="row">行索引，从 0 开始。</param>
    /// <returns>转换后的64位无符号整数值。</returns>
    public virtual UInt64 ToUInt64(int row) => Valid.ToUInt64(this.GetValue(row));

    /// <summary>
    /// 获取当前游标位置的值并转换为布尔值。
    /// </summary>
    /// <returns>转换后的布尔值。</returns>
    public Boolean ToBoolean() => this.ToBoolean(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为字节值。
    /// </summary>
    /// <returns>转换后的字节值。</returns>
    public Byte ToByte() => this.ToByte(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为字符值。
    /// </summary>
    /// <returns>转换后的字符值。</returns>
    public Char ToChar() => this.ToChar(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为日期时间值。
    /// </summary>
    /// <returns>转换后的日期时间值。</returns>
    public DateTime ToDateTime() => this.ToDateTime(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为十进制数值。
    /// </summary>
    /// <returns>转换后的十进制数值。</returns>
    public Decimal ToDecimal() => this.ToDecimal(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为双精度浮点数值。
    /// </summary>
    /// <returns>转换后的双精度浮点数值。</returns>
    public Double ToDouble() => this.ToDouble(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为16位有符号整数值。
    /// </summary>
    /// <returns>转换后的16位有符号整数值。</returns>
    public Int16 ToInt16() => this.ToInt16(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为32位有符号整数值。
    /// </summary>
    /// <returns>转换后的32位有符号整数值。</returns>
    public Int32 ToInt32() => this.ToInt32(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为64位有符号整数值。
    /// </summary>
    /// <returns>转换后的64位有符号整数值。</returns>
    public Int64 ToInt64() => this.ToInt64(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为8位有符号整数值。
    /// </summary>
    /// <returns>转换后的8位有符号整数值。</returns>
    public SByte ToSByte() => this.ToSByte(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为单精度浮点数值。
    /// </summary>
    /// <returns>转换后的单精度浮点数值。</returns>
    public Single ToSingle() => this.ToSingle(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为字符串。
    /// </summary>
    /// <returns>转换后的字符串值。</returns>
    public override String ToString() => this.ToString(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为16位无符号整数值。
    /// </summary>
    /// <returns>转换后的16位无符号整数值。</returns>
    public UInt16 ToUInt16() => this.ToUInt16(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为32位无符号整数值。
    /// </summary>
    /// <returns>转换后的32位无符号整数值。</returns>
    public UInt32 ToUInt32() => this.ToUInt32(this.Record.Cursor);

    /// <summary>
    /// 获取当前游标位置的值并转换为64位无符号整数值。
    /// </summary>
    /// <returns>转换后的64位无符号整数值。</returns>
    public UInt64 ToUInt64() => this.ToUInt64(this.Record.Cursor);
    #endregion
}