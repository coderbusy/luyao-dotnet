using System;
using System.Collections.Generic;

namespace LuYao;

partial class Valid
{
    private static readonly ISet<string> FALSE_STRINGS = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "0", "false", "no", "off", "n", "f", "null"
    };

    /// <summary>
    /// 将对象转换为布尔值。支持 null、DBNull、bool 类型及其字符串表示。
    /// </summary>
    /// <param name="value">要转换的对象。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(object? value)
    {
        if (value is null || Convert.IsDBNull(value)) return default;
        if (Nullable.GetUnderlyingType(value.GetType()) != null)
        {
            // 如果是可空类型，递归调用 ToBoolean
        }
        switch (value)
        {
            case string v1: return ToBoolean(v1);
            case char v2: return ToBoolean(v2);
            case double v3: return ToBoolean(v3);
            case bool v4: return v4;
            default:
                break;
        }
        return ToBoolean(value.ToString());
    }
    /// <summary>
    /// 将字符串转换为布尔值。常见的表示“假”的字符串（如 "0", "false", "no", "off", "n", "f", "null"）会被识别为 false，其余情况为 true。
    /// </summary>
    /// <param name="value">要转换的字符串。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(string? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value)) return false;
        if (FALSE_STRINGS.Contains(value)) return false;
        string str = value.Trim();
        return FALSE_STRINGS.Contains(str) ? false : true;
    }


    /// <summary>
    /// 将字符转换为布尔值。字符的 Unicode 值大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的字符。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(char value) => value > 0;

    /// <summary>
    /// 将可空字符转换为布尔值。值存在且大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的可空字符。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(char? value) => value.HasValue && ToBoolean(value.Value);

    /// <summary>
    /// 将可空布尔值转换为布尔值。值存在且为 true 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的可空布尔值。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(bool? value) => value.HasValue && value.Value;

    /// <summary>
    /// 将字节转换为布尔值。大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的字节。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(byte value) => value > 0;

    /// <summary>
    /// 将可空字节转换为布尔值。值存在且大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的可空字节。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(byte? value) => value.HasValue && ToBoolean(value.Value);

    /// <summary>
    /// 将有符号字节转换为布尔值。大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的有符号字节。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(sbyte value) => value > 0;

    /// <summary>
    /// 将可空有符号字节转换为布尔值。值存在且大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的可空有符号字节。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(sbyte? value) => value.HasValue && ToBoolean(value.Value);

    /// <summary>
    /// 将短整型转换为布尔值。大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的短整型。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(short value) => value > 0;

    /// <summary>
    /// 将可空短整型转换为布尔值。值存在且大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的可空短整型。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(short? value) => value.HasValue && ToBoolean(value.Value);

    /// <summary>
    /// 将无符号短整型转换为布尔值。大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的无符号短整型。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(ushort value) => value > 0;

    /// <summary>
    /// 将可空无符号短整型转换为布尔值。值存在且大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的可空无符号短整型。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(ushort? value) => value.HasValue && ToBoolean(value.Value);

    /// <summary>
    /// 将整型转换为布尔值。大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的整型。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(int value) => value > 0;

    /// <summary>
    /// 将可空整型转换为布尔值。值存在且大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的可空整型。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(int? value) => value.HasValue && ToBoolean(value.Value);

    /// <summary>
    /// 将无符号整型转换为布尔值。大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的无符号整型。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(uint value) => value > 0;

    /// <summary>
    /// 将可空无符号整型转换为布尔值。值存在且大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的可空无符号整型。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(uint? value) => value.HasValue && ToBoolean(value.Value);

    /// <summary>
    /// 将长整型转换为布尔值。大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的长整型。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(long value) => value > 0;

    /// <summary>
    /// 将可空长整型转换为布尔值。值存在且大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的可空长整型。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(long? value) => value.HasValue && ToBoolean(value.Value);

    /// <summary>
    /// 将无符号长整型转换为布尔值。大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的无符号长整型。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(ulong value) => value > 0;

    /// <summary>
    /// 将可空无符号长整型转换为布尔值。值存在且大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的可空无符号长整型。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(ulong? value) => value.HasValue && ToBoolean(value.Value);

    /// <summary>
    /// 将单精度浮点数转换为布尔值。大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的单精度浮点数。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(float value) => value > 0;

    /// <summary>
    /// 将可空单精度浮点数转换为布尔值。值存在且大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的可空单精度浮点数。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(float? value) => value.HasValue && ToBoolean(value.Value);

    /// <summary>
    /// 将双精度浮点数转换为布尔值。大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的双精度浮点数。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(double value) => value > 0;

    /// <summary>
    /// 将可空双精度浮点数转换为布尔值。值存在且大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的可空双精度浮点数。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(double? value) => value.HasValue && ToBoolean(value.Value);

    /// <summary>
    /// 将高精度浮点数转换为布尔值。大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的高精度浮点数。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(decimal value) => value > 0;

    /// <summary>
    /// 将可空高精度浮点数转换为布尔值。值存在且大于 0 时返回 true，否则返回 false。
    /// </summary>
    /// <param name="value">要转换的可空高精度浮点数。</param>
    /// <returns>转换后的布尔值。</returns>
    public static bool ToBoolean(decimal? value) => value.HasValue && ToBoolean(value.Value);
}
