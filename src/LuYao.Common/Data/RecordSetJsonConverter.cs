#if !NET45 && !NET461
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LuYao.Data;

/// <summary>
/// 将 <see cref="RecordSet"/> 以 Base64 二进制格式进行 JSON 序列化/反序列化。
/// </summary>
public class RecordSetJsonConverter : JsonConverter<RecordSet>
{
    /// <inheritdoc/>
    public override RecordSet? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        if (reader.TokenType == JsonTokenType.String)
        {
            // 在分配字符串/字节数组前先检查 Base64 长度上限，防御超大输入引发的 OOM。
            long base64Length = reader.HasValueSequence ? reader.ValueSequence.Length : reader.ValueSpan.Length;
            if (base64Length > RecordBinaryPayloadHelper.MaxBase64Length)
                throw new JsonException($"RecordSet Base64 payload exceeds maximum allowed length {RecordBinaryPayloadHelper.MaxBase64Length}.");

            var base64 = reader.GetString()!;
            try
            {
                var bytes = Convert.FromBase64String(base64);
                bytes = RecordBinaryPayloadHelper.Decode(bytes);
                return RecordSet.FromBytes(bytes);
            }
            catch (FormatException ex)
            {
                throw new JsonException($"Failed to read RecordSet from Base64 JSON string. TokenType={reader.TokenType}, Base64Length={base64.Length}.", ex);
            }
            catch (EndOfStreamException ex)
            {
                throw new JsonException($"Failed to read RecordSet from binary payload. TokenType={reader.TokenType}, Base64Length={base64.Length}.", ex);
            }
            catch (InvalidDataException ex)
            {
                throw new JsonException($"Failed to read RecordSet from binary payload. TokenType={reader.TokenType}, Base64Length={base64.Length}.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new JsonException($"Failed to read RecordSet from binary payload. TokenType={reader.TokenType}, Base64Length={base64.Length}.", ex);
            }
        }
        throw new JsonException($"Unexpected token type {reader.TokenType} for RecordSet.");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, RecordSet value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }
        var bytes = RecordBinaryPayloadHelper.Encode(value.ToBytes());
        writer.WriteStringValue(Convert.ToBase64String(bytes));
    }
}
#endif
