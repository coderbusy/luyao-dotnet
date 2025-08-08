using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace LuYao.Data;

static class Helpers
{
    private static readonly ConcurrentDictionary<Type, Func<Record, string, RecordDataCode, Type, RecordColumn>> _cache
        = new();

    private static Func<Record, string, RecordDataCode, Type, RecordColumn> GetConstructor(Type type)
    {
        return _cache.GetOrAdd(type, t =>
        {
            var genericType = typeof(RecordColumn<>).MakeGenericType(t);
            var ctor = genericType.GetConstructor(new[] {
                typeof(Record),
                typeof(string),
                typeof(RecordDataCode),
                typeof(Type)
            });

            if (ctor == null) throw new InvalidOperationException($"Constructor not found for {genericType}");
            // Parameters: (object record, string name, RecordDataType dataType, Type type)
            var pRecord = Expression.Parameter(typeof(Record), "record");
            var pName = Expression.Parameter(typeof(string), "name");
            var pDataType = Expression.Parameter(typeof(RecordDataCode), "dataType");
            var pType = Expression.Parameter(typeof(Type), "type");

            // new RecordColumn<T>(record, name, dataType, type)
            var newExpr = Expression.New(ctor, pRecord, pName, pDataType, pType);

            var lambda = Expression.Lambda<Func<Record, string, RecordDataCode, Type, RecordColumn>>(newExpr, pRecord, pName, pDataType, pType);
            return lambda.Compile();
        });
    }

    public static RecordColumn MakeRecordColumn(Record re, string name, Type type)
    {
        var ctor = GetConstructor(type);
        return ctor.Invoke(re, name, RecordDataCode.Object, type);
    }

    public static Type? ToType(RecordDataCode type)
    {
        return type switch
        {
            RecordDataCode.Boolean => typeof(bool),
            RecordDataCode.Byte => typeof(byte),
            RecordDataCode.Char => typeof(char),
            RecordDataCode.DateTime => typeof(DateTime),
            RecordDataCode.Decimal => typeof(decimal),
            RecordDataCode.Double => typeof(double),
            RecordDataCode.Int16 => typeof(short),
            RecordDataCode.Int32 => typeof(int),
            RecordDataCode.Int64 => typeof(long),
            RecordDataCode.SByte => typeof(sbyte),
            RecordDataCode.Single => typeof(float),
            RecordDataCode.String => typeof(string),
            RecordDataCode.UInt16 => typeof(ushort),
            RecordDataCode.UInt32 => typeof(uint),
            RecordDataCode.UInt64 => typeof(ulong),
            _ => null
        };
    }
}
