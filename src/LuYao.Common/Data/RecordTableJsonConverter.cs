#if !NET45 && !NET461
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LuYao.Data;

/// <summary>
/// 将 <see cref="RecordTable"/> 以 Base64 二进制格式进行 JSON 序列化/反序列化。
/// </summary>
public class RecordTableJsonConverter : JsonConverter<RecordTable>
{
    /// <inheritdoc/>
    public override RecordTable? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        if (reader.TokenType == JsonTokenType.String)
        {
            // 在分配字符串/字节数组前先检查 Base64 长度上限，防御超大输入引发的 OOM。
            long base64Length = reader.HasValueSequence ? reader.ValueSequence.Length : reader.ValueSpan.Length;
            if (base64Length > RecordBinaryPayloadHelper.MaxBase64Length)
                throw new JsonException($"RecordTable Base64 payload exceeds maximum allowed length {RecordBinaryPayloadHelper.MaxBase64Length}.");

            var base64 = reader.GetString()!;
            try
            {
                var bytes = Convert.FromBase64String(base64);
                bytes = RecordBinaryPayloadHelper.Decode(bytes);
                using var ms = new MemoryStream(bytes, writable: false);
                using var br = new BinaryReader(ms, Encoding.UTF8, leaveOpen: true);
                var record = new RecordTable();
                record.ReadFrom(br);
                return record;
            }
            catch (FormatException ex)
            {
                throw new JsonException($"Failed to read RecordTable from Base64 JSON string. TokenType={reader.TokenType}, Base64Length={base64.Length}.", ex);
            }
            catch (EndOfStreamException ex)
            {
                throw new JsonException($"Failed to read RecordTable from binary payload. TokenType={reader.TokenType}, Base64Length={base64.Length}.", ex);
            }
            catch (InvalidDataException ex)
            {
                throw new JsonException($"Failed to read RecordTable from binary payload. TokenType={reader.TokenType}, Base64Length={base64.Length}.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new JsonException($"Failed to read RecordTable from binary payload. TokenType={reader.TokenType}, Base64Length={base64.Length}.", ex);
            }
        }
        throw new JsonException($"Unexpected token type {reader.TokenType} for RecordTable.");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, RecordTable value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true);
        value.WriteTo(bw);
        bw.Flush();

        string base64;
        if (ms.TryGetBuffer(out ArraySegment<byte> buffer) && buffer.Array != null)
            base64 = Convert.ToBase64String(RecordBinaryPayloadHelper.Encode(buffer.Array, buffer.Offset, (int)ms.Length));
        else
            base64 = Convert.ToBase64String(RecordBinaryPayloadHelper.Encode(ms.ToArray()));
        writer.WriteStringValue(base64);
    }
}
#endif
