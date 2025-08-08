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
        string type = this.GetTypeName();

        writer.Write(Name);
        writer.Write(code);
        writer.Write(type);
    }

    public void Read(BinaryReader reader)
    {
        this.Name = reader.ReadString();
        this.Code = (RecordDataCode)reader.ReadInt32();
        string typeName = reader.ReadString();
        this.ParseTypeName(typeName);
    }
}
