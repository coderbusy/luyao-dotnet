namespace LuYao.Data.Binary;

/// <summary>
/// 指定 <see cref="RecordBinaryPayloadCodec"/> 序列化时使用的压缩算法。
/// </summary>
public enum RecordPayloadCompression : byte
{
    /// <summary>不压缩。</summary>
    None = 0,

    /// <summary>使用 GZip 压缩。</summary>
    GZip = 1,
}
