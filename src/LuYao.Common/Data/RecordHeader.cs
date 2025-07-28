using LuYao.Text;
using System;
using System.IO;

namespace LuYao.Data;

/// <summary>
/// 表示记录的模式信息。
/// </summary>
public class RecordHeader
{
    /// <summary>
    /// 初始化 <see cref="RecordHeader"/> 类的新实例。
    /// </summary>
    public RecordHeader()
    {

    }

    /// <summary>
    /// 使用指定的记录初始化 <see cref="RecordHeader"/> 类的新实例。
    /// </summary>
    /// <param name="re">记录对象。</param>
    public RecordHeader(Record re)
    {
        if (re == null) throw new ArgumentNullException(nameof(re));
        this.Name = re.Name;
        this.Columns = re.Columns.Count;
        this.Count = re.Count;
    }

    /// <summary>
    /// 获取或设置版本号。
    /// </summary>
    public byte Version { get; set; } = 1;

    /// <summary>
    /// 获取或设置记录的名称。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置列的数量。
    /// </summary>
    public int Columns { get; set; }

    /// <summary>
    /// 获取或设置记录的数量。
    /// </summary>
    public int Count { get; set; }

    internal void Load(BinaryReader r)
    {
        byte version = r.ReadByte();
        this.Version = version;
        switch (version)
        {
            case 1:
                this.Name = r.ReadString();
                this.Columns = r.ReadInt32();
                this.Count = r.ReadInt32();
                break;
            default: throw new NotSupportedException();
        }
    }

    internal void Save(BinaryWriter w)
    {
        //version,[name|type,value...]
        w.Write(Version);

        //Name
        w.Write(this.Name);

        //Columns
        w.Write(this.Columns);

        //Count
        w.Write(this.Count);
    }
}
