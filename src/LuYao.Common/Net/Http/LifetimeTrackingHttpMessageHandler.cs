using System.Net.Http;

namespace LuYao.Net.Http;

// 这是一个标记类，用于检查底层处理程序是否应被释放。HttpClient
// 共享对此类实例的引用，当它超出作用域时，内部处理程序就可以被释放。
internal sealed class LifetimeTrackingHttpMessageHandler : DelegatingHandler
{
    public LifetimeTrackingHttpMessageHandler(HttpMessageHandler innerHandler)
        : base(innerHandler) { }

    protected override void Dispose(bool disposing)
    {
        // 此对象的生命周期由 ActiveHandlerTrackingEntry 单独跟踪
    }
}
