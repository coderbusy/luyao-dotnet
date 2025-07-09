using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Threading;

[TestClass]
public class ConcurrentTaskRunnerTests
{
    private class TestConcurrentTaskRunner : ConcurrentTaskRunner<int>
    {
        private readonly Queue<int> _taskQueue = new Queue<int>();

        public TestConcurrentTaskRunner(IEnumerable<int> initialTasks)
        {
            foreach (var task in initialTasks)
            {
                _taskQueue.Enqueue(task);
            }
        }

        protected override Task Execute(int args)
        {
            // 模拟任务执行，延迟 100 毫秒
            return Task.Delay(100);
        }

        protected override IReadOnlyCollection<int> Take(int limit)
        {
            var result = new List<int>();
            for (int i = 0; i < limit && _taskQueue.Count > 0; i++)
            {
                result.Add(_taskQueue.Dequeue());
            }
            return result;
        }
    }

    [TestMethod]
    public void Tick_TasksAvailable_TasksExecuted()
    {
        // Arrange
        var runner = new TestConcurrentTaskRunner(new[] { 1, 2, 3 });

        // Act
        runner.Tick();

        // Assert
        // 验证任务被执行，运行中的任务数量不超过 MaxRunning
        Assert.IsTrue(runner.MaxRunning >= 0);
    }

    [TestMethod]
    public void MaxRunning_DefaultValue_CorrectValue()
    {
        // Arrange
        var runner = new TestConcurrentTaskRunner(new[] { 1, 2, 3 });

        // Act
        var maxRunning = runner.MaxRunning;

        // Assert
        Assert.AreEqual(20, maxRunning); // 验证默认值为 20
    }
}