using LuYao.Data.Models;
using System;
using System.IO;

namespace LuYao.Data.Binary;

/// <summary>
/// 提供将记录数据以二进制格式写入流的适配器实现。
/// </summary>
/// <remarks>
/// 该类继承自 <see cref="RecordSaveAdapter"/>，专门用于将记录数据序列化为二进制格式。
/// 它使用 <see cref="BinaryWriter"/> 将各种数据类型写入底层流中。
/// </remarks>
public class BinaryRecordSaveAdapter : RecordSaveAdapter
{
    /// <summary>
    /// 获取用于写入二进制数据的 <see cref="BinaryWriter"/> 实例。
    /// </summary>
    /// <value>用于执行二进制写入操作的 <see cref="BinaryWriter"/> 对象。</value>
    public BinaryWriter Writer { get; }

    /// <summary>
    /// 使用指定的二进制写入器初始化 <see cref="BinaryRecordSaveAdapter"/> 类的新实例。
    /// </summary>
    /// <param name="writer">用于写入二进制数据的 <see cref="BinaryWriter"/> 实例。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="writer"/> 为 null 时抛出。</exception>
    public BinaryRecordSaveAdapter(BinaryWriter writer)
    {
        Writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    /// <remarks>
    /// 将列信息转换为 <see cref="BinaryRecordColumnInfo"/> 并写入到二进制流中。
    /// </remarks>
    public override void WriteColumn(RecordColumnInfo column)
    {
        var b = new BinaryRecordColumnInfo(column);
        b.Write(Writer);
    }

    /// <remarks>
    /// 将记录头信息转换为 <see cref="BinaryRecordHeader"/> 并写入到二进制流中。
    /// </remarks>
    public override void WriteHeader(RecordHeader header)
    {
        var h = new BinaryRecordHeader(header);
        h.Write(Writer);
    }

    /// <exception cref="NotImplementedException">复杂类型的二进制读写暂不支持。</exception>
    public override void WriteObject(string name, int index, object? value) => throw new NotImplementedException("复杂类型的二进制读写暂不支持");

    /// <remarks>
    /// 将布尔值直接写入二进制流。
    /// </remarks>
    public override void WriteBoolean(string name, int index, bool value) => Writer.Write(value);

    /// <remarks>
    /// 将字节值直接写入二进制流。
    /// </remarks>
    public override void WriteByte(string name, int index, byte value) => Writer.Write(value);

    /// <remarks>
    /// 将字符值直接写入二进制流。
    /// </remarks>
    public override void WriteChar(string name, int index, char value) => Writer.Write(value);

    /// <remarks>
    /// 将日期时间值转换为二进制格式（使用 <see cref="DateTime.ToBinary()"/>）后写入二进制流。
    /// </remarks>
    public override void WriteDateTime(string name, int index, DateTime value) => Writer.Write(value.ToBinary());

    /// <remarks>
    /// 将十进制数值直接写入二进制流。
    /// </remarks>
    public override void WriteDecimal(string name, int index, decimal value) => Writer.Write(value);

    /// <remarks>
    /// 将双精度浮点数值直接写入二进制流。
    /// </remarks>
    public override void WriteDouble(string name, int index, double value) => Writer.Write(value);

    /// <remarks>
    /// 在二进制格式中，行结束标记不需要特殊处理，此方法为空实现。
    /// </remarks>
    public override void WriteEndRow() { }

    /// <remarks>
    /// 将16位有符号整数值直接写入二进制流。
    /// </remarks>
    public override void WriteInt16(string name, int index, short value) => Writer.Write(value);

    /// <remarks>
    /// 将32位有符号整数值直接写入二进制流。
    /// </remarks>
    public override void WriteInt32(string name, int index, int value) => Writer.Write(value);

    /// <remarks>
    /// 将64位有符号整数值直接写入二进制流。
    /// </remarks>
    public override void WriteInt64(string name, int index, long value) => Writer.Write(value);

    /// <remarks>
    /// 将8位有符号整数值直接写入二进制流。
    /// </remarks>
    public override void WriteSByte(string name, int index, sbyte value) => Writer.Write(value);

    /// <remarks>
    /// 将单精度浮点数值直接写入二进制流。
    /// </remarks>
    public override void WriteSingle(string name, int index, float value) => Writer.Write(value);

    /// <remarks>
    /// 在二进制格式中，行开始标记不需要特殊处理，此方法为空实现。
    /// </remarks>
    public override void WriteStarRow() { }

    /// <remarks>
    /// 将字符串值直接写入二进制流。
    /// </remarks>
    public override void WriteString(string name, int index, string value) => Writer.Write(value);

    /// <remarks>
    /// 将16位无符号整数值直接写入二进制流。
    /// </remarks>
    public override void WriteUInt16(string name, int index, ushort value) => Writer.Write(value);

    /// <remarks>
    /// 将32位无符号整数值直接写入二进制流。
    /// </remarks>
    public override void WriteUInt32(string name, int index, uint value) => Writer.Write(value);

    /// <remarks>
    /// 将64位无符号整数值直接写入二进制流。
    /// </remarks>
    public override void WriteUInt64(string name, int index, ulong value) => Writer.Write(value);
}
