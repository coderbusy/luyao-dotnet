using LuYao.Data.Binary;
using System;
using System.IO;

namespace LuYao.Data.Formatters;

/// <summary>
/// 提供 Record 对象的二进制序列化和反序列化功能。
/// 支持将 Record 数据写入二进制流和从二进制流读取 Record 数据。
/// </summary>
public class BinaryRecordFormatter
{
    /// <summary>
    /// 将 Record 对象写入到二进制流中。
    /// 按照头部信息、列信息、行数据的顺序进行序列化。
    /// </summary>
    /// <param name="re">要写入的 Record 对象。</param>
    /// <param name="writer">用于写入二进制数据的 BinaryWriter 实例。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="re"/> 或 <paramref name="writer"/> 为 null 时抛出。</exception>
    public void Write(Record re, BinaryWriter writer)
    {
        if (re == null) throw new ArgumentNullException(nameof(re));
        if (writer == null) throw new ArgumentNullException(nameof(writer));
        //header
        var header = new BinaryRecordHeader(re);
        header.Write(writer);
        //columns
        for (int i = 0; i < re.Columns.Count; i++)
        {
            RecordColumn col = re.Columns[i];
            var w = new BinaryRecordColumnInfo(col);
            w.Write(writer);
        }
        //rows
        re.MoveFirst();
        while (re.Read())
        {
            for (int c = 0; c < re.Columns.Count; c++)
            {
                RecordColumn col = re.Columns[c];
                switch (col.Code)
                {
                    case RecordDataCode.Boolean: writer.Write(col.GetBoolean()); break;
                    case RecordDataCode.Byte: writer.Write(col.GetByte()); break;
                    case RecordDataCode.Char: writer.Write(col.GetChar()); break;
                    case RecordDataCode.DateTime: writer.Write(col.GetDateTime().ToBinary()); break;
                    case RecordDataCode.Decimal: writer.Write(col.GetDecimal()); break;
                    case RecordDataCode.Double: writer.Write(col.GetDouble()); break;
                    case RecordDataCode.Int16: writer.Write(col.GetInt16()); break;
                    case RecordDataCode.Int32: writer.Write(col.GetInt32()); break;
                    case RecordDataCode.Int64: writer.Write(col.GetInt64()); break;
                    case RecordDataCode.SByte: writer.Write(col.GetSByte()); break;
                    case RecordDataCode.Single: writer.Write(col.GetSingle()); break;
                    case RecordDataCode.String: writer.Write(col.GetString()); break;
                    case RecordDataCode.UInt16: writer.Write(col.GetUInt16()); break;
                    case RecordDataCode.UInt32: writer.Write(col.GetUInt32()); break;
                    case RecordDataCode.UInt64: writer.Write(col.GetUInt64()); break;
                    default:
                        {
                            object? v = col.GetValue(re.Cursor);
                            byte[] data = Arrays.Empty<byte>();
                            if (v != null) data = Serialize(v, col.Type);
                            int len = data?.Length ?? 0;
                            writer.Write(len);
                            if (len > 0) writer.Write(data!);
                        }
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 从二进制流中读取数据并构造 Record 对象。
    /// 按照头部信息、列信息、行数据的顺序进行反序列化。
    /// </summary>
    /// <param name="reader">用于读取二进制数据的 BinaryReader 实例。</param>
    /// <returns>从二进制流中反序列化得到的 Record 对象。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="reader"/> 为 null 时抛出。</exception>
    public Record Read(BinaryReader reader)
    {
        var re = new Record();
        //header
        var header = new BinaryRecordHeader();
        header.Read(reader);
        if (!string.IsNullOrWhiteSpace(header.Name)) re.Name = header.Name;
        //columns
        for (int i = 0; i < header.Columns; i++)
        {
            var col = new BinaryRecordColumnInfo();
            col.Read(reader);
            re.Columns.Add(col.Name, col.Type);
        }
        //rows
        for (int i = 0; i < header.Count; i++)
        {
            RecordRow row = re.AddRow();
            for (int c = 0; c < re.Columns.Count; c++)
            {
                RecordColumn col = re.Columns[c];
                switch (col.Code)
                {
                    case RecordDataCode.Boolean: col.Set(reader.ReadBoolean()); break;
                    case RecordDataCode.Byte: col.Set(reader.ReadByte()); break;
                    case RecordDataCode.Char: col.Set(reader.ReadChar()); break;
                    case RecordDataCode.DateTime: col.Set(DateTime.FromBinary(reader.ReadInt64())); break;
                    case RecordDataCode.Decimal: col.Set(reader.ReadDecimal()); break;
                    case RecordDataCode.Double: col.Set(reader.ReadDouble()); break;
                    case RecordDataCode.Int16: col.Set(reader.ReadInt16()); break;
                    case RecordDataCode.Int32: col.Set(reader.ReadInt32()); break;
                    case RecordDataCode.Int64: col.Set(reader.ReadInt64()); break;
                    case RecordDataCode.SByte: col.Set(reader.ReadSByte()); break;
                    case RecordDataCode.Single: col.Set(reader.ReadSingle()); break;
                    case RecordDataCode.String: col.Set(reader.ReadString()); break;
                    case RecordDataCode.UInt16: col.Set(reader.ReadUInt16()); break;
                    case RecordDataCode.UInt32: col.Set(reader.ReadUInt32()); break;
                    case RecordDataCode.UInt64: col.Set(reader.ReadUInt64()); break;
                    default:
                        int len = reader.ReadInt32();
                        if (len > 0)
                        {
                            var bytes = reader.ReadBytes(len);
                            object value = Deserialize(bytes, col.Type);
                            col.SetValue(value, row);
                        }
                        break;
                }
            }
        }
        return re;
    }

    /// <summary>
    /// 将指定类型的对象序列化为字节数组。
    /// 此方法为虚方法，派生类可重写以支持复杂类型的序列化。
    /// </summary>
    /// <param name="value">要序列化的对象值，可以为 null。</param>
    /// <param name="type">对象的类型信息。</param>
    /// <returns>序列化后的字节数组。</returns>
    /// <exception cref="NotImplementedException">基类实现总是抛出此异常，提示需要派生类实现。</exception>
    public virtual byte[] Serialize(object value, Type type)
    {
        if (value == null) return Arrays.Empty<byte>();
        dynamic n = value;
        if (type == typeof(Guid)) return n.ToByteArray();
        throw new NotImplementedException("复杂类型的二进制读写暂不支持。请派生 RecordBinaryFormatter 类型并实现 Serialize 方法。");
    }

    /// <summary>
    /// 将字节数组反序列化为指定类型的对象。
    /// 此方法为虚方法，派生类可重写以支持复杂类型的反序列化。
    /// </summary>
    /// <param name="data">包含序列化数据的字节数组。</param>
    /// <param name="type">要反序列化到的目标类型。</param>
    /// <returns>反序列化后的对象实例。</returns>
    /// <exception cref="NotImplementedException">基类实现总是抛出此异常，提示需要派生类实现。</exception>
    public virtual object Deserialize(byte[] data, Type type)
    {
        if (type == typeof(Guid)) return new Guid(data);
        throw new NotImplementedException("复杂类型的二进制读写暂不支持。请派生 RecordBinaryFormatter 类型并实现 Deserialize 方法。");
    }
}
