using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LuYao.Data;

/// <summary>
/// Record 与实体类型之间的静态加载器，使用表达式树实现高性能转换
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public static class RecordLoader<T> where T : class, new()
{
    private sealed class PropertyMapping
    {
        public PropertyInfo Property { get; set; } = null!;
        public string ColumnName { get; set; } = null!;
        public Type PropertyType { get; set; } = null!;

        public Action<RecordRow, RecordColumn, T>? PopulateObject { get; set; }
        public Action<T, RecordRow, RecordColumn>? WriteToRow { get; set; }
    }

    private static readonly SortedDictionary<string, PropertyMapping> _mappings;
    static RecordLoader()
    {
        var mappings = CreateMappings();
        _mappings = new SortedDictionary<string, PropertyMapping>();
        foreach (var mapping in mappings)
        {
            _mappings.Add(mapping.ColumnName, mapping);
        }
    }

    private static PropertyMapping[] CreateMappings()
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite)
            .ToArray();

        var mappings = new List<PropertyMapping>();

        foreach (var property in properties)
        {
            var mapping = new PropertyMapping
            {
                Property = property,
                PropertyType = property.PropertyType,
                ColumnName = GetColumnName(property)
            };

            // 创建读取委托（从 RecordRow 到实体）
            mapping.PopulateObject = CreatePopulateObjectDelegate(property);

            // 创建写入委托（从实体到 RecordRow）
            mapping.WriteToRow = CreateWriteRowDelegate(property);

            mappings.Add(mapping);
        }

        return mappings.ToArray();
    }

    private static string GetColumnName(PropertyInfo property)
    {
        var attr = property.GetCustomAttribute<RecordColumnNameAttribute>();
        return attr?.Name ?? property.Name;
    }

    private static Action<RecordRow, RecordColumn, T> CreatePopulateObjectDelegate(PropertyInfo property)
    {
        // 参数: (RecordRow row, RecordColumn column, T target)
        var rowParam = Expression.Parameter(typeof(RecordRow), "row");
        var columnParam = Expression.Parameter(typeof(RecordColumn), "column");
        var targetParam = Expression.Parameter(typeof(T), "target");

        var propertyType = property.PropertyType;
        var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        Expression valueExpression;

        // 根据属性类型选择合适的 RecordRow 方法
        if (underlyingType == typeof(bool))
            valueExpression = Expression.Call(rowParam, nameof(RecordRow.ToBoolean), null, columnParam);
        else if (underlyingType == typeof(byte))
            valueExpression = Expression.Call(rowParam, nameof(RecordRow.ToByte), null, columnParam);
        else if (underlyingType == typeof(char))
            valueExpression = Expression.Call(rowParam, nameof(RecordRow.ToChar), null, columnParam);
        else if (underlyingType == typeof(DateTime))
            valueExpression = Expression.Call(rowParam, nameof(RecordRow.ToDateTime), null, columnParam);
        else if (underlyingType == typeof(decimal))
            valueExpression = Expression.Call(rowParam, nameof(RecordRow.ToDecimal), null, columnParam);
        else if (underlyingType == typeof(double))
            valueExpression = Expression.Call(rowParam, nameof(RecordRow.ToDouble), null, columnParam);
        else if (underlyingType == typeof(short))
            valueExpression = Expression.Call(rowParam, nameof(RecordRow.ToInt16), null, columnParam);
        else if (underlyingType == typeof(int))
            valueExpression = Expression.Call(rowParam, nameof(RecordRow.ToInt32), null, columnParam);
        else if (underlyingType == typeof(long))
            valueExpression = Expression.Call(rowParam, nameof(RecordRow.ToInt64), null, columnParam);
        else if (underlyingType == typeof(sbyte))
            valueExpression = Expression.Call(rowParam, nameof(RecordRow.ToSByte), null, columnParam);
        else if (underlyingType == typeof(float))
            valueExpression = Expression.Call(rowParam, nameof(RecordRow.ToSingle), null, columnParam);
        else if (underlyingType == typeof(string))
            valueExpression = Expression.Call(rowParam, nameof(RecordRow.ToString), null, columnParam);
        else if (underlyingType == typeof(ushort))
            valueExpression = Expression.Call(rowParam, nameof(RecordRow.ToUInt16), null, columnParam);
        else if (underlyingType == typeof(uint))
            valueExpression = Expression.Call(rowParam, nameof(RecordRow.ToUInt32), null, columnParam);
        else if (underlyingType == typeof(ulong))
            valueExpression = Expression.Call(rowParam, nameof(RecordRow.ToUInt64), null, columnParam);
        else
        {
            // 使用泛型方法 To<T>
            var toMethod = typeof(RecordRow).GetMethod(nameof(RecordRow.To))!.MakeGenericMethod(underlyingType);
            valueExpression = Expression.Call(rowParam, toMethod, columnParam);
        }

        // 处理可空类型
        if (propertyType != underlyingType)
        {
            // 构造可空类型
            valueExpression = Expression.Convert(valueExpression, propertyType);
        }

        // 属性赋值表达式
        var propertyAccess = Expression.Property(targetParam, property);
        var assignExpression = Expression.Assign(propertyAccess, valueExpression);

        // 编译表达式
        var lambda = Expression.Lambda<Action<RecordRow, RecordColumn, T>>(
            assignExpression, rowParam, columnParam, targetParam);

        return lambda.Compile();
    }

    private static Action<T, RecordRow, RecordColumn> CreateWriteRowDelegate(PropertyInfo property)
    {
        // 参数: (T instance, RecordRow row, RecordColumn column)
        var instanceParam = Expression.Parameter(typeof(T), "instance");
        var rowParam = Expression.Parameter(typeof(RecordRow), "row");
        var columnParam = Expression.Parameter(typeof(RecordColumn), "column");

        var propertyType = property.PropertyType;
        var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        // 获取属性值
        var propertyAccess = Expression.Property(instanceParam, property);
        Expression valueExpression = propertyAccess;

        // 处理可空类型
        if (propertyType != underlyingType)
        {
            // 如果是可空类型，需要获取 Value 属性
            valueExpression = Expression.Property(propertyAccess, "Value");
        }

        Expression setExpression;

        // 根据属性类型选择合适的 RecordRow.Set 方法
        if (underlyingType == typeof(bool))
            setExpression = Expression.Call(rowParam, nameof(RecordRow.Set), null, valueExpression, columnParam);
        else if (underlyingType == typeof(byte))
            setExpression = Expression.Call(rowParam, nameof(RecordRow.Set), null, valueExpression, columnParam);
        else if (underlyingType == typeof(char))
            setExpression = Expression.Call(rowParam, nameof(RecordRow.Set), null, valueExpression, columnParam);
        else if (underlyingType == typeof(DateTime))
            setExpression = Expression.Call(rowParam, nameof(RecordRow.Set), null, valueExpression, columnParam);
        else if (underlyingType == typeof(decimal))
            setExpression = Expression.Call(rowParam, nameof(RecordRow.Set), null, valueExpression, columnParam);
        else if (underlyingType == typeof(double))
            setExpression = Expression.Call(rowParam, nameof(RecordRow.Set), null, valueExpression, columnParam);
        else if (underlyingType == typeof(short))
            setExpression = Expression.Call(rowParam, nameof(RecordRow.Set), null, valueExpression, columnParam);
        else if (underlyingType == typeof(int))
            setExpression = Expression.Call(rowParam, nameof(RecordRow.Set), null, valueExpression, columnParam);
        else if (underlyingType == typeof(long))
            setExpression = Expression.Call(rowParam, nameof(RecordRow.Set), null, valueExpression, columnParam);
        else if (underlyingType == typeof(sbyte))
            setExpression = Expression.Call(rowParam, nameof(RecordRow.Set), null, valueExpression, columnParam);
        else if (underlyingType == typeof(float))
            setExpression = Expression.Call(rowParam, nameof(RecordRow.Set), null, valueExpression, columnParam);
        else if (underlyingType == typeof(string))
            setExpression = Expression.Call(rowParam, nameof(RecordRow.Set), null, valueExpression, columnParam);
        else if (underlyingType == typeof(ushort))
            setExpression = Expression.Call(rowParam, nameof(RecordRow.Set), null, valueExpression, columnParam);
        else if (underlyingType == typeof(uint))
            setExpression = Expression.Call(rowParam, nameof(RecordRow.Set), null, valueExpression, columnParam);
        else if (underlyingType == typeof(ulong))
            setExpression = Expression.Call(rowParam, nameof(RecordRow.Set), null, valueExpression, columnParam);
        else
        {
            // 使用 SetValue 方法处理其他类型
            var boxedValue = Expression.Convert(valueExpression, typeof(object));
            setExpression = Expression.Call(rowParam, nameof(RecordRow.SetValue), null, boxedValue, columnParam);
        }

        // 处理可空类型的 null 检查
        if (propertyType != underlyingType)
        {
            var hasValueProperty = Expression.Property(propertyAccess, "HasValue");
            setExpression = Expression.IfThen(hasValueProperty, setExpression);
        }

        // 编译表达式
        var lambda = Expression.Lambda<Action<T, RecordRow, RecordColumn>>(
            setExpression, instanceParam, rowParam, columnParam);

        return lambda.Compile();
    }

    /// <summary>
    /// 将 <see cref="RecordRow"/> 的数据填充到指定的实体对象 <typeparamref name="T"/> 中。
    /// </summary>
    /// <param name="row">包含数据的 <see cref="RecordRow"/> 实例。</param>
    /// <param name="target">要填充数据的目标实体对象。</param>
    public static void Populate(RecordRow row, T target)
    {
        Record record = row.Record;
        foreach (var column in record.Columns)
        {
            if (_mappings.TryGetValue(column.Name, out var mapping) && mapping.PopulateObject != null)
            {
                mapping.PopulateObject(row, column, target);
            }
        }
    }

    /// <summary>
    /// 将实体类型 <typeparamref name="T"/> 的属性名称写入到 <see cref="Record"/> 的列头中。
    /// </summary>
    /// <param name="re">要写入列头的 <see cref="Record"/> 实例。</param>
    public static void WriteHeader(Record re)
    {
        foreach (var pair in _mappings)
        {
            var mapping = pair.Value;
            if (mapping.WriteToRow != null)
            {
                re.Columns.Add(mapping.ColumnName, mapping.PropertyType);
            }
        }
    }

    /// <summary>
    /// 将指定实体对象 <typeparamref name="T"/> 的数据写入到 <see cref="RecordRow"/> 实例中。
    /// </summary>
    /// <param name="instance">要读取数据的实体对象。</param>
    /// <param name="row">要写入数据的 <see cref="RecordRow"/> 实例。</param>
    public static void WriteData(T instance, RecordRow row)
    {
        Record record = row.Record;
        foreach (var column in record.Columns)
        {
            if (_mappings.TryGetValue(column.Name, out var mapping) && mapping.WriteToRow != null)
            {
                mapping.WriteToRow(instance, row, column);
            }
        }
    }
}
