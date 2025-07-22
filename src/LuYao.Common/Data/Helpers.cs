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
            TypeCode.Boolean => new ColumnData<bool>(capacity),
            TypeCode.Byte => new ColumnData<byte>(capacity),
            TypeCode.Char => new ColumnData<char>(capacity),
            TypeCode.DateTime => new ColumnData<DateTime>(capacity),
            TypeCode.Decimal => new ColumnData<decimal>(capacity),
            TypeCode.Double => new ColumnData<double>(capacity),
            TypeCode.Int16 => new ColumnData<short>(capacity),
            TypeCode.Int32 => new ColumnData<int>(capacity),
            TypeCode.Int64 => new ColumnData<long>(capacity),
            TypeCode.SByte => new ColumnData<sbyte>(capacity),
            TypeCode.Single => new ColumnData<float>(capacity),
            TypeCode.String => new ColumnData<string>(capacity),
            TypeCode.UInt16 => new ColumnData<ushort>(capacity),
            TypeCode.UInt32 => new ColumnData<uint>(capacity),
            TypeCode.UInt64 => new ColumnData<ulong>(capacity),
            _ => throw new NotSupportedException()
        };
    }
}