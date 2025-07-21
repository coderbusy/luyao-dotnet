using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Data;

/// <summary>
/// 基于分隔符的字符串转换器，用于将对象序列化为分隔字符串或从分隔字符串反序列化。
/// </summary>
public partial class DelimiterBasedStringConverter<T> where T : class, new()
{
    public DelimiterBasedStringConverter(string delimiter)
    {
        this.Delimiter = delimiter;
    }

    public DelimiterBasedStringConverter() : this("|")
    {

    }

    /// <summary>
    /// 获取或设置用于分隔字符串的分隔符（默认为 '|'）。
    /// </summary>
    public string Delimiter { get; }

    /// <summary>
    /// 序列化
    /// </summary>
    public string Serialize(T value) => throw new NotImplementedException();
    /// <summary>
    /// 反序列化
    /// </summary>
    public T Deserialize(string value) => throw new NotImplementedException();

    private struct Item
    {
        public string Name { get; set; }
        public Func<T, String> Reader { get; set; }
        public Action<T, String> Writer { get; set; }
    }
    private List<Item> items = new List<Item>();
    public void Add<TValue>(Expression<Func<T, TValue>> propertySelector, Func<TValue, String> toString, Func<String, TValue> toValue)
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

    public void Add(Expression<Func<T, String>> propertySelector) => this.Add(propertySelector, static str => str, static str => str);

    public void Add(Expression<Func<T, bool>> propertySelector) => this.Add(propertySelector, Valid.ToString, Valid.ToBoolean);

    public void Add(Expression<Func<T, Int32>> propertySelector) => this.Add(propertySelector, Valid.ToString, Valid.ToInt32);
}
