using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LuYao.Data;

static class RecordLoader<T> where T : class
{
    private static readonly IReadOnlyList<Field> _fields = ReadFields();

    private static IReadOnlyList<Field> ReadFields()
    {
        var ret = new List<Field>();
        var type = typeof(T);
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanRead) continue;
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

        Expression valueExpression;

        // 根据属性类型选择合适的 RecordRow 方法
        var typeCode = Type.GetTypeCode(underlyingType);
        valueExpression = typeCode switch
        {
            _ => MakeObject(rowParam, columnParam, underlyingType)
        };

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
        Expression valueExpression = propertyAccess;

        // 处理可空类型
        if (propertyType != underlyingType)
        {
            // 如果是可空类型，需要获取 Value 属性
            valueExpression = Expression.Property(propertyAccess, "Value");
        }

        Expression setExpression;
        // 根据属性类型选择合适的 RecordRow.Set 方法
        var typeCode = Type.GetTypeCode(underlyingType);
        setExpression = typeCode switch
        {
            _ => Expression.Call(columnParam, nameof(RecordColumn.SetValue), null, Expression.Convert(valueExpression, typeof(object)), rowExpression)
        };

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

    public class Field
    {
        public Field(PropertyInfo property)
        {
            this.Property = property ?? throw new ArgumentNullException(nameof(property));
            var attr = property.GetCustomAttribute<RecordColumnNameAttribute>();
            if (attr != null) this.Column = attr.Name;
            if (this.Column == null || string.IsNullOrEmpty(this.Column)) this.Column = property.Name;
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
        Record re = row.Record;
        foreach (RecordColumn col in re.Columns)
        {
            var field = _fields.FirstOrDefault(f => f.Column == col.Name);
            if (field != null && field.WriteToRow != null)
            {
                field.WriteToRow.Invoke(instance, row, col);
            }
        }
    }

    public static void Populate(RecordRow row, T target)
    {
        Record re = row.Record;
        foreach (RecordColumn col in re.Columns)
        {
            var field = _fields.FirstOrDefault(f => f.Column == col.Name);
            if (field != null && field.PopulateObject != null)
            {
                field.PopulateObject.Invoke(target, row, col);
            }
        }
    }
}
