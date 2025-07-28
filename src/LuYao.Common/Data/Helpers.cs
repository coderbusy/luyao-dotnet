using System;
using System.Collections.Generic;
using System.IO;

namespace LuYao.Data;

internal static class Helpers
{
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

    public static ColumnData MakeData(RecordDataType type, int capacity)
    {
        return type switch
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
            RecordDataType.Object => throw new NotSupportedException(),
            _ => throw new NotSupportedException()
        };
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