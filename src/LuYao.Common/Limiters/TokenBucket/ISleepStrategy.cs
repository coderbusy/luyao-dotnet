using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Limiters.TokenBucket;

/// <summary>
/// 定义休眠策略的接口。
/// 用于在令牌桶算法中控制线程等待的方式。
/// </summary>
public interface ISleepStrategy
{
    /// <summary>
    /// 执行休眠操作。
    /// 实现类应根据具体策略进行线程等待。
    /// </summary>
    void Sleep();
}
