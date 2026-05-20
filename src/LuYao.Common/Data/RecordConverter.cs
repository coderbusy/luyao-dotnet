using System;

namespace LuYao.Data;

/// <summary>
/// 定义在 DTO 属性类型与 RecordTable 列类型之间进行双向转换的转换器基类。
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Convert"/> 方法承担双向转换，方向由 <paramref name="sourceType"/> 和 <paramref name="targetType"/> 参数决定：
/// <list type="bullet">
///   <item><description>DTO → Table：sourceType 为属性类型，targetType 为列类型（受支持类型）。</description></item>
///   <item><description>Table → DTO：sourceType 为列类型，targetType 为属性类型。</description></item>
/// </list>
/// </para>
/// <para>
/// 推荐继承泛型子类 <see cref="RecordConverter{TSource, TTarget}"/>，它提供了类型安全的重写方式。
/// </para>
/// </remarks>
public abstract class RecordConverter
{
    /// <summary>
    /// 判断此转换器是否支持从 <paramref name="sourceType"/> 到 <paramref name="targetType"/> 的转换。
    /// </summary>
    /// <param name="sourceType">来源类型。</param>
    /// <param name="targetType">目标类型。</param>
    /// <returns>支持该转换时返回 <see langword="true"/>，否则返回 <see langword="false"/>。</returns>
    public abstract bool CanConvert(Type sourceType, Type targetType);

    /// <summary>
    /// 将 <paramref name="value"/> 从 <paramref name="sourceType"/> 转换为 <paramref name="targetType"/>。
    /// </summary>
    /// <param name="sourceType">来源类型。</param>
    /// <param name="targetType">目标类型。</param>
    /// <param name="value">待转换的值。</param>
    /// <returns>转换后的值。</returns>
    /// <exception cref="NotSupportedException">
    /// 当 <see cref="CanConvert"/> 返回 <see langword="false"/> 时调用此方法应抛出。
    /// </exception>
    public abstract object? Convert(Type sourceType, Type targetType, object? value);
}

/// <summary>
/// 提供类型安全的双向转换器基类。
/// </summary>
/// <typeparam name="TSource">其中一端的类型（通常为 DTO 属性类型或 RecordTable 列类型）。</typeparam>
/// <typeparam name="TTarget">另一端的类型。</typeparam>
/// <remarks>
/// 此类自动支持 <c>TSource → TTarget</c> 和 <c>TTarget → TSource</c> 两个方向。
/// </remarks>
public abstract class RecordConverter<TSource, TTarget> : RecordConverter
{
    /// <inheritdoc/>
    public override bool CanConvert(Type sourceType, Type targetType)
    {
        return (sourceType == typeof(TSource) && targetType == typeof(TTarget))
            || (sourceType == typeof(TTarget) && targetType == typeof(TSource));
    }

    /// <inheritdoc/>
    public override object? Convert(Type sourceType, Type targetType, object? value)
    {
        if (sourceType == typeof(TSource) && targetType == typeof(TTarget))
            return ConvertForward(value is TSource v ? v : (TSource?)value);
        if (sourceType == typeof(TTarget) && targetType == typeof(TSource))
            return ConvertBackward(value is TTarget v ? v : (TTarget?)value);
        throw new NotSupportedException(
            $"{GetType().Name} 不支持从 {sourceType.Name} 到 {targetType.Name} 的转换。");
    }

    /// <summary>
    /// 将 <typeparamref name="TSource"/> 转换为 <typeparamref name="TTarget"/>（DTO → Table 方向）。
    /// </summary>
    protected abstract TTarget? ConvertForward(TSource? value);

    /// <summary>
    /// 将 <typeparamref name="TTarget"/> 转换回 <typeparamref name="TSource"/>（Table → DTO 方向）。
    /// </summary>
    protected abstract TSource? ConvertBackward(TTarget? value);
}
