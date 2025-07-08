using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Limiters.TokenBucket;

/// <summary>
/// 令牌桶接口，定义了令牌桶限流器的基本操作。
/// </summary>
public interface ITokenBucket
{
    /// <summary>
    /// 尝试消耗一个令牌。
    /// </summary>
    /// <returns>如果成功消耗令牌，则返回 true；否则返回 false。</returns>
    bool TryConsume();

    /// <summary>
    /// 尝试消耗指定数量的令牌。
    /// </summary>
    /// <param name="numTokens">要消耗的令牌数量。</param>
    /// <returns>如果成功消耗指定数量的令牌，则返回 true；否则返回 false。</returns>
    bool TryConsume(long numTokens);

    /// <summary>
    /// 消耗一个令牌（如果没有足够的令牌则阻塞直到可用）。
    /// </summary>
    void Consume();

    /// <summary>
    /// 消耗指定数量的令牌（如果没有足够的令牌则阻塞直到可用）。
    /// </summary>
    /// <param name="numTokens">要消耗的令牌数量。</param>
    void Consume(long numTokens);
}
