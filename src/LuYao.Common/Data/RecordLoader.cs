using System;
using System.Collections.Generic;
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
    private static readonly Action<RecordRow, T> _populateAction;
    private static readonly Action<T, RecordRow> _writeAction;
    private static readonly PropertyMapping[] _propertyMappings;

    private sealed class PropertyMapping
    {
        public PropertyInfo Property { get; set; } = null!;
        public string ColumnName { get; set; } = null!;
        public Type PropertyType { get; set; } = null!;
    }

    static RecordLoader()
    {
        // 1. 通过反射获取实体类型 T 的属性信息,注意关联 RecordColumnNameAttribute 
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite)
            .ToArray();

        var mappings = new List<PropertyMapping>();

        foreach (var property in properties)
        {
            var columnNameAttr = property.GetCustomAttribute<RecordColumnNameAttribute>();
            var columnName = columnNameAttr?.Name ?? property.Name;

            mappings.Add(new PropertyMapping
            {
                Property = property,
                ColumnName = columnName,
                PropertyType = property.PropertyType
            });
        }

        _propertyMappings = mappings.ToArray();

        // 2. 使用表达式树生成一个委托，用于将 RecordRow 的数据填充到实体对象 T 中
        _populateAction = BuildPopulateAction();

        // 3. 使用表达式树生成一个委托，用于将实体对象 T 的数据写入到 RecordRow 中
        _writeAction = BuildWriteAction();
    }

    private static Action<RecordRow, T> BuildPopulateAction()
    {
        var rowParam = Expression.Parameter(typeof(RecordRow), "row");
        var targetParam = Expression.Parameter(typeof(T), "target");
        var statements = new List<Expression>();
        var variables = new List<ParameterExpression>();

        foreach (var mapping in _propertyMappings)
        {
            // 为每个属性创建一个独立的列变量
            var columnVar = Expression.Variable(typeof(RecordColumn), $"column_{mapping.Property.Name}");
            variables.Add(columnVar);

            // 获取列：RecordColumn column = row.Record.Columns.Find(columnName);
            var findColumnCall = Expression.Call(
                Expression.Property(Expression.Property(rowParam, "Record"), "Columns"),
                typeof(RecordColumnCollection).GetMethod("Find")!,
                Expression.Constant(mapping.ColumnName)
            );

            var assignColumn = Expression.Assign(columnVar, findColumnCall);
            var columnNotNull = Expression.NotEqual(columnVar, Expression.Constant(null));

            // 根据属性类型调用相应的转换方法
            Expression getValue = GetReadExpression(rowParam, columnVar, mapping.PropertyType);

            // 设置属性值：target.Property = getValue
            var setProperty = Expression.Assign(
                Expression.Property(targetParam, mapping.Property),
                getValue
            );

            // 只有当列存在时才设置属性
            var conditionalSet = Expression.IfThen(columnNotNull, setProperty);

            statements.Add(assignColumn);
            statements.Add(conditionalSet);
        }

        var body = Expression.Block(variables, statements);
        return Expression.Lambda<Action<RecordRow, T>>(body, rowParam, targetParam).Compile();
    }

    private static Action<T, RecordRow> BuildWriteAction()
    {
        var instanceParam = Expression.Parameter(typeof(T), "instance");
        var rowParam = Expression.Parameter(typeof(RecordRow), "row");
        var statements = new List<Expression>();
        var variables = new List<ParameterExpression>();

        foreach (var mapping in _propertyMappings)
        {
            // 为每个属性创建一个独立的列变量
            var columnVar = Expression.Variable(typeof(RecordColumn), $"column_{mapping.Property.Name}");
            variables.Add(columnVar);

            // 获取列：RecordColumn column = row.Record.Columns.Find(columnName);
            var findColumnCall = Expression.Call(
                Expression.Property(Expression.Property(rowParam, "Record"), "Columns"),
                typeof(RecordColumnCollection).GetMethod("Find")!,
                Expression.Constant(mapping.ColumnName)
            );

            var assignColumn = Expression.Assign(columnVar, findColumnCall);
            var columnNotNull = Expression.NotEqual(columnVar, Expression.Constant(null));

            // 获取属性值
            var propertyValue = Expression.Property(instanceParam, mapping.Property);

            // 根据属性类型调用相应的 Set 方法
            Expression setValue = GetWriteExpression(rowParam, columnVar, propertyValue, mapping.PropertyType);

            // 只有当列存在时才设置值
            var conditionalSet = Expression.IfThen(columnNotNull, setValue);

            statements.Add(assignColumn);
            statements.Add(conditionalSet);
        }

        var body = Expression.Block(variables, statements);
        return Expression.Lambda<Action<T, RecordRow>>(body, instanceParam, rowParam).Compile();
    }

    private static Expression GetReadExpression(ParameterExpression rowParam, ParameterExpression columnVar, Type propertyType)
    {
        // 处理可空类型
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var underlyingType = Nullable.GetUnderlyingType(propertyType)!;
            var getValue = GetStrongTypedReadExpression(rowParam, columnVar, underlyingType);
            return Expression.Convert(getValue, propertyType);
        }

        return GetStrongTypedReadExpression(rowParam, columnVar, propertyType);
    }

    private static Expression GetStrongTypedReadExpression(ParameterExpression rowParam, ParameterExpression columnVar, Type type)
    {
        return type switch
        {
            Type t when t == typeof(bool) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.ToBoolean), new[] { typeof(RecordColumn) })!, columnVar),
            Type t when t == typeof(byte) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.ToByte), new[] { typeof(RecordColumn) })!, columnVar),
            Type t when t == typeof(char) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.ToChar), new[] { typeof(RecordColumn) })!, columnVar),
            Type t when t == typeof(DateTime) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.ToDateTime), new[] { typeof(RecordColumn) })!, columnVar),
            Type t when t == typeof(decimal) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.ToDecimal), new[] { typeof(RecordColumn) })!, columnVar),
            Type t when t == typeof(double) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.ToDouble), new[] { typeof(RecordColumn) })!, columnVar),
            Type t when t == typeof(short) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.ToInt16), new[] { typeof(RecordColumn) })!, columnVar),
            Type t when t == typeof(int) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.ToInt32), new[] { typeof(RecordColumn) })!, columnVar),
            Type t when t == typeof(long) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.ToInt64), new[] { typeof(RecordColumn) })!, columnVar),
            Type t when t == typeof(sbyte) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.ToSByte), new[] { typeof(RecordColumn) })!, columnVar),
            Type t when t == typeof(float) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.ToSingle), new[] { typeof(RecordColumn) })!, columnVar),
            Type t when t == typeof(string) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.ToString), new[] { typeof(RecordColumn) })!, columnVar),
            Type t when t == typeof(ushort) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.ToUInt16), new[] { typeof(RecordColumn) })!, columnVar),
            Type t when t == typeof(uint) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.ToUInt32), new[] { typeof(RecordColumn) })!, columnVar),
            Type t when t == typeof(ulong) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.ToUInt64), new[] { typeof(RecordColumn) })!, columnVar),
            _ => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.To))!.MakeGenericMethod(type), columnVar)
        };
    }

    private static Expression GetWriteExpression(ParameterExpression rowParam, ParameterExpression columnVar,
        Expression propertyValue, Type propertyType)
    {
        // 处理可空类型
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var underlyingType = Nullable.GetUnderlyingType(propertyType)!;
            var hasValue = Expression.Property(propertyValue, "HasValue");
            var value = Expression.Property(propertyValue, "Value");

            var setValue = GetStrongTypedWriteExpression(rowParam, columnVar, value, underlyingType);

            // 只有当值不为 null 时才设置
            return Expression.IfThen(hasValue, setValue);
        }

        return GetStrongTypedWriteExpression(rowParam, columnVar, propertyValue, propertyType);
    }

    private static Expression GetStrongTypedWriteExpression(ParameterExpression rowParam, ParameterExpression columnVar,
        Expression value, Type type)
    {
        return type switch
        {
            Type t when t == typeof(bool) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.Set), new[] { typeof(bool), typeof(RecordColumn) })!, value, columnVar),
            Type t when t == typeof(byte) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.Set), new[] { typeof(byte), typeof(RecordColumn) })!, value, columnVar),
            Type t when t == typeof(char) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.Set), new[] { typeof(char), typeof(RecordColumn) })!, value, columnVar),
            Type t when t == typeof(DateTime) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.Set), new[] { typeof(DateTime), typeof(RecordColumn) })!, value, columnVar),
            Type t when t == typeof(decimal) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.Set), new[] { typeof(decimal), typeof(RecordColumn) })!, value, columnVar),
            Type t when t == typeof(double) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.Set), new[] { typeof(double), typeof(RecordColumn) })!, value, columnVar),
            Type t when t == typeof(short) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.Set), new[] { typeof(short), typeof(RecordColumn) })!, value, columnVar),
            Type t when t == typeof(int) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.Set), new[] { typeof(int), typeof(RecordColumn) })!, value, columnVar),
            Type t when t == typeof(long) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.Set), new[] { typeof(long), typeof(RecordColumn) })!, value, columnVar),
            Type t when t == typeof(sbyte) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.Set), new[] { typeof(sbyte), typeof(RecordColumn) })!, value, columnVar),
            Type t when t == typeof(float) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.Set), new[] { typeof(float), typeof(RecordColumn) })!, value, columnVar),
            Type t when t == typeof(string) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.Set), new[] { typeof(string), typeof(RecordColumn) })!, value, columnVar),
            Type t when t == typeof(ushort) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.Set), new[] { typeof(ushort), typeof(RecordColumn) })!, value, columnVar),
            Type t when t == typeof(uint) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.Set), new[] { typeof(uint), typeof(RecordColumn) })!, value, columnVar),
            Type t when t == typeof(ulong) => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.Set), new[] { typeof(ulong), typeof(RecordColumn) })!, value, columnVar),
            _ => Expression.Call(rowParam, 
                typeof(RecordRow).GetMethod(nameof(RecordRow.SetValue), new[] { typeof(object), typeof(RecordColumn) })!,
                Expression.Convert(value, typeof(object)), columnVar)
        };
    }

    /// <summary>
    /// 将 <see cref="RecordRow"/> 的数据填充到指定的实体对象 <typeparamref name="T"/> 中。
    /// </summary>
    /// <param name="row">包含数据的 <see cref="RecordRow"/> 实例。</param>
    /// <param name="target">要填充数据的目标实体对象。</param>
    public static void Populate(RecordRow row, T target)
    {
        _populateAction(row, target);
    }

    /// <summary>
    /// 将指定实体对象 <typeparamref name="T"/> 的数据写入到 <see cref="RecordRow"/> 实例中。
    /// </summary>
    /// <param name="instance">要读取数据的实体对象。</param>
    /// <param name="row">要写入数据的 <see cref="RecordRow"/> 实例。</param>
    public static void WriteRecord(T instance, RecordRow row)
    {
        _writeAction(instance, row);
    }
}
