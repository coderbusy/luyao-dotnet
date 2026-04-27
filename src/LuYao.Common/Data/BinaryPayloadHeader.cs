using System;
using System.IO;

namespace LuYao.Data;

internal enum BinaryPayloadType : byte
{
    Frame = 1,
    FrameSet = 2
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

    /// <summary>
    /// 尝试从二进制数据读取类型头中的 payload 类型。
    /// </summary>
    /// <param name="data">二进制数据。</param>
    /// <param name="payloadType">成功时返回读取到的 payload 类型。</param>
    /// <returns>当数据包含有效类型头时返回 true；否则返回 false。</returns>
    public static bool TryGetPayloadType(byte[] data, out BinaryPayloadType payloadType)
    {
        payloadType = default;
        if (data == null) return false;
        if (data.Length < 4) return false;
        if (data[0] != Marker || data[1] != Signature1 || data[2] != Signature2) return false;

        byte rawType = data[3];
        if (rawType != (byte)BinaryPayloadType.Frame && rawType != (byte)BinaryPayloadType.FrameSet)
            return false;

        payloadType = (BinaryPayloadType)rawType;
        return true;
    }
}
