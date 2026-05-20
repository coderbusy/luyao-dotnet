using LuYao.Data.Attributes;
using LuYao.Data.Meta;
using System.Reflection;

namespace LuYao.Data.Mapping;

/// <summary>
/// 根据 <see cref="RecordMappingOptions"/> 解析属性对应的列名。
/// 优先级：<see cref="RecordColumnNameAttribute"/> &gt; <see cref="RecordNamingPolicy"/> &gt; 属性原名。
/// </summary>
internal static class ColumnNameResolver
{
    /// <summary>
    /// 解析 <paramref name="prop"/> 对应的列名。
    /// </summary>
    /// <param name="prop">属性描述符。</param>
    /// <param name="options">映射选项；为 <see langword="null"/> 时使用属性原名。</param>
    /// <returns>解析后的列名。</returns>
    internal static string Resolve(XProp prop, RecordMappingOptions? options)
    {
        // 优先级1：RecordColumnNameAttribute
        var attr = prop.GetCustomAttribute<RecordColumnNameAttribute>();
        if (attr != null) return attr.Name;

        // 优先级2：NamingPolicy
        if (options?.NamingPolicy != null) return options.NamingPolicy.ConvertName(prop.Name);

        // 优先级3：属性原名
        return prop.Name;
    }
}
