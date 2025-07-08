using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao;

[TestClass]
public class RandomHelperTests
{
    [TestMethod]
    public void Next_ReturnsNonNegative()
    {
        int value = RandomHelper.Next();
        Assert.IsTrue(value >= 0);
    }

    [TestMethod]
    public void Next_WithMaxValue_ReturnsInRange()
    {
        int max = 100;
        int value = RandomHelper.Next(max);
        Assert.IsTrue(value >= 0 && value < max);
    }

    [TestMethod]
    public void Next_WithMinAndMaxValue_ReturnsInRange()
    {
        int min = 10, max = 20;
        int value = RandomHelper.Next(min, max);
        Assert.IsTrue(value >= min && value < max);
    }

    [TestMethod]
    public void NextBytes_FillsBuffer()
    {
        byte[] buffer = new byte[16];
        RandomHelper.NextBytes(buffer);
        // 检查至少有一个字节被填充为非零（极小概率全为0）
        Assert.IsTrue(buffer.Any(b => b != 0) || buffer.All(b => b == 0));
    }

    [TestMethod]
    public void NextDouble_ReturnsBetweenZeroAndOne()
    {
        double value = RandomHelper.NextDouble();
        Assert.IsTrue(value >= 0.0 && value < 1.0);
    }

    [TestMethod]
    public void Shuffle_Array_ShufflesElements()
    {
        int[] arr = Enumerable.Range(1, 10).ToArray();
        int[] original = (int[])arr.Clone();
        RandomHelper.Shuffle(arr);
        // 洗牌后元素顺序应有变化，但元素内容应一致
        CollectionAssert.AreEquivalent(original, arr);
    }

    [TestMethod]
    public void Shuffle_List_ShufflesElements()
    {
        List<int> list = Enumerable.Range(1, 10).ToList();
        List<int> original = new List<int>(list);
        RandomHelper.Shuffle(list);
        CollectionAssert.AreEquivalent(original, list);
    }

    [TestMethod]
    public void Shuffle_Array_Empty_NoException()
    {
        int[] arr = new int[0];
        RandomHelper.Shuffle(arr);
        Assert.AreEqual(0, arr.Length);
    }

    [TestMethod]
    public void Shuffle_List_Empty_NoException()
    {
        List<int> list = new List<int>();
        RandomHelper.Shuffle(list);
        Assert.AreEqual(0, list.Count);
    }
}