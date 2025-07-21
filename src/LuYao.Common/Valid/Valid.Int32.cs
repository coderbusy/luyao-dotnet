using System;
using System.Diagnostics;

namespace LuYao;

partial class Valid
{
    /// <summary>
    /// 将字符串转换为 Int32。如果字符串为 null、空或无法解析，则返回 0。
    /// </summary>
    public static int ToInt32(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;
        if (int.TryParse(value, out int result)) return result;
        return 0;
    }

    /// <summary>
    /// 将字符转换为 Int32。
    /// </summary>
    public static int ToInt32(char c) => (int)c;

    /// <summary>
    /// 将布尔值转换为 Int32。true 返回 1，false 返回 0。
    /// </summary>
    public static int ToInt32(bool value) => value ? 1 : 0;

    /// <summary>
    /// 将字节类型转换为 Int32。
    /// </summary>
    public static int ToInt32(byte value) => (int)value;

    /// <summary>
    /// 将有符号字节类型转换为 Int32。
    /// </summary>
    public static int ToInt32(sbyte value) => (int)value;

    /// <summary>
    /// 将短整型转换为 Int32。
    /// </summary>
    public static int ToInt32(short value) => (int)value;

    /// <summary>
    /// 将无符号短整型转换为 Int32。
    /// </summary>
    public static int ToInt32(ushort value) => (int)value;

    /// <summary>
    /// 返回自身（Int32）。
    /// </summary>
    public static int ToInt32(int value) => value;

    /// <summary>
    /// 将无符号整型转换为 Int32。
    /// </summary>
    public static int ToInt32(uint value) => (int)value;

    /// <summary>
    /// 将长整型转换为 Int32。
    /// </summary>
    public static int ToInt32(long value) => (int)value;

    /// <summary>
    /// 将无符号长整型转换为 Int32。
    /// </summary>
    public static int ToInt32(ulong value) => (int)value;

    /// <summary>
    /// 将十进制类型转换为 Int32。
    /// </summary>
    public static int ToInt32(decimal value) => decimal.ToInt32(value);

    /// <summary>
    /// 将单精度浮点数类型转换为 Int32。
    /// </summary>
    public static int ToInt32(float value) => (int)value;

    /// <summary>
    /// 将双精度浮点数类型转换为 Int32。
    /// </summary>
    public static int ToInt32(double value) => (int)value;

    /// <summary>
    /// 将 <see cref="DateTime"/> 类型转换为 Int32，格式为 yyyyMMdd。
    /// </summary>
    /// <param name="d">要转换的日期。</param>
    /// <returns>返回格式为 yyyyMMdd 的整数。</returns>
    public static int ToInt32(DateTime d) { return d.Year * 10000 + d.Month * 100 + d.Day; }

    /// <summary>
    /// 将对象转换为 Int32。如果对象为 null、DBNull 或无法转换，则返回 0。
    /// </summary>
    /// <param name="value">要转换的对象。</param>
    /// <returns>转换后的 Int32 值，无法转换时返回 0。</returns>
    public static int ToInt32(object value)
    {
        if (value == null || Convert.IsDBNull(value) || !(value is IConvertible convertible)) return 0;
        if (value is string str) return ToInt32(str);
        try { return System.Convert.ToInt32(convertible); }
        catch (Exception ex)
        {
            Debug.WriteLine(value.ToString() + " 转成 int 有误！\r\n" + ex.StackTrace);
            return 0;
        }
    }

    /// <summary>
    /// 将可空字符类型转换为 Int32。null 返回 0。
    /// </summary>
    public static int ToInt32(char? c) => c.HasValue ? ToInt32(c.Value) : 0;

    /// <summary>
    /// 将可空布尔类型转换为 Int32。null 返回 0。
    /// </summary>
    public static int ToInt32(bool? value) => value.HasValue ? ToInt32(value.Value) : 0;

    /// <summary>
    /// 将可空字节类型转换为 Int32。null 返回 0。
    /// </summary>
    public static int ToInt32(byte? value) => value.HasValue ? ToInt32(value.Value) : 0;

    /// <summary>
    /// 将可空有符号字节类型转换为 Int32。null 返回 0。
    /// </summary>
    public static int ToInt32(sbyte? value) => value.HasValue ? ToInt32(value.Value) : 0;

    /// <summary>
    /// 将可空短整型转换为 Int32。null 返回 0。
    /// </summary>
    public static int ToInt32(short? value) => value.HasValue ? ToInt32(value.Value) : 0;

    /// <summary>
    /// 将可空无符号短整型转换为 Int32。null 返回 0。
    /// </summary>
    public static int ToInt32(ushort? value) => value.HasValue ? ToInt32(value.Value) : 0;

    /// <summary>
    /// 将可空整型转换为 Int32。null 返回 0。
    /// </summary>
    public static int ToInt32(int? value) => value.HasValue ? ToInt32(value.Value) : 0;

    /// <summary>
    /// 将可空无符号整型转换为 Int32。null 返回 0。
    /// </summary>
    public static int ToInt32(uint? value) => value.HasValue ? ToInt32(value.Value) : 0;

    /// <summary>
    /// 将可空长整型转换为 Int32。null 返回 0。
    /// </summary>
    public static int ToInt32(long? value) => value.HasValue ? ToInt32(value.Value) : 0;

    /// <summary>
    /// 将可空无符号长整型转换为 Int32。null 返回 0。
    /// </summary>
    public static int ToInt32(ulong? value) => value.HasValue ? ToInt32(value.Value) : 0;

    /// <summary>
    /// 将可空十进制类型转换为 Int32。null 返回 0。
    /// </summary>
    public static int ToInt32(decimal? value) => value.HasValue ? ToInt32(value.Value) : 0;

    /// <summary>
    /// 将可空单精度浮点数类型转换为 Int32。null 返回 0。
    /// </summary>
    public static int ToInt32(float? value) => value.HasValue ? ToInt32(value.Value) : 0;

    /// <summary>
    /// 将可空双精度浮点数类型转换为 Int32。null 返回 0。
    /// </summary>
    public static int ToInt32(double? value) => value.HasValue ? ToInt32(value.Value) : 0;
}
