using LuYao.Data.Mapping;
using System;

namespace LuYao.Data.Attributes;

/// <summary>
/// 声明属性在 <see cref="RecordTable"/> 中的列存储类型，优先级高于 <see cref="RecordMappingOptions"/> 的全局策略。
/// </summary>
/// <remarks>
/// <para>
/// 仅在属性类型不是 RecordTable 原生支持的类型时才有意义。对于原生支持的类型，此特性会被忽略。
/// </para>
/// <para>
/// 使用 <see cref="RecordColumnStorageTarget.String"/> 或 <see cref="RecordColumnStorageTarget.Bytes"/> 时，
/// 必须在 <see cref="RecordMappingOptions"/> 中注册该属性类型对应的 <see cref="RecordConverter"/>，否则将在映射时抛出异常。
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // 将 Guid 属性以字符串形式存储到 RecordTable
/// [RecordColumnStorage(RecordColumnStorageTarget.String)]
/// public Guid Id { get; set; }
///
/// // 跳过该属性，不创建对应列
/// [RecordColumnStorage(RecordColumnStorageTarget.Skip)]
/// public MyComplexObject Payload { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class RecordColumnStorageAttribute : RecordAttribute
{
    /// <summary>
    /// 初始化 <see cref="RecordColumnStorageAttribute"/> 的新实例。
    /// </summary>
    /// <param name="target">指定的存储类型。</param>
    public RecordColumnStorageAttribute(RecordColumnStorageTarget target)
    {
        Target = target;
    }

    /// <summary>
    /// 属性在 RecordTable 中的列存储类型。
    /// </summary>
    public RecordColumnStorageTarget Target { get; }
}
