using LuYao.Data.Models;
using LuYao.Text.Json;
using System;
using System.Collections.Generic;

namespace LuYao.Data.Json;

/// <summary>
/// 提供将记录数据以JSON格式写入流的适配器实现。
/// </summary>
/// <remarks>
/// 该类继承自 <see cref="RecordSaveAdapter"/>，专门用于将记录数据序列化为JSON格式。
/// 它使用 <see cref="JsonWriter"/> 将各种数据类型写入底层流中。
/// JSON结构包含header、columns和rows等部分。
/// </remarks>
public class JsonRecordSaveAdapter : RecordSaveAdapter
{
    /// <summary>
    /// 获取用于写入JSON数据的 <see cref="JsonWriter"/> 实例。
    /// </summary>
    /// <value>用于执行JSON写入操作的 <see cref="JsonWriter"/> 对象。</value>
    public JsonWriter Writer { get; }

    /// <inheritdoc/>
    public override IReadOnlyList<RecordSection> Layout { get; } = [RecordSection.Head, RecordSection.Columns, RecordSection.Rows];

    /// <summary>
    /// 使用指定的JSON写入器初始化 <see cref="JsonRecordSaveAdapter"/> 类的新实例。
    /// </summary>
    /// <param name="writer">用于写入JSON数据的 <see cref="JsonWriter"/> 实例。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="writer"/> 为 null 时抛出。</exception>
    public JsonRecordSaveAdapter(JsonWriter writer)
    {
        Writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    /// <remarks>
    /// 将列信息写入JSON，作为对象并将属性存储为键值对。
    /// </remarks>
    public override void WriteColumn(RecordColumnInfo column)
    {
        string type = column.GetTypeName();
        Writer.WriteStartObject();
        Writer.WritePropertyName("name");
        Writer.WriteValue(column.Name);
        Writer.WritePropertyName("code");
        Writer.WriteValue(Valid.ToString(column.Code));
        Writer.WritePropertyName("type");
        Writer.WriteValue(type);
        Writer.WriteEndObject();
    }

    /// <remarks>
    /// 将记录头信息写入JSON，作为对象并将属性存储为键值对。
    /// </remarks>
    public override void WriteHeader(RecordHeader header)
    {
        Writer.WriteStartObject();
        Writer.WritePropertyName("name");
        Writer.WriteValue(header.Name);
        Writer.WritePropertyName("columns");
        Writer.WriteValue(header.Columns);
        Writer.WritePropertyName("count");
        Writer.WriteValue(header.Count);
        Writer.WriteEndObject();
    }

    /// <exception cref="NotImplementedException">复杂类型的JSON读写暂不支持。</exception>
    public override void WriteObject(string name, int index, object? value) => throw new NotImplementedException("复杂类型的JSON读写暂不支持");

    /// <remarks>
    /// 将布尔值作为属性写入当前行对象。
    /// </remarks>
    public override void WriteBoolean(string name, int index, bool value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue(value);
    }

    /// <remarks>
    /// 将字节值作为属性写入当前行对象。
    /// </remarks>
    public override void WriteByte(string name, int index, byte value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((long)value);
    }

    /// <remarks>
    /// 将字符值作为属性写入当前行对象。
    /// </remarks>
    public override void WriteChar(string name, int index, char value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue(Valid.ToString(value));
    }

    /// <remarks>
    /// 将日期时间值转换为ISO 8601格式后作为属性写入当前行对象。
    /// </remarks>
    public override void WriteDateTime(string name, int index, DateTime value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue(value.ToString("O"));
    }

    /// <remarks>
    /// 将十进制数值作为属性写入当前行对象。
    /// </remarks>
    public override void WriteDecimal(string name, int index, decimal value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((double)value);
    }

    /// <remarks>
    /// 将双精度浮点数值作为属性写入当前行对象。
    /// </remarks>
    public override void WriteDouble(string name, int index, double value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue(value);
    }

    /// <remarks>
    /// 结束当前行对象的写入。
    /// </remarks>
    public override void WriteEndRow() => Writer.WriteEndObject();

    /// <remarks>
    /// 将16位有符号整数值作为属性写入当前行对象。
    /// </remarks>
    public override void WriteInt16(string name, int index, short value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((long)value);
    }

    /// <remarks>
    /// 将32位有符号整数值作为属性写入当前行对象。
    /// </remarks>
    public override void WriteInt32(string name, int index, int value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((long)value);
    }

    /// <remarks>
    /// 将64位有符号整数值作为属性写入当前行对象。
    /// </remarks>
    public override void WriteInt64(string name, int index, long value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue(value);
    }

    /// <remarks>
    /// 将8位有符号整数值作为属性写入当前行对象。
    /// </remarks>
    public override void WriteSByte(string name, int index, sbyte value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((long)value);
    }

    /// <remarks>
    /// 将单精度浮点数值作为属性写入当前行对象。
    /// </remarks>
    public override void WriteSingle(string name, int index, float value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((double)value);
    }

    /// <remarks>
    /// 开始写入新的行对象。
    /// </remarks>
    public override void WriteStarRow() => Writer.WriteStartObject();

    /// <remarks>
    /// 将字符串值作为属性写入当前行对象。
    /// </remarks>
    public override void WriteString(string name, int index, string value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue(value);
    }

    /// <remarks>
    /// 将16位无符号整数值作为属性写入当前行对象。
    /// </remarks>
    public override void WriteUInt16(string name, int index, ushort value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((long)value);
    }

    /// <remarks>
    /// 将32位无符号整数值作为属性写入当前行对象。
    /// </remarks>
    public override void WriteUInt32(string name, int index, uint value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((long)value);
    }

    /// <remarks>
    /// 将64位无符号整数值作为属性写入当前行对象。
    /// </remarks>
    public override void WriteUInt64(string name, int index, ulong value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((long)value);
    }

    /// <inheritdoc/>
    public override void WriteStart()
    {
        Writer.WriteStartObject();
    }

    /// <inheritdoc/>
    public override void WriteEnd()
    {
        Writer.WriteEndObject();
    }

    /// <inheritdoc/>
    public override void WriteStartSection(RecordSection section)
    {
        Writer.WritePropertyName(section.ToString().ToLowerInvariant());
        Writer.WriteStartArray();
    }

    /// <inheritdoc/>
    public override void WriteEndSection()
    {
        Writer.WriteEndArray();
    }
}