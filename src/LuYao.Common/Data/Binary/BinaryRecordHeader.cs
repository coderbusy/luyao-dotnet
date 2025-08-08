using LuYao.Data.Models;
using System;
using System.IO;

namespace LuYao.Data.Binary;

internal class BinaryRecordHeader : RecordHeader
{

    /// <summary>
    /// 获取或设置版本号。
    /// </summary>
    public byte Version { get; set; } = 1;

    public BinaryRecordHeader()
    {

    }

    public BinaryRecordHeader(RecordHeader header)
    {
        Name = header.Name;
        Columns = header.Columns;
        Count = header.Count;
    }

    public void Write(BinaryWriter writer)
    {
        if (writer == null) throw new ArgumentNullException(nameof(writer));
        writer.Write(Version);
        writer.Write(Name ?? string.Empty);
        writer.Write(Columns);
        writer.Write(Count);
    }

    public void Read(BinaryReader reader)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        Version = reader.ReadByte();
        Name = reader.ReadString();
        Columns = reader.ReadInt32();
        Count = reader.ReadInt32();
    }
}
