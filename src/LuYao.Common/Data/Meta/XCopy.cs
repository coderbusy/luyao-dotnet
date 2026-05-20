using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LuYao.Data.Meta;

/// <summary>
/// 提供在对象与 <see cref="RecordRow"/> 之间进行属性双向复制的静态工具类。
/// 以运行时实际类型为键缓存属性列表，天然支持派生类的新增属性。
/// </summary>
/// <remarks>
/// 所有映射逻辑均委托给 <see cref="RecordMappingContext"/> 执行。
/// 无参重载等价于使用 <see cref="RecordMappingOptions.Default"/>。
/// </remarks>
public static class XCopy
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<XProp>> _readableCache
        = new ConcurrentDictionary<Type, IReadOnlyList<XProp>>();
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<XProp>> _writableCache
        = new ConcurrentDictionary<Type, IReadOnlyList<XProp>>();

    internal static IReadOnlyList<XProp> GetReadableProps(Type runtimeType)
        => _readableCache.GetOrAdd(runtimeType, t =>
        {
            var all = XProp.GetAll(t);
            var list = new List<XProp>(all.Count);
            foreach (var p in all)
            {
                if (Helpers.IsSupportedForReading(p)) list.Add(p);
            }
            return list.AsReadOnly();
        });

    internal static IReadOnlyList<XProp> GetWritableProps(Type runtimeType)
        => _writableCache.GetOrAdd(runtimeType, t =>
        {
            var all = XProp.GetAll(t);
            var list = new List<XProp>(all.Count);
            foreach (var p in all)
            {
                if (Helpers.IsSupportedForWriting(p)) list.Add(p);
            }
            return list.AsReadOnly();
        });

    #region RecordRow

    /// <summary>
    /// 将对象 <paramref name="data"/> 的可读属性值写入 <paramref name="target"/> 对应的列。
    /// 使用运行时实际类型扫描属性。若目标行所在表中不存在对应列则静默跳过，不会自动建列。
    /// </summary>
    /// <param name="data">数据来源对象，不可为 null。</param>
    /// <param name="target">目标行。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="data"/> 为 null 时抛出。</exception>
    public static void CopyTo(object data, RecordRow target)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        CopyTo(data.GetType(), data, target, RecordMappingOptions.Default);
    }

    /// <summary>
    /// 将 <paramref name="data"/> 中由 <paramref name="type"/> 指定类型的可读属性值写入
    /// <paramref name="target"/> 对应的列。若目标行所在表中不存在对应列则静默跳过，不会自动建列。
    /// </summary>
    /// <param name="type">用于扫描属性的声明类型，不可为 null。</param>
    /// <param name="data">数据来源对象，不可为 null。</param>
    /// <param name="target">目标行。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="type"/> 或 <paramref name="data"/> 为 null 时抛出。</exception>
    public static void CopyTo(Type type, object data, RecordRow target)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (data == null) throw new ArgumentNullException(nameof(data));
        CopyTo(type, data, target, RecordMappingOptions.Default);
    }

    /// <summary>
    /// 将对象 <paramref name="data"/> 的可读属性值写入 <paramref name="target"/> 对应的列，
    /// 使用指定的映射选项。若目标行所在表中不存在对应列则静默跳过，不会自动建列。
    /// </summary>
    /// <param name="data">数据来源对象，不可为 null。</param>
    /// <param name="target">目标行。</param>
    /// <param name="options">映射选项，不可为 null。</param>
    /// <exception cref="ArgumentNullException">任一参数为 null 时抛出。</exception>
    public static void CopyTo(object data, RecordRow target, RecordMappingOptions options)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        CopyTo(data.GetType(), data, target, options);
    }

    /// <summary>
    /// 将 <paramref name="data"/> 中由 <paramref name="type"/> 指定类型的可读属性值写入
    /// <paramref name="target"/> 对应的列，使用指定的映射选项。
    /// 若目标行所在表中不存在对应列则静默跳过，不会自动建列。
    /// </summary>
    /// <param name="type">用于扫描属性的声明类型，不可为 null。</param>
    /// <param name="data">数据来源对象，不可为 null。</param>
    /// <param name="target">目标行。</param>
    /// <param name="options">映射选项，不可为 null。</param>
    /// <exception cref="ArgumentNullException">任一参数为 null 时抛出。</exception>
    public static void CopyTo(Type type, object data, RecordRow target, RecordMappingOptions options)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (options == null) throw new ArgumentNullException(nameof(options));
        new RecordMappingContext(options).CopyDtoToRow(type, data, target);
    }

    /// <summary>
    /// 将 <paramref name="source"/> 中与对象属性对应的列值写回对象 <paramref name="data"/>。
    /// 使用运行时实际类型扫描属性。
    /// </summary>
    /// <param name="data">目标对象，不可为 null。</param>
    /// <param name="source">数据来源行。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="data"/> 为 null 时抛出。</exception>
    public static void CopyFrom(object data, RecordRow source)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        CopyFrom(data.GetType(), data, source, RecordMappingOptions.Default);
    }

    /// <summary>
    /// 将 <paramref name="source"/> 中与由 <paramref name="type"/> 指定类型属性对应的列值
    /// 写回对象 <paramref name="data"/>。
    /// </summary>
    /// <param name="type">用于扫描属性的声明类型，不可为 null。</param>
    /// <param name="data">目标对象，不可为 null。</param>
    /// <param name="source">数据来源行。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="type"/> 或 <paramref name="data"/> 为 null 时抛出。</exception>
    public static void CopyFrom(Type type, object data, RecordRow source)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (data == null) throw new ArgumentNullException(nameof(data));
        CopyFrom(type, data, source, RecordMappingOptions.Default);
    }

    /// <summary>
    /// 将 <paramref name="source"/> 中的列值写回对象 <paramref name="data"/>，
    /// 使用指定的映射选项。
    /// </summary>
    /// <param name="data">目标对象，不可为 null。</param>
    /// <param name="source">数据来源行。</param>
    /// <param name="options">映射选项，不可为 null。</param>
    /// <exception cref="ArgumentNullException">任一参数为 null 时抛出。</exception>
    public static void CopyFrom(object data, RecordRow source, RecordMappingOptions options)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        CopyFrom(data.GetType(), data, source, options);
    }

    /// <summary>
    /// 将 <paramref name="source"/> 中与由 <paramref name="type"/> 指定类型属性对应的列值
    /// 写回对象 <paramref name="data"/>，使用指定的映射选项（支持自定义转换器与列名策略）。
    /// </summary>
    /// <param name="type">用于扫描属性的声明类型，不可为 null。</param>
    /// <param name="data">目标对象，不可为 null。</param>
    /// <param name="source">数据来源行。</param>
    /// <param name="options">映射选项，不可为 null。</param>
    /// <exception cref="ArgumentNullException">任一参数为 null 时抛出。</exception>
    public static void CopyFrom(Type type, object data, RecordRow source, RecordMappingOptions options)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (options == null) throw new ArgumentNullException(nameof(options));
        new RecordMappingContext(options).CopyRowToDto(type, data, source);
    }

    /// <summary>
    /// 将对象 <paramref name="data"/> 的可读属性值写入 <paramref name="target"/> 对应的列。
    /// 若目标行所在表中不存在对应列则<b>自动创建列</b>后再写入。
    /// 使用运行时实际类型扫描属性。
    /// </summary>
    /// <param name="data">数据来源对象，不可为 null。</param>
    /// <param name="target">目标行。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="data"/> 为 null 时抛出。</exception>
    public static void WriteTo(object data, RecordRow target)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        WriteTo(data.GetType(), data, target, RecordMappingOptions.Default);
    }

    /// <summary>
    /// 将 <paramref name="data"/> 中由 <paramref name="type"/> 指定类型的可读属性值写入
    /// <paramref name="target"/> 对应的列。若目标行所在表中不存在对应列则<b>自动创建列</b>后再写入。
    /// </summary>
    /// <param name="type">用于扫描属性的声明类型，不可为 null。</param>
    /// <param name="data">数据来源对象，不可为 null。</param>
    /// <param name="target">目标行。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="type"/> 或 <paramref name="data"/> 为 null 时抛出。</exception>
    public static void WriteTo(Type type, object data, RecordRow target)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (data == null) throw new ArgumentNullException(nameof(data));
        WriteTo(type, data, target, RecordMappingOptions.Default);
    }

    /// <summary>
    /// 将对象 <paramref name="data"/> 的可读属性值写入 <paramref name="target"/> 对应的列
    /// （自动建列），使用指定的映射选项。
    /// </summary>
    /// <param name="data">数据来源对象，不可为 null。</param>
    /// <param name="target">目标行。</param>
    /// <param name="options">映射选项，不可为 null。</param>
    /// <exception cref="ArgumentNullException">任一参数为 null 时抛出。</exception>
    public static void WriteTo(object data, RecordRow target, RecordMappingOptions options)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        WriteTo(data.GetType(), data, target, options);
    }

    /// <summary>
    /// 将 <paramref name="data"/> 中由 <paramref name="type"/> 指定类型的可读属性值写入
    /// <paramref name="target"/> 对应的列（自动建列），使用指定的映射选项。
    /// </summary>
    /// <param name="type">用于扫描属性的声明类型，不可为 null。</param>
    /// <param name="data">数据来源对象，不可为 null。</param>
    /// <param name="target">目标行。</param>
    /// <param name="options">映射选项，不可为 null。</param>
    /// <exception cref="ArgumentNullException">任一参数为 null 时抛出。</exception>
    public static void WriteTo(Type type, object data, RecordRow target, RecordMappingOptions options)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (options == null) throw new ArgumentNullException(nameof(options));
        new RecordMappingContext(options).WriteDtoToRow(type, data, target);
    }

    #endregion
}

/// <summary>
/// 提供在强类型对象与 <see cref="RecordRow"/> 之间进行属性双向复制的静态工具类。
/// 为 <see cref="XCopy"/> 的泛型薄封装，固定按编译期类型 <typeparamref name="T"/> 扫描属性，
/// 逻辑委托给 <see cref="XCopy"/> 的显式 <see cref="Type"/> 重载。
/// </summary>
/// <typeparam name="T">要映射的对象类型，必须为引用类型。</typeparam>
public static class XCopy<T> where T : class
{
    #region RecordRow
    /// <summary>
    /// 将对象 <paramref name="data"/> 的可读属性值写入 <paramref name="target"/> 对应的列。
    /// 固定按编译期类型 <typeparamref name="T"/> 扫描属性，
    /// 即使 <paramref name="data"/> 的运行时类型为派生类，也不会处理派生类新增属性。
    /// </summary>
    /// <param name="data">数据来源对象。</param>
    /// <param name="target">目标行；若列不存在则静默跳过，<b>不会自动建列</b>。</param>
    public static void CopyTo(T data, RecordRow target) => XCopy.CopyTo(typeof(T), data, target);

    /// <summary>将对象 <paramref name="data"/> 的可读属性值写入 <paramref name="target"/> 对应的列，使用指定的映射选项。</summary>
    public static void CopyTo(T data, RecordRow target, RecordMappingOptions options) => XCopy.CopyTo(typeof(T), data, target, options);

    /// <summary>
    /// 将 <paramref name="source"/> 中与对象属性同名的列值写回对象 <paramref name="data"/>。
    /// 固定按编译期类型 <typeparamref name="T"/> 扫描属性，
    /// 即使 <paramref name="data"/> 的运行时类型为派生类，也不会处理派生类新增属性。
    /// </summary>
    /// <param name="data">目标对象。</param>
    /// <param name="source">数据来源行。</param>
    public static void CopyFrom(T data, RecordRow source) => XCopy.CopyFrom(typeof(T), data, source);

    /// <summary>将 <paramref name="source"/> 中的列值写回对象 <paramref name="data"/>，使用指定的映射选项。</summary>
    public static void CopyFrom(T data, RecordRow source, RecordMappingOptions options) => XCopy.CopyFrom(typeof(T), data, source, options);

    /// <summary>
    /// 将对象 <paramref name="data"/> 的可读属性值写入 <paramref name="target"/> 对应的列。
    /// 若目标行所在表中不存在对应列，则<b>自动创建列</b>后再写入。
    /// 固定按编译期类型 <typeparamref name="T"/> 扫描属性，
    /// 即使 <paramref name="data"/> 的运行时类型为派生类，也不会处理派生类新增属性。
    /// </summary>
    /// <param name="data">数据来源对象。</param>
    /// <param name="target">目标行。</param>
    public static void WriteTo(T data, RecordRow target) => XCopy.WriteTo(typeof(T), data, target);

    /// <summary>将对象 <paramref name="data"/> 的可读属性值写入 <paramref name="target"/> 对应的列（自动建列），使用指定的映射选项。</summary>
    public static void WriteTo(T data, RecordRow target, RecordMappingOptions options) => XCopy.WriteTo(typeof(T), data, target, options);
    #endregion
}

/// <summary>
/// 提供在两种强类型对象之间按属性名称进行浅拷贝的静态工具类。
/// 仅复制源类型与目标类型中同名、类型相同且均受支持的可读/可写属性。
/// </summary>
/// <typeparam name="TSource">数据来源对象类型。</typeparam>
/// <typeparam name="TTarget">数据目标对象类型，必须具有无参构造函数。</typeparam>
public static class XCopy<TSource, TTarget> where TSource : class where TTarget : class, new()
{
    // 预先构建源→目标属性映射对，避免每次调用重复查找。
    private static readonly IReadOnlyList<PropPair> _map = BuildMap();

    private readonly struct PropPair
    {
        internal readonly IXProp Source;
        internal readonly IXProp Target;

        internal PropPair(IXProp source, IXProp target)
        {
            Source = source;
            Target = target;
        }

        public void Copy(TSource sourceInstance, TTarget targetInstance)
        {
            Target.SetValue(targetInstance, Source.GetValue(sourceInstance));
        }
    }

    private static IReadOnlyList<PropPair> BuildMap()
    {
        var sourceProps = XProp.GetAll(typeof(TSource));
        var targetProps = XProp.GetAll(typeof(TTarget));

        // 以目标属性名建立快速查找字典
        var targetIndex = new Dictionary<string, IXProp>(StringComparer.Ordinal);
        foreach (var tp in targetProps)
        {
            if (tp.CanWrite) targetIndex[tp.Name] = tp;
        }

        var map = new List<PropPair>();
        foreach (var sp in sourceProps)
        {
            if (!sp.CanRead) continue;
            if (!targetIndex.TryGetValue(sp.Name, out var tp)) continue;
            // 仅当属性类型完全一致时才映射
            if (sp.Type != tp.Type) continue;
            map.Add(new PropPair(sp, tp));
        }

        return map.AsReadOnly();
    }

    /// <summary>
    /// 创建一个新的 <typeparamref name="TTarget"/> 实例，并将 <paramref name="source"/> 中
    /// 与目标类型同名、类型相同且均受支持的属性值复制过去。
    /// </summary>
    /// <param name="source">数据来源对象，不可为 null。</param>
    /// <returns>填充了来源属性值的新目标对象。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 为 null 时抛出。</exception>
    public static TTarget Copy(TSource source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        var target = new TTarget();
        CopyTo(source, target);
        return target;
    }

    /// <summary>
    /// 将 <paramref name="source"/> 中支持的属性值复制到已有的 <paramref name="target"/> 实例。
    /// </summary>
    /// <param name="source">数据来源对象，不可为 null。</param>
    /// <param name="target">数据目标对象，不可为 null。</param>
    /// <exception cref="ArgumentNullException">当任一参数为 null 时抛出。</exception>
    public static void CopyTo(TSource source, TTarget target)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (target == null) throw new ArgumentNullException(nameof(target));

        foreach (var pair in _map)
        {
            pair.Copy(source, target);
        }
    }
}
