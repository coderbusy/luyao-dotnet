using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace LuYao.Data.Meta;


/// <summary>
/// 封装某个类型的单个公共属性，提供快速的反射读写访问（通过编译的委托实现）。
/// </summary>
public class XProp : IXProp
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<XProp>> _cache =
        new ConcurrentDictionary<Type, IReadOnlyList<XProp>>();

    /// <summary>
    /// 扫描指定类型的所有公共实例属性，并以缓存方式返回对应的 <see cref="XProp"/> 列表。
    /// </summary>
    /// <param name="type">要扫描的目标类型。</param>
    /// <returns>该类型所有公共实例属性对应的 <see cref="XProp"/> 只读列表。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="type"/> 为 null 时抛出。</exception>
    public static IReadOnlyList<XProp> GetAll(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        return _cache.GetOrAdd(type, static t =>
        {
            var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var list = new List<XProp>(props.Length);
            foreach (var p in props)
            {
                if (p.GetIndexParameters().Length > 0) continue; // 跳过索引器属性
                list.Add(new XProp(p));
            }
            return list.AsReadOnly();
        });
    }

    private readonly Func<object, object?>? _getter;
    private readonly Action<object, object?>? _setter;
    private readonly string _name;
    private readonly Type _type;

    private XProp(PropertyInfo property)
    {
        _property = property;
        _name = property.Name;
        _type = property.PropertyType;

        if (property.CanRead && property.GetGetMethod(nonPublic: false) is { } getMethod)
        {
            // (object instance) => (object)((T)instance).Prop
            var instance = System.Linq.Expressions.Expression.Parameter(typeof(object), "instance");
            var castInstance = System.Linq.Expressions.Expression.Convert(instance, property.DeclaringType!);
            var propAccess = System.Linq.Expressions.Expression.Property(castInstance, property);
            var castResult = System.Linq.Expressions.Expression.Convert(propAccess, typeof(object));
            _getter = System.Linq.Expressions.Expression.Lambda<Func<object, object?>>(castResult, instance).Compile();
        }

        if (property.CanWrite && property.GetSetMethod(nonPublic: false) is { } setMethod)
        {
            // (object instance, object value) => ((T)instance).Prop = (TProp)value
            var instance = System.Linq.Expressions.Expression.Parameter(typeof(object), "instance");
            var value = System.Linq.Expressions.Expression.Parameter(typeof(object), "value");
            var castInstance = System.Linq.Expressions.Expression.Convert(instance, property.DeclaringType!);
            var castValue = System.Linq.Expressions.Expression.Convert(value, property.PropertyType);
            var assign = System.Linq.Expressions.Expression.Call(castInstance, setMethod, castValue);
            _setter = System.Linq.Expressions.Expression.Lambda<Action<object, object?>>(assign, instance, value).Compile();
        }
    }

    private readonly PropertyInfo _property;

    /// <summary>
    /// 属性类型
    /// </summary>
    public Type Type => _type;

    /// <summary>
    /// 属性名称
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// 可否读取
    /// </summary>
    public bool CanRead => _getter is not null;

    /// <summary>
    /// 可否写入
    /// </summary>
    public bool CanWrite => _setter is not null;

    /// <summary>
    /// 读取指定对象的属性值。
    /// </summary>
    /// <param name="instance">属性所属的对象实例。</param>
    /// <returns>属性当前值。</returns>
    /// <exception cref="InvalidOperationException">属性不可读时抛出。</exception>
    public object? GetValue(object instance)
    {
        if (_getter is null) throw new InvalidOperationException($"属性 {Name} 不可读。");
        return _getter(instance);
    }

    /// <summary>
    /// 设置指定对象的属性值。
    /// </summary>
    /// <param name="instance">属性所属的对象实例。</param>
    /// <param name="value">要写入的值。</param>
    /// <exception cref="InvalidOperationException">属性不可写时抛出。</exception>
    public void SetValue(object instance, object? value)
    {
        if (_setter is null) throw new InvalidOperationException($"属性 {Name} 不可写。");
        _setter(instance, value);
    }
}
