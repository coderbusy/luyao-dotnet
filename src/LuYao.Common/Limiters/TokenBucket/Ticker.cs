using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Limiters.TokenBucket;

public abstract class Ticker
{
    private class SystemTicker : Ticker
    {
        public override long Read() => DateTime.Now.Ticks;
    }

    private static readonly Ticker SystemTickerInstance = new SystemTicker();

    public abstract long Read();

    public static Ticker Default() => SystemTickerInstance;
}
