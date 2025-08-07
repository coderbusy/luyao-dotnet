using System;

namespace LuYao.Data;

/// <summary>
/// 记录游标接口，提供从列存储数据集合中读取各种数据类型值的功能。
/// 此接口被 <see cref="Record"/> 和 <see cref="RecordRow"/> 实现，
/// 支持通过列名或列对象访问当前游标位置的数据。
/// </summary>
interface IRecordCursor
{
    /// <summary>
    /// 根据列名获取布尔值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的布尔值，否则返回默认值 false。</returns>
    Boolean GetBoolean(string name);

    /// <summary>
    /// 根据列对象获取布尔值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的布尔值，否则通过列名查找获取。</returns>
    Boolean GetBoolean(RecordColumn col);

    /// <summary>
    /// 根据列名获取字节值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的字节值，否则返回默认值 0。</returns>
    Byte GetByte(string name);

    /// <summary>
    /// 根据列对象获取字节值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的字节值，否则通过列名查找获取。</returns>
    Byte GetByte(RecordColumn col);

    /// <summary>
    /// 根据列名获取字符值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的字符值，否则返回默认值 '\0'。</returns>
    Char GetChar(string name);

    /// <summary>
    /// 根据列对象获取字符值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的字符值，否则通过列名查找获取。</returns>
    Char GetChar(RecordColumn col);

    /// <summary>
    /// 根据列名获取日期时间值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的日期时间值，否则返回默认值。</returns>
    DateTime GetDateTime(string name);

    /// <summary>
    /// 根据列对象获取日期时间值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的日期时间值，否则通过列名查找获取。</returns>
    DateTime GetDateTime(RecordColumn col);

    /// <summary>
    /// 根据列名获取十进制数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的十进制数值，否则返回默认值 0。</returns>
    Decimal GetDecimal(string name);

    /// <summary>
    /// 根据列对象获取十进制数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的十进制数值，否则通过列名查找获取。</returns>
    Decimal GetDecimal(RecordColumn col);

    /// <summary>
    /// 根据列名获取双精度浮点数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的双精度浮点数值，否则返回默认值 0.0。</returns>
    Double GetDouble(string name);

    /// <summary>
    /// 根据列对象获取双精度浮点数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的双精度浮点数值，否则通过列名查找获取。</returns>
    Double GetDouble(RecordColumn col);

    /// <summary>
    /// 根据列名获取16位有符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的16位有符号整数值，否则返回默认值 0。</returns>
    Int16 GetInt16(string name);

    /// <summary>
    /// 根据列对象获取16位有符号整数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的16位有符号整数值，否则通过列名查找获取。</returns>
    Int16 GetInt16(RecordColumn col);

    /// <summary>
    /// 根据列名获取32位有符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的32位有符号整数值，否则返回默认值 0。</returns>
    Int32 GetInt32(string name);

    /// <summary>
    /// 根据列对象获取32位有符号整数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的32位有符号整数值，否则通过列名查找获取。</returns>
    Int32 GetInt32(RecordColumn col);

    /// <summary>
    /// 根据列名获取64位有符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的64位有符号整数值，否则返回默认值 0。</returns>
    Int64 GetInt64(string name);

    /// <summary>
    /// 根据列对象获取64位有符号整数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的64位有符号整数值，否则通过列名查找获取。</returns>
    Int64 GetInt64(RecordColumn col);

    /// <summary>
    /// 根据列名获取8位有符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的8位有符号整数值，否则返回默认值 0。</returns>
    SByte GetSByte(string name);

    /// <summary>
    /// 根据列对象获取8位有符号整数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的8位有符号整数值，否则通过列名查找获取。</returns>
    SByte GetSByte(RecordColumn col);

    /// <summary>
    /// 根据列名获取单精度浮点数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的单精度浮点数值，否则返回默认值 0.0f。</returns>
    Single GetSingle(string name);

    /// <summary>
    /// 根据列对象获取单精度浮点数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的单精度浮点数值，否则通过列名查找获取。</returns>
    Single GetSingle(RecordColumn col);

    /// <summary>
    /// 根据列名获取字符串值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的字符串值，否则返回 null。</returns>
    String? GetString(string name);

    /// <summary>
    /// 根据列对象获取字符串值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的字符串值，否则通过列名查找获取。</returns>
    String? GetString(RecordColumn col);

    /// <summary>
    /// 根据列名获取16位无符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的16位无符号整数值，否则返回默认值 0。</returns>
    UInt16 GetUInt16(string name);

    /// <summary>
    /// 根据列对象获取16位无符号整数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的16位无符号整数值，否则通过列名查找获取。</returns>
    UInt16 GetUInt16(RecordColumn col);

    /// <summary>
    /// 根据列名获取32位无符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的32位无符号整数值，否则返回默认值 0。</returns>
    UInt32 GetUInt32(string name);

    /// <summary>
    /// 根据列对象获取32位无符号整数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的32位无符号整数值，否则通过列名查找获取。</returns>
    UInt32 GetUInt32(RecordColumn col);

    /// <summary>
    /// 根据列名获取64位无符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的64位无符号整数值，否则返回默认值 0。</returns>
    UInt64 GetUInt64(string name);

    /// <summary>
    /// 根据列对象获取64位无符号整数值。
    /// </summary>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的64位无符号整数值，否则通过列名查找获取。</returns>
    UInt64 GetUInt64(RecordColumn col);

    /// <summary>
    /// 根据列名获取指定泛型类型的值。
    /// </summary>
    /// <typeparam name="T">要获取的值的类型。</typeparam>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的泛型类型值，否则返回该类型的默认值。</returns>
    T? Get<T>(string name);

    /// <summary>
    /// 根据列对象获取指定泛型类型的值。
    /// </summary>
    /// <typeparam name="T">要获取的值的类型。</typeparam>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的泛型类型值，否则返回该类型的默认值。</returns>
    T? Get<T>(RecordColumn col);
}