namespace LuYao.Text.Json;

/// <summary>
/// JSON �������ö��
/// </summary>
public enum JsonTokenType
{
    /// <summary>
    /// ��Ч��δ��ʼ���ı��
    /// </summary>
    None = 0,

    /// <summary>
    /// ����ʼ {
    /// </summary>
    StartObject = 1,

    /// <summary>
    /// ������� }
    /// </summary>
    EndObject = 2,

    /// <summary>
    /// ���鿪ʼ [
    /// </summary>
    StartArray = 3,

    /// <summary>
    /// ������� ]
    /// </summary>
    EndArray = 4,

    /// <summary>
    /// ������
    /// </summary>
    PropertyName = 5,

    /// <summary>
    /// �ַ���ֵ
    /// </summary>
    String = 6,

    /// <summary>
    /// ����ֵ
    /// </summary>
    Number = 7,

    /// <summary>
    /// ����ֵ
    /// </summary>
    Boolean = 8,

    /// <summary>
    /// null ֵ
    /// </summary>
    Null = 9,

    /// <summary>
    /// ע��
    /// </summary>
    Comment = 10,

    /// <summary>
    /// ԭʼ JSON
    /// </summary>
    Raw = 11
}

/// <summary>
/// JSON ��ǽṹ
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
    /// �������
    /// </summary>
    public JsonTokenType Type { get; }

    /// <summary>
    /// ���ֵ
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// ��Դ�ַ����е���ʼλ��
    /// </summary>
    public int StartIndex { get; }

    /// <summary>
    /// ��ǳ���
    /// </summary>
    public int Length { get; }
}