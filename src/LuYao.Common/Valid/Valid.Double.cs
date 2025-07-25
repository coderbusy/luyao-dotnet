using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Double ToDouble(Boolean value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Double ToDouble(Char value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Double ToDouble(SByte value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Double ToDouble(Byte value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Double ToDouble(Int16 value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Double ToDouble(UInt16 value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Double ToDouble(Int32 value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Double ToDouble(UInt32 value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Double ToDouble(Int64 value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Double ToDouble(UInt64 value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Double ToDouble(Single value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Double ToDouble(Double value) => value;
    /// <inheritdoc/>
    public static Double ToDouble(Decimal value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Double ToDouble(DateTime value) => Convert.ToDouble(value);
}
