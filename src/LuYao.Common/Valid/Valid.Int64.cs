using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Int64 ToInt64(DateTime value) => value.ToBinary();
    /// <inheritdoc/>
    public static Int64 ToInt64(Boolean value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(Char value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(SByte value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(Byte value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(Int16 value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(UInt16 value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(Int32 value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(UInt32 value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(Int64 value) => value;
    /// <inheritdoc/>
    public static Int64 ToInt64(UInt64 value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(Single value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(Double value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(Decimal value) => Convert.ToInt64(value);
}
