using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Boolean ToBoolean(Boolean value) => value;
    /// <inheritdoc/>
    public static Char ToChar(Boolean value) => default;
    /// <inheritdoc/>
    public static SByte ToSByte(Boolean value) => value ? ((sbyte)1) : ((sbyte)0);
    /// <inheritdoc/>
    public static Byte ToByte(Boolean value) => value ? (byte)1 : (byte)0;
    /// <inheritdoc/>
    public static Int16 ToInt16(Boolean value) => value ? ((short)1) : ((short)0);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(Boolean value) => value ? ((ushort)1) : ((ushort)0);
    /// <inheritdoc/>
    public static Int32 ToInt32(Boolean value) => value ? 1 : 0;
    /// <inheritdoc/>
    public static UInt32 ToUInt32(Boolean value) => value ? 1u : 0u;
    /// <inheritdoc/>
    public static Int64 ToInt64(Boolean value) => value ? 1 : 0;
    /// <inheritdoc/>
    public static UInt64 ToUInt64(Boolean value) => value ? 1u : 0u;
    /// <inheritdoc/>
    public static Single ToSingle(Boolean value) => value ? 1 : 0;
    /// <inheritdoc/>
    public static Double ToDouble(Boolean value) => value ? 1 : 0;
    /// <inheritdoc/>
    public static Decimal ToDecimal(Boolean value) => value ? 1 : 0;
    /// <inheritdoc/>
    public static DateTime ToDateTime(Boolean value) => default;
    /// <inheritdoc/>
    public static String ToString(Boolean value) => value ? "1" : "0";
}
