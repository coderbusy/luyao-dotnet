using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Limiters.TokenBucket;

/// <summary>
/// 表示令牌桶的补充策略接口。
/// </summary>
public interface IRefillStrategy
{
    /// <summary>
    /// 执行补充操作，返回本次应补充的令牌数量。
    /// </summary>
    /// <returns>应补充的令牌数量。</returns>
    long Refill();
}
