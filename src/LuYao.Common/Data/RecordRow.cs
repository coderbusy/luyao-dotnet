using System;

namespace LuYao.Data;

/// <summary>
/// 代表一行数据，提供对列存储数据集合中特定行数据的访问。
/// 实现了 <see cref="IRecordCursor"/> 接口，支持类型安全的数据读取操作。
/// </summary>
public struct RecordRow : IRecordCursor
{
    /// <summary>
    /// 初始化 <see cref="RecordRow"/> 结构体的新实例。
    /// </summary>
    /// <param name="record">包含此行的数据集合。</param>
    /// <param name="row">行索引，必须在有效范围内。</param>
    /// <exception cref="ArgumentOutOfRangeException">当行索引超出记录范围时抛出。</exception>
    /// <exception cref="ArgumentNullException">当记录参数为 null 时抛出。</exception>
    internal RecordRow(Record record, int row)
    {
        this.Record = record ?? throw new ArgumentNullException(nameof(record));
        if (row < 0 || row >= record.Count) throw new ArgumentOutOfRangeException(nameof(row));
        this.Row = row;
    }

    /// <summary>
    /// 获取包含此行的数据集合。
    /// </summary>
    /// <value>关联的 <see cref="Record"/> 实例。</value>
    public Record Record { get; }

    /// <summary>
    /// 获取当前行在数据集合中的索引位置。
    /// </summary>
    /// <value>从零开始的行索引。</value>
    public int Row { get; }

    /// <summary>
    /// 定义从 <see cref="RecordRow"/> 到 <see cref="int"/> 的隐式转换。
    /// 返回行的索引位置。
    /// </summary>
    /// <param name="rowRef">要转换的 <see cref="RecordRow"/> 实例。</param>
    /// <returns>该行在数据集合中的索引位置。</returns>
    public static implicit operator int(RecordRow rowRef) => rowRef.Row;

    #region IRecordCursor 实现

    /// <summary>
    /// 根据列名获取当前行指定列的布尔值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的布尔值，否则返回 false。</returns>
    public Boolean GetBoolean(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.GetBoolean(this.Row) : default;
    }

    /// <summary>
    /// 根据列对象获取当前行指定列的布尔值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回布尔值，否则通过列名查找获取。</returns>
    public Boolean GetBoolean(RecordColumn col) => col.Record == this.Record ? col.GetBoolean(this.Row) : GetBoolean(col.Name);

    /// <summary>
    /// 根据列名获取当前行指定列的字节值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的字节值，否则返回 0。</returns>
    public Byte GetByte(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.GetByte(this.Row) : default;
    }

    /// <summary>
    /// 根据列对象获取当前行指定列的字节值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回字节值，否则通过列名查找获取。</returns>
    public Byte GetByte(RecordColumn col) => col.Record == this.Record ? col.GetByte(this.Row) : GetByte(col.Name);

    /// <summary>
    /// 根据列名获取当前行指定列的字符值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的字符值，否则返回 '\0'。</returns>
    public Char GetChar(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.GetChar(this.Row) : default;
    }

    /// <summary>
    /// 根据列对象获取当前行指定列的字符值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回字符值，否则通过列名查找获取。</returns>
    public Char GetChar(RecordColumn col) => col.Record == this.Record ? col.GetChar(this.Row) : GetChar(col.Name);

    /// <summary>
    /// 根据列名获取当前行指定列的日期时间值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的日期时间值，否则返回默认日期时间。</returns>
    public DateTime GetDateTime(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.GetDateTime(this.Row) : default;
    }

    /// <summary>
    /// 根据列对象获取当前行指定列的日期时间值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回日期时间值，否则通过列名查找获取。</returns>
    public DateTime GetDateTime(RecordColumn col) => col.Record == this.Record ? col.GetDateTime(this.Row) : GetDateTime(col.Name);

    /// <summary>
    /// 根据列名获取当前行指定列的十进制数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的十进制数值，否则返回 0。</returns>
    public Decimal GetDecimal(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.GetDecimal(this.Row) : default;
    }

    /// <summary>
    /// 根据列对象获取当前行指定列的十进制数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回十进制数值，否则通过列名查找获取。</returns>
    public Decimal GetDecimal(RecordColumn col) => col.Record == this.Record ? col.GetDecimal(this.Row) : GetDecimal(col.Name);

    /// <summary>
    /// 根据列名获取当前行指定列的双精度浮点数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的双精度浮点数值，否则返回 0.0。</returns>
    public Double GetDouble(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.GetDouble(this.Row) : default;
    }

    /// <summary>
    /// 根据列对象获取当前行指定列的双精度浮点数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回双精度浮点数值，否则通过列名查找获取。</returns>
    public Double GetDouble(RecordColumn col) => col.Record == this.Record ? col.GetDouble(this.Row) : GetDouble(col.Name);

    /// <summary>
    /// 根据列名获取当前行指定列的16位有符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的16位有符号整数值，否则返回 0。</returns>
    public Int16 GetInt16(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.GetInt16(this.Row) : default;
    }

    /// <summary>
    /// 根据列对象获取当前行指定列的16位有符号整数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回16位有符号整数值，否则通过列名查找获取。</returns>
    public Int16 GetInt16(RecordColumn col) => col.Record == this.Record ? col.GetInt16(this.Row) : GetInt16(col.Name);

    /// <summary>
    /// 根据列名获取当前行指定列的32位有符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的32位有符号整数值，否则返回 0。</returns>
    public Int32 GetInt32(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.GetInt32(this.Row) : default;
    }

    /// <summary>
    /// 根据列对象获取当前行指定列的32位有符号整数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回32位有符号整数值，否则通过列名查找获取。</returns>
    public Int32 GetInt32(RecordColumn col) => col.Record == this.Record ? col.GetInt32(this.Row) : GetInt32(col.Name);

    /// <summary>
    /// 根据列名获取当前行指定列的64位有符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的64位有符号整数值，否则返回 0。</returns>
    public Int64 GetInt64(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.GetInt64(this.Row) : default;
    }

    /// <summary>
    /// 根据列对象获取当前行指定列的64位有符号整数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回64位有符号整数值，否则通过列名查找获取。</returns>
    public Int64 GetInt64(RecordColumn col) => col.Record == this.Record ? col.GetInt64(this.Row) : GetInt64(col.Name);

    /// <summary>
    /// 根据列名获取当前行指定列的8位有符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的8位有符号整数值，否则返回 0。</returns>
    public SByte GetSByte(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.GetSByte(this.Row) : default;
    }

    /// <summary>
    /// 根据列对象获取当前行指定列的8位有符号整数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回8位有符号整数值，否则通过列名查找获取。</returns>
    public SByte GetSByte(RecordColumn col) => col.Record == this.Record ? col.GetSByte(this.Row) : GetSByte(col.Name);

    /// <summary>
    /// 根据列名获取当前行指定列的单精度浮点数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的单精度浮点数值，否则返回 0.0f。</returns>
    public Single GetSingle(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.GetSingle(this.Row) : default;
    }

    /// <summary>
    /// 根据列对象获取当前行指定列的单精度浮点数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回单精度浮点数值，否则通过列名查找获取。</returns>
    public Single GetSingle(RecordColumn col) => col.Record == this.Record ? col.GetSingle(this.Row) : GetSingle(col.Name);

    /// <summary>
    /// 根据列名获取当前行指定列的字符串值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的字符串值，否则返回 null。</returns>
    public String? GetString(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.GetString(this.Row) : default;
    }

    /// <summary>
    /// 根据列对象获取当前行指定列的字符串值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回字符串值，否则通过列名查找获取。</returns>
    public String? GetString(RecordColumn col) => col.Record == this.Record ? col.GetString(this.Row) : GetString(col.Name);

    /// <summary>
    /// 根据列名获取当前行指定列的16位无符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的16位无符号整数值，否则返回 0。</returns>
    public UInt16 GetUInt16(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.GetUInt16(this.Row) : default;
    }

    /// <summary>
    /// 根据列对象获取当前行指定列的16位无符号整数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回16位无符号整数值，否则通过列名查找获取。</returns>
    public UInt16 GetUInt16(RecordColumn col) => col.Record == this.Record ? col.GetUInt16(this.Row) : GetUInt16(col.Name);

    /// <summary>
    /// 根据列名获取当前行指定列的32位无符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的32位无符号整数值，否则返回 0。</returns>
    public UInt32 GetUInt32(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.GetUInt32(this.Row) : default;
    }

    /// <summary>
    /// 根据列对象获取当前行指定列的32位无符号整数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回32位无符号整数值，否则通过列名查找获取。</returns>
    public UInt32 GetUInt32(RecordColumn col) => col.Record == this.Record ? col.GetUInt32(this.Row) : GetUInt32(col.Name);

    /// <summary>
    /// 根据列名获取当前行指定列的64位无符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的64位无符号整数值，否则返回 0。</returns>
    public UInt64 GetUInt64(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.GetUInt64(this.Row) : default;
    }

    /// <summary>
    /// 根据列对象获取当前行指定列的64位无符号整数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回64位无符号整数值，否则通过列名查找获取。</returns>
    public UInt64 GetUInt64(RecordColumn col) => col.Record == this.Record ? col.GetUInt64(this.Row) : GetUInt64(col.Name);

    /// <summary>
    /// 根据列对象获取当前行指定列的泛型类型值。
    /// </summary>
    /// <typeparam name="T">要获取的值的类型。</typeparam>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则直接返回转换后的泛型类型值，否则返回该类型的默认值。</returns>
    public T? Get<T>(RecordColumn col) => col.Record == this.Record ? col.Get<T>(this.Row) : default;

    /// <summary>
    /// 根据列名获取当前行指定列的泛型类型值。
    /// </summary>
    /// <typeparam name="T">要获取的值的类型。</typeparam>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的泛型类型值，否则返回该类型的默认值。</returns>
    public T? Get<T>(string name)
    {
        var col = this.Record.Columns.Find(name);
        return col != null ? col.Get<T>(this.Row) : default;
    }

    #endregion
}