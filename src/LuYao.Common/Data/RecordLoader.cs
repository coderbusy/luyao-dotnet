using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LuYao.Data;

static class RecordLoader<T> where T : class
{
    private static readonly IReadOnlyDictionary<string, Field> _fieldsByName;
    private static readonly IReadOnlyList<Field> _fields;

    static RecordLoader()
    {
        var fieldList = ReadFields();
        _fields = fieldList;
        _fieldsByName = fieldList.ToDictionary(f => f.Column);
    }

    private static IReadOnlyList<Field> ReadFields()
    {
        var ret = new List<Field>();
        var type = typeof(T);
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.CanRead && p.CanWrite);

        foreach (var prop in props)
        {
            var field = new Field(prop);
            field.PopulateObject = MakePopulateObject(prop);
            field.WriteToRow = MakeWriteToRow(prop);
            ret.Add(field);
        }
        return ret;
    }

    private static Action<T, RecordRow, RecordColumn> MakePopulateObject(PropertyInfo property)
    {
        var targetParam = Expression.Parameter(typeof(T), "target");
        var rowParam = Expression.Parameter(typeof(RecordRow), "row");
        var columnParam = Expression.Parameter(typeof(RecordColumn), "column");

        var propertyType = property.PropertyType;
        var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        // 获取值表达式
        Expression valueExpression = MakeObject(rowParam, columnParam, underlyingType);

        // 处理可空类型
        if (propertyType != underlyingType)
        {
            valueExpression = Expression.Convert(valueExpression, propertyType);
        }

        // 属性赋值表达式
        var propertyAccess = Expression.Property(targetParam, property);
        var assignExpression = Expression.Assign(propertyAccess, valueExpression);

        // 编译表达式
        var lambda = Expression.Lambda<Action<T, RecordRow, RecordColumn>>(
            assignExpression,
            targetParam, rowParam, columnParam
        );

        return lambda.Compile();
    }

    private static MethodCallExpression MakeObject(ParameterExpression rowParam, ParameterExpression columnParam, Type underlyingType)
    {
        var method = typeof(RecordRow)
            .GetMethod(nameof(RecordRow.Get), [typeof(RecordColumn)])!
            .MakeGenericMethod(underlyingType);
        return Expression.Call(rowParam, method, columnParam);
    }

    private static Action<T, RecordRow, RecordColumn> MakeWriteToRow(PropertyInfo property)
    {
        var instanceParam = Expression.Parameter(typeof(T), "instance");
        var rowParam = Expression.Parameter(typeof(RecordRow), "row");
        var columnParam = Expression.Parameter(typeof(RecordColumn), "column");

        var rowExpression = Expression.Property(rowParam, nameof(RecordRow.Row));

        var propertyType = property.PropertyType;
        var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        // 获取属性值
        var propertyAccess = Expression.Property(instanceParam, property);

        // 准备要传递给 SetValue 的表达式
        Expression valueExpr;
        if (propertyType != underlyingType)
        {
            // 处理可空类型的 null 检查
            var hasValueProperty = Expression.Property(propertyAccess, "HasValue");
            var valueProperty = Expression.Property(propertyAccess, "Value");
            var convertedValue = Expression.Convert(valueProperty, typeof(object));

            // 条件表达式：如果 hasValue 则转换 value 为 object，否则为 null
            valueExpr = Expression.Condition(
                hasValueProperty,
                convertedValue,
                Expression.Constant(null, typeof(object))
            );
        }
        else
        {
            // 非可空类型直接转换为 object
            valueExpr = Expression.Convert(propertyAccess, typeof(object));
        }

        // SetValue 方法调用
        var setValueExpr = Expression.Call(
            columnParam,
            nameof(RecordColumn.SetValue),
            null,
            valueExpr,
            rowExpression
        );

        // 编译表达式
        var lambda = Expression.Lambda<Action<T, RecordRow, RecordColumn>>(
            setValueExpr,
            instanceParam, rowParam, columnParam
        );

        return lambda.Compile();
    }

    public class Field
    {
        public Field(PropertyInfo property)
        {
            this.Property = property ?? throw new ArgumentNullException(nameof(property));
            var attr = property.GetCustomAttribute<RecordColumnNameAttribute>();
            this.Column = attr?.Name ?? property.Name;
        }

        public PropertyInfo Property { get; }
        public string Column { get; }
        public Action<T, RecordRow, RecordColumn>? PopulateObject { get; set; }
        public Action<T, RecordRow, RecordColumn>? WriteToRow { get; set; }
    }

    public static void WriteHeader(Record re)
    {
        foreach (var field in _fields)
        {
            if (field.WriteToRow != null)
            {
                re.Columns.Add(field.Column, field.Property.PropertyType);
            }
        }
    }

    public static void WriteToRow(T instance, RecordRow row)
    {
        Record record = row.Record;
        foreach (RecordColumn col in record.Columns)
        {
            if (_fieldsByName.TryGetValue(col.Name, out var field) && field.WriteToRow != null)
            {
                field.WriteToRow.Invoke(instance, row, col);
            }
        }
    }

    public static void Populate(RecordRow row, T target)
    {
        Record record = row.Record;
        foreach (RecordColumn col in record.Columns)
        {
            if (_fieldsByName.TryGetValue(col.Name, out var field) && field.PopulateObject != null)
            {
                field.PopulateObject.Invoke(target, row, col);
            }
        }
    }
}
