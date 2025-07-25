using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static UInt16 ToUInt16(Boolean value) => value ? ((ushort)1) : ((ushort)0);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(Char value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(SByte value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(Byte value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(Int16 value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(UInt16 value) => value;
    /// <inheritdoc/>
    public static UInt16 ToUInt16(Int32 value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(UInt32 value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(Int64 value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(UInt64 value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(Single value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(Double value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(Decimal value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(DateTime value) => Convert.ToUInt16(value);
}
