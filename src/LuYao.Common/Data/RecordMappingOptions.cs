using System;
using System.Collections.Generic;

namespace LuYao.Data;

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

    // ─── 自定义类型转换器 ──────────────────────────────────────────────────────────

    private List<RecordConverter>? _converters;

    /// <summary>
    /// 注册自定义双向转换器。
    /// 转换器同时用于 DTO → Table（写）和 Table → DTO（读）两个方向。
    /// </summary>
    /// <param name="converter">转换器实例，不可为 null。</param>
    /// <exception cref="ArgumentNullException"><paramref name="converter"/> 为 null。</exception>
    /// <exception cref="InvalidOperationException">实例已被冻结时抛出。</exception>
    public void AddConverter(RecordConverter converter)
    {
        if (converter == null) throw new ArgumentNullException(nameof(converter));
        ThrowIfReadOnly();
        if (_converters == null) _converters = new List<RecordConverter>();
        _converters.Add(converter);
    }

    /// <summary>
    /// 查找支持从 <paramref name="sourceType"/> 到 <paramref name="targetType"/> 转换的已注册转换器。
    /// 不包含托底转换器 <see cref="Meta.DefaultRecordConverter"/>。
    /// </summary>
    /// <param name="sourceType">来源类型。</param>
    /// <param name="targetType">目标类型。</param>
    /// <returns>匹配的转换器；未找到则返回 <see langword="null"/>。</returns>
    internal RecordConverter? FindConverter(Type sourceType, Type targetType)
    {
        if (_converters == null) return null;
        foreach (var c in _converters)
        {
            if (c.CanConvert(sourceType, targetType)) return c;
        }
        return null;
    }
}
