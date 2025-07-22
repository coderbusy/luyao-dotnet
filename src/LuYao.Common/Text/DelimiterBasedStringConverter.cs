using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Text;

/// <summary>
/// 基于分隔符的字符串转换器，用于将对象序列化为分隔字符串或从分隔字符串反序列化。
/// </summary>
public partial class DelimiterBasedStringConverter<T> where T : class, new()
{
    /// <summary>
    /// 使用指定分隔符初始化 <see cref="DelimiterBasedStringConverter{T}"/> 实例。
    /// </summary>
    /// <param name="delimiter">分隔符字符串。</param>
    public DelimiterBasedStringConverter(string delimiter)
    {
        Delimiter = delimiter;
    }

    /// <summary>
    /// 使用默认分隔符 '|' 初始化 <see cref="DelimiterBasedStringConverter{T}"/> 实例。
    /// </summary>
    public DelimiterBasedStringConverter() : this("|")
    {

    }

    /// <summary>
    /// 获取或设置用于分隔字符串的分隔符（默认为 '|'）。
    /// </summary>
    public string Delimiter { get; }

    /// <summary>
    /// 将对象序列化为分隔字符串。
    /// </summary>
    /// <param name="value">要序列化的对象。</param>
    /// <returns>序列化后的分隔字符串。</returns>
    public string Serialize(T value)
    {
        if (value is null) return string.Empty;
        var sb = new StringBuilder();
        for (int i = 0; i < items.Count; i++)
        {
            if (i > 0) sb.Append(Delimiter);
            Item item = items[i];
            sb.Append(item.Reader.Invoke(value));
        }
        return sb.ToString();
    }
    /// <summary>
    /// 从分隔字符串反序列化为对象。
    /// </summary>
    /// <param name="value">分隔字符串。</param>
    /// <returns>反序列化后的对象。</returns>
    public T Deserialize(string value)
    {
        var ret = new T();
        if (!string.IsNullOrWhiteSpace(value))
        {
            int count = items.Count;
            string[] parts = value.Split(new[] { Delimiter }, count, StringSplitOptions.None);
            for (int i = 0; i < count && i < parts.Length; i++)
            {
                Item item = items[i];
                item.Writer.Invoke(ret, parts[i]);
            }
        }
        return ret;
    }

    private struct Item
    {
        public string Name { get; set; }
        public Func<T, string> Reader { get; set; }
        public Action<T, string> Writer { get; set; }
    }
    private List<Item> items = new List<Item>();

    /// <summary>
    /// 添加属性的自定义序列化与反序列化方法。
    /// </summary>
    /// <typeparam name="TValue">属性类型。</typeparam>
    /// <param name="propertySelector">属性选择表达式。</param>
    /// <param name="toString">属性值转字符串的方法。</param>
    /// <param name="toValue">字符串转属性值的方法。</param>
    public void Add<TValue>(Expression<Func<T, TValue>> propertySelector, Func<TValue, string> toString, Func<string, TValue> toValue)
    {
        // 1. 获取属性名称
        if (propertySelector.Body is not MemberExpression memberExpr)
            throw new ArgumentException("propertySelector 必须是属性访问表达式", nameof(propertySelector));
        var propertyInfo = typeof(T).GetProperty(memberExpr.Member.Name);
        if (propertyInfo == null || !propertyInfo.CanRead || !propertyInfo.CanWrite)
            throw new ArgumentException($"属性 {memberExpr.Member.Name} 不存在或不可读写", nameof(propertySelector));

        string name = memberExpr.Member.Name;

        // 2. 创建读取器
        var param = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.Property(param, propertyInfo);
        var toStringExpr = Expression.Invoke(Expression.Constant(toString), propertyAccess);
        var readerLambda = Expression.Lambda<Func<T, string>>(toStringExpr, param);
        var reader = readerLambda.Compile();

        // 3. 创建写入器
        var targetParam = Expression.Parameter(typeof(T), "target");
        var valueParam = Expression.Parameter(typeof(string), "value");
        var toValueExpr = Expression.Invoke(Expression.Constant(toValue), valueParam);
        var assignExpr = Expression.Assign(Expression.Property(targetParam, propertyInfo), toValueExpr);
        var writerLambda = Expression.Lambda<Action<T, string>>(assignExpr, targetParam, valueParam);
        var writer = writerLambda.Compile();

        items.Add(new Item
        {
            Name = name,
            Reader = reader,
            Writer = writer
        });
    }

    /// <summary>
    /// 添加字符串类型属性的序列化与反序列化方法。
    /// </summary>
    /// <param name="propertySelector">属性选择表达式。</param>
    public void Add(Expression<Func<T, string>> propertySelector) => Add(propertySelector, static str => str, static str => str);

    /// <summary>
    /// 添加布尔类型属性的序列化与反序列化方法。
    /// </summary>
    /// <param name="propertySelector">属性选择表达式。</param>
    public void Add(Expression<Func<T, bool>> propertySelector) => Add(propertySelector, Valid.ToString, Valid.ToBoolean);

    /// <summary>
    /// 添加整型属性的序列化与反序列化方法。
    /// </summary>
    /// <param name="propertySelector">属性选择表达式。</param>
    public void Add(Expression<Func<T, int>> propertySelector) => Add(propertySelector, Valid.ToString, Valid.ToInt32);

    /// <summary>
    /// 返回 T 的类型名称以及所有已添加的属性名称。
    /// </summary>
    public override string ToString()
    {
        // 获取类型名称
        string typeName = typeof(T).Name;
        // 获取所有已添加的属性名称
        var propertyNames = items.Select(item => item.Name);
        string properties = string.Join(", ", propertyNames);
        // 返回格式化字符串
        return $"类型: {typeName}, 属性: [{properties}]";
    }
}
