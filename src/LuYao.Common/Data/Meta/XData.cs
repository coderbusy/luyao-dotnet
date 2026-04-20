using System;
using System.Collections.Generic;

namespace LuYao.Data.Meta;

/// <summary>
/// 提供对强类型对象属性的按名称读写操作，底层使用 <see cref="XProp"/> 加速反射访问。
/// </summary>
/// <typeparam name="T">目标类型，必须具有无参构造函数。</typeparam>
public static class XData<T> where T : class
{
    private static readonly IReadOnlyList<XProp> _props = XProp.GetAll(typeof(T));

    private static XProp Get(string name)
    {
        foreach (var p in _props)
        {
            if (string.Equals(p.Name, name, StringComparison.Ordinal))
            {
                return p;
            }
        }
        throw new ArgumentException($"类型 {typeof(T).FullName} 上未找到属性 '{name}'。", nameof(name));
    }

    /// <summary>
    /// 按属性名写入值。
    /// </summary>
    /// <param name="data">目标对象实例。</param>
    /// <param name="name">属性名称（大小写敏感）。</param>
    /// <param name="value">要写入的值。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="data"/> 为 null 时抛出。</exception>
    /// <exception cref="ArgumentException">属性不存在时抛出。</exception>
    /// <exception cref="InvalidOperationException">属性不可写时抛出。</exception>
    public static void Set(T data, string name, object? value)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        Get(name).SetValue(data, value);
    }

    /// <summary>
    /// 按属性名读取值。
    /// </summary>
    /// <param name="data">目标对象实例。</param>
    /// <param name="name">属性名称（大小写敏感）。</param>
    /// <returns>属性当前值。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="data"/> 为 null 时抛出。</exception>
    /// <exception cref="ArgumentException">属性不存在时抛出。</exception>
    /// <exception cref="InvalidOperationException">属性不可读时抛出。</exception>
    public static object? Get(T data, string name)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        return Get(name).GetValue(data);
    }

    /// <summary>
    /// 为指定对象实例创建一个 <see cref="IPropertyAccessor"/>，支持按属性名读写。
    /// </summary>
    /// <param name="data">目标对象实例。</param>
    /// <returns>绑定到 <paramref name="data"/> 的 <see cref="IPropertyAccessor"/> 实例。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="data"/> 为 null 时抛出。</exception>
    public static IPropertyAccessor CreatePropertyAccessor(T data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        return new Indexer(data);
    }

    private struct Indexer : IPropertyAccessor
    {
        private readonly T _data;

        public Indexer(T data) => _data = data;

        public IReadOnlyList<IXProp> Props => _props;

        public object? this[string name]
        {
            get
            {
                foreach (var p in _props)
                {
                    if (string.Equals(p.Name, name, StringComparison.Ordinal))
                    {
                        return p.GetValue(_data);
                    }
                }
                return null;
            }
            set
            {
                foreach (var p in _props)
                {
                    if (string.Equals(p.Name, name, StringComparison.Ordinal))
                    {
                        p.SetValue(_data, value); return;
                    }
                }
            }
        }
    }
}
