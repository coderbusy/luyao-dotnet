using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Collections;

[TestClass]
public class EnumerableExtensionsTests
{
    [TestMethod]
    public void ToSortedDistinctList_IntListWithDuplicates_ReturnsSortedDistinctList()
    {
        var input = new List<int> { 3, 1, 2, 3, 2, 1 };
        var result = EnumerableExtensions.ToSortedDistinctList(input);

        CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, result);
    }

    [TestMethod]
    public void ToSortedDistinctList_EmptyList_ReturnsEmptyList()
    {
        var input = new List<int>();
        var result = EnumerableExtensions.ToSortedDistinctList(input);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void ToSortedDistinctList_CustomComparer_SortsAccordingToComparer()
    {
        var input = new List<string> { "a", "B", "b", "A" };
        var comparer = StringComparer.OrdinalIgnoreCase;
        var result = EnumerableExtensions.ToSortedDistinctList(input, comparer);

        CollectionAssert.AreEqual(new List<string> { "a", "B" }, result);
    }

    [TestMethod]
    public void SplitToBatch_ListWithExactBatches_ReturnsCorrectBatches()
    {
        var input = Enumerable.Range(1, 6).ToList();
        var batches = input.SplitToBatch(2).ToList();

        Assert.AreEqual(3, batches.Count);
        CollectionAssert.AreEqual(new List<int> { 1, 2 }, batches[0].ToList());
        CollectionAssert.AreEqual(new List<int> { 3, 4 }, batches[1].ToList());
        CollectionAssert.AreEqual(new List<int> { 5, 6 }, batches[2].ToList());
    }

    [TestMethod]
    public void SplitToBatch_ListWithRemainder_ReturnsLastBatchWithFewerItems()
    {
        var input = Enumerable.Range(1, 5).ToList();
        var batches = input.SplitToBatch(2).ToList();

        Assert.AreEqual(3, batches.Count);
        CollectionAssert.AreEqual(new List<int> { 1, 2 }, batches[0].ToList());
        CollectionAssert.AreEqual(new List<int> { 3, 4 }, batches[1].ToList());
        CollectionAssert.AreEqual(new List<int> { 5 }, batches[2].ToList());
    }

    [TestMethod]
    public void SplitToBatch_NullSource_ThrowsArgumentNullException()
    {
        List<int>? input = null;
        Assert.ThrowsException<ArgumentNullException>(() => input.SplitToBatch(2).ToList());
    }

    [TestMethod]
    public void SplitToBatch_BatchSizeZero_ThrowsArgumentOutOfRangeException()
    {
        var input = Enumerable.Range(1, 3);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => input.SplitToBatch(0).ToList());
    }

    [TestMethod]
    public void SplitToBatch_BatchSizeNegative_ThrowsArgumentOutOfRangeException()
    {
        var input = Enumerable.Range(1, 3);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => input.SplitToBatch(-1).ToList());
    }
}