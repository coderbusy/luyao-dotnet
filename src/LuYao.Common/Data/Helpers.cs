using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace LuYao.Data;

static class Helpers
{
    private static readonly ConcurrentDictionary<Type, Func<Record, string, Type, RecordColumn>> _cache
        = new();

    private static Func<Record, string, Type, RecordColumn> GetConstructor(Type type)
    {
        return _cache.GetOrAdd(type, t =>
        {
            var genericType = typeof(RecordColumn<>).MakeGenericType(t);
            var ctor = genericType.GetConstructor(new[] {
                typeof(Record),
                typeof(string),
                typeof(Type)
            });

            if (ctor == null) throw new InvalidOperationException($"Constructor not found for {genericType}");
            // Parameters: (object record, string name, Type type)
            var pRecord = Expression.Parameter(typeof(Record), "record");
            var pName = Expression.Parameter(typeof(string), "name");
            var pType = Expression.Parameter(typeof(Type), "type");

            // new RecordColumn<T>(record, name, type)
            var newExpr = Expression.New(ctor, pRecord, pName, pType);

            var lambda = Expression.Lambda<Func<Record, string, Type, RecordColumn>>(newExpr, pRecord, pName, pType);
            return lambda.Compile();
        });
    }

    public static RecordColumn MakeRecordColumn(Record re, string name, Type type)
    {
        var ctor = GetConstructor(type);
        return ctor.Invoke(re, name, type);
    }
}
