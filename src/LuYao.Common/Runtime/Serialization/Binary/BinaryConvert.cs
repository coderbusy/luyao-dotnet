using System;
using System.IO;

namespace LuYao.Runtime.Serialization.Binary;

public static class BinaryConvert
{
    public static byte[] ToBytes(Guid guid)
    {
        return guid.ToByteArray();
    }

    public static Guid ToGuid(byte[] bytes)
    {
        return new Guid(bytes);
    }
}
