using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace LuYao.Net.Http;

/// <summary>
/// HttpClient 辅助类，提供便捷的 HttpClient 实例创建与证书验证自定义支持。
/// </summary>
public static class HttpClientHelper
{
    private static readonly HttpClientActivatorOption Option = new HttpClientActivatorOption
    {
        Factory = Factory
    };

    private static HttpMessageHandler Factory(string name)
    {
        var handler = new HttpClientHandler();
#if NETSTANDARD2_0_OR_GREATER
        handler.ServerCertificateCustomValidationCallback = OnServerCertificateCustomValidationCallback;
#endif
        return handler;
    }

    private static bool OnServerCertificateCustomValidationCallback(
        HttpRequestMessage message,
        X509Certificate2 certificate,
        X509Chain chain,
        SslPolicyErrors errors)
    {
        if (ServerCertificateCustomValidationCallback == null) return errors == SslPolicyErrors.None;
        return ServerCertificateCustomValidationCallback(message, certificate, chain, errors);
    }

    private static readonly HttpClientActivator Activator = new HttpClientActivator(Option);

    /// <summary>
    /// 服务器证书自定义验证回调。
    /// 设置此委托可自定义 HTTPS 证书验证逻辑。
    /// </summary>
    public static Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ServerCertificateCustomValidationCallback { get; set; }

    /// <summary>
    /// 创建一个默认名称（空字符串）的 <see cref="HttpClient"/> 实例。
    /// </summary>
    /// <returns>新建或复用的 <see cref="HttpClient"/> 实例。</returns>
    public static HttpClient Create() => Activator.Create();

    /// <summary>
    /// 创建一个带有指定名称的 <see cref="HttpClient"/> 实例。
    /// 每个名称对应独立的处理程序生命周期。
    /// </summary>
    /// <param name="name">HttpClient 实例的名称。</param>
    /// <returns>新建或复用的 <see cref="HttpClient"/> 实例。</returns>
    public static HttpClient Create(string name) => Activator.Create(name);
}
