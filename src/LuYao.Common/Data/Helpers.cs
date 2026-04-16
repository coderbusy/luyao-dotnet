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

    #region Type Alias (compact serialization)

    private static readonly Dictionary<Type, string> TypeToAlias = new()
    {
        [typeof(bool)] = "b",
        [typeof(sbyte)] = "i1", [typeof(short)] = "i2", [typeof(int)] = "i4", [typeof(long)] = "i8",
        [typeof(byte)] = "u1", [typeof(ushort)] = "u2", [typeof(uint)] = "u4", [typeof(ulong)] = "u8",
        [typeof(float)] = "r4", [typeof(double)] = "r8", [typeof(decimal)] = "dc",
        [typeof(char)] = "c", [typeof(string)] = "s",
        [typeof(DateTime)] = "dt", [typeof(DateTimeOffset)] = "dto", [typeof(TimeSpan)] = "ts",
        [typeof(Guid)] = "g",
        [typeof(byte[])] = "bin",
    };

    private static readonly Dictionary<string, Type> AliasToType;

    /// <summary>
    /// 获取类型的紧凑别名。支持 Nullable 类型（别名后加 '?'）。
    /// </summary>
    internal static string GetTypeAlias(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null && TypeToAlias.TryGetValue(underlying, out var alias))
            return alias + "?";
        if (TypeToAlias.TryGetValue(type, out alias))
            return alias;
        return type.FullName!;
    }

    /// <summary>
    /// 从紧凑别名还原类型。支持 '?' 后缀表示 Nullable。
    /// 兼容完整类型名（用于向后兼容旧格式）。
    /// </summary>
    internal static Type GetTypeFromAlias(string alias)
    {
        if (alias.EndsWith("?"))
        {
            var baseAlias = alias.Substring(0, alias.Length - 1);
            if (AliasToType.TryGetValue(baseAlias, out var baseType))
                return typeof(Nullable<>).MakeGenericType(baseType);
        }
        if (AliasToType.TryGetValue(alias, out var type))
            return type;
        // fallback: full type name (backward compat)
        return Type.GetType(alias, throwOnError: true)!;
    }

    #endregion

    #region Type Code (compact binary serialization)

    private static readonly Dictionary<Type, sbyte> TypeToCode = new()
    {
        [typeof(bool)] = 1,
        [typeof(sbyte)] = 2, [typeof(short)] = 3, [typeof(int)] = 4, [typeof(long)] = 5,
        [typeof(byte)] = 6, [typeof(ushort)] = 7, [typeof(uint)] = 8, [typeof(ulong)] = 9,
        [typeof(float)] = 10, [typeof(double)] = 11, [typeof(decimal)] = 12,
        [typeof(char)] = 13, [typeof(string)] = 14,
        [typeof(DateTime)] = 15, [typeof(DateTimeOffset)] = 16, [typeof(TimeSpan)] = 17,
        [typeof(Guid)] = 18,
        [typeof(byte[])] = 19,
    };

    private static readonly Dictionary<sbyte, Type> CodeToType;

    static Helpers()
    {
        AliasToType = new Dictionary<string, Type>(TypeToAlias.Count * 2, StringComparer.Ordinal);
        foreach (var kv in TypeToAlias)
        {
            AliasToType[kv.Value] = kv.Key;
        }

        CodeToType = new Dictionary<sbyte, Type>(TypeToCode.Count);
        foreach (var kv in TypeToCode)
        {
            CodeToType[kv.Value] = kv.Key;
        }
    }

    /// <summary>
    /// 获取类型的紧凑 sbyte 编码。Nullable 类型使用负值。
    /// </summary>
    internal static sbyte GetTypeCode(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null && TypeToCode.TryGetValue(underlying, out var code))
            return (sbyte)(-code);
        if (TypeToCode.TryGetValue(type, out code))
            return code;
        throw new NotSupportedException($"类型 '{type.FullName}' 无对应类型编码");
    }

    /// <summary>
    /// 从 sbyte 编码还原类型。负值表示 Nullable。
    /// </summary>
    internal static Type GetTypeFromCode(sbyte code)
    {
        if (code < 0)
        {
            var baseCode = (sbyte)(-code);
            if (CodeToType.TryGetValue(baseCode, out var baseType))
                return typeof(Nullable<>).MakeGenericType(baseType);
        }
        if (CodeToType.TryGetValue(code, out var type))
            return type;
        throw new NotSupportedException($"未知类型编码: {code}");
    }

    #endregion
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
