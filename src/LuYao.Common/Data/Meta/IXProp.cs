using System;

namespace LuYao.Data.Meta;

/// <summary>
/// 表示一个可按对象实例读写的属性描述符。
/// </summary>
/// <remarks>
/// 由 <see cref="XProp"/>（基于反射）和 <see cref="RecordColumn"/>（基于列存储）实现。
/// <c>GetValue</c>/<c>SetValue</c> 的 <c>instance</c> 参数语义取决于具体实现：
/// 对于 <see cref="XProp"/>，<c>instance</c> 为承载属性的 CLR 对象；
/// 对于 <see cref="RecordColumn"/>，<c>instance</c> 为 <see cref="RecordRow"/>（值类型装箱后传入）。
/// </remarks>
public interface IXProp
{
    /// <summary>属性名称。</summary>
    string Name { get; }

    /// <summary>属性类型。</summary>
    Type Type { get; }

    /// <summary>是否可读。</summary>
    bool CanRead { get; }

    /// <summary>是否可写。</summary>
    bool CanWrite { get; }

    /// <summary>
    /// 读取指定实例的属性值。
    /// </summary>
    /// <param name="instance">属性所属的对象实例。</param>
    /// <returns>属性当前值。</returns>
    /// <exception cref="InvalidOperationException">属性不可读时抛出。</exception>
    object? GetValue(object instance);

    /// <summary>
    /// 设置指定实例的属性值。
    /// </summary>
    /// <param name="instance">属性所属的对象实例。</param>
    /// <param name="value">要写入的值。</param>
    /// <exception cref="InvalidOperationException">属性不可写时抛出。</exception>
    void SetValue(object instance, object? value);
}
