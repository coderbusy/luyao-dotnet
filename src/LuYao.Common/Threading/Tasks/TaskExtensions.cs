using System.Threading.Tasks;

namespace LuYao.Threading.Tasks;

public static class TaskExtensions
{
    public static bool IsCompletedSuccessfully(this Task t)
    {
#if NETSTANDARD2_1_OR_GREATER
        return t.IsCompletedSuccessfully;
#else
        return t.Status == TaskStatus.RanToCompletion && !t.IsFaulted && !t.IsCanceled;
#endif
    }
}