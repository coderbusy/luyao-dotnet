using LuYao.Data.Meta;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LuYao.Data;

static class Helpers
{
    #region FrameColumnType <-> Type 双向映射

    private static readonly Dictionary<Type, FrameColumnType> TypeToColumnType = new()
    {
        [typeof(bool)] = FrameColumnType.Boolean,
        [typeof(sbyte)] = FrameColumnType.SByte,
        [typeof(short)] = FrameColumnType.Int16,
        [typeof(int)] = FrameColumnType.Int32,
        [typeof(long)] = FrameColumnType.Int64,
        [typeof(byte)] = FrameColumnType.Byte,
        [typeof(ushort)] = FrameColumnType.UInt16,
        [typeof(uint)] = FrameColumnType.UInt32,
        [typeof(ulong)] = FrameColumnType.UInt64,
        [typeof(float)] = FrameColumnType.Single,
        [typeof(double)] = FrameColumnType.Double,
        [typeof(decimal)] = FrameColumnType.Decimal,
        [typeof(char)] = FrameColumnType.Char,
        [typeof(string)] = FrameColumnType.String,
        [typeof(DateTime)] = FrameColumnType.DateTime,
        [typeof(DateTimeOffset)] = FrameColumnType.DateTimeOffset,
        [typeof(TimeSpan)] = FrameColumnType.TimeSpan,
        [typeof(Guid)] = FrameColumnType.Guid,
        [typeof(byte[])] = FrameColumnType.ByteArray,
    };

    private static readonly Dictionary<FrameColumnType, Type> ColumnTypeToType;

    static Helpers()
    {
        ColumnTypeToType = new Dictionary<FrameColumnType, Type>(TypeToColumnType.Count);
        foreach (var kv in TypeToColumnType)
        {
            ColumnTypeToType[kv.Value] = kv.Key;
        }
    }

    /// <summary>
    /// 从 CLR <see cref="Type"/> 获取基础 <see cref="FrameColumnType"/>（不含 Nullable 信息）。
    /// </summary>
    internal static FrameColumnType GetColumnType(Type type)
    {
        var lookup = NormalizeColumnLookupType(type);
        if (TypeToColumnType.TryGetValue(lookup, out var ct))
            return ct;
        throw new NotSupportedException($"类型 '{type.FullName}' 不是支持的列类型");
    }

    /// <summary>
    /// 判断 CLR <see cref="Type"/> 是否为 Nullable 值类型。
    /// </summary>
    internal static bool IsNullableType(Type type)
    {
        return Nullable.GetUnderlyingType(type) != null;
    }

    /// <summary>
    /// 从基础 <see cref="FrameColumnType"/> 和 <paramref name="isNullable"/> 还原 CLR <see cref="Type"/>。
    /// </summary>
    internal static Type GetClrType(FrameColumnType columnType, bool isNullable)
    {
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
        return TypeToColumnType.ContainsKey(NormalizeColumnLookupType(type));
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
            throw new NotSupportedException($"类型 '{type.FullName}' 不是支持的列类型。支持的类型包括：bool, 整数类型, 浮点类型, char, string, DateTime, DateTimeOffset, TimeSpan, Guid, byte[]、枚举及其 Nullable 形式。");
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
    private static readonly ConcurrentDictionary<Type, Func<Frame, string, Type, FrameColumn>> _cache = new();

    private static Func<Frame, string, Type, FrameColumn> GetConstructor(Type type)
    {
        return _cache.GetOrAdd(type, static t =>
        {
            // 创建泛型类型
            var genericType = typeof(FrameColumn<>).MakeGenericType(t);

            // 获取构造函数
            var ctor = genericType.GetConstructor(new[] {
                typeof(Frame),
                typeof(string),
                typeof(Type)
            }) ?? throw new InvalidOperationException($"Constructor not found for {genericType}");

            // 定义参数表达式
            var pRecord = Expression.Parameter(typeof(Frame), "record");
            var pName = Expression.Parameter(typeof(string), "name");
            var pType = Expression.Parameter(typeof(Type), "type");

            // 创建构造函数调用表达式
            var newExpr = Expression.New(ctor, pRecord, pName, pType);

            // 编译表达式树为委托
            return Expression.Lambda<Func<Frame, string, Type, FrameColumn>>(
                newExpr, pRecord, pName, pType).Compile();
        });
    }

    public static FrameColumn MakeFrameColumn<T>(Frame record, string name)
    {
        // 使用泛型方法优化，避免运行时查找类型
        var ctor = GetConstructor(typeof(T));
        return ctor.Invoke(record, name, typeof(T));
    }

    public static FrameColumn MakeFrameColumn(Frame record, string name, Type type)
    {
        if (record == null) throw new ArgumentNullException(nameof(record));
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (type == null) throw new ArgumentNullException(nameof(type));

        ValidateColumnType(type);

        var ctor = GetConstructor(type);
        return ctor.Invoke(record, name, type);
    }
}
