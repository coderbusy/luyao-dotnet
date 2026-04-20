using System;
using System.Collections.Generic;

namespace LuYao.Data.Meta;

/// <summary>
/// 提供在强类型对象与 <see cref="RecordRow"/> 之间进行属性双向复制的静态工具类。
/// </summary>
/// <typeparam name="T">要映射的对象类型，必须为引用类型。</typeparam>
public static class XCopy<T> where T : class
{
    #region RecordRow
    /// <summary>
    /// 将对象 <paramref name="data"/> 的可读属性值写入 <paramref name="row"/> 对应的列。
    /// </summary>
    /// <param name="data">数据来源对象。</param>
    /// <param name="row">目标行；仅写入类型受支持的可读属性。</param>
    public static void CopyTo(T data, RecordRow row)
    {
        var props = XProp.GetAll(typeof(T));
        var re = row.Record;

        foreach (var prop in props)
        {
            if (!Helpers.IsSupportedForReading(prop)) continue;
            row[prop.Name] = prop.GetValue(data);
        }
    }

    /// <summary>
    /// 将 <paramref name="row"/> 中与对象属性同名的列值写回对象 <paramref name="data"/>。
    /// </summary>
    /// <param name="data">目标对象；仅更新类型受支持且行中存在对应列的可写属性。</param>
    /// <param name="row">数据来源行。</param>
    public static void CopyFrom(T data, RecordRow row)
    {
        var props = XProp.GetAll(typeof(T));
        var re = row.Record;
        foreach (var prop in props)
        {
            if (!Helpers.IsSupportedForWriting(prop)) continue;
            if (!re.Columns.Contains(prop.Name)) continue;
            prop.SetValue(data, row[prop.Name]);
        }
    }
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

    private sealed class PropPair
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