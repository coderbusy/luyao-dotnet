using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Limiters.TokenBucket;

public interface ITokenBucket
{
    bool TryConsume();

    bool TryConsume(long numTokens);

    void Consume();

    void Consume(long numTokens);
}
