using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Single ToSingle(Boolean value) => value ? 1 : 0;
    /// <inheritdoc/>
    public static Single ToSingle(Char value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Single ToSingle(SByte value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Single ToSingle(Byte value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Single ToSingle(Int16 value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Single ToSingle(UInt16 value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Single ToSingle(Int32 value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Single ToSingle(UInt32 value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Single ToSingle(Int64 value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Single ToSingle(UInt64 value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Single ToSingle(Single value) => value;
    /// <inheritdoc/>
    public static Single ToSingle(Double value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Single ToSingle(Decimal value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Single ToSingle(DateTime value) => Convert.ToSingle(value);
}
