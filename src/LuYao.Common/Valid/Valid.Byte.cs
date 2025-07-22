using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Byte ToByte(object value) => Convert.ToByte(value);
    /// <inheritdoc/>
    public static Byte ToByte(string value) => Byte.TryParse(value, out var b) ? b : default;
}
