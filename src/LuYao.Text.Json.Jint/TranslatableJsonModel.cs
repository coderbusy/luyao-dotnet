using Jint;
using Jint.Native;
using Jint.Native.Array;
using Jint.Native.Object;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace LuYao.Text.Json;

/// <summary>
/// 一个抽象类，用于支持 JSON 数据的可翻译模型。
/// </summary>
/// <typeparam name="T">继承自 TranslatableJsonModel 的具体类型。</typeparam>
public abstract class TranslatableJsonModel<T> where T : TranslatableJsonModel<T>
{
    /// <summary>
    /// 将 JSON 字符串转换为模型对象。
    /// </summary>
    /// <param name="json">JSON 格式的字符串。</param>
    /// <returns>转换后的模型对象。</returns>
    public static T Transform(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new System.ArgumentException(nameof(json), "JSON string cannot be null or empty.");
        using var e = new Engine();
        if (!string.IsNullOrWhiteSpace(json))
        {
            JsonConvert.DeserializeObject(json);
            e.Evaluate($"var model = {json};");
        }

        var result = BuildOutput(e.Evaluate(TranslatableHelper.Get<T>()));
        return JsonConvert.DeserializeObject<T>(result)!;
    }

    /// <summary>
    /// 将对象转换为模型对象。
    /// </summary>
    /// <param name="model">要转换的对象。</param>
    /// <returns>转换后的模型对象。</returns>
    public static T Transform(object model)
    {
        if (model == null) throw new System.ArgumentNullException(nameof(model), "Model cannot be null.");
        using var e = new Engine();
        if (model != null) e.SetValue("model", model);
        var result = BuildOutput(e.Evaluate(TranslatableHelper.Get<T>()));
        return JsonConvert.DeserializeObject<T>(result)!;
    }

    private static string BuildOutput(JsValue value)
    {
        if (value.IsObject())
        {
            var sb = new StringBuilder();
            using var sw = new StringWriter(sb);
            var w = new JsonTextWriter(sw)
            {
                Formatting = Formatting.Indented
            };
            Write(value, w);
            return sb.ToString();
        }

        return value.ToString();
    }

    private static void Write(JsValue value, JsonWriter w)
    {
        switch (value)
        {
            case ArrayInstance array:
                w.WriteStartArray();

                foreach (var item in array) Write(item, w);

                w.WriteEndArray();
                break;
            case JsDate date: w.WriteValue(date.ToDateTime()); break;
            case JsNumber number: w.WriteRawValue(number.ToString()); break;
            case JsBigInt bigInt: w.WriteValue(bigInt.ToObject()); break;
            case JsBoolean boolean: w.WriteValue(boolean.ToObject()); break;
            case ObjectInstance instance:
                w.WriteStartObject();

                foreach (var item in instance.GetOwnProperties())
                {
                    w.WritePropertyName(item.Key.ToString());
                    Write(item.Value.Value, w);
                }

                w.WriteEndObject();
                break;
            case JsString str: w.WriteValue(str.ToString()); break;
            default: w.WriteRawValue(value.ToString()); break;
        }
    }
}