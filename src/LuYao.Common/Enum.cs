using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NETCOREAPP2_1_OR_GREATER
using System.Collections.Immutable;
#endif

namespace LuYao;

/// <summary>
/// Enum 的强类型版本，提供解析和性能改进。
/// </summary>
/// <typeparam name="T">枚举的类型</typeparam>
public static class Enum<T>
    where T : struct, IConvertible
{
    static readonly IReadOnlyList<T> all = Enum.GetValues(typeof(T)).Cast<T>().ToList();
    static readonly Dictionary<string, T> insensitiveNames = all.ToDictionary(
        k => Enum.GetName(typeof(T), k).ToUpperInvariant()
    );
    static readonly Dictionary<string, T> sensitiveNames = all.ToDictionary(
        k => Enum.GetName(typeof(T), k)
    );
    static readonly Dictionary<int, T> values = all.ToDictionary(k => Convert.ToInt32(k));
    static readonly Dictionary<T, string> names = all.ToDictionary(k => k, v => v.ToString());

    /// <summary>
    /// 判断枚举值是否已定义
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsDefined(T value) => names.Keys.Contains(value);

    /// <summary>
    /// 判断枚举名称是否已定义
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsDefined(string value) => sensitiveNames.Keys.Contains(value);

    /// <summary>
    /// 判断枚举整数值是否已定义
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsDefined(int value) => values.Keys.Contains(value);

    /// <summary>
    /// 获取枚举的所有值
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<T> GetValues() => all;

#if NETCOREAPP2_1_OR_GREATER
    /// <summary>
    /// 获取枚举的所有名称
    /// </summary>
    public static IReadOnlyList<string> GetNames() => names.Values.ToImmutableArray();
#else
    /// <summary>
    /// 获取枚举的所有名称
    /// </summary>
    public static IReadOnlyList<string> GetNames() => names.Values.ToArray();
#endif

    /// <summary>
    /// 获取枚举值对应的名称
    /// </summary>
    public static string GetName(T value)
    {
        names.TryGetValue(value, out string name);
        return name;
    }

    /// <summary>
    /// 解析枚举名称为枚举值
    /// </summary>
    public static T Parse(string value)
    {
        if (!sensitiveNames.TryGetValue(value, out T parsed))
            throw new ArgumentException(
                "指定的值不是枚举定义的命名常量之一。",
                nameof(value)
            );
        return parsed;
    }

    /// <summary>
    /// 解析枚举名称为枚举值，可忽略大小写
    /// </summary>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static T Parse(string value, bool ignoreCase)
    {
        if (!ignoreCase) return Parse(value);

        if (!insensitiveNames.TryGetValue(value.ToUpperInvariant(), out T parsed)) throw new ArgumentException("指定的值不是枚举定义的命名常量之一。", nameof(value));
        return parsed;
    }

    /// <summary>
    /// 尝试解析枚举名称为枚举值
    /// </summary>
    public static bool TryParse(string value, out T returnValue) =>
        sensitiveNames.TryGetValue(value, out returnValue);

    /// <summary>
    /// 尝试解析枚举名称为枚举值，可忽略大小写
    /// </summary>
    public static bool TryParse(string value, bool ignoreCase, out T returnValue)
    {
        return ignoreCase
            ? insensitiveNames.TryGetValue(value.ToUpperInvariant(), out returnValue)
            : TryParse(value, out returnValue);
    }

    /// <summary>
    /// 解析枚举名称为枚举值，若解析失败返回 null
    /// </summary>
    public static T? ParseOrNull(string value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        if (sensitiveNames.TryGetValue(value, out T foundValue))
            return foundValue;

        return null;
    }

    /// <summary>
    /// 解析枚举名称为枚举值，可忽略大小写，若解析失败返回 null
    /// </summary>
    public static T? ParseOrNull(string value, bool ignoreCase)
    {
        if (!ignoreCase)
            return ParseOrNull(value);

        if (string.IsNullOrEmpty(value))
            return null;

        if (insensitiveNames.TryGetValue(value.ToUpperInvariant(), out T foundValue))
            return foundValue;

        return null;
    }

    /// <summary>
    /// 将整数值转换为枚举值，若转换失败返回 null
    /// </summary>
    public static T? CastOrNull(int value)
    {
        if (values.TryGetValue(value, out T foundValue))
            return foundValue;

        return null;
    }

    /// <summary>
    /// 获取枚举标志位中包含的所有标志
    /// </summary>
    public static IEnumerable<T> GetFlags(T flagEnum)
    {
        var flagInt = Convert.ToInt32(flagEnum);
        return all.Where(e => (Convert.ToInt32(e) & flagInt) != 0);
    }

    /// <summary>
    /// 设置枚举标志位
    /// </summary>
    public static T SetFlags(IEnumerable<T> flags)
    {
        var combined = flags.Aggregate(
            default(int),
            (current, flag) => current | Convert.ToInt32(flag)
        );
        return (T)Convert.ChangeType(combined, typeof(T));
    }
}
