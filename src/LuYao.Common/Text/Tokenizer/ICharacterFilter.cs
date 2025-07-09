namespace LuYao.Text.Tokenizer;

/// <summary>
/// 字符过滤器接口，用于对输入文本进行字符级别的预处理。
/// </summary>
public interface ICharacterFilter
{
    /// <summary>
    /// 对输入文本进行过滤处理，返回处理后的文本。
    /// </summary>
    /// <param name="text">要过滤的文本。</param>
    /// <returns>过滤后的文本。</returns>
    string Filter(string text);
}
