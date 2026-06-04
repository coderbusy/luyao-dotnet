using LuYao.Data.Meta;
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
        [typeof(byte[])] = RecordColumnType.Binary,
    };

    private static readonly Dictionary<RecordColumnType, Type> ColumnTypeToType;

    static Helpers()
    {
        ColumnTypeToType = new Dictionary<RecordColumnType, Type>(TypeToColumnType.Count);
        foreach (var kv in TypeToColumnType)
        {
            ColumnTypeToType[kv.Value] = kv.Key;
        }
    }

    /// <summary>
    /// From CLR <see cref="Type"/> get the base <see cref="RecordColumnType"/> (excluding Nullable and array info).
    /// </summary>
    internal static RecordColumnType GetColumnType(Type type)
    {
        // byte[] maps directly to Binary
        if (type == typeof(byte[]))
        {
            return RecordColumnType.Binary;
        }

        var lookup = NormalizeColumnLookupType(type);

        if (TypeToColumnType.TryGetValue(lookup, out var ct))
            return ct;
        throw new NotSupportedException($"类型 '{type.FullName}' 不是支持的列类型");
    }

    /// <summary>
    /// 判断 CLR <see cref="Type"/> 是否为 Nullable 值类型，或其数组元素类型为 Nullable。
    /// </summary>
    internal static bool IsNullableType(Type type)
    {
        return Nullable.GetUnderlyingType(type) != null;
    }

    /// <summary>
    /// 从基础 <see cref="RecordColumnType"/>、<paramref name="isNullable"/> 和 <paramref name="arrayRank"/> 还原 CLR <see cref="Type"/>。
    /// </summary>
    internal static Type GetClrType(RecordColumnType columnType, bool isNullable)
    {
        if (columnType == RecordColumnType.Binary)
            return typeof(byte[]);

        if (!ColumnTypeToType.TryGetValue(columnType, out var baseType))
            throw new NotSupportedException($"未知列类型枚举值: {columnType}");

        if (isNullable && baseType.IsValueType)
            return typeof(Nullable<>).MakeGenericType(baseType);
        return baseType;
    }

    #endregion

    internal static bool IsSupportedColumnType(Type type)
    {
        if (type == null) return false;

        // byte[] is directly supported as Binary
        if (type == typeof(byte[])) return true;

        var normalized = NormalizeColumnLookupType(type);
        return TypeToColumnType.ContainsKey(normalized);
    }

    internal static bool IsSupportedForReading(IXProp p)
    {
        if (p != null && p.CanRead && IsSupportedColumnType(p.Type)) return true;
        return false;
    }
    internal static bool IsSupportedForWriting(IXProp p)
    {
        if (p != null && p.CanWrite && IsSupportedColumnType(p.Type)) return true;
        return false;
    }

    /// <summary>
    /// 验证列类型是否在白名单内，不在则抛出异常。
    /// </summary>
    internal static void ValidateColumnType(Type type)
    {
        if (!IsSupportedColumnType(type))
        {
            throw new NotSupportedException($"类型 '{type.FullName}' 不是支持的列类型。支持的类型包括：bool, 整数类型, 浮点类型, char, string, DateTime, DateTimeOffset, TimeSpan, Guid, byte[]、枚举及其 Nullable 形式，以及上述类型的数组（如 int[], string[,]）。");
        }
    }

    /// <summary>
    /// 规范化列类型查找目标：先展开 Nullable，再将枚举转换为其基础数值类型。
    /// </summary>
    private static Type NormalizeColumnLookupType(Type type)
    {
        var effective = Nullable.GetUnderlyingType(type) ?? type;
        return effective.IsEnum ? Enum.GetUnderlyingType(effective) : effective;
    }
    private static readonly ConcurrentDictionary<Type, Func<RecordTable, string, Type, RecordColumn>> _cache = new();

    private static Func<RecordTable, string, Type, RecordColumn> GetConstructor(Type type)
    {
        return _cache.GetOrAdd(type, static t =>
        {
            // 创建泛型类型
            var genericType = typeof(RecordColumn<>).MakeGenericType(t);

            // 获取构造函数
            var ctor = genericType.GetConstructor(new[] {
                typeof(RecordTable),
                typeof(string),
                typeof(Type)
            }) ?? throw new InvalidOperationException($"Constructor not found for {genericType}");

            // 定义参数表达式
            var pRecord = Expression.Parameter(typeof(RecordTable), "table");
            var pName = Expression.Parameter(typeof(string), "name");
            var pType = Expression.Parameter(typeof(Type), "type");

            // 创建构造函数调用表达式
            var newExpr = Expression.New(ctor, pRecord, pName, pType);

            // 编译表达式树为委托
            return Expression.Lambda<Func<RecordTable, string, Type, RecordColumn>>(
                newExpr, pRecord, pName, pType).Compile();
        });
    }

    public static RecordColumn MakeRecordColumn<T>(RecordTable table, string name)
    {
        // 使用泛型方法优化，避免运行时查找类型
        var ctor = GetConstructor(typeof(T));
        return ctor.Invoke(table, name, typeof(T));
    }

    public static RecordColumn MakeRecordColumn(RecordTable table, string name, Type type)
    {
        if (table == null) throw new ArgumentNullException(nameof(table));
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (type == null) throw new ArgumentNullException(nameof(type));

        ValidateColumnType(type);

        var ctor = GetConstructor(type);
        return ctor.Invoke(table, name, type);
    }
}
