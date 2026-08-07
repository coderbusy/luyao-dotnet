using System;
using System.Collections.Generic;

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
    /// From CLR <see cref="Type"/> get its <see cref="RecordColumnType"/>.
    /// </summary>
    internal static RecordColumnType GetColumnType(Type type)
    {
        // byte[] maps directly to Binary
        if (type == typeof(byte[]))
        {
            return RecordColumnType.Binary;
        }

        if (TypeToColumnType.TryGetValue(type, out var ct))
            return ct;
        throw new NotSupportedException($"类型 '{type.FullName}' 不是支持的列类型");
    }

    internal static Type GetClrType(RecordColumnType columnType)
    {
        if (columnType == RecordColumnType.Binary)
            return typeof(byte[]);

        if (!ColumnTypeToType.TryGetValue(columnType, out var baseType))
            throw new NotSupportedException($"未知列类型枚举值: {columnType}");

        return baseType;
    }

    #endregion

    internal static bool IsSupportedColumnType(Type type)
    {
        if (type == null) return false;

        // byte[] is directly supported as Binary
        if (type == typeof(byte[])) return true;

        return TypeToColumnType.ContainsKey(type);
    }

    /// 验证列类型是否在白名单内，不在则抛出异常。
    internal static void ValidateColumnType(Type type)
    {
        if (!IsSupportedColumnType(type))
        {
            throw new NotSupportedException($"类型 '{type.FullName}' 不是支持的列类型。支持类型由 RecordColumnType 定义；byte[] 作为 Binary 支持，其他数组、Nullable 和枚举不受支持。");
        }
    }

    public static RecordColumn MakeRecordColumn<T>(RecordTable table, string name)
    {
        return MakeRecordColumn(table, name, typeof(T));
    }

    public static RecordColumn MakeRecordColumn(RecordTable table, string name, Type type)
    {
        if (table == null) throw new ArgumentNullException(nameof(table));
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (type == null) throw new ArgumentNullException(nameof(type));

        ValidateColumnType(type);

        return MakeRecordColumn(table, name, GetColumnType(type), type);
    }

    internal static RecordColumn MakeRecordColumn(RecordTable table, string name, RecordColumnType columnType, Type? clrType = null)
    {
        var type = clrType ?? GetClrType(columnType);
        return columnType switch
        {
            RecordColumnType.Boolean => new RecordColumn<bool>(table, name, type),
            RecordColumnType.SByte => new RecordColumn<sbyte>(table, name, type),
            RecordColumnType.Int16 => new RecordColumn<short>(table, name, type),
            RecordColumnType.Int32 => new RecordColumn<int>(table, name, type),
            RecordColumnType.Int64 => new RecordColumn<long>(table, name, type),
            RecordColumnType.Byte => new RecordColumn<byte>(table, name, type),
            RecordColumnType.UInt16 => new RecordColumn<ushort>(table, name, type),
            RecordColumnType.UInt32 => new RecordColumn<uint>(table, name, type),
            RecordColumnType.UInt64 => new RecordColumn<ulong>(table, name, type),
            RecordColumnType.Single => new RecordColumn<float>(table, name, type),
            RecordColumnType.Double => new RecordColumn<double>(table, name, type),
            RecordColumnType.Decimal => new RecordColumn<decimal>(table, name, type),
            RecordColumnType.Char => new RecordColumn<char>(table, name, type),
            RecordColumnType.String => new RecordColumn<string>(table, name, type),
            RecordColumnType.DateTime => new RecordColumn<DateTime>(table, name, type),
            RecordColumnType.DateTimeOffset => new RecordColumn<DateTimeOffset>(table, name, type),
            RecordColumnType.TimeSpan => new RecordColumn<TimeSpan>(table, name, type),
            RecordColumnType.Guid => new RecordColumn<Guid>(table, name, type),
            RecordColumnType.Binary => new RecordColumn<byte[]>(table, name, type),
            _ => throw new NotSupportedException($"未知列类型: {columnType}")
        };
    }
}


