using System;
using System.IO;

namespace LuYao.Data;

internal enum BinaryPayloadType : byte
{
    Record = 1,
    RecordSet = 2
}

internal static class BinaryPayloadHeader
{
    private const byte Marker = 0xFF;
    private const byte Signature1 = (byte)'L';
    private const byte Signature2 = (byte)'Y';

    public static void Write(BinaryWriter writer, BinaryPayloadType payloadType)
    {
        writer.Write(Marker);
        writer.Write(Signature1);
        writer.Write(Signature2);
        writer.Write((byte)payloadType);
    }

    public static byte ReadHeaderAndVersion(BinaryReader reader, BinaryPayloadType expectedType)
    {
        byte first = reader.ReadByte();
        if (first != Marker)
        {
            return first;
        }

        byte sig1 = reader.ReadByte();
        byte sig2 = reader.ReadByte();
        if (sig1 != Signature1 || sig2 != Signature2)
            throw new InvalidOperationException($"无效的二进制文件头签名，期望 'LY'，实际 0x{sig1:X2}{sig2:X2}。");

        var actualType = (BinaryPayloadType)reader.ReadByte();
        if (actualType != expectedType)
            throw new InvalidOperationException($"二进制数据类型不匹配，期望 {expectedType}，实际 {actualType}。");

        return reader.ReadByte();
    }
}
