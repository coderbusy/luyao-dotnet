using System.Threading.Tasks;

namespace LuYao.Threading.Tasks;

/// <summary>
/// 为 <see cref="Task"/> 提供扩展方法。
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// 确定指定的 <see cref="Task"/> 是否已成功完成。
    /// </summary>
    /// <param name="t">要检查的任务。</param>
    /// <returns>如果任务已成功完成，则为 <c>true</c>；否则为 <c>false</c>。</returns>
    public static bool IsCompletedSuccessfully(this Task t)
    {
#if NETSTANDARD2_1_OR_GREATER
        return t.IsCompletedSuccessfully;
#else
        return t.Status == TaskStatus.RanToCompletion && !t.IsFaulted && !t.IsCanceled;
#endif
    }
}