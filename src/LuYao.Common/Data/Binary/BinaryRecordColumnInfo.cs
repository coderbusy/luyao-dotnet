using LuYao.Data.Models;
using System.IO;

namespace LuYao.Data.Binary;

internal class BinaryRecordColumnInfo : RecordColumnInfo
{
    public BinaryRecordColumnInfo()
    {

    }

    public BinaryRecordColumnInfo(RecordColumnInfo col)
    {
        this.Name = col.Name;
        this.Code = col.Code;
        this.Type = col.Type;
    }


    public void Write(BinaryWriter writer)
    {
        int code = (int)this.Code;
        string type = string.Empty;
        var t = Helpers.ToType(this.Code);
        if (this.Type != t) type = this.Type.AssemblyQualifiedName!;

        writer.Write(Name);
        writer.Write(code);
        writer.Write(type);
    }

    public void Read(BinaryReader reader)
    {
        this.Name = reader.ReadString();
        this.Code = (RecordDataCode)reader.ReadInt32();
        string typeName = reader.ReadString();
        System.Type? type = Helpers.ToType(this.Code);
        if (type == null && !string.IsNullOrEmpty(typeName))
        {
            type = System.Type.GetType(typeName);
        }
        if (type == null)
        {
            if (string.IsNullOrWhiteSpace(typeName)) throw new InvalidDataException($"数据列 {this.Name} 的类型信息缺失");
            throw new InvalidDataException($"数据列 {this.Name} 的类型找不到: {typeName}");
        }
        this.Type = type;
    }
}
