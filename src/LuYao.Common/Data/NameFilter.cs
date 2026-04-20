using LuYao.Data.Meta;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LuYao.Data;

/// <summary>
/// 针对类型 <typeparamref name="T"/> 的属性名称过滤器，支持链式调用，
/// 用于声明式地选取或排除需要参与列映射的属性。
/// </summary>
/// <remarks>
/// 默认构造时包含类型 <typeparamref name="T"/> 中所有可读且受支持的属性，顺序与属性声明顺序一致。
/// 手动调用 <see cref="Include"/> 时，列名按调用顺序追加；<see cref="ToNames"/> 返回结果与内部列表顺序完全一致，且不含重复项。
/// </remarks>
/// <typeparam name="T">提供属性来源的对象类型。</typeparam>
public class NameFilter<T> where T : class
{
    // 使用 List 保持插入顺序（用户手动 Include 的顺序即为最终列顺序），通过 Contains 检查保证无重复。
    private readonly List<string> _names = new List<string>();

    /// <summary>
    /// 初始化 <see cref="NameFilter{T}"/> 的新实例，默认包含 <typeparamref name="T"/> 的所有可读属性。
    /// </summary>
    public NameFilter()
    {
        this.All();
    }

    /// <summary>
    /// 重置为包含 <typeparamref name="T"/> 中所有可读且受支持的属性，顺序按属性声明顺序。
    /// </summary>
    /// <returns>当前实例，支持链式调用。</returns>
    public NameFilter<T> All()
    {
        _names.Clear();
        var props = XProp.GetAll(typeof(T));
        foreach (var p in props)
        {
            if (Helpers.IsSupportedForReading(p))
                _names.Add(p.Name);
        }
        return this;
    }

    /// <summary>
    /// 清空所有已选取的属性名。
    /// </summary>
    /// <returns>当前实例，支持链式调用。</returns>
    public NameFilter<T> Clear()
    {
        _names.Clear();
        return this;
    }

    /// <summary>
    /// 将指定属性追加到选取列表末尾。若已存在则保持原有位置，不重复添加。
    /// </summary>
    /// <param name="expre">指向目标属性的 Lambda 表达式，例如 <c>x => x.Name</c>。</param>
    /// <returns>当前实例，支持链式调用。</returns>
    /// <exception cref="ArgumentException"><paramref name="expre"/> 不是成员访问表达式。</exception>
    public NameFilter<T> Include(Expression<Func<T, object>> expre)
    {
        var name = GetMemberName(expre);
        if (!_names.Contains(name))
            _names.Add(name);
        return this;
    }

    /// <summary>
    /// 将指定属性从选取列表中移除。若不存在则忽略。
    /// </summary>
    /// <param name="expre">指向目标属性的 Lambda 表达式，例如 <c>x => x.Email</c>。</param>
    /// <returns>当前实例，支持链式调用。</returns>
    /// <exception cref="ArgumentException"><paramref name="expre"/> 不是成员访问表达式。</exception>
    public NameFilter<T> Exclude(Expression<Func<T, object>> expre)
    {
        var name = GetMemberName(expre);
        _names.Remove(name);
        return this;
    }

    /// <summary>
    /// 将当前选取列表物化为属性名数组。
    /// 结果顺序与内部列表一致：<see cref="All"/> 按属性声明顺序，手动 <see cref="Include"/> 按调用顺序。
    /// 结果不含重复项。
    /// </summary>
    /// <returns>已选中属性名的有序、无重复数组。</returns>
    public string[] ToNames()
    {
        return _names.ToArray();
    }

    /// <summary>
    /// 从成员访问 Lambda 表达式中提取属性名称。
    /// </summary>
    private static string GetMemberName(Expression<Func<T, object>> expre)
    {
        Expression body = expre.Body;
        if (body is UnaryExpression unary)
            body = unary.Operand;
        if (body is MemberExpression member)
            return member.Member.Name;
        throw new ArgumentException("Expression must be a member access.", nameof(expre));
    }
}
