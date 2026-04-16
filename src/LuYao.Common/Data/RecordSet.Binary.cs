using System;
using System.IO;
using System.Text;

namespace LuYao.Data;

public partial class RecordSet
{
    private const byte BinaryFormatVersion = 1;

    /// <summary>
    /// 将当前 <see cref="RecordSet"/> 写入二进制流。
    /// </summary>
    /// <param name="stream">目标流。</param>
    public void WriteTo(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
        WriteTo(writer);
    }

    /// <summary>
    /// 将当前 <see cref="RecordSet"/> 写入 <see cref="BinaryWriter"/>。
    /// </summary>
    /// <param name="writer">目标写入器。</param>
    public void WriteTo(BinaryWriter writer)
    {
        if (writer == null) throw new ArgumentNullException(nameof(writer));
        writer.Write(BinaryFormatVersion);
        writer.Write(_records.Count);
        foreach (var kvp in _records)
        {
            // 以字典键作为权威名称，防止 Record.Name 被外部修改后产生漂移
            kvp.Value.Name = kvp.Key;
            kvp.Value.WriteTo(writer);
        }
    }

    /// <summary>
    /// 从二进制流读取并填充当前 <see cref="RecordSet"/> 实例。
    /// </summary>
    /// <param name="stream">源流。</param>
    public void ReadFrom(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
        ReadFrom(reader);
    }

    /// <summary>
    /// 从 <see cref="BinaryReader"/> 读取并填充当前 <see cref="RecordSet"/> 实例。
    /// </summary>
    /// <param name="reader">源读取器。</param>
    public void ReadFrom(BinaryReader reader)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        byte version = reader.ReadByte();
        if (version != BinaryFormatVersion)
            throw new InvalidOperationException($"不支持的二进制格式版本: {version}");

        this.Clear();
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            var record = new Record();
            record.ReadFrom(reader);
            this.Add(record.Name, record);
        }
    }

    /// <summary>
    /// 从二进制流创建新的 <see cref="RecordSet"/> 实例。
    /// </summary>
    /// <param name="stream">源流。</param>
    /// <returns>反序列化的 <see cref="RecordSet"/> 实例。</returns>
    public static RecordSet FromStream(Stream stream)
    {
        var set = new RecordSet();
        set.ReadFrom(stream);
        return set;
    }

    /// <summary>
    /// 序列化为字节数组。
    /// </summary>
    public byte[] ToBytes()
    {
        using var ms = new MemoryStream();
        WriteTo(ms);
        return ms.ToArray();
    }

    /// <summary>
    /// 从字节数组反序列化。
    /// </summary>
    /// <param name="data">二进制数据。</param>
    /// <returns>反序列化的 <see cref="RecordSet"/> 实例。</returns>
    public static RecordSet FromBytes(byte[] data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        using var ms = new MemoryStream(data, writable: false);
        return FromStream(ms);
    }
}
