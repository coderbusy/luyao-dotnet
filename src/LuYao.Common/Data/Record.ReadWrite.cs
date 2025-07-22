using System;
using System.IO;

namespace LuYao.Data;

partial class Record
{
    /// <summary>
    /// 将当前 <see cref="Record"/> 实例的数据以二进制格式写入指定的 <see cref="BinaryWriter"/>。
    /// </summary>
    /// <param name="writer">用于写入数据的 <see cref="BinaryWriter"/> 实例。</param>
    public void Write(BinaryWriter writer)
    {
        //字符串化的文件头
        string header = new RecordSchema(this).SerializeToString();
        writer.Write(header);

        //写入列
        foreach (var col in this.Columns)
        {
            writer.Write(col.Name);//写入列名
            writer.Write((int)col.Code);//写入列代码
        }

        //按行写入数据
        for (int r = 0; r < this.Count; r++)
        {
            foreach (var col in this.Columns)
            {
                switch (col.Code)
                {
                    case TypeCode.Boolean: writer.Write(col.Data.ToBoolean(r)); break;
                    case TypeCode.Byte: writer.Write(col.Data.ToByte(r)); break;
                    case TypeCode.Char: writer.Write(col.Data.ToChar(r)); break;
                    case TypeCode.DateTime: writer.Write(col.Data.ToInt64(r)); break;
                    case TypeCode.Decimal: writer.Write(col.Data.ToDecimal(r)); break;
                    case TypeCode.Double: writer.Write(col.Data.ToDouble(r)); break;
                    case TypeCode.Int16: writer.Write(col.Data.ToInt16(r)); break;
                    case TypeCode.Int32: writer.Write(col.Data.ToInt32(r)); break;
                    case TypeCode.Int64: writer.Write(col.Data.ToInt64(r)); break;
                    case TypeCode.SByte: writer.Write(col.Data.ToSByte(r)); break;
                    case TypeCode.Single: writer.Write(col.Data.ToSingle(r)); break;
                    case TypeCode.String: writer.Write(col.Data.ToString(r)); break;
                    case TypeCode.UInt16: writer.Write(col.Data.ToUInt16(r)); break;
                    case TypeCode.UInt32: writer.Write(col.Data.ToUInt32(r)); break;
                    case TypeCode.UInt64: writer.Write(col.Data.ToUInt64(r)); break;
                }
            }
        }
    }

    /// <summary>
    /// 从指定的 <see cref="BinaryReader"/> 读取二进制格式的数据并填充到当前 <see cref="Record"/> 实例。
    /// </summary>
    /// <param name="reader">用于读取数据的 <see cref="BinaryReader"/> 实例。</param>
    public void Read(BinaryReader reader)
    {
        //读取字符串化的文件头
        string h = reader.ReadString();
        var header = RecordSchema.FromString(h);
        this.Name = header.Name;

        //构建列集合
        this.Columns.Clear();
        this.Columns.SetCapacity(header.Count);
        for (int i = 0; i < header.Columns; i++)
        {
            string n = reader.ReadString();
            TypeCode code = (TypeCode)reader.ReadInt32();
            this.Columns.Add(n, code);
        }

        //读取数据行
        for (int r = 0; r < header.Count; r++)
        {
            foreach (var col in this.Columns)
            {
                switch (col.Code)
                {
                    case TypeCode.Boolean: col.Data.SetValue(reader.ReadBoolean(), r); break;
                    case TypeCode.Byte: col.Data.SetValue(reader.ReadByte(), r); break;
                    case TypeCode.Char: col.Data.SetValue(reader.ReadChar(), r); break;
                    case TypeCode.DateTime: col.Data.SetValue(reader.ReadInt64(), r); break;
                    case TypeCode.Decimal: col.Data.SetValue(reader.ReadDecimal(), r); break;
                    case TypeCode.Double: col.Data.SetValue(reader.ReadDouble(), r); break;
                    case TypeCode.Int16: col.Data.SetValue(reader.ReadInt16(), r); break;
                    case TypeCode.Int32: col.Data.SetValue(reader.ReadInt32(), r); break;
                    case TypeCode.Int64: col.Data.SetValue(reader.ReadInt64(), r); break;
                    case TypeCode.SByte: col.Data.SetValue(reader.ReadSByte(), r); break;
                    case TypeCode.Single: col.Data.SetValue(reader.ReadSingle(), r); break;
                    case TypeCode.String: col.Data.SetValue(reader.ReadString(), r); break;
                    case TypeCode.UInt16: col.Data.SetValue(reader.ReadUInt16(), r); break;
                    case TypeCode.UInt32: col.Data.SetValue(reader.ReadUInt32(), r); break;
                    case TypeCode.UInt64: col.Data.SetValue(reader.ReadUInt64(), r); break;
                }
            }
        }
    }
}
