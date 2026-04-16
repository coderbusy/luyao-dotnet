namespace LuYao.Net.Http.FakeUserAgent;

/// <summary>
/// 提供从嵌入式数据集中选择随机用户代理的功能。
/// </summary>
public interface IUserAgentSelector
{
    /// <summary>
    /// 数据总量
    /// </summary>
    int Total { get; }
    /// <summary>
    /// 随机一个用户代理。
    /// </summary>
    BrowserItem? Random();
}
