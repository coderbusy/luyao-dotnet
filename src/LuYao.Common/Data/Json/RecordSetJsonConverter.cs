#if !NET45 && !NET461
using LuYao.Data.Binary;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LuYao.Data.Json;

/// <summary>
/// 将 <see cref="RecordSet"/> 以 Base64 二进制格式进行 JSON 序列化/反序列化。
/// </summary>
public class RecordSetJsonConverter : JsonConverter<RecordSet>
{
    private readonly RecordBinaryPayloadCodec _codec;

    /// <summary>使用默认编解码器（GZip 压缩）创建转换器。</summary>
    public RecordSetJsonConverter() : this(RecordBinaryPayloadCodec.Default) { }

    /// <summary>使用指定编解码器创建转换器。</summary>
    /// <param name="codec">编解码器实例。</param>
    public RecordSetJsonConverter(RecordBinaryPayloadCodec codec)
    {
        if (codec == null) throw new ArgumentNullException(nameof(codec));
        _codec = codec;
    }

    /// <inheritdoc/>
    public override RecordSet? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        if (reader.TokenType == JsonTokenType.String)
        {
            // 在分配字符串/字节数组前先检查 Base64 长度上限，防御超大输入引发的 OOM。
            long base64Length = reader.HasValueSequence ? reader.ValueSequence.Length : reader.ValueSpan.Length;
            if (base64Length > _codec.MaxBase64Length)
                throw new JsonException($"RecordSet Base64 payload exceeds maximum allowed length {_codec.MaxBase64Length}.");

            var base64 = reader.GetString()!;
            try
            {
                var bytes = Convert.FromBase64String(base64);
                bytes = _codec.Decode(bytes);
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
        var bytes = _codec.Encode(value.ToBytes());
        writer.WriteStringValue(Convert.ToBase64String(bytes));
    }
}
#endif
