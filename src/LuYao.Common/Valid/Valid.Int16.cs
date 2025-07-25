using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Int16 ToInt16(Boolean value) => value ? ((short)1) : ((short)0);
    /// <inheritdoc/>
    public static Int16 ToInt16(Char value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(SByte value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(Byte value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(Int16 value) => value;
    /// <inheritdoc/>
    public static Int16 ToInt16(UInt16 value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(Int32 value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(UInt32 value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(Int64 value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(UInt64 value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(Single value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(Double value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(Decimal value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(DateTime value) => Convert.ToInt16(value);

}
