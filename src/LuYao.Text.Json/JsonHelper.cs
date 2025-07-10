using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Text.Json;

/// <summary>
/// 提供用于处理 JSON 数据的辅助方法。
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// 从输入字符串中提取 JSON 数据。
    /// </summary>
    /// <param name="str">包含 JSON 数据的输入字符串。</param>
    /// <param name="formatting">指定 JSON 的格式化方式，默认为 <see cref="Formatting.None"/>。</param>
    /// <returns>提取的 JSON 数据字符串。</returns>
    public static string ExtractJson(string str, Formatting formatting = Formatting.None)
    {
        var level = 0;
        var total = 0;
        var sb = new StringBuilder();
        using (var sr = new StringReader(str))
        using (var reader = new JsonTextReader(sr))
        using (var sw = new StringWriter(sb))
        using (var writer = new JsonTextWriter(sw) { Formatting = formatting })
        {
            while (reader.Read())
            {
                total++;
                var type = reader.TokenType;
                switch (type)
                {
                    case JsonToken.StartArray:
                    case JsonToken.StartObject:
                    case JsonToken.StartConstructor:
                        level++;
                        break;
                    case JsonToken.EndArray:
                    case JsonToken.EndObject:
                    case JsonToken.EndConstructor:
                        level--;
                        break;
                }

                switch (type)
                {
                    case JsonToken.None: writer.WriteNull(); break;
                    case JsonToken.StartObject: writer.WriteStartObject(); break;
                    case JsonToken.StartArray: writer.WriteStartArray(); break;
                    case JsonToken.StartConstructor: writer.WriteStartConstructor(reader.Value?.ToString() ?? string.Empty); break;
                    case JsonToken.PropertyName: writer.WritePropertyName(reader.Value?.ToString() ?? string.Empty); break;
                    case JsonToken.Comment: writer.WriteComment(reader.Value?.ToString() ?? string.Empty); break;
                    case JsonToken.Raw: writer.WriteRaw(reader.Value?.ToString() ?? string.Empty); break;
                    case JsonToken.Integer: writer.WriteValue(reader.Value); break;
                    case JsonToken.Float: writer.WriteValue(reader.Value); break;
                    case JsonToken.String: writer.WriteValue(reader.Value); break;
                    case JsonToken.Boolean: writer.WriteValue(reader.Value); break;
                    case JsonToken.Null: writer.WriteNull(); break;
                    case JsonToken.Undefined: writer.WriteUndefined(); break;
                    case JsonToken.EndObject: writer.WriteEndObject(); break;
                    case JsonToken.EndArray: writer.WriteEndArray(); break;
                    case JsonToken.EndConstructor: writer.WriteEndConstructor(); break;
                    case JsonToken.Date: writer.WriteValue(reader.Value); break;
                    case JsonToken.Bytes: writer.WriteValue(reader.Value); break;
                }
                if (total > 0 && level == 0) break;
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// 从输入字符串中提取指定名称的值。
    /// </summary>
    /// <param name="str">包含 JSON 数据的输入字符串。</param>
    /// <param name="names">需要提取的属性名称列表。</param>
    /// <returns>包含提取值的 <see cref="NameValueCollection"/>。</returns>
    public static NameValueCollection ExtractValueByNames(string str, params string[] names)
    {
        var nv = new NameValueCollection();
        var ss = new Stack<string>();
        using (var sr = new StringReader(str))
        using (var reader = new JsonTextReader(sr))
        {
            while (reader.Read())
            {
                var type = reader.TokenType;
                switch (type)
                {
                    case JsonToken.StartArray:
                    case JsonToken.StartObject:
                    case JsonToken.StartConstructor:
                        ss.Push(string.Empty);
                        break;
                    case JsonToken.EndArray:
                    case JsonToken.EndObject:
                    case JsonToken.EndConstructor:
                        ss.Pop();
                        break;
                    case JsonToken.PropertyName:
                        if (ss.Count > 0) ss.Pop();
                        ss.Push(reader.Value?.ToString() ?? string.Empty);
                        break;
                }

                var isValue = false;

                switch (type)
                {
                    case JsonToken.Raw: isValue = true; break;
                    case JsonToken.Integer: isValue = true; break;
                    case JsonToken.Float: isValue = true; break;
                    case JsonToken.String: isValue = true; break;
                    case JsonToken.Boolean: isValue = true; break;
                }

                if (isValue && ss.Count > 0)
                {
                    var name = ss.Peek();
                    if (Array.IndexOf(names, name) != -1)
                    {
                        nv.Add(name, reader.Value?.ToString() ?? String.Empty);
                    }
                }
            }
        }
        return nv;
    }
}
