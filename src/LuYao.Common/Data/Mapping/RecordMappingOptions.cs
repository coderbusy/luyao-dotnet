using LuYao.Data.Attributes;
using System;

namespace LuYao.Data.Mapping;

/// <summary>
/// 控制 <see cref="RecordTable"/> 与 DTO 之间映射行为的选项类。
/// </summary>
/// <remarks>
/// <para>
/// 实例首次参与映射任务后将被冻结（只读），之后任何属性赋值或 <see cref="AddConverter"/> 调用
/// 均会抛出 <see cref="InvalidOperationException"/>。
/// </para>
/// <para>
/// 可通过 <see cref="Default"/> 获取全局默认实例；也可自行创建实例并在首次使用前配置。
/// </para>
/// </remarks>
public sealed class RecordMappingOptions
{
    // ─── 静态默认实例 ───────────────────────────────────────────────────────────

    /// <summary>全局默认选项实例。</summary>
    public static RecordMappingOptions Default { get; } = new RecordMappingOptions();

    // ─── 只读控制 ────────────────────────────────────────────────────────────────

    private bool _isReadOnly;

    /// <summary>
    /// 将当前实例标记为只读（冻结）。首次映射任务开始时由框架自动调用，调用后属性不可再更改。
    /// </summary>
    internal void MakeReadOnly()
    {
        _isReadOnly = true;
    }

    /// <summary>是否已被冻结（只读）。</summary>
    public bool IsReadOnly => _isReadOnly;

    private void ThrowIfReadOnly()
    {
        if (_isReadOnly)
            throw new InvalidOperationException(
                "RecordMappingOptions 已被冻结，首次参与映射后不可再修改。");
    }

    // ─── NamingPolicy ────────────────────────────────────────────────────────────

    private RecordNamingPolicy? _namingPolicy;

    /// <summary>
    /// 列名转换策略。<see langword="null"/> 表示使用属性原名（除非有 <see cref="Attributes.RecordColumnNameAttribute"/> 标记）。
    /// </summary>
    public RecordNamingPolicy? NamingPolicy
    {
        get => _namingPolicy;
        set { ThrowIfReadOnly(); _namingPolicy = value; }
    }

    // ─── UnsupportedTypeHandling ─────────────────────────────────────────────────

    private UnsupportedTypeHandling _unsupportedTypeHandling = UnsupportedTypeHandling.Skip;

    /// <summary>
    /// DTO → RecordTable 时遇到不支持的属性类型的处理策略，默认为 <see cref="UnsupportedTypeHandling.Skip"/>。
    /// </summary>
    public UnsupportedTypeHandling UnsupportedTypeHandling
    {
        get => _unsupportedTypeHandling;
        set { ThrowIfReadOnly(); _unsupportedTypeHandling = value; }
    }

    // ─── ConversionFailureHandling ───────────────────────────────────────────────

    private ConversionFailureHandling _conversionFailureHandling = ConversionFailureHandling.Skip;

    /// <summary>
    /// RecordTable → DTO 时列值类型转换失败的处理策略，默认为 <see cref="ConversionFailureHandling.Skip"/>。
    /// </summary>
    public ConversionFailureHandling ConversionFailureHandling
    {
        get => _conversionFailureHandling;
        set { ThrowIfReadOnly(); _conversionFailureHandling = value; }
    }
}
