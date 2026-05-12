using System;
using System.IO;
using System.Text;

namespace LuYao.Data;

public partial class RecordTable
{
    // 二进制格式版本号，用于未来兼容性检查
    private const byte BinaryFormatVersion = 1;

    /// <summary>
    /// 将当前 <see cref="RecordTable"/> 写入二进制流。
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
    /// 将当前 <see cref="RecordTable"/> 写入 <see cref="BinaryWriter"/>。
    /// </summary>
    /// <param name="writer">目标写入器。</param>
    public void WriteTo(BinaryWriter writer)
    {
        if (writer == null) throw new ArgumentNullException(nameof(writer));

        // Header
        BinaryPayloadHeader.Write(writer, BinaryPayloadType.RecordTable);
        writer.Write(BinaryFormatVersion);
        writer.Write(this.Name ?? string.Empty);
        writer.Write(this.Page);
        writer.Write(this._pageSize);
        writer.Write(this._maxCount);

        // Schema: column count + (name, type, isNullable, arrayRank) per column
        writer.Write(this.Columns.Count);
        for (int c = 0; c < this.Columns.Count; c++)
        {
            var col = this.Columns[c];
            writer.Write(col.Name);
            writer.Write((byte)col.ColumnType);
            writer.Write(col.IsNullable);
            writer.Write((byte)col.ArrayRank);
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
    /// 从二进制流读取并填充当前 <see cref="RecordTable"/> 实例。
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
    /// 从 <see cref="BinaryReader"/> 读取并填充当前 <see cref="RecordTable"/> 实例。
    /// </summary>
    /// <param name="reader">源读取器。</param>
    public void ReadFrom(BinaryReader reader)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));

        // Header
        byte version = BinaryPayloadHeader.ReadHeaderAndVersion(reader, BinaryPayloadType.RecordTable);
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
            var columnType = (RecordColumnType)reader.ReadByte();
            bool isNullable = reader.ReadBoolean();
            int arrayRank = reader.ReadByte();
            Type clrType = Helpers.GetClrType(columnType, isNullable, arrayRank);
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
    /// 从二进制流创建新的 <see cref="RecordTable"/> 实例。
    /// </summary>
    /// <param name="stream">源流。</param>
    /// <returns>反序列化的 <see cref="RecordTable"/> 实例。</returns>
    public static RecordTable FromStream(Stream stream)
    {
        var table = new RecordTable();
        table.ReadFrom(stream);
        return table;
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
    /// <returns>反序列化的 <see cref="RecordTable"/> 实例。</returns>
    public static RecordTable FromBytes(byte[] data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        using var ms = new MemoryStream(data, writable: false);
        return FromStream(ms);
    }

    /// <summary>
    /// 检测二进制数据是否为带类型头的 <see cref="RecordTable"/>。
    /// </summary>
    /// <param name="data">二进制数据。</param>
    /// <returns>当数据包含 <see cref="RecordTable"/> 类型头时返回 true；否则返回 false。</returns>
    public static bool IsBinaryPayload(byte[] data)
    {
        if (data == null) return false;
        return BinaryPayloadHeader.TryGetPayloadType(data, out var payloadType)
            && payloadType == BinaryPayloadType.RecordTable;
    }

    #region 列数据序列化

    private static void WriteColumnData(BinaryWriter writer, RecordColumn col, int rowCount)
    {
        bool needsNullCheck = col.IsNullable || col.ColumnType == RecordColumnType.String || col.ArrayRank > 0;

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

            if (col.ArrayRank > 0)
            {
                WriteArrayValue(writer, (Array)val!, col.ColumnType);
            }
            else
            {
                WritePrimitiveValue(writer, val!, col.ColumnType);
            }
        }
    }

    private static void ReadColumnData(BinaryReader reader, RecordColumn col, int rowCount)
    {
        bool needsNullCheck = col.IsNullable || col.ColumnType == RecordColumnType.String || col.ArrayRank > 0;

        for (int r = 0; r < rowCount; r++)
        {
            if (needsNullCheck)
            {
                bool hasValue = reader.ReadBoolean();
                if (!hasValue) continue;
            }

            object val;
            if (col.ArrayRank > 0)
            {
                val = ReadArrayValue(reader, col.ColumnType, col.ArrayRank, col.IsNullable);
            }
            else
            {
                val = ReadPrimitiveValue(reader, col.ColumnType);
            }
            col.Set(r, val);
        }
    }

    private static void WriteArrayValue(BinaryWriter writer, Array array, RecordColumnType elementType)
    {
        int rank = array.Rank;
        writer.Write((byte)rank);

        // 写入各维度长度
        for (int i = 0; i < rank; i++)
        {
            writer.Write(array.GetLength(i));
        }

        // 写入元素（扁平化遍历）
        int totalElements = array.Length;
        int[] indices = new int[rank];

        for (int flatIndex = 0; flatIndex < totalElements; flatIndex++)
        {
            GetMultiDimensionalIndices(flatIndex, array, indices);
            var element = array.GetValue(indices);

            // 元素可能为 null（如 int?[] 或 string[]）
            if (element == null)
            {
                writer.Write(false);
                continue;
            }

            writer.Write(true);
            WritePrimitiveValue(writer, element, elementType);
        }
    }

    private static Array ReadArrayValue(BinaryReader reader, RecordColumnType elementType, int expectedRank, bool isElementNullable)
    {
        int rank = reader.ReadByte();
        if (rank != expectedRank)
        {
            throw new InvalidOperationException($"数组维度不匹配：期望 {expectedRank}，实际 {rank}");
        }

        // 读取各维度长度
        int[] lengths = new int[rank];
        int totalElements = 1;
        for (int i = 0; i < rank; i++)
        {
            lengths[i] = reader.ReadInt32();
            totalElements *= lengths[i];
        }

        // 创建数组（考虑元素是否可空）
        Type clrType = Helpers.GetClrType(elementType, isNullable: isElementNullable, arrayRank: 0);
        Array array = rank == 1 
            ? Array.CreateInstance(clrType, lengths[0])
            : Array.CreateInstance(clrType, lengths);

        // 读取元素
        int[] indices = new int[rank];
        for (int flatIndex = 0; flatIndex < totalElements; flatIndex++)
        {
            GetMultiDimensionalIndices(flatIndex, array, indices);

            bool hasValue = reader.ReadBoolean();
            if (!hasValue) continue;

            var element = ReadPrimitiveValue(reader, elementType);
            array.SetValue(element, indices);
        }

        return array;
    }

    private static void GetMultiDimensionalIndices(int flatIndex, Array array, int[] indices)
    {
        int rank = array.Rank;
        int remaining = flatIndex;

        for (int i = rank - 1; i >= 0; i--)
        {
            int length = array.GetLength(i);
            indices[i] = remaining % length;
            remaining /= length;
        }
    }

    private static void WritePrimitiveValue(BinaryWriter writer, object value, RecordColumnType columnType)
    {
        if (value.GetType().IsEnum)
        {
            value = Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
        }

        switch (columnType)
        {
            case RecordColumnType.Boolean: writer.Write((bool)value); break;
            case RecordColumnType.SByte: writer.Write((sbyte)value); break;
            case RecordColumnType.Int16: writer.Write((short)value); break;
            case RecordColumnType.Int32: writer.Write((int)value); break;
            case RecordColumnType.Int64: writer.Write((long)value); break;
            case RecordColumnType.Byte: writer.Write((byte)value); break;
            case RecordColumnType.UInt16: writer.Write((ushort)value); break;
            case RecordColumnType.UInt32: writer.Write((uint)value); break;
            case RecordColumnType.UInt64: writer.Write((ulong)value); break;
            case RecordColumnType.Single: writer.Write((float)value); break;
            case RecordColumnType.Double: writer.Write((double)value); break;
            case RecordColumnType.Decimal: writer.Write((decimal)value); break;
            case RecordColumnType.Char: writer.Write((char)value); break;
            case RecordColumnType.String: writer.Write((string)value); break;
            case RecordColumnType.DateTime: writer.Write(((DateTime)value).ToBinary()); break;
            case RecordColumnType.DateTimeOffset:
                var dto = (DateTimeOffset)value;
                writer.Write(dto.Ticks);
                writer.Write((short)dto.Offset.TotalMinutes);
                break;
            case RecordColumnType.TimeSpan: writer.Write(((TimeSpan)value).Ticks); break;
            case RecordColumnType.Guid: writer.Write(((Guid)value).ToByteArray()); break;
            default:
                throw new NotSupportedException($"不支持的列类型: {columnType}");
        }
    }

    private static object ReadPrimitiveValue(BinaryReader reader, RecordColumnType columnType)
    {
        switch (columnType)
        {
            case RecordColumnType.Boolean: return reader.ReadBoolean();
            case RecordColumnType.SByte: return reader.ReadSByte();
            case RecordColumnType.Int16: return reader.ReadInt16();
            case RecordColumnType.Int32: return reader.ReadInt32();
            case RecordColumnType.Int64: return reader.ReadInt64();
            case RecordColumnType.Byte: return reader.ReadByte();
            case RecordColumnType.UInt16: return reader.ReadUInt16();
            case RecordColumnType.UInt32: return reader.ReadUInt32();
            case RecordColumnType.UInt64: return reader.ReadUInt64();
            case RecordColumnType.Single: return reader.ReadSingle();
            case RecordColumnType.Double: return reader.ReadDouble();
            case RecordColumnType.Decimal: return reader.ReadDecimal();
            case RecordColumnType.Char: return reader.ReadChar();
            case RecordColumnType.String: return reader.ReadString();
            case RecordColumnType.DateTime: return DateTime.FromBinary(reader.ReadInt64());
            case RecordColumnType.DateTimeOffset:
                long ticks = reader.ReadInt64();
                short offsetMinutes = reader.ReadInt16();
                return new DateTimeOffset(ticks, TimeSpan.FromMinutes(offsetMinutes));
            case RecordColumnType.TimeSpan: return new TimeSpan(reader.ReadInt64());
            case RecordColumnType.Guid: return new Guid(reader.ReadBytes(16));
            default:
                throw new NotSupportedException($"不支持的列类型: {columnType}");
        }
    }

    #endregion
}
