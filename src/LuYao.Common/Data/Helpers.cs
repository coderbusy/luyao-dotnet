using System;
using System.Collections.Generic;

namespace LuYao.Data;

internal static class Helpers
{
    public static IEnumerable<TypeCode> ListTypeCode()
    {
        yield return TypeCode.Boolean;
        yield return TypeCode.Byte;
        yield return TypeCode.Char;
        yield return TypeCode.DateTime;
        yield return TypeCode.Decimal;
        yield return TypeCode.Double;
        yield return TypeCode.Int16;
        yield return TypeCode.Int32;
        yield return TypeCode.Int64;
        yield return TypeCode.SByte;
        yield return TypeCode.Single;
        yield return TypeCode.String;
        yield return TypeCode.UInt16;
        yield return TypeCode.UInt32;
        yield return TypeCode.UInt64;
    }

    public static Type ToType(TypeCode type)
    {
        return type switch
        {
            TypeCode.Boolean => typeof(bool),
            TypeCode.Byte => typeof(byte),
            TypeCode.Char => typeof(char),
            TypeCode.DateTime => typeof(DateTime),
            TypeCode.Decimal => typeof(decimal),
            TypeCode.Double => typeof(double),
            TypeCode.Int16 => typeof(short),
            TypeCode.Int32 => typeof(int),
            TypeCode.Int64 => typeof(long),
            TypeCode.SByte => typeof(sbyte),
            TypeCode.Single => typeof(float),
            TypeCode.String => typeof(string),
            TypeCode.UInt16 => typeof(ushort),
            TypeCode.UInt32 => typeof(uint),
            TypeCode.UInt64 => typeof(ulong),
            _ => throw new NotSupportedException()
        };
    }

    public static ColumnData MakeData(TypeCode type, int capacity)
    {
        return type switch
        {
            TypeCode.Boolean => new BooleanColumnData(capacity),
            TypeCode.Byte => new ByteColumnData(capacity),
            TypeCode.Char => new CharColumnData(capacity),
            TypeCode.DateTime => new DateTimeColumnData(capacity),
            TypeCode.Decimal => new DecimalColumnData(capacity),
            TypeCode.Double => new DoubleColumnData(capacity),
            TypeCode.Int16 => new Int16ColumnData(capacity),
            TypeCode.Int32 => new Int32ColumnData(capacity),
            TypeCode.Int64 => new Int64ColumnData(capacity),
            TypeCode.SByte => new SByteColumnData(capacity),
            TypeCode.Single => new SingleColumnData(capacity),
            TypeCode.String => new StringColumnData(capacity),
            TypeCode.UInt16 => new UInt16ColumnData(capacity),
            TypeCode.UInt32 => new UInt32ColumnData(capacity),
            TypeCode.UInt64 => new UInt64ColumnData(capacity),
            _ => throw new NotSupportedException()
        };
    }
}