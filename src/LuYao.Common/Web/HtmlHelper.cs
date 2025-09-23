using System.Text.RegularExpressions;

namespace LuYao.Web;

/// <summary>
/// 提供 HTML 相关的辅助方法
/// </summary>
public static class HtmlHelper
{
    /// <summary>
    /// 用于检测各种 HTML 元素的正则表达式模式集合
    /// </summary>
    private static readonly Regex[] HtmlPatterns = new[]
    {
        // 开始标签：匹配如 <div>, <span class="test">, <img src="test.jpg" /> 等
        new Regex(@"<\s*[a-zA-Z][a-zA-Z0-9]*\b[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        
        // 结束标签：匹配如 </div>, </span>, </ div > 等
        new Regex(@"<\s*/\s*[a-zA-Z][a-zA-Z0-9]*\s*>", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        
        // DOCTYPE 声明：匹配如 <!DOCTYPE html>, <!doctype HTML> 等
        new Regex(@"<!DOCTYPE\s+html>", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        
        // HTML 注释：匹配如 <!-- 注释内容 -->, 支持多行注释
        new Regex(@"<!--.*?-->", RegexOptions.Compiled | RegexOptions.Singleline),
        
        // HTML 实体：匹配如 &amp;, &#169;, &#xa9; 等各种实体编码
        new Regex(@"&(#\d+|#x[0-9a-f]+|[a-zA-Z]+);", RegexOptions.Compiled | RegexOptions.IgnoreCase),
    };

    /// <summary>
    /// 检测字符串是否包含 HTML 内容
    /// </summary>
    /// <param name="input">要检测的字符串</param>
    /// <returns>
    /// 如果字符串包含以下任一 HTML 元素则返回 <c>true</c>：
    /// <list type="bullet">
    /// <item><description>HTML 标签（开始标签、结束标签、自闭合标签）</description></item>
    /// <item><description>DOCTYPE 声明</description></item>
    /// <item><description>HTML 注释</description></item>
    /// <item><description>HTML 实体（命名实体、数字实体、十六进制实体）</description></item>
    /// </list>
    /// 如果输入为 <c>null</c>、空字符串或仅包含空白字符，则返回 <c>false</c>
    /// </returns>
    /// <example>
    /// <code>
    /// // 返回 true 的示例
    /// bool result1 = HtmlHelper.ContainsHtml("&lt;div&gt;Hello&lt;/div&gt;"); // HTML 标签
    /// bool result2 = HtmlHelper.ContainsHtml("Hello &amp; World"); // HTML 实体
    /// bool result3 = HtmlHelper.ContainsHtml("&lt;!-- 注释 --&gt;"); // HTML 注释
    /// bool result4 = HtmlHelper.ContainsHtml("&lt;!DOCTYPE html&gt;"); // DOCTYPE
    /// 
    /// // 返回 false 的示例
    /// bool result5 = HtmlHelper.ContainsHtml("纯文本内容"); // 纯文本
    /// bool result6 = HtmlHelper.ContainsHtml("5 &lt; 10 &gt; 3"); // 数学表达式
    /// bool result7 = HtmlHelper.ContainsHtml(null); // null 值
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>此方法使用预编译的正则表达式进行模式匹配，具有良好的性能表现。</para>
    /// <para>检测是大小写不敏感的，能够识别各种格式的 HTML 内容。</para>
    /// <para>不会误报包含 &lt; 或 &gt; 符号但不构成有效 HTML 结构的纯文本。</para>
    /// </remarks>
    public static bool ContainsHtml(string input)
    {
        // 空值检查：null、空字符串或仅空白字符直接返回 false
        if (string.IsNullOrWhiteSpace(input)) return false;

        // 依次检查所有 HTML 模式，只要匹配任一模式即认为包含 HTML
        foreach (var pattern in HtmlPatterns)
        {
            if (pattern.IsMatch(input)) return true;
        }

        // 未匹配任何 HTML 模式，返回 false
        return false;
    }
}
