using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace LuYao.Net.Http;

/// <summary>
/// HttpClient 激活器的配置选项。
/// </summary>
public class HttpClientActivatorOption
{
    private Func<string, HttpMessageHandler> factory = static (str) => new HttpClientHandler();

    /// <summary>
    /// 获取或设置用于创建 <see cref="HttpMessageHandler"/> 的工厂方法。
    /// 参数为名称，返回对应的消息处理器实例。
    /// </summary>
    public Func<string, HttpMessageHandler> Factory
    {
        get => factory;
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            factory = value;
        }
    }

    internal static readonly TimeSpan MinimumHandlerLifetime = TimeSpan.FromSeconds(1);

    private TimeSpan _handlerLifetime = TimeSpan.FromMinutes(2);

    /// <summary>
    /// 获取或设置消息处理器的生命周期。
    /// 小于 <see cref="MinimumHandlerLifetime"/> 的值将抛出异常。
    /// </summary>
    public TimeSpan HandlerLifetime
    {
        get => _handlerLifetime;
        set
        {
            if (value != Timeout.InfiniteTimeSpan && value < MinimumHandlerLifetime)
                throw new ArgumentException(nameof(value));

            _handlerLifetime = value;
        }
    }

    /// <summary>
    /// 获取用于配置 <see cref="HttpClient"/> 的操作集合。
    /// 每个操作将在 HttpClient 创建时被依次调用。
    /// </summary>
    public IList<Action<HttpClient>> HttpClientActions { get; } = new List<Action<HttpClient>>();
}
