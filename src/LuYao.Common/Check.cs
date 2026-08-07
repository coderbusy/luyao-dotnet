using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LuYao;

/// <summary>
/// 提供一组用于参数验证的静态方法。
/// 验证参数以减少样板参数检查代码。
/// </summary>
[DebuggerStepThrough]
public static class Check
{
    /// <summary>
    /// 检查提供的值是否不为 null，如果是则抛出 <see cref="ArgumentNullException"/>。
    /// </summary>
    /// <typeparam name="T">要检查的值的类型。</typeparam>
    /// <param name="value">要检查是否为 null 的值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <returns>验证值（如果不为空）。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="value"/> 为 null 时抛出。</exception>
    public static T NotNull<T>(T? value, string parameterName)
    {
        if (value == null)
        {
            throw new ArgumentNullException(parameterName);
        }

        return value;
    }

    /// <summary>
    /// 检查提供的值是否不为 null，如果是，则抛出 <see cref="ArgumentNullException"/> 和自定义消息。
    /// </summary>
    /// <typeparam name="T">要检查的值的类型。</typeparam>
    /// <param name="value">要检查是否为 null 的值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <param name="message">自定义错误消息。</param>
    /// <returns>验证值（如果不为空）。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="value"/> 为 null 时抛出。</exception>
    public static T NotNull<T>(T? value, string parameterName, string message)
    {
        if (value == null)
        {
            throw new ArgumentNullException(parameterName, message);
        }

        return value;
    }

    /// <summary>
    /// 检查提供的字符串是否不为空并验证其长度限制。
    /// </summary>
    /// <param name="value">要检查的字符串值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <param name="maxLength">字符串允许的最大长度。默认为 <see cref="int.MaxValue"/>。</param>
    /// <param name="minLength">字符串允许的最小长度。默认为 0。</param>
    /// <returns>已验证的字符串（如果有效）。</returns>
    /// <exception cref="ArgumentException">当字符串为 null、超过 maxLength 或短于 minLength 时抛出。</exception>
    public static string NotNull(
        string? value,
        string parameterName,
        int maxLength = int.MaxValue,
        int minLength = 0)
    {
        if (value == null)
        {
            throw new ArgumentException($"{parameterName} can not be null!", parameterName);
        }

        if (value.Length > maxLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
        }

        if (minLength > 0 && value.Length < minLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
        }

        return value;
    }

    /// <summary>
    /// 检查提供的字符串是否不为 null、空或空格，并验证其长度限制。
    /// </summary>
    /// <param name="value">要检查的字符串值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <param name="maxLength">字符串允许的最大长度。默认为 <see cref="int.MaxValue"/>。</param>
    /// <param name="minLength">字符串允许的最小长度。默认为 0。</param>
    /// <returns>已验证的字符串（如果有效）。</returns>
    /// <exception cref="ArgumentException">当字符串为 null、空、空白、超过 maxLength 或短于 minLength 时抛出。</exception>
    public static string NotNullOrWhiteSpace(
        string? value,
        string parameterName,
        int maxLength = int.MaxValue,
        int minLength = 0)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{parameterName} can not be null, empty or white space!", parameterName);
        }

        if (value!.Length > maxLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
        }

        if (minLength > 0 && value!.Length < minLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
        }

        return value;
    }

    /// <summary>
    /// 检查提供的字符串是否不为 null 或空并验证其长度约束。
    /// </summary>
    /// <param name="value">要检查的字符串值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <param name="maxLength">字符串允许的最大长度。默认为 <see cref="int.MaxValue"/>。</param>
    /// <param name="minLength">字符串允许的最小长度。默认为 0。</param>
    /// <returns>已验证的字符串（如果有效）。</returns>
    /// <exception cref="ArgumentException">当字符串为 null、空、超过 maxLength 或短于 minLength 时抛出。</exception>
    public static string NotNullOrEmpty(
        string? value,
        string parameterName,
        int maxLength = int.MaxValue,
        int minLength = 0)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"{parameterName} can not be null or empty!", parameterName);
        }

        if (value!.Length > maxLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
        }

        if (minLength > 0 && value!.Length < minLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
        }

        return value;
    }

    /// <summary>
    /// 检查提供的集合是否不为 null 或为空。
    /// </summary>
    /// <typeparam name="T">集合中项目的类型。</typeparam>
    /// <param name="value">要检查的集合。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <returns>已验证的集合（如果不为 null 或为空）。</returns>
    /// <exception cref="ArgumentException">当集合为 null 或为空时抛出。</exception>
    public static ICollection<T> NotNullOrEmpty<T>(
        ICollection<T>? value,
        string parameterName)
    {
        if (value == null || value.Count <= 0)
        {
            throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
        }

        return value;
    }

    /// <summary>
    /// 检查提供的类型是否可分配给指定的基类型。
    /// </summary>
    /// <typeparam name="TBaseType">用于检查可分配性的基本类型。</typeparam>
    /// <param name="type">要检查的类型。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <returns>已验证的类型（如果可分配）。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="type"/> 为 null 时抛出。</exception>
    /// <exception cref="ArgumentException">当类型不可分配给 <typeparamref name="TBaseType"/> 时抛出。</exception>
    public static Type AssignableTo<TBaseType>(
        Type type,
        string parameterName)
    {
        NotNull(type, parameterName);

        if (!typeof(TBaseType).IsAssignableFrom(type))
        {
            throw new ArgumentException($"{parameterName} (type of {type.AssemblyQualifiedName}) should be assignable to the {typeof(TBaseType).FullName}!");
        }

        return type;
    }

    /// <summary>
    /// 使用可选的最小和最大约束验证字符串的长度。
    /// 允许空值，除非 minLength 大于 0。
    /// </summary>
    /// <param name="value">要检查的字符串值。如果 minLength 为 0，则可以为 null。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <param name="maxLength">字符串允许的最大长度。</param>
    /// <param name="minLength">字符串允许的最小长度。默认为 0。</param>
    /// <returns>已验证的字符串（如果有效）。</returns>
    /// <exception cref="ArgumentException">当字符串违反长度限制时抛出。</exception>
    public static string? Length(
        string? value,
        string parameterName,
        int maxLength,
        int minLength = 0)
    {
        if (minLength > 0)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
            }

            if (value!.Length < minLength)
            {
                throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
            }
        }

        if (value != null && value.Length > maxLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
        }

        return value;
    }

    /// <summary>
    /// 检查提供的 16 位有符号整数是否为正（大于零）。
    /// </summary>
    /// <param name="value">要检查的值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <returns>验证值（如果为正）。</returns>
    /// <exception cref="ArgumentException">当值为零或负数时抛出。</exception>
    public static Int16 Positive(
        Int16 value,
        string parameterName)
    {
        if (value == 0)
        {
            throw new ArgumentException($"{parameterName} is equal to zero");
        }
        else if (value < 0)
        {
            throw new ArgumentException($"{parameterName} is less than zero");
        }
        return value;
    }

    /// <summary>
    /// 检查提供的 32 位有符号整数是否为正（大于零）。
    /// </summary>
    /// <param name="value">要检查的值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <returns>验证值（如果为正）。</returns>
    /// <exception cref="ArgumentException">当值为零或负数时抛出。</exception>
    public static Int32 Positive(
        Int32 value,
        string parameterName)
    {
        if (value == 0)
        {
            throw new ArgumentException($"{parameterName} is equal to zero");
        }
        else if (value < 0)
        {
            throw new ArgumentException($"{parameterName} is less than zero");
        }
        return value;
    }

    /// <summary>
    /// 检查提供的 64 位有符号整数是否为正（大于零）。
    /// </summary>
    /// <param name="value">要检查的值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <returns>验证值（如果为正）。</returns>
    /// <exception cref="ArgumentException">当值为零或负数时抛出。</exception>
    public static Int64 Positive(
        Int64 value,
        string parameterName)
    {
        if (value == 0)
        {
            throw new ArgumentException($"{parameterName} is equal to zero");
        }
        else if (value < 0)
        {
            throw new ArgumentException($"{parameterName} is less than zero");
        }
        return value;
    }

    /// <summary>
    /// 检查提供的单精度浮点数是否为正（大于零）。
    /// </summary>
    /// <param name="value">要检查的值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <returns>验证值（如果为正）。</returns>
    /// <exception cref="ArgumentException">当值为零或负数时抛出。</exception>
    public static float Positive(
        float value,
        string parameterName)
    {
        if (value == 0)
        {
            throw new ArgumentException($"{parameterName} is equal to zero");
        }
        else if (value < 0)
        {
            throw new ArgumentException($"{parameterName} is less than zero");
        }
        return value;
    }

    /// <summary>
    /// 检查提供的双精度浮点数是否为正（大于零）。
    /// </summary>
    /// <param name="value">要检查的值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <returns>验证值（如果为正）。</returns>
    /// <exception cref="ArgumentException">当值为零或负数时抛出。</exception>
    public static double Positive(
        double value,
        string parameterName)
    {
        if (value == 0)
        {
            throw new ArgumentException($"{parameterName} is equal to zero");
        }
        else if (value < 0)
        {
            throw new ArgumentException($"{parameterName} is less than zero");
        }
        return value;
    }

    /// <summary>
    /// 检查提供的十进制数是否为正数（大于零）。
    /// </summary>
    /// <param name="value">要检查的值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <returns>验证值（如果为正）。</returns>
    /// <exception cref="ArgumentException">当值为零或负数时抛出。</exception>
    public static decimal Positive(
        decimal value,
        string parameterName)
    {
        if (value == 0)
        {
            throw new ArgumentException($"{parameterName} is equal to zero");
        }
        else if (value < 0)
        {
            throw new ArgumentException($"{parameterName} is less than zero");
        }
        return value;
    }

    /// <summary>
    /// 检查提供的 16 位有符号整数是否在指定范围内。
    /// </summary>
    /// <param name="value">要检查的值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <param name="minimumValue">允许的最小值（含）。</param>
    /// <param name="maximumValue">最大允许值（含）。默认为 <see cref="Int16.MaxValue"/>。</param>
    /// <returns>验证值（如果在范围内）。</returns>
    /// <exception cref="ArgumentException">当值超出指定范围时抛出。</exception>
    public static Int16 Range(
        Int16 value,
        string parameterName,
        Int16 minimumValue,
        Int16 maximumValue = Int16.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }

        return value;
    }

    /// <summary>
    /// 检查提供的 32 位有符号整数是否在指定范围内。
    /// </summary>
    /// <param name="value">要检查的值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <param name="minimumValue">允许的最小值（含）。</param>
    /// <param name="maximumValue">最大允许值（含）。默认为 <see cref="Int32.MaxValue"/>。</param>
    /// <returns>验证值（如果在范围内）。</returns>
    /// <exception cref="ArgumentException">当值超出指定范围时抛出。</exception>
    public static Int32 Range(
        Int32 value,
        string parameterName,
        Int32 minimumValue,
        Int32 maximumValue = Int32.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }

        return value;
    }

    /// <summary>
    /// 检查提供的 64 位有符号整数是否在指定范围内。
    /// </summary>
    /// <param name="value">要检查的值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <param name="minimumValue">允许的最小值（含）。</param>
    /// <param name="maximumValue">最大允许值（含）。默认为 <see cref="Int64.MaxValue"/>。</param>
    /// <returns>验证值（如果在范围内）。</returns>
    /// <exception cref="ArgumentException">当值超出指定范围时抛出。</exception>
    public static Int64 Range(
        Int64 value,
        string parameterName,
        Int64 minimumValue,
        Int64 maximumValue = Int64.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }

        return value;
    }

    /// <summary>
    /// 检查提供的单精度浮点数是否在指定范围内。
    /// </summary>
    /// <param name="value">要检查的值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <param name="minimumValue">允许的最小值（含）。</param>
    /// <param name="maximumValue">最大允许值（含）。默认为 <see cref="float.MaxValue"/>。</param>
    /// <returns>验证值（如果在范围内）。</returns>
    /// <exception cref="ArgumentException">当值超出指定范围时抛出。</exception>
    public static float Range(
        float value,
        string parameterName,
        float minimumValue,
        float maximumValue = float.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }
        return value;
    }

    /// <summary>
    /// 检查提供的双精度浮点数是否在指定范围内。
    /// </summary>
    /// <param name="value">要检查的值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <param name="minimumValue">允许的最小值（含）。</param>
    /// <param name="maximumValue">最大允许值（含）。默认为 <see cref="double.MaxValue"/>。</param>
    /// <returns>验证值（如果在范围内）。</returns>
    /// <exception cref="ArgumentException">当值超出指定范围时抛出。</exception>
    public static double Range(
        double value,
        string parameterName,
        double minimumValue,
        double maximumValue = double.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }

        return value;
    }

    /// <summary>
    /// 检查提供的十进制数是否在指定范围内。
    /// </summary>
    /// <param name="value">要检查的值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <param name="minimumValue">允许的最小值（含）。</param>
    /// <param name="maximumValue">最大允许值（含）。默认为 <see cref="decimal.MaxValue"/>。</param>
    /// <returns>验证值（如果在范围内）。</returns>
    /// <exception cref="ArgumentException">当值超出指定范围时抛出。</exception>
    public static decimal Range(
        decimal value,
        string parameterName,
        decimal minimumValue,
        decimal maximumValue = decimal.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }

        return value;
    }

    /// <summary>
    /// 检查提供的可为 null 的结构体值是否不为 null 且不等于其默认值。
    /// </summary>
    /// <typeparam name="T">要检查的值的结构类型。</typeparam>
    /// <param name="value">要检查的可为空值。</param>
    /// <param name="parameterName">正在检查的参数的名称。</param>
    /// <returns>非空结构值（如果有效）。</returns>
    /// <exception cref="ArgumentException">当值为 null 或等于结构体的默认值时抛出。</exception>
    public static T NotDefaultOrNull<T>(
        T? value,
        string parameterName)
        where T : struct
    {
        if (value == null)
        {
            throw new ArgumentException($"{parameterName} is null!", parameterName);
        }

        if (value.Value.Equals(default(T)))
        {
            throw new ArgumentException($"{parameterName} has a default value!", parameterName);
        }

        return value.Value;
    }
}
