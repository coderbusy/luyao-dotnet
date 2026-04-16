using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LuYao.Data;

static class Helpers
{
    /// <summary>
    /// 列类型白名单（封闭，不可外部扩展）。
    /// </summary>
    private static readonly HashSet<Type> SupportedColumnTypes = new()
    {
        typeof(bool),
        typeof(sbyte), typeof(short), typeof(int), typeof(long),
        typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
        typeof(float), typeof(double), typeof(decimal),
        typeof(char), typeof(string),
        typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan),
        typeof(Guid),
        typeof(byte[]),
    };

    /// <summary>
    /// 判断指定类型是否为支持的列类型（包括其 Nullable 形式）。
    /// </summary>
    internal static bool IsSupportedColumnType(Type type)
    {
        if (type == null) return false;
        if (SupportedColumnTypes.Contains(type)) return true;
        var underlying = Nullable.GetUnderlyingType(type);
        return underlying != null && SupportedColumnTypes.Contains(underlying);
    }

    /// <summary>
    /// 验证列类型是否在白名单内，不在则抛出异常。
    /// </summary>
    internal static void ValidateColumnType(Type type)
    {
        if (!IsSupportedColumnType(type))
        {
            throw new NotSupportedException($"类型 '{type.FullName}' 不是支持的列类型。支持的类型包括：bool, 整数类型, 浮点类型, char, string, DateTime, DateTimeOffset, TimeSpan, Guid, byte[] 及其 Nullable 形式。");
        }
    }

    private static readonly ConcurrentDictionary<Type, Func<Record, string, Type, RecordColumn>> _cache = new();

    private static Func<Record, string, Type, RecordColumn> GetConstructor(Type type)
    {
        return _cache.GetOrAdd(type, static t =>
        {
            // 创建泛型类型
            var genericType = typeof(RecordColumn<>).MakeGenericType(t);

            // 获取构造函数
            var ctor = genericType.GetConstructor(new[] {
                typeof(Record),
                typeof(string),
                typeof(Type)
            }) ?? throw new InvalidOperationException($"Constructor not found for {genericType}");

            // 定义参数表达式
            var pRecord = Expression.Parameter(typeof(Record), "record");
            var pName = Expression.Parameter(typeof(string), "name");
            var pType = Expression.Parameter(typeof(Type), "type");

            // 创建构造函数调用表达式
            var newExpr = Expression.New(ctor, pRecord, pName, pType);

            // 编译表达式树为委托
            return Expression.Lambda<Func<Record, string, Type, RecordColumn>>(
                newExpr, pRecord, pName, pType).Compile();
        });
    }

    public static RecordColumn MakeRecordColumn<T>(Record record, string name)
    {
        // 使用泛型方法优化，避免运行时查找类型
        var ctor = GetConstructor(typeof(T));
        return ctor.Invoke(record, name, typeof(T));
    }

    public static RecordColumn MakeRecordColumn(Record record, string name, Type type)
    {
        if (record == null) throw new ArgumentNullException(nameof(record));
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (type == null) throw new ArgumentNullException(nameof(type));

        ValidateColumnType(type);

        var ctor = GetConstructor(type);
        return ctor.Invoke(record, name, type);
    }
}
