using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace LuYao.Data;

static class Helpers
{
    private static readonly ConcurrentDictionary<Type, Func<Record, string, RecordDataType, Type, RecordColumn>> _cache
        = new();

    private static Func<Record, string, RecordDataType, Type, RecordColumn> GetConstructor(Type type)
    {
        return _cache.GetOrAdd(type, t =>
        {
            var genericType = typeof(RecordColumn<>).MakeGenericType(t);
            var ctor = genericType.GetConstructor(new[] {
                typeof(Record),
                typeof(string),
                typeof(RecordDataType),
                typeof(Type)
            });

            if (ctor == null) throw new InvalidOperationException($"Constructor not found for {genericType}");
            // Parameters: (object record, string name, RecordDataType dataType, Type type)
            var pRecord = Expression.Parameter(typeof(Record), "record");
            var pName = Expression.Parameter(typeof(string), "name");
            var pDataType = Expression.Parameter(typeof(RecordDataType), "dataType");
            var pType = Expression.Parameter(typeof(Type), "type");

            // new RecordColumn<T>(record, name, dataType, type)
            var newExpr = Expression.New(ctor, pRecord, pName, pDataType, pType);

            var lambda = Expression.Lambda<Func<Record, string, RecordDataType, Type, RecordColumn>>(newExpr, pRecord, pName, pDataType, pType);
            return lambda.Compile();
        });
    }

    public static RecordColumn MakeRecordColumn(Record re, string name, Type type)
    {
        var ctor = GetConstructor(type);
        return ctor.Invoke(re, name, RecordDataType.Object, type);
    }
}
