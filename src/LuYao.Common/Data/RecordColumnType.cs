namespace LuYao.Data;

/// <summary>
/// 定义 <see cref="RecordColumn"/> 支持的所有列数据类型。
/// 白名单封闭，不可外部扩展。
/// </summary>
/// <remarks>
/// <para>正值表示非空类型，负值表示对应的 <see cref="System.Nullable{T}"/> 形式。</para>
/// <para>例如 <see cref="Int32"/> = 4，则 Nullable&lt;int&gt; 在序列化时用 -4 表示。</para>
/// </remarks>
public enum RecordColumnType : sbyte
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

    // --- Nullable 形式使用负值 ---

    /// <summary>可空布尔 (<see cref="T:bool?"/>)。</summary>
    NullableBoolean = -1,

    /// <summary>可空有符号 8 位整数 (<see cref="T:sbyte?"/>)。</summary>
    NullableSByte = -2,

    /// <summary>可空有符号 16 位整数 (<see cref="T:short?"/>)。</summary>
    NullableInt16 = -3,

    /// <summary>可空有符号 32 位整数 (<see cref="T:int?"/>)。</summary>
    NullableInt32 = -4,

    /// <summary>可空有符号 64 位整数 (<see cref="T:long?"/>)。</summary>
    NullableInt64 = -5,

    /// <summary>可空无符号 8 位整数 (<see cref="T:byte?"/>)。</summary>
    NullableByte = -6,

    /// <summary>可空无符号 16 位整数 (<see cref="T:ushort?"/>)。</summary>
    NullableUInt16 = -7,

    /// <summary>可空无符号 32 位整数 (<see cref="T:uint?"/>)。</summary>
    NullableUInt32 = -8,

    /// <summary>可空无符号 64 位整数 (<see cref="T:ulong?"/>)。</summary>
    NullableUInt64 = -9,

    /// <summary>可空单精度浮点数 (<see cref="T:float?"/>)。</summary>
    NullableSingle = -10,

    /// <summary>可空双精度浮点数 (<see cref="T:double?"/>)。</summary>
    NullableDouble = -11,

    /// <summary>可空十进制数 (<see cref="T:decimal?"/>)。</summary>
    NullableDecimal = -12,

    /// <summary>可空字符 (<see cref="T:char?"/>)。</summary>
    NullableChar = -13,

    /// <summary>可空日期时间 (<see cref="T:DateTime?"/>)。</summary>
    NullableDateTime = -15,

    /// <summary>可空日期时间偏移量 (<see cref="T:DateTimeOffset?"/>)。</summary>
    NullableDateTimeOffset = -16,

    /// <summary>可空时间间隔 (<see cref="T:TimeSpan?"/>)。</summary>
    NullableTimeSpan = -17,

    /// <summary>可空全局唯一标识符 (<see cref="T:Guid?"/>)。</summary>
    NullableGuid = -18,
}
