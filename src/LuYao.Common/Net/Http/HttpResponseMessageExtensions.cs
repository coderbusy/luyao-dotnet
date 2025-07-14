using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LuYao.Net.Http;

/// <summary>
/// HttpResponseMessage 扩展方法类
/// </summary>
public static class HttpResponseMessageExtensions
{
    private static Regex CharSetRegex = new Regex(
        @"charset=(?<encoding>['""\w-]+)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    /// <summary>
    /// 异步读取 HttpResponseMessage 的内容为字符串，并自动检测和使用正确的编码（优先使用响应头或 HTML 元素中的 charset）。
    /// </summary>
    /// <param name="response">HTTP 响应消息对象。</param>
    /// <returns>以字符串形式返回响应内容。</returns>
    /// <exception cref="ArgumentNullException">当 response 为 null 时抛出。</exception>
    public static async Task<string> ReadAsHtmlAsync(this HttpResponseMessage response)
    {
        if (response == null) throw new ArgumentNullException(nameof(response));
        Encoding? encoding = null;
        var type = response.Content.Headers.ContentType;
        if (type != null && !string.IsNullOrWhiteSpace(type.CharSet)) encoding = GetEncoding(type.CharSet.Trim(' ', '\'', '"'));
        var data = await response.Content.ReadAsByteArrayAsync();
        var buffer = Encoding.ASCII.GetString(data, 0, Math.Min(0x200, data.Length));
        var m = CharSetRegex.Match(buffer);
        if (m.Success) encoding = GetEncoding(m.Groups["encoding"].Value.Trim(' ', '\'', '"'));
        return (encoding ?? Encoding.UTF8).GetString(data);
    }

    private static Encoding GetEncoding(string name)
    {
        switch (name.ToLowerInvariant())
        {
            case "utf8": return Encoding.UTF8;
        }
        return Encoding.GetEncoding(name);
    }
}
