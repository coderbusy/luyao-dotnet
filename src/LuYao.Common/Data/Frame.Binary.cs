using System;
using System.IO;
using System.Text;

namespace LuYao.Data;

public partial class Frame
{
    // 二进制格式版本号，用于未来兼容性检查
    private const byte BinaryFormatVersion = 1;

    /// <summary>
    /// 将当前 <see cref="Frame"/> 写入二进制流。
    /// </summary>
    /// <param name="stream">目标流。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="stream"/> 为 null 时抛出。</exception>
    public void WriteTo(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
        WriteTo(writer);
    }

    /// <summary>
    /// 将当前 <see cref="Frame"/> 写入 <see cref="BinaryWriter"/>。
    /// </summary>
    /// <param name="writer">目标写入器。</param>
    public void WriteTo(BinaryWriter writer)
    {
        if (writer == null) throw new ArgumentNullException(nameof(writer));

        // Header
        BinaryPayloadHeader.Write(writer, BinaryPayloadType.Frame);
        writer.Write(BinaryFormatVersion);
        writer.Write(this.Name ?? string.Empty);
        writer.Write(this.Page);
        writer.Write(this._pageSize);
        writer.Write(this._maxCount);

        // Schema: column count + (name, type, isNullable) per column
        writer.Write(this.Columns.Count);
        for (int c = 0; c < this.Columns.Count; c++)
        {
            var col = this.Columns[c];
            writer.Write(col.Name);
            writer.Write((byte)col.ColumnType);
            writer.Write(col.IsNullable);
        }

        // Row count
        int rowCount = this.Count;
        writer.Write(rowCount);

        // Column data: each column writes its data array sequentially
        for (int c = 0; c < this.Columns.Count; c++)
        {
            WriteColumnData(writer, this.Columns[c], rowCount);
        }
    }

    /// <summary>
    /// 从二进制流读取并填充当前 <see cref="Frame"/> 实例。
    /// </summary>
    /// <param name="stream">源流。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="stream"/> 为 null 时抛出。</exception>
    public void ReadFrom(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
        ReadFrom(reader);
    }

    /// <summary>
    /// 从 <see cref="BinaryReader"/> 读取并填充当前 <see cref="Frame"/> 实例。
    /// </summary>
    /// <param name="reader">源读取器。</param>
    public void ReadFrom(BinaryReader reader)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));

        // Header
        byte version = BinaryPayloadHeader.ReadHeaderAndVersion(reader, BinaryPayloadType.Frame);
        if (version != BinaryFormatVersion)
            throw new InvalidOperationException($"不支持的二进制格式版本: {version}");

        this.Name = reader.ReadString();
        this.Page = reader.ReadInt32();
        this._pageSize = reader.ReadInt32();
        this._maxCount = reader.ReadInt32();

        // Schema
        this.Columns.Clear();
        int colCount = reader.ReadInt32();
        for (int c = 0; c < colCount; c++)
        {
            string name = reader.ReadString();
            var columnType = (FrameColumnType)reader.ReadByte();
            bool isNullable = reader.ReadBoolean();
            Type clrType = Helpers.GetClrType(columnType, isNullable);
            this.Columns.Add(name, clrType);
        }

        // Row count
        int rowCount = reader.ReadInt32();

        // 预分配行
        for (int r = 0; r < rowCount; r++)
        {
            this.AddRow();
        }

        // Column data
        for (int c = 0; c < colCount; c++)
        {
            ReadColumnData(reader, this.Columns[c], rowCount);
        }
    }

    /// <summary>
    /// 从二进制流创建新的 <see cref="Frame"/> 实例。
    /// </summary>
    /// <param name="stream">源流。</param>
    /// <returns>反序列化的 <see cref="Frame"/> 实例。</returns>
    public static Frame FromStream(Stream stream)
    {
        var record = new Frame();
        record.ReadFrom(stream);
        return record;
    }

    /// <summary>
    /// 序列化为字节数组。
    /// </summary>
    /// <returns>二进制表示。</returns>
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
    /// <returns>反序列化的 <see cref="Frame"/> 实例。</returns>
    public static Frame FromBytes(byte[] data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        using var ms = new MemoryStream(data, writable: false);
        return FromStream(ms);
    }

    /// <summary>
    /// 检测二进制数据是否为带类型头的 <see cref="Frame"/>。
    /// </summary>
    /// <param name="data">二进制数据。</param>
    /// <returns>当数据包含 <see cref="Frame"/> 类型头时返回 true；否则返回 false。</returns>
    public static bool IsBinaryPayload(byte[] data)
    {
        if (data == null) return false;
        return BinaryPayloadHeader.TryGetPayloadType(data, out var payloadType)
            && payloadType == BinaryPayloadType.Frame;
    }

    #region 列数据序列化

    private static void WriteColumnData(BinaryWriter writer, FrameColumn col, int rowCount)
    {
        bool needsNullCheck = col.IsNullable || col.ColumnType == FrameColumnType.String || col.ColumnType == FrameColumnType.ByteArray;

        for (int r = 0; r < rowCount; r++)
        {
            var val = col.Get(r);
            if (needsNullCheck)
            {
                if (val is null)
                {
                    writer.Write(false);
                    continue;
                }
                writer.Write(true);
            }
            WritePrimitiveValue(writer, val!, col.ColumnType);
        }
    }

    private static void ReadColumnData(BinaryReader reader, FrameColumn col, int rowCount)
    {
        bool needsNullCheck = col.IsNullable || col.ColumnType == FrameColumnType.String || col.ColumnType == FrameColumnType.ByteArray;

        for (int r = 0; r < rowCount; r++)
        {
            if (needsNullCheck)
            {
                bool hasValue = reader.ReadBoolean();
                if (!hasValue) continue;
            }
            var val = ReadPrimitiveValue(reader, col.ColumnType);
            col.Set(r, val);
        }
    }

    private static void WritePrimitiveValue(BinaryWriter writer, object value, FrameColumnType columnType)
    {
        if (value.GetType().IsEnum)
        {
            value = Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
        }

        switch (columnType)
        {
            case FrameColumnType.Boolean: writer.Write((bool)value); break;
            case FrameColumnType.SByte: writer.Write((sbyte)value); break;
            case FrameColumnType.Int16: writer.Write((short)value); break;
            case FrameColumnType.Int32: writer.Write((int)value); break;
            case FrameColumnType.Int64: writer.Write((long)value); break;
            case FrameColumnType.Byte: writer.Write((byte)value); break;
            case FrameColumnType.UInt16: writer.Write((ushort)value); break;
            case FrameColumnType.UInt32: writer.Write((uint)value); break;
            case FrameColumnType.UInt64: writer.Write((ulong)value); break;
            case FrameColumnType.Single: writer.Write((float)value); break;
            case FrameColumnType.Double: writer.Write((double)value); break;
            case FrameColumnType.Decimal: writer.Write((decimal)value); break;
            case FrameColumnType.Char: writer.Write((char)value); break;
            case FrameColumnType.String: writer.Write((string)value); break;
            case FrameColumnType.DateTime: writer.Write(((DateTime)value).ToBinary()); break;
            case FrameColumnType.DateTimeOffset:
                var dto = (DateTimeOffset)value;
                writer.Write(dto.Ticks);
                writer.Write((short)dto.Offset.TotalMinutes);
                break;
            case FrameColumnType.TimeSpan: writer.Write(((TimeSpan)value).Ticks); break;
            case FrameColumnType.Guid: writer.Write(((Guid)value).ToByteArray()); break;
            case FrameColumnType.ByteArray:
                var bytes = (byte[])value;
                writer.Write(bytes.Length);
                writer.Write(bytes);
                break;
            default:
                throw new NotSupportedException($"不支持的列类型: {columnType}");
        }
    }

    private static object ReadPrimitiveValue(BinaryReader reader, FrameColumnType columnType)
    {
        switch (columnType)
        {
            case FrameColumnType.Boolean: return reader.ReadBoolean();
            case FrameColumnType.SByte: return reader.ReadSByte();
            case FrameColumnType.Int16: return reader.ReadInt16();
            case FrameColumnType.Int32: return reader.ReadInt32();
            case FrameColumnType.Int64: return reader.ReadInt64();
            case FrameColumnType.Byte: return reader.ReadByte();
            case FrameColumnType.UInt16: return reader.ReadUInt16();
            case FrameColumnType.UInt32: return reader.ReadUInt32();
            case FrameColumnType.UInt64: return reader.ReadUInt64();
            case FrameColumnType.Single: return reader.ReadSingle();
            case FrameColumnType.Double: return reader.ReadDouble();
            case FrameColumnType.Decimal: return reader.ReadDecimal();
            case FrameColumnType.Char: return reader.ReadChar();
            case FrameColumnType.String: return reader.ReadString();
            case FrameColumnType.DateTime: return DateTime.FromBinary(reader.ReadInt64());
            case FrameColumnType.DateTimeOffset:
                long ticks = reader.ReadInt64();
                short offsetMinutes = reader.ReadInt16();
                return new DateTimeOffset(ticks, TimeSpan.FromMinutes(offsetMinutes));
            case FrameColumnType.TimeSpan: return new TimeSpan(reader.ReadInt64());
            case FrameColumnType.Guid: return new Guid(reader.ReadBytes(16));
            case FrameColumnType.ByteArray:
                int len = reader.ReadInt32();
                return reader.ReadBytes(len);
            default:
                throw new NotSupportedException($"不支持的列类型: {columnType}");
        }
    }

    #endregion
}
