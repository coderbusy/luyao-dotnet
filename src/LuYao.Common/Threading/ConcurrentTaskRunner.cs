using LuYao.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuYao.Threading;

/// <summary>
/// 表示一个并发任务运行器的抽象基类，用于管理和执行并发任务。
/// </summary>
public abstract class ConcurrentTaskRunner<T>
{
    private readonly List<Task> _running = new List<Task>();

    private long _ticks = 0;
    private int _schedule = 0;
    private int _done = 0;
    private int _success = 0;
    private int _fail = 0;

    /// <summary>
    /// 获取当前的滴答计数。
    /// </summary>
    public long Ticks => _ticks;
    /// <summary>
    /// 获取已调度的任务数量。
    /// </summary>
    public int Schedule => _schedule;
    /// <summary>
    /// 获取已完成的任务数量。
    /// </summary>
    public int Done => _done;
    /// <summary>
    /// 获取成功完成的任务数量。
    /// </summary>
    public int Success => _success;
    /// <summary>
    /// 获取失败的任务数量。
    /// </summary>
    public int Fail => _fail;

    /// <summary>
    /// 抽象方法，执行指定的任务参数。
    /// </summary>
    /// <param name="args">任务参数。</param>
    /// <returns>表示任务的异步操作。</returns>
    protected abstract Task Execute(T args);

    /// <summary>
    /// 抽象方法，获取指定数量的任务参数。
    /// </summary>
    /// <param name="limit">获取的任务参数数量限制。</param>
    /// <returns>只读集合，包含任务参数。</returns>
    protected abstract IReadOnlyCollection<T> Take(int limit);

    /// <summary>
    /// 获取允许同时运行的最大任务数量。
    /// </summary>
    public virtual int MaxRunning => 20;

    /// <summary>
    /// 当任务成功完成时调用。
    /// </summary>
    /// <param name="tasks">成功完成的任务列表。</param>
    protected virtual void OnSuccessfully(IReadOnlyList<Task> tasks) { }

    /// <summary>
    /// 当任务失败时调用。
    /// </summary>
    /// <param name="tasks">失败的任务列表。</param>
    protected virtual void OnFailed(IReadOnlyList<Task> tasks) { }

    /// <summary>
    /// 执行任务调度逻辑，管理任务的运行和完成状态。
    /// </summary>
    public virtual void Tick()
    {
        Interlocked.Increment(ref _ticks);
        lock (this)
        {
            var done = _running.Where(t => t.IsCompleted).ToArray();
            if (done.Any())
            {
                var succ = new List<Task>();
                var fail = new List<Task>();

                foreach (var task in done)
                {
                    Interlocked.Increment(ref _done);
                    _running.Remove(task);
                    if (task.IsCompletedSuccessfully())
                    {
                        Interlocked.Increment(ref _success);
                        succ.Add(task);
                    }
                    else
                    {
                        Interlocked.Increment(ref _fail);
                        fail.Add(task);
                    }
                }

                if (succ.Any()) OnSuccessfully(succ);
                if (fail.Any()) OnFailed(fail);
            }
            if (_running.Count < MaxRunning)
            {
                var limit = MaxRunning - _running.Count;
                if (limit > 0)
                {
                    var args = Take(limit);
                    if (args is { Count: > 0 })
                    {
                        foreach (var arg in args)
                        {
                            Interlocked.Increment(ref _schedule);
                            _running.Add(Task.Run(() => Execute(arg)));
                        }
                    }
                }
            }
        }
    }
}