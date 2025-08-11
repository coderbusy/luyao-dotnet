using LuYao.Data.Models;
using System.IO;

namespace LuYao.Data.Formatters;

internal class BinaryRecordColumnInfo : RecordColumnInfo
{
    public BinaryRecordColumnInfo()
    {

    }

    public BinaryRecordColumnInfo(RecordColumnInfo col)
    {
        Name = col.Name;
        Code = col.Code;
        Type = col.Type;
    }

    public BinaryRecordColumnInfo(RecordColumn col) : this(new RecordColumnInfo(col))
    {

    }


    public void Write(BinaryWriter writer)
    {
        int code = (int)Code;
        string type = GetTypeName();

        writer.Write(Name);
        writer.Write(code);
        writer.Write(type);
    }

    public void Read(BinaryReader reader)
    {
        Name = reader.ReadString();
        Code = (RecordDataCode)reader.ReadInt32();
        string typeName = reader.ReadString();
        ParseTypeName(typeName);
    }
}
