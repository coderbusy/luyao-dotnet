using LuYao.Data.Models;
using System;
using System.Collections.Generic;

namespace LuYao.Data;

/// <summary>
/// 记录保存适配器基类，提供将记录数据写入不同格式的抽象接口。
/// </summary>
public abstract class RecordSaveAdapter
{
    /// <summary>
    /// 获取记录的布局结构，定义了数据写入的顺序和组织方式。
    /// </summary>
    /// <value>包含记录各部分的只读列表，通常包括头部、列信息和行数据等部分。</value>
    public abstract IReadOnlyList<RecordSection> Layout { get; }

    /// <summary>
    /// 开始写入记录数据。此方法在整个写入过程的开始阶段调用。
    /// </summary>
    public abstract void WriteStart();

    /// <summary>
    /// 结束写入记录数据。此方法在整个写入过程的结束阶段调用。
    /// </summary>
    public abstract void WriteEnd();

    /// <summary>
    /// 开始写入指定的记录部分。
    /// </summary>
    /// <param name="section">要开始写入的记录部分，如头部、列或行。</param>
    public abstract void WriteStartSection(RecordSection section);

    /// <summary>
    /// 结束当前记录部分的写入。
    /// </summary>
    public abstract void WriteEndSection();

    /// <summary>
    /// 写入记录头部信息。
    /// </summary>
    /// <param name="header">包含记录元数据的头部信息，如记录名称、列数和记录数等。</param>
    public abstract void WriteHeader(RecordHeader header);

    /// <summary>
    /// 写入列信息。
    /// </summary>
    /// <param name="column">要写入的列信息，包含列名、数据类型等属性。</param>
    public abstract void WriteColumn(RecordColumnInfo column);

    /// <summary>
    /// 开始写入一行数据。
    /// </summary>
    public abstract void WriteStarRow();

    /// <summary>
    /// 结束当前行数据的写入。
    /// </summary>
    public abstract void WriteEndRow();

    /// <summary>
    /// 写入布尔值数据。
    /// </summary>
    /// <param name="name">列名称。</param>
    /// <param name="index">列索引。</param>
    /// <param name="value">要写入的布尔值。</param>
    public abstract void WriteBoolean(string name, int index, bool value);

    /// <summary>
    /// 写入字节值数据。
    /// </summary>
    /// <param name="name">列名称。</param>
    /// <param name="index">列索引。</param>
    /// <param name="value">要写入的字节值。</param>
    public abstract void WriteByte(string name, int index, byte value);

    /// <summary>
    /// 写入字符值数据。
    /// </summary>
    /// <param name="name">列名称。</param>
    /// <param name="index">列索引。</param>
    /// <param name="value">要写入的字符值。</param>
    public abstract void WriteChar(string name, int index, char value);

    /// <summary>
    /// 写入日期时间值数据。
    /// </summary>
    /// <param name="name">列名称。</param>
    /// <param name="index">列索引。</param>
    /// <param name="value">要写入的日期时间值。</param>
    public abstract void WriteDateTime(string name, int index, DateTime value);

    /// <summary>
    /// 写入十进制数值数据。
    /// </summary>
    /// <param name="name">列名称。</param>
    /// <param name="index">列索引。</param>
    /// <param name="value">要写入的十进制数值。</param>
    public abstract void WriteDecimal(string name, int index, decimal value);

    /// <summary>
    /// 写入双精度浮点数值数据。
    /// </summary>
    /// <param name="name">列名称。</param>
    /// <param name="index">列索引。</param>
    /// <param name="value">要写入的双精度浮点数值。</param>
    public abstract void WriteDouble(string name, int index, double value);

    /// <summary>
    /// 写入16位有符号整数值数据。
    /// </summary>
    /// <param name="name">列名称。</param>
    /// <param name="index">列索引。</param>
    /// <param name="value">要写入的16位有符号整数值。</param>
    public abstract void WriteInt16(string name, int index, short value);

    /// <summary>
    /// 写入32位有符号整数值数据。
    /// </summary>
    /// <param name="name">列名称。</param>
    /// <param name="index">列索引。</param>
    /// <param name="value">要写入的32位有符号整数值。</param>
    public abstract void WriteInt32(string name, int index, int value);

    /// <summary>
    /// 写入64位有符号整数值数据。
    /// </summary>
    /// <param name="name">列名称。</param>
    /// <param name="index">列索引。</param>
    /// <param name="value">要写入的64位有符号整数值。</param>
    public abstract void WriteInt64(string name, int index, long value);

    /// <summary>
    /// 写入8位有符号整数值数据。
    /// </summary>
    /// <param name="name">列名称。</param>
    /// <param name="index">列索引。</param>
    /// <param name="value">要写入的8位有符号整数值。</param>
    public abstract void WriteSByte(string name, int index, sbyte value);

    /// <summary>
    /// 写入单精度浮点数值数据。
    /// </summary>
    /// <param name="name">列名称。</param>
    /// <param name="index">列索引。</param>
    /// <param name="value">要写入的单精度浮点数值。</param>
    public abstract void WriteSingle(string name, int index, float value);

    /// <summary>
    /// 写入字符串值数据。
    /// </summary>
    /// <param name="name">列名称。</param>
    /// <param name="index">列索引。</param>
    /// <param name="value">要写入的字符串值。</param>
    public abstract void WriteString(string name, int index, string value);

    /// <summary>
    /// 写入16位无符号整数值数据。
    /// </summary>
    /// <param name="name">列名称。</param>
    /// <param name="index">列索引。</param>
    /// <param name="value">要写入的16位无符号整数值。</param>
    public abstract void WriteUInt16(string name, int index, ushort value);

    /// <summary>
    /// 写入32位无符号整数值数据。
    /// </summary>
    /// <param name="name">列名称。</param>
    /// <param name="index">列索引。</param>
    /// <param name="value">要写入的32位无符号整数值。</param>
    public abstract void WriteUInt32(string name, int index, uint value);

    /// <summary>
    /// 写入64位无符号整数值数据。
    /// </summary>
    /// <param name="name">列名称。</param>
    /// <param name="index">列索引。</param>
    /// <param name="value">要写入的64位无符号整数值。</param>
    public abstract void WriteUInt64(string name, int index, ulong value);

    /// <summary>
    /// 写入对象值数据。此方法用于处理通用对象类型的数据写入。
    /// </summary>
    /// <param name="name">列名称。</param>
    /// <param name="index">列索引。</param>
    /// <param name="value">要写入的对象值，可以为 null。</param>
    public abstract void WriteObject(string name, int index, object? value);
}