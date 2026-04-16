using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LuYao.Data;

static class Helpers
{
    #region RecordColumnType <-> Type 双向映射

    private static readonly Dictionary<Type, RecordColumnType> TypeToColumnType = new()
    {
        [typeof(bool)] = RecordColumnType.Boolean,
        [typeof(sbyte)] = RecordColumnType.SByte,
        [typeof(short)] = RecordColumnType.Int16,
        [typeof(int)] = RecordColumnType.Int32,
        [typeof(long)] = RecordColumnType.Int64,
        [typeof(byte)] = RecordColumnType.Byte,
        [typeof(ushort)] = RecordColumnType.UInt16,
        [typeof(uint)] = RecordColumnType.UInt32,
        [typeof(ulong)] = RecordColumnType.UInt64,
        [typeof(float)] = RecordColumnType.Single,
        [typeof(double)] = RecordColumnType.Double,
        [typeof(decimal)] = RecordColumnType.Decimal,
        [typeof(char)] = RecordColumnType.Char,
        [typeof(string)] = RecordColumnType.String,
        [typeof(DateTime)] = RecordColumnType.DateTime,
        [typeof(DateTimeOffset)] = RecordColumnType.DateTimeOffset,
        [typeof(TimeSpan)] = RecordColumnType.TimeSpan,
        [typeof(Guid)] = RecordColumnType.Guid,
        [typeof(byte[])] = RecordColumnType.ByteArray,
    };

    private static readonly Dictionary<RecordColumnType, Type> ColumnTypeToType;

    static Helpers()
    {
        ColumnTypeToType = new Dictionary<RecordColumnType, Type>(TypeToColumnType.Count * 2);
        foreach (var kv in TypeToColumnType)
        {
            ColumnTypeToType[kv.Value] = kv.Key;
            // 同时注册 Nullable 映射（负值）
            if (kv.Key.IsValueType)
            {
                var nullableEnum = (RecordColumnType)(-(sbyte)kv.Value);
                ColumnTypeToType[nullableEnum] = typeof(Nullable<>).MakeGenericType(kv.Key);
            }
        }
    }

    /// <summary>
    /// 从 CLR <see cref="Type"/> 获取 <see cref="RecordColumnType"/>。
    /// </summary>
    internal static RecordColumnType GetColumnType(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null && TypeToColumnType.TryGetValue(underlying, out var ct))
            return (RecordColumnType)(-(sbyte)ct);
        if (TypeToColumnType.TryGetValue(type, out ct))
            return ct;
        throw new NotSupportedException($"类型 '{type.FullName}' 不是支持的列类型");
    }

    /// <summary>
    /// 从 <see cref="RecordColumnType"/> 获取 CLR <see cref="Type"/>。
    /// </summary>
    internal static Type GetClrType(RecordColumnType columnType)
    {
        if (ColumnTypeToType.TryGetValue(columnType, out var type))
            return type;
        throw new NotSupportedException($"未知列类型枚举值: {columnType}");
    }

    #endregion

    internal static bool IsSupportedColumnType(Type type)
    {
        if (type == null) return false;
        if (TypeToColumnType.ContainsKey(type)) return true;
        var underlying = Nullable.GetUnderlyingType(type);
        return underlying != null && TypeToColumnType.ContainsKey(underlying);
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
