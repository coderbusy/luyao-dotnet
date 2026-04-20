using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace LuYao.Data.Meta;

/// <summary>
/// 提供对任意对象属性的按名称读写操作，通过运行时反射调用 <see cref="XData{T}"/> 实现。
/// </summary>
public static class XMeta
{
    // 每个运行时类型只编译一次委托，后续复用
    private static readonly ConcurrentDictionary<Type, Func<object, IPropertyAccessor>> _cache =
        new ConcurrentDictionary<Type, Func<object, IPropertyAccessor>>();

    /// <summary>
    /// 为指定对象实例创建一个 <see cref="IPropertyAccessor"/>，支持按属性名读写。
    /// </summary>
    /// <param name="data">目标对象实例。</param>
    /// <returns>绑定到 <paramref name="data"/> 的 <see cref="IPropertyAccessor"/> 实例。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="data"/> 为 null 时抛出。</exception>
    /// <remarks>
    /// 写入时，若指定的属性名不存在，则静默跳过，不抛出异常。
    /// 读取时，若指定的属性名不存在，则返回 <see langword="null"/>。
    /// </remarks>
    public static IPropertyAccessor CreatePropertyAccessor(object data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        var factory = _cache.GetOrAdd(data.GetType(), BuildFactory);
        return factory(data);
    }

    /// <summary>
    /// 为指定对象实例创建一个 <see cref="IPropertyAccessor"/>，支持按属性名读写。
    /// </summary>
    /// <param name="data">目标对象实例。</param>
    /// <returns>绑定到 <paramref name="data"/> 的 <see cref="IPropertyAccessor"/> 实例。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="data"/> 为 null 时抛出。</exception>
    /// <remarks>
    /// 写入时，若指定的属性名不存在，则静默跳过，不抛出异常。
    /// 读取时，若指定的属性名不存在，则返回 <see langword="null"/>。
    /// </remarks>
    public static IPropertyAccessor CreatePropertyAccessor<T>(T data) where T : class
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        return XData<T>.CreatePropertyAccessor(data);
    }

    private static Func<object, IPropertyAccessor> BuildFactory(Type type)
    {
        var indexerType = typeof(XData<>).MakeGenericType(type);
        var method = indexerType.GetMethod("CreatePropertyAccessor", [type]);
        if (method == null)
            throw new InvalidOperationException($"未找到 {indexerType.FullName} 上的 CreatePropertyAccessor 方法。");

        // 编译: (object obj) => XData<T>.CreatePropertyAccessor((T)obj)
        var param = Expression.Parameter(typeof(object), "obj");
        var call = Expression.Call(method, Expression.Convert(param, type));
        return Expression.Lambda<Func<object, IPropertyAccessor>>(call, param).Compile();
    }
}
