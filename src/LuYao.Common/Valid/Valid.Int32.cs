namespace LuYao;

partial class Valid
{
    /// <summary>
    /// 转换字符串为 Int32。
    /// </summary>
    public static int ToInt32(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;
        if (int.TryParse(value, out int result)) return result;
        return 0;
    }
}
