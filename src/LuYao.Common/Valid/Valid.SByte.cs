using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static SByte ToSByte(Boolean value) => value ? ((sbyte)1) : ((sbyte)0);
    /// <inheritdoc/>
    public static SByte ToSByte(Char value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static SByte ToSByte(SByte value) => value;
    /// <inheritdoc/>
    public static SByte ToSByte(Byte value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static SByte ToSByte(Int16 value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static SByte ToSByte(UInt16 value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static SByte ToSByte(Int32 value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static SByte ToSByte(UInt32 value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static SByte ToSByte(Int64 value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static SByte ToSByte(UInt64 value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static SByte ToSByte(Single value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static SByte ToSByte(Double value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static SByte ToSByte(Decimal value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static SByte ToSByte(DateTime value) => Convert.ToSByte(value);
}
