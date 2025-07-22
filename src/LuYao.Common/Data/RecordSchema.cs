using LuYao.Text;
using System;

namespace LuYao.Data;

/// <summary>
/// 表示记录的模式信息。
/// </summary>
public class RecordSchema
{
    /// <summary>
    /// 初始化 <see cref="RecordSchema"/> 类的新实例。
    /// </summary>
    public RecordSchema()
    {

    }

    /// <summary>
    /// 使用指定的记录初始化 <see cref="RecordSchema"/> 类的新实例。
    /// </summary>
    /// <param name="re">记录对象。</param>
    public RecordSchema(Record re)
    {
        if (re == null) throw new ArgumentNullException(nameof(re));
        this.Name = re.Name;
        this.Columns = re.Columns.Count;
        this.Count = re.Count;
    }

    /// <summary>
    /// 获取或设置版本号。
    /// </summary>
    public byte Version { get; set; } = 1;

    /// <summary>
    /// 获取或设置记录的名称。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置列的数量。
    /// </summary>
    public int Columns { get; set; }

    /// <summary>
    /// 获取或设置记录的数量。
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 基于分隔符的字符串转换器，用于序列化和反序列化 <see cref="RecordSchema"/> 对象。
    /// </summary>
    static DelimiterBasedStringConverter<RecordSchema> Converter { get; } = new DelimiterBasedStringConverter<RecordSchema>()
        .Add(h => h.Version)
        .Add(h => h.Name)
        .Add(h => h.Columns)
        .Add(h => h.Count);

    /// <summary>
    /// 将当前对象序列化为字符串。
    /// </summary>
    /// <returns>序列化后的字符串。</returns>
    public string SerializeToString() => Converter.Serialize(this);

    /// <summary>
    /// 从字符串反序列化为 <see cref="RecordSchema"/> 对象。
    /// </summary>
    /// <param name="value">序列化的字符串。</param>
    /// <returns>反序列化后的 <see cref="RecordSchema"/> 对象。</returns>
    public static RecordSchema FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return new RecordSchema();
        return Converter.Deserialize(value);
    }
}
