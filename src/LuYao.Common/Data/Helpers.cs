using System;

namespace LuYao.Data;

internal static class Helpers
{
    public static Type ToType(DataType type)
    {
        return type switch
        {
            DataType.Boolean => typeof(bool),
            DataType.Byte => typeof(byte),
            DataType.Int32 => typeof(int),
            DataType.Int64 => typeof(long),
            DataType.Float => typeof(float),
            DataType.Double => typeof(double),
            DataType.DateTime => typeof(DateTime),
            DataType.DateTimeOffset => typeof(DateTimeOffset),
            DataType.String => typeof(string),
            DataType.Bytes => typeof(byte[]),
            DataType.Object => typeof(object),
            _ => typeof(Object)
        };
    }

    public static Type MakeType(DataType type, int dimension)
    {
        var elementType = ToType(type);
        if (dimension <= 0) return elementType;
        if (dimension == 1) return elementType.MakeArrayType();
        return elementType.MakeArrayType(dimension);
    }
}
