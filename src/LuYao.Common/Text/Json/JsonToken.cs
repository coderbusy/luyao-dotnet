namespace LuYao.Text.Json;

/// <summary>
/// JSON 标记类型枚举
/// </summary>
public enum JsonTokenType
{
    /// <summary>
    /// 无效或未初始化的标记
    /// </summary>
    None = 0,

    /// <summary>
    /// 对象开始 {
    /// </summary>
    StartObject = 1,

    /// <summary>
    /// 对象结束 }
    /// </summary>
    EndObject = 2,

    /// <summary>
    /// 数组开始 [
    /// </summary>
    StartArray = 3,

    /// <summary>
    /// 数组结束 ]
    /// </summary>
    EndArray = 4,

    /// <summary>
    /// 属性名
    /// </summary>
    PropertyName = 5,

    /// <summary>
    /// 字符串值
    /// </summary>
    String = 6,

    /// <summary>
    /// 数字值
    /// </summary>
    Number = 7,

    /// <summary>
    /// 布尔值
    /// </summary>
    Boolean = 8,

    /// <summary>
    /// null 值
    /// </summary>
    Null = 9,

    /// <summary>
    /// 注释
    /// </summary>
    Comment = 10,

    /// <summary>
    /// 原始 JSON
    /// </summary>
    Raw = 11
}

/// <summary>
/// JSON 标记结构
/// </summary>
public readonly struct JsonToken
{
    public JsonToken(JsonTokenType type, object? value = null, int startIndex = 0, int length = 0)
    {
        Type = type;
        Value = value;
        StartIndex = startIndex;
        Length = length;
    }

    /// <summary>
    /// 标记类型
    /// </summary>
    public JsonTokenType Type { get; }

    /// <summary>
    /// 标记值
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// 在源字符串中的起始位置
    /// </summary>
    public int StartIndex { get; }

    /// <summary>
    /// 标记长度
    /// </summary>
    public int Length { get; }
}