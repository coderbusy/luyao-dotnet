using System;

namespace LuYao.Data;

/// <summary>
/// 标记属性对应的列名称，优先级高于 <see cref="RecordNamingPolicy"/>。
/// 对标 <see cref="System.Text.Json.Serialization.JsonPropertyNameAttribute"/>。
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class RecordColumnNameAttribute : RecordAttribute
{
    /// <summary>
    /// 初始化 <see cref="RecordColumnNameAttribute"/> 的新实例。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 为 null 或空白时抛出。</exception>
    public RecordColumnNameAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        Name = name;
    }

    /// <summary>
    /// 列的名称。
    /// </summary>
    public string Name { get; }
}
