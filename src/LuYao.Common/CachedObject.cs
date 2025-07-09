using System;
using System.Diagnostics;

namespace LuYao;

/// <summary>
/// 表示一个通用的缓存对象。
/// </summary>
/// <typeparam name="T">缓存的值的类型。</typeparam>
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public readonly struct CachedObject<T>
{
    /// <summary>
    /// 使用指定的值并且没有过期日期初始化 <see cref="CachedObject{T}"/> 结构的新实例。
    /// </summary>
    /// <param name="value">缓存的值。</param>
    public CachedObject(T value)
    {
        Value = value;
        ExpirationDate = DateTimeOffset.MaxValue;
        CachedDate = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// 使用指定的值和过期日期初始化 <see cref="CachedObject{T}"/> 结构的新实例。
    /// </summary>
    /// <param name="value">缓存的值。</param>
    /// <param name="expirationDate">对象的过期日期。</param>
    public CachedObject(T value, DateTimeOffset expirationDate)
        : this(value)
    {
        ExpirationDate = expirationDate;
    }

    /// <summary>
    /// 使用指定的值和有效时长初始化 <see cref="CachedObject{T}"/> 结构的新实例。
    /// </summary>
    /// <param name="value">缓存的值。</param>
    /// <param name="duration">对象的有效时长。</param>
    public CachedObject(T value, TimeSpan duration)
        : this(value)
    {
        ExpirationDate = CachedDate.Add(duration);
    }

    /// <summary>
    /// 获取缓存的对象。
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// 获取对象的过期日期。
    /// </summary>
    public DateTimeOffset ExpirationDate { get; }

    /// <summary>
    /// 获取对象被缓存的日期。
    /// </summary>
    public DateTimeOffset CachedDate { get; }

    /// <summary>
    /// 返回该对象是否已过期。
    /// </summary>
    /// <returns>如果对象已过期返回 <see langword="true"/>，否则返回 <see langword="false"/>。</returns>
    public bool IsExpired() => DateTimeOffset.UtcNow > ExpirationDate;

    /// <inheritdoc/>
    public override string ToString() =>
        $"{nameof(Value)}: {Value}, {nameof(IsExpired)}: {IsExpired()}";

    private string DebuggerDisplay => ToString();
}

