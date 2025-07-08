using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Limiters.TokenBucket;

/// <summary>
/// 抽象的计时器类型，用于获取当前时间的刻度值。
/// </summary>
public abstract class Ticker
{
    private class SystemTicker : Ticker
    {
        public override long Read() => DateTime.Now.Ticks;
    }

    private static readonly Ticker SystemTickerInstance = new SystemTicker();

    /// <summary>
    /// 读取当前的时间刻度值。
    /// </summary>
    /// <returns>当前时间的刻度值（long 类型）。</returns>
    public abstract long Read();

    /// <summary>
    /// 获取默认的计时器实例。
    /// </summary>
    /// <returns>默认的 <see cref="Ticker"/> 实例。</returns>
    public static Ticker Default() => SystemTickerInstance;
}
