using System;

namespace LuYao.Data;

internal static class Helpers
{
    public static Type ToType(TypeCode type)
    {
        return type switch
        {
            TypeCode.Boolean => typeof(bool),
            TypeCode.Char => typeof(char),
            TypeCode.SByte => typeof(sbyte),
            TypeCode.Byte => typeof(byte),
            TypeCode.Int16 => typeof(short),
            TypeCode.UInt16 => typeof(ushort),
            TypeCode.Int32 => typeof(int),
            TypeCode.UInt32 => typeof(uint),
            TypeCode.Int64 => typeof(long),
            TypeCode.UInt64 => typeof(ulong),
            TypeCode.Single => typeof(float),
            TypeCode.Double => typeof(double),
            TypeCode.Decimal => typeof(decimal),
            TypeCode.DateTime => typeof(DateTime),
            TypeCode.String => typeof(string),
            _ => throw new NotSupportedException()
        };
    }
}