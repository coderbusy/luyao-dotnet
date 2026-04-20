using System.Collections.Generic;

namespace LuYao.Data.Meta;

/// <summary>
/// 提供对对象属性的按名称读写操作。
/// </summary>
/// <remarks>
/// 写入时，若指定的属性名不存在，则静默跳过，不抛出异常。
/// 读取时，若指定的属性名不存在，则返回 <see langword="null"/>。
/// </remarks>
public interface IPropertyAccessor
{
    /// <summary>
    /// 获取当前对象所有可访问的属性元数据列表。
    /// </summary>
    IReadOnlyList<IXProp> Props { get; }

    /// <summary>
    /// 按属性名读写属性值。
    /// </summary>
    /// <param name="name">属性名称（大小写敏感）。</param>
    /// <value>
    /// 读取时，若属性不存在则返回 <see langword="null"/>；
    /// 写入时，若属性不存在则静默跳过。
    /// </value>
    object? this[string name] { get; set; }
}
