namespace LuYao.Data;

/// <summary>
/// 定义 <see cref="FrameColumn"/> 支持的所有基础列数据类型。
/// 白名单封闭，不可外部扩展。可空性通过 <see cref="FrameColumn.IsNullable"/> 独立表达。
/// </summary>
public enum FrameColumnType : byte
{
    /// <summary>布尔类型 (<see cref="bool"/>)。</summary>
    Boolean = 1,

    /// <summary>有符号 8 位整数 (<see cref="sbyte"/>)。</summary>
    SByte = 2,

    /// <summary>有符号 16 位整数 (<see cref="short"/>)。</summary>
    Int16 = 3,

    /// <summary>有符号 32 位整数 (<see cref="int"/>)。</summary>
    Int32 = 4,

    /// <summary>有符号 64 位整数 (<see cref="long"/>)。</summary>
    Int64 = 5,

    /// <summary>无符号 8 位整数 (<see cref="byte"/>)。</summary>
    Byte = 6,

    /// <summary>无符号 16 位整数 (<see cref="ushort"/>)。</summary>
    UInt16 = 7,

    /// <summary>无符号 32 位整数 (<see cref="uint"/>)。</summary>
    UInt32 = 8,

    /// <summary>无符号 64 位整数 (<see cref="ulong"/>)。</summary>
    UInt64 = 9,

    /// <summary>单精度浮点数 (<see cref="float"/>)。</summary>
    Single = 10,

    /// <summary>双精度浮点数 (<see cref="double"/>)。</summary>
    Double = 11,

    /// <summary>十进制数 (<see cref="decimal"/>)。</summary>
    Decimal = 12,

    /// <summary>字符 (<see cref="char"/>)。</summary>
    Char = 13,

    /// <summary>字符串 (<see cref="string"/>)。</summary>
    String = 14,

    /// <summary>日期时间 (<see cref="System.DateTime"/>)。</summary>
    DateTime = 15,

    /// <summary>日期时间偏移量 (<see cref="System.DateTimeOffset"/>)。</summary>
    DateTimeOffset = 16,

    /// <summary>时间间隔 (<see cref="System.TimeSpan"/>)。</summary>
    TimeSpan = 17,

    /// <summary>全局唯一标识符 (<see cref="System.Guid"/>)。</summary>
    Guid = 18,

    /// <summary>字节数组 (<see cref="T:byte[]"/>)。</summary>
    ByteArray = 19,
}
