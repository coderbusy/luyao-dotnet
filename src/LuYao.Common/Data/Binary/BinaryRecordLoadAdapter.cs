using LuYao.Data.Models;
using System;
using System.IO;

namespace LuYao.Data.Binary;

/// <summary>
/// 二进制记录加载适配器，用于从二进制流中读取记录数据。
/// </summary>
/// <remarks>
/// 此适配器实现了 <see cref="RecordLoadAdapter"/> 抽象类，提供了从二进制格式读取记录数据的功能。
/// 适配器按行读取数据，支持多种基本数据类型的反序列化。
/// </remarks>
public class BinaryRecordLoadAdapter : RecordLoadAdapter
{
    /// <summary>
    /// 获取用于读取二进制数据的 <see cref="BinaryReader"/> 实例。
    /// </summary>
    /// <value>二进制读取器实例。</value>
    public BinaryReader Reader { get; }

    /// <summary>
    /// 初始化 <see cref="BinaryRecordLoadAdapter"/> 类的新实例。
    /// </summary>
    /// <param name="reader">用于读取二进制数据的 <see cref="BinaryReader"/> 实例。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="reader"/> 为 null 时抛出。</exception>
    public BinaryRecordLoadAdapter(BinaryReader reader)
    {
        this.Reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    /// <summary>
    /// 剩余未读取的行数。
    /// </summary>
    private int _remainRow = 0;

    /// <summary>
    /// 总列数。
    /// </summary>
    private int _totalColumn = 0;

    /// <summary>
    /// 当前列索引。
    /// </summary>
    private int _columnIndex = 0;

    private bool _isReading = false;

    /// <summary>
    /// 获取键类型，对于二进制适配器始终返回 <see cref="RecordLoadKeyKind.Index"/>。
    /// </summary>
    /// <value>键类型为索引模式。</value>
    public override RecordLoadKeyKind KeyKind => RecordLoadKeyKind.Index;

    /// <summary>
    /// 获取当前列的索引位置。
    /// </summary>
    /// <value>当前列的零基索引。</value>
    public override int Index => _columnIndex;

    /// <summary>
    /// 获取当前列的名称，对于基于索引的适配器始终返回空字符串。
    /// </summary>
    /// <value>空字符串，因为此适配器基于索引而非名称。</value>
    public override string Name => string.Empty;

    /// <summary>
    /// 从二进制流中读取记录头信息。
    /// </summary>
    /// <returns>包含记录元数据的 <see cref="RecordHeader"/> 实例。</returns>
    /// <remarks>
    /// 此方法会初始化内部状态，包括剩余行数和总列数，这些信息用于后续的数据读取操作。
    /// </remarks>
    public override RecordHeader ReadHeader()
    {
        var ret = new BinaryRecordHeader();
        ret.Read(this.Reader);
        _remainRow = ret.Count;
        _totalColumn = ret.Columns;
        return ret;
    }

    /// <summary>
    /// 从二进制流中读取列信息。
    /// </summary>
    /// <returns>包含列元数据的 <see cref="RecordColumnInfo"/> 实例。</returns>
    public override RecordColumnInfo ReadColumn()
    {
        var ret = new BinaryRecordColumnInfo();
        ret.Read(this.Reader);
        return ret;
    }

    /// <summary>
    /// 移动到下一行进行读取。
    /// </summary>
    /// <returns>如果成功移动到下一行则返回 true；如果已到达数据末尾则返回 false。</returns>
    /// <remarks>
    /// 此方法会重置列索引为 0，并减少剩余行数计数器。
    /// </remarks>
    public override bool ReadRow()
    {
        if (_remainRow <= 0) return false;
        _remainRow--;
        _isReading = false;
        return true;
    }

    /// <summary>
    /// 移动到下一个字段进行读取。
    /// </summary>
    /// <returns>如果成功移动到下一个字段则返回 true；如果已到达行末尾则返回 false。</returns>
    /// <remarks>
    /// 此方法会增加列索引计数器，用于跟踪当前读取位置。
    /// </remarks>
    public override bool ReadField()
    {
        if (_isReading)
        {
            if (_columnIndex < _totalColumn - 1)
            {
                _columnIndex++;
                return true;
            }

            return false;
        }
        else
        {
            _isReading = true;
            _columnIndex = 0;
            return true;
        }
    }

    /// <summary>
    /// 从二进制流中读取布尔值。
    /// </summary>
    /// <returns>读取的布尔值。</returns>
    public override bool ReadBoolean() => this.Reader.ReadBoolean();

    /// <summary>
    /// 从二进制流中读取字节值。
    /// </summary>
    /// <returns>读取的字节值。</returns>
    public override byte ReadByte() => this.Reader.ReadByte();

    /// <summary>
    /// 从二进制流中读取字符值。
    /// </summary>
    /// <returns>读取的字符值。</returns>
    public override char ReadChar() => this.Reader.ReadChar();

    /// <summary>
    /// 从二进制流中读取日期时间值。
    /// </summary>
    /// <returns>从二进制格式转换的日期时间值。</returns>
    /// <remarks>
    /// 此方法通过读取 64 位整数并使用 <see cref="DateTime.FromBinary(long)"/> 方法进行转换。
    /// </remarks>
    public override DateTime ReadDateTime()
    {
        var i = this.Reader.ReadInt64();
        return DateTime.FromBinary(i);
    }

    /// <summary>
    /// 从二进制流中读取十进制数值。
    /// </summary>
    /// <returns>读取的十进制数值。</returns>
    public override decimal ReadDecimal() => this.Reader.ReadDecimal();

    /// <summary>
    /// 从二进制流中读取双精度浮点数值。
    /// </summary>
    /// <returns>读取的双精度浮点数值。</returns>
    public override double ReadDouble() => this.Reader.ReadDouble();

    /// <summary>
    /// 从二进制流中读取 16 位有符号整数值。
    /// </summary>
    /// <returns>读取的 16 位有符号整数值。</returns>
    public override short ReadInt16() => this.Reader.ReadInt16();

    /// <summary>
    /// 从二进制流中读取 32 位有符号整数值。
    /// </summary>
    /// <returns>读取的 32 位有符号整数值。</returns>
    public override int ReadInt32() => this.Reader.ReadInt32();

    /// <summary>
    /// 从二进制流中读取 64 位有符号整数值。
    /// </summary>
    /// <returns>读取的 64 位有符号整数值。</returns>
    public override long ReadInt64() => this.Reader.ReadInt64();

    /// <summary>
    /// 读取复杂对象类型。
    /// </summary>
    /// <param name="type">对象类型。</param>
    /// <returns>此方法始终抛出 <see cref="NotImplementedException"/>。</returns>
    /// <exception cref="NotImplementedException">复杂类型的二进制读写暂不支持。</exception>
    public override object? ReadObject(object type) => throw new NotImplementedException("复杂类型的二进制读写暂不支持");

    /// <summary>
    /// 从二进制流中读取 8 位有符号整数值。
    /// </summary>
    /// <returns>读取的 8 位有符号整数值。</returns>
    public override sbyte ReadSByte() => this.Reader.ReadSByte();

    /// <summary>
    /// 从二进制流中读取单精度浮点数值。
    /// </summary>
    /// <returns>读取的单精度浮点数值。</returns>
    public override float ReadSingle() => this.Reader.ReadSingle();

    /// <summary>
    /// 从二进制流中读取字符串值。
    /// </summary>
    /// <returns>读取的字符串值。</returns>
    public override string ReadString() => this.Reader.ReadString();

    /// <summary>
    /// 从二进制流中读取 16 位无符号整数值。
    /// </summary>
    /// <returns>读取的 16 位无符号整数值。</returns>
    public override ushort ReadUInt16() => this.Reader.ReadUInt16();

    /// <summary>
    /// 从二进制流中读取 32 位无符号整数值。
    /// </summary>
    /// <returns>读取的 32 位无符号整数值。</returns>
    public override uint ReadUInt32() => this.Reader.ReadUInt32();

    /// <summary>
    /// 从二进制流中读取 64 位无符号整数值。
    /// </summary>
    /// <returns>读取的 64 位无符号整数值。</returns>
    public override ulong ReadUInt64() => this.Reader.ReadUInt64();
}