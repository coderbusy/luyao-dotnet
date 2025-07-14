using System;
using System.Linq;

namespace LuYao.Net.Http.FakeUserAgent;

/// <summary>
/// 提供从嵌入式数据集中选择随机用户代理的功能。
/// </summary>
public partial class UserAgentSelector : IUserAgentSelector
{
    private readonly BrowserItem[] _browsers;

    /// <summary>
    /// 初始化 UserAgentSelector 类的新实例。
    /// </summary>
    /// <param name="browsers">浏览器项数组。</param>
    public UserAgentSelector(BrowserItem[] browsers)
    {
        this._browsers = browsers ?? throw new ArgumentNullException(nameof(browsers));
    }

    ///<inheritdoc/>
    public BrowserItem? Random()
    {
        if (_browsers.Length == 0) return null;
        var idx = RandomHelper.Next(_browsers.Length);
        return _browsers[idx];
    }

    ///<inheritdoc/>
    public int Total => _browsers.Length;

    private static UserAgentSelector? _all = null;

    /// <summary>
    /// 获取包含所有浏览器项的 UserAgentSelector 实例。
    /// </summary>
    public static UserAgentSelector All
    {
        get
        {
            if (_all == null)
            {
                lock (typeof(UserAgentSelector))
                {
                    if (_all == null) _all = new UserAgentSelector(BrowserItem.List().ToArray());
                }
            }
            return _all;
        }
    }

    /// <summary>
    /// 根据指定的条件筛选浏览器项，并返回一个新的 UserAgentSelector 实例。
    /// </summary>
    /// <param name="predicate">用于筛选浏览器项的条件。</param>
    /// <returns>包含筛选后浏览器项的 UserAgentSelector 实例。</returns>
    public static UserAgentSelector Filter(Func<BrowserItem, bool> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        var filteredBrowsers = BrowserItem.List().Where(predicate).ToArray();
        return new UserAgentSelector(filteredBrowsers);
    }
}
