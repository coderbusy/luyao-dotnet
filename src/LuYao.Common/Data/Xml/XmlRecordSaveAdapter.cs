using LuYao.Data.Models;
using System;
using System.Collections.Generic;
using System.Xml;

namespace LuYao.Data.Xml;

/// <summary>
/// 提供将记录数据以XML格式写入流的适配器实现。
/// </summary>
/// <remarks>
/// 该类继承自 <see cref="RecordSaveAdapter"/>，专门用于将记录数据序列化为XML格式。
/// 它使用 <see cref="XmlWriter"/> 将各种数据类型写入底层流中。
/// XML结构包含header和row元素，其他数据均存储在attribute中。
/// </remarks>
public class XmlRecordSaveAdapter : RecordSaveAdapter
{
    /// <summary>
    /// 获取用于写入XML数据的 <see cref="XmlWriter"/> 实例。
    /// </summary>
    /// <value>用于执行XML写入操作的 <see cref="XmlWriter"/> 对象。</value>
    public XmlWriter Writer { get; }

    /// <inheritdoc/>
    public override IReadOnlyList<RecordSection> Layout { get; } = [RecordSection.Head, RecordSection.Columns, RecordSection.Rows];

    /// <summary>
    /// 使用指定的XML写入器初始化 <see cref="XmlRecordSaveAdapter"/> 类的新实例。
    /// </summary>
    /// <param name="writer">用于写入XML数据的 <see cref="XmlWriter"/> 实例。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="writer"/> 为 null 时抛出。</exception>
    public XmlRecordSaveAdapter(XmlWriter writer)
    {
        Writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    /// <remarks>
    /// 将列信息写入XML，作为column元素并将属性存储在attribute中。
    /// </remarks>
    public override void WriteColumn(RecordColumnInfo column)
    {
        Writer.WriteStartElement("column");
        Writer.WriteAttributeString("name", column.Name);
        Writer.WriteAttributeString("code", Valid.ToString(column.Code));
        string type = column.Code.ToString();
        var t = Helpers.ToType(column.Code);
        if (t != column.Type) type = column.Type.AssemblyQualifiedName!;
        Writer.WriteAttributeString("type", type);
        Writer.WriteEndElement();
    }

    /// <remarks>
    /// 将记录头信息写入XML，作为header元素并将属性存储在attribute中。
    /// </remarks>
    public override void WriteHeader(RecordHeader header)
    {
        Writer.WriteStartElement("header");
        Writer.WriteAttributeString("name", header.Name);
        Writer.WriteAttributeString("columns", Valid.ToString(header.Columns));
        Writer.WriteAttributeString("count", Valid.ToString(header.Count));
        Writer.WriteEndElement();
    }

    /// <exception cref="NotImplementedException">复杂类型的XML读写暂不支持。</exception>
    public override void WriteObject(string name, int index, object? value) => throw new NotImplementedException("复杂类型的XML读写暂不支持");

    /// <remarks>
    /// 将布尔值作为属性写入当前行元素。
    /// </remarks>
    public override void WriteBoolean(string name, int index, bool value) => Writer.WriteAttributeString(name, Valid.ToString(value));

    /// <remarks>
    /// 将字节值作为属性写入当前行元素。
    /// </remarks>
    public override void WriteByte(string name, int index, byte value) => Writer.WriteAttributeString(name, Valid.ToString(value));

    /// <remarks>
    /// 将字符值作为属性写入当前行元素。
    /// </remarks>
    public override void WriteChar(string name, int index, char value) => Writer.WriteAttributeString(name, Valid.ToString(value));

    /// <remarks>
    /// 将日期时间值转换为ISO 8601格式后作为属性写入当前行元素。
    /// </remarks>
    public override void WriteDateTime(string name, int index, DateTime value) => Writer.WriteAttributeString(name, value.ToString("O"));

    /// <remarks>
    /// 将十进制数值作为属性写入当前行元素。
    /// </remarks>
    public override void WriteDecimal(string name, int index, decimal value) => Writer.WriteAttributeString(name, Valid.ToString(value));

    /// <remarks>
    /// 将双精度浮点数值作为属性写入当前行元素。
    /// </remarks>
    public override void WriteDouble(string name, int index, double value) => Writer.WriteAttributeString(name, Valid.ToString(value));

    /// <remarks>
    /// 结束当前行元素的写入。
    /// </remarks>
    public override void WriteEndRow() => Writer.WriteEndElement();

    /// <remarks>
    /// 将16位有符号整数值作为属性写入当前行元素。
    /// </remarks>
    public override void WriteInt16(string name, int index, short value) => Writer.WriteAttributeString(name, Valid.ToString(value));

    /// <remarks>
    /// 将32位有符号整数值作为属性写入当前行元素。
    /// </remarks>
    public override void WriteInt32(string name, int index, int value) => Writer.WriteAttributeString(name, Valid.ToString(value));

    /// <remarks>
    /// 将64位有符号整数值作为属性写入当前行元素。
    /// </remarks>
    public override void WriteInt64(string name, int index, long value) => Writer.WriteAttributeString(name, Valid.ToString(value));

    /// <remarks>
    /// 将8位有符号整数值作为属性写入当前行元素。
    /// </remarks>
    public override void WriteSByte(string name, int index, sbyte value) => Writer.WriteAttributeString(name, Valid.ToString(value));

    /// <remarks>
    /// 将单精度浮点数值作为属性写入当前行元素。
    /// </remarks>
    public override void WriteSingle(string name, int index, float value) => Writer.WriteAttributeString(name, Valid.ToString(value));

    /// <remarks>
    /// 开始写入新的行元素。
    /// </remarks>
    public override void WriteStarRow() => Writer.WriteStartElement("row");

    /// <remarks>
    /// 将字符串值作为属性写入当前行元素。
    /// </remarks>
    public override void WriteString(string name, int index, string value) => Writer.WriteAttributeString(name, value);

    /// <remarks>
    /// 将16位无符号整数值作为属性写入当前行元素。
    /// </remarks>
    public override void WriteUInt16(string name, int index, ushort value) => Writer.WriteAttributeString(name, Valid.ToString(value));

    /// <remarks>
    /// 将32位无符号整数值作为属性写入当前行元素。
    /// </remarks>
    public override void WriteUInt32(string name, int index, uint value) => Writer.WriteAttributeString(name, Valid.ToString(value));

    /// <remarks>
    /// 将64位无符号整数值作为属性写入当前行元素。
    /// </remarks>
    public override void WriteUInt64(string name, int index, ulong value) => Writer.WriteAttributeString(name, Valid.ToString(value));

    /// <inheritdoc/>
    public override void WriteStart()
    {
        this.Writer.WriteStartElement("record");
    }

    /// <inheritdoc/>
    public override void WriteEnd()
    {
        this.Writer.WriteEndElement();
    }

    /// <inheritdoc/>
    public override void WriteStartSection(RecordSection section)
    {
    }

    /// <inheritdoc/>
    public override void WriteEndSection()
    {
    }
}