using System;
using System.Data.Common;
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
            writer.Write((byte)col.Code);//写入列代码
            string ext = string.Empty;
            writer.Write(ext); //写入扩展信息（目前为空）
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
            TypeCode code = (TypeCode)reader.ReadByte();
            string ext = reader.ReadString(); //读取扩展信息（目前为空）
            this.Columns.Add(n, code);
        }

        //读取数据行
        for (int r = 0; r < header.Count; r++)
        {
            foreach (var col in this.Columns)
            {
                switch (col.Code)
                {
                    case TypeCode.Boolean: col.Data.Set(reader.ReadBoolean(), r); break;
                    case TypeCode.Byte: col.Data.Set(reader.ReadByte(), r); break;
                    case TypeCode.Char: col.Data.Set(reader.ReadChar(), r); break;
                    case TypeCode.DateTime: col.Data.Set(reader.ReadInt64(), r); break;
                    case TypeCode.Decimal: col.Data.Set(reader.ReadDecimal(), r); break;
                    case TypeCode.Double: col.Data.Set(reader.ReadDouble(), r); break;
                    case TypeCode.Int16: col.Data.Set(reader.ReadInt16(), r); break;
                    case TypeCode.Int32: col.Data.Set(reader.ReadInt32(), r); break;
                    case TypeCode.Int64: col.Data.Set(reader.ReadInt64(), r); break;
                    case TypeCode.SByte: col.Data.Set(reader.ReadSByte(), r); break;
                    case TypeCode.Single: col.Data.Set(reader.ReadSingle(), r); break;
                    case TypeCode.String: col.Data.Set(reader.ReadString(), r); break;
                    case TypeCode.UInt16: col.Data.Set(reader.ReadUInt16(), r); break;
                    case TypeCode.UInt32: col.Data.Set(reader.ReadUInt32(), r); break;
                    case TypeCode.UInt64: col.Data.Set(reader.ReadUInt64(), r); break;
                }
            }
        }
    }



    /// <summary>
    /// 从指定的 <see cref="DbDataReader"/> 读取数据并填充到当前 <see cref="Record"/> 实例。
    /// </summary>
    /// <param name="reader">用于读取数据的 <see cref="DbDataReader"/> 实例。</param>
    public void Read(DbDataReader reader)
    {
        RecordColumn[] columns = new RecordColumn[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
        {
            string name = reader.GetName(i);
            Type type = reader.GetFieldType(i);
            TypeCode code = Type.GetTypeCode(type);
            if (Helpers.IsExists(code)) continue;
            int idx = this.Columns.IndexOf(name);
            if (idx >= 0)
            {
                columns[i] = this.Columns[idx];
            }
            else
            {
                columns[i] = this.Columns.Add(name, code);
            }
        }
        while (reader.Read())
        {
            var row = this.AddRow();
            for (int i = 0; i < columns.Length; i++)
            {
                RecordColumn? col = columns[i];
                if (col == null) continue;
                switch (col.Code)
                {
                    case TypeCode.Boolean: row.Set(reader.GetBoolean(i), col); break;
                    case TypeCode.Byte: row.Set(reader.GetByte(i), col); break;
                    case TypeCode.Char: row.Set(reader.GetChar(i), col); break;
                    case TypeCode.DateTime: row.Set(reader.GetDateTime(i), col); break;
                    case TypeCode.Decimal: row.Set(reader.GetDecimal(i), col); break;
                    case TypeCode.Double: row.Set(reader.GetDouble(i), col); break;
                    case TypeCode.Int16: row.Set(reader.GetInt16(i), col); break;
                    case TypeCode.Int32: row.Set(reader.GetInt32(i), col); break;
                    case TypeCode.Int64: row.Set(reader.GetInt64(i), col); break;
                    case TypeCode.SByte: row.SetValue(reader.GetValue(i), col); break;
                    case TypeCode.Single: row.Set(reader.GetFloat(i), col); break;
                    case TypeCode.String: row.Set(reader.GetString(i), col); break;
                    case TypeCode.UInt16: row.SetValue(reader.GetValue(i), col); break;
                    case TypeCode.UInt32: row.SetValue(reader.GetValue(i), col); break;
                    case TypeCode.UInt64: row.SetValue(reader.GetValue(i), col); break;
                }
            }
        }
    }
}
