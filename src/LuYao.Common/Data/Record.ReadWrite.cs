﻿using System;
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
                    case RecordDataType.Boolean: writer.Write(col.Data.ToBoolean(r)); break;
                    case RecordDataType.Byte: writer.Write(col.Data.ToByte(r)); break;
                    case RecordDataType.Char: writer.Write(col.Data.ToChar(r)); break;
                    case RecordDataType.DateTime: writer.Write(col.Data.ToInt64(r)); break;
                    case RecordDataType.Decimal: writer.Write(col.Data.ToDecimal(r)); break;
                    case RecordDataType.Double: writer.Write(col.Data.ToDouble(r)); break;
                    case RecordDataType.Int16: writer.Write(col.Data.ToInt16(r)); break;
                    case RecordDataType.Int32: writer.Write(col.Data.ToInt32(r)); break;
                    case RecordDataType.Int64: writer.Write(col.Data.ToInt64(r)); break;
                    case RecordDataType.SByte: writer.Write(col.Data.ToSByte(r)); break;
                    case RecordDataType.Single: writer.Write(col.Data.ToSingle(r)); break;
                    case RecordDataType.String: writer.Write(col.Data.ToString(r)); break;
                    case RecordDataType.UInt16: writer.Write(col.Data.ToUInt16(r)); break;
                    case RecordDataType.UInt32: writer.Write(col.Data.ToUInt32(r)); break;
                    case RecordDataType.UInt64: writer.Write(col.Data.ToUInt64(r)); break;
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
            byte codeValue = reader.ReadByte();
            if (!Enum.IsDefined(typeof(RecordDataType), codeValue)) throw new InvalidDataException($"Invalid RecordDataType value: {codeValue}");
            RecordDataType code = (RecordDataType)codeValue;
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
                    case RecordDataType.Boolean: col.Data.Set(reader.ReadBoolean(), r); break;
                    case RecordDataType.Byte: col.Data.Set(reader.ReadByte(), r); break;
                    case RecordDataType.Char: col.Data.Set(reader.ReadChar(), r); break;
                    case RecordDataType.DateTime: col.Data.Set(reader.ReadInt64(), r); break;
                    case RecordDataType.Decimal: col.Data.Set(reader.ReadDecimal(), r); break;
                    case RecordDataType.Double: col.Data.Set(reader.ReadDouble(), r); break;
                    case RecordDataType.Int16: col.Data.Set(reader.ReadInt16(), r); break;
                    case RecordDataType.Int32: col.Data.Set(reader.ReadInt32(), r); break;
                    case RecordDataType.Int64: col.Data.Set(reader.ReadInt64(), r); break;
                    case RecordDataType.SByte: col.Data.Set(reader.ReadSByte(), r); break;
                    case RecordDataType.Single: col.Data.Set(reader.ReadSingle(), r); break;
                    case RecordDataType.String: col.Data.Set(reader.ReadString(), r); break;
                    case RecordDataType.UInt16: col.Data.Set(reader.ReadUInt16(), r); break;
                    case RecordDataType.UInt32: col.Data.Set(reader.ReadUInt32(), r); break;
                    case RecordDataType.UInt64: col.Data.Set(reader.ReadUInt64(), r); break;
                }
            }
        }
    }
}
