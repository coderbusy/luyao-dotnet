using System;

namespace LuYao.Data;

/// <summary>
/// 表示记录数据类型的代码，映射到 <see cref="TypeCode"/> 枚举。
/// </summary>
public enum RecordDataType
{
    ///<summary>
    /// 对象类型。对应 <see cref="TypeCode.Object"/>
    /// </summary>
    Object = TypeCode.Object,
    /// <summary>
    /// 布尔类型。对应 <see cref="TypeCode.Boolean"/>
    /// </summary>
    Boolean = TypeCode.Boolean,
    /// <summary>
    /// 字节类型。对应 <see cref="TypeCode.Byte"/>
    /// </summary>
    Byte = TypeCode.Byte,
    /// <summary>
    /// 字符类型。对应 <see cref="TypeCode.Char"/>
    /// </summary>
    Char = TypeCode.Char,
    /// <summary>
    /// 日期时间类型。对应 <see cref="TypeCode.DateTime"/>
    /// </summary>
    DateTime = TypeCode.DateTime,
    /// <summary>
    /// 十进制类型。对应 <see cref="TypeCode.Decimal"/>
    /// </summary>
    Decimal = TypeCode.Decimal,
    /// <summary>
    /// 双精度浮点类型。对应 <see cref="TypeCode.Double"/>
    /// </summary>
    Double = TypeCode.Double,
    /// <summary>
    /// 16 位整数类型。对应 <see cref="TypeCode.Int16"/>
    /// </summary>
    Int16 = TypeCode.Int16,
    /// <summary>
    /// 32 位整数类型。对应 <see cref="TypeCode.Int32"/>
    /// </summary>
    Int32 = TypeCode.Int32,
    /// <summary>
    /// 64 位整数类型。对应 <see cref="TypeCode.Int64"/>
    /// </summary>
    Int64 = TypeCode.Int64,
    /// <summary>
    /// 有符号字节类型。对应 <see cref="TypeCode.SByte"/>
    /// </summary>
    SByte = TypeCode.SByte,
    /// <summary>
    /// 单精度浮点类型。对应 <see cref="TypeCode.Single"/>
    /// </summary>
    Single = TypeCode.Single,
    /// <summary>
    /// 字符串类型。对应 <see cref="TypeCode.String"/>
    /// </summary>
    String = TypeCode.String,
    /// <summary>
    /// 16 位无符号整数类型。对应 <see cref="TypeCode.UInt16"/>
    /// </summary>
    UInt16 = TypeCode.UInt16,
    /// <summary>
    /// 32 位无符号整数类型。对应 <see cref="TypeCode.UInt32"/>
    /// </summary>
    UInt32 = TypeCode.UInt32,
    /// <summary>
    /// 64 位无符号整数类型。对应 <see cref="TypeCode.UInt64"/>
    /// </summary>
    UInt64 = TypeCode.UInt64
}
