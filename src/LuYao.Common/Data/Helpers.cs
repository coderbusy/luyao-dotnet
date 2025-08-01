using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace LuYao.Data;

internal static class Helpers
{
    private static class FactoryCache<T>
    {
        public static readonly Func<int, ColumnData> Factory = CreateFactory();

        private static Func<int, ColumnData> CreateFactory()
        {
            var genericType = typeof(GenericColumnData<>).MakeGenericType(typeof(T));
            var constructor = genericType.GetConstructor(new[] { typeof(int) });

            if (constructor == null) throw new InvalidOperationException($"Constructor not found for {genericType}");

            var capacityParam = Expression.Parameter(typeof(int), "capacity");
            var newExpression = Expression.New(constructor, capacityParam);
            var lambda = Expression.Lambda<Func<int, ColumnData>>(newExpression, capacityParam);

            return lambda.Compile();
        }
    }

    public static Type ToType(RecordDataType type)
    {
        return type switch
        {
            RecordDataType.Boolean => typeof(bool),
            RecordDataType.Byte => typeof(byte),
            RecordDataType.Char => typeof(char),
            RecordDataType.DateTime => typeof(DateTime),
            RecordDataType.Decimal => typeof(decimal),
            RecordDataType.Double => typeof(double),
            RecordDataType.Int16 => typeof(short),
            RecordDataType.Int32 => typeof(int),
            RecordDataType.Int64 => typeof(long),
            RecordDataType.SByte => typeof(sbyte),
            RecordDataType.Single => typeof(float),
            RecordDataType.String => typeof(string),
            RecordDataType.UInt16 => typeof(ushort),
            RecordDataType.UInt32 => typeof(uint),
            RecordDataType.UInt64 => typeof(ulong),
            _ => throw new NotSupportedException()
        };
    }

    public static ColumnData MakeData(RecordDataType dt, int capacity, Type type)
    {
        return dt switch
        {
            RecordDataType.Boolean => new BooleanColumnData(capacity),
            RecordDataType.Byte => new ByteColumnData(capacity),
            RecordDataType.Char => new CharColumnData(capacity),
            RecordDataType.DateTime => new DateTimeColumnData(capacity),
            RecordDataType.Decimal => new DecimalColumnData(capacity),
            RecordDataType.Double => new DoubleColumnData(capacity),
            RecordDataType.Int16 => new Int16ColumnData(capacity),
            RecordDataType.Int32 => new Int32ColumnData(capacity),
            RecordDataType.Int64 => new Int64ColumnData(capacity),
            RecordDataType.SByte => new SByteColumnData(capacity),
            RecordDataType.Single => new SingleColumnData(capacity),
            RecordDataType.String => new StringColumnData(capacity),
            RecordDataType.UInt16 => new UInt16ColumnData(capacity),
            RecordDataType.UInt32 => new UInt32ColumnData(capacity),
            RecordDataType.UInt64 => new UInt64ColumnData(capacity),
            RecordDataType.Object => MakeData(capacity, type),
            _ => throw new NotSupportedException()
        };
    }

    private static ColumnData MakeData(int capacity, Type type)
    {
        var factoryMethod = typeof(FactoryCache<>).MakeGenericType(type).GetField("Factory");
        var factory = (Func<int, ColumnData>)factoryMethod!.GetValue(null)!;
        return factory(capacity);
    }

    public static RecordDataType ReadDataType(BinaryReader reader)
    {
        byte codeValue = reader.ReadByte();
        if (!Enum.IsDefined(typeof(RecordDataType), codeValue)) throw new InvalidDataException($"Invalid RecordDataType value: {codeValue}");
        return (RecordDataType)codeValue;
    }

    public static void WriteDataType(BinaryWriter writer, RecordDataType type)
    {
        writer.Write((byte)type);
    }
}