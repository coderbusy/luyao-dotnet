using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace LuYao.Collections.Generic;

[TestClass]
public class ValueArrayEqualityComparerTests
{
    #region Integer Array Tests

    [TestMethod]
    public void Equals_SameReference_ReturnsTrue()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<int>();
        int[] array = { 1, 2, 3 };

        // Act
        bool result = comparer.Equals(array, array);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_BothNull_ReturnsTrue()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<int>();

        // Act
        bool result = comparer.Equals(null, null);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_FirstNull_ReturnsFalse()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<int>();
        int[] array = { 1, 2, 3 };

        // Act
        bool result = comparer.Equals(null, array);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_SecondNull_ReturnsFalse()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<int>();
        int[] array = { 1, 2, 3 };

        // Act
        bool result = comparer.Equals(array, null);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_DifferentLengths_ReturnsFalse()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<int>();
        int[] array1 = { 1, 2, 3 };
        int[] array2 = { 1, 2 };

        // Act
        bool result = comparer.Equals(array1, array2);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_SameContentDifferentArrays_ReturnsTrue()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<int>();
        int[] array1 = { 1, 2, 3 };
        int[] array2 = { 1, 2, 3 };

        // Act
        bool result = comparer.Equals(array1, array2);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_DifferentContent_ReturnsFalse()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<int>();
        int[] array1 = { 1, 2, 3 };
        int[] array2 = { 1, 2, 4 };

        // Act
        bool result = comparer.Equals(array1, array2);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_EmptyArrays_ReturnsTrue()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<int>();
        int[] array1 = { };
        int[] array2 = { };

        // Act
        bool result = comparer.Equals(array1, array2);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetHashCode_NullArray_ReturnsZero()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<int>();

        // Act
        int hash = comparer.GetHashCode(null);

        // Assert
        Assert.AreEqual(0, hash);
    }

    [TestMethod]
    public void GetHashCode_EmptyArray_ReturnsNonZero()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<int>();
        int[] array = { };

        // Act
        int hash = comparer.GetHashCode(array);

        // Assert
        Assert.AreEqual(17, hash);
    }

    [TestMethod]
    public void GetHashCode_SameContentDifferentArrays_ReturnsSameHashCode()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<int>();
        int[] array1 = { 1, 2, 3 };
        int[] array2 = { 1, 2, 3 };

        // Act
        int hash1 = comparer.GetHashCode(array1);
        int hash2 = comparer.GetHashCode(array2);

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void GetHashCode_DifferentContent_ReturnsDifferentHashCode()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<int>();
        int[] array1 = { 1, 2, 3 };
        int[] array2 = { 1, 2, 4 };

        // Act
        int hash1 = comparer.GetHashCode(array1);
        int hash2 = comparer.GetHashCode(array2);

        // Assert
        Assert.AreNotEqual(hash1, hash2);
    }

    #endregion

    #region Dictionary Integration Tests

    [TestMethod]
    public void DictionaryWithComparer_SameContentKeys_FindsValue()
    {
        // Arrange
        var dict = new Dictionary<int[], string>(new ValueArrayEqualityComparer<int>());
        int[] key1 = { 1, 2, 3 };
        int[] key2 = { 1, 2, 3 };

        // Act
        dict[key1] = "Value1";
        string result = dict[key2];

        // Assert
        Assert.AreEqual("Value1", result);
    }

    [TestMethod]
    public void DictionaryWithComparer_DifferentContentKeys_DoesNotFindValue()
    {
        // Arrange
        var dict = new Dictionary<int[], string>(new ValueArrayEqualityComparer<int>());
        int[] key1 = { 1, 2, 3 };
        int[] key2 = { 1, 2, 4 };

        // Act
        dict[key1] = "Value1";
        bool exists = dict.ContainsKey(key2);

        // Assert
        Assert.IsFalse(exists);
    }

    #endregion

    #region Other Value Types Tests

    [TestMethod]
    public void Equals_DoubleArrays_SameContent_ReturnsTrue()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<double>();
        double[] array1 = { 1.0, 2.5, 3.7 };
        double[] array2 = { 1.0, 2.5, 3.7 };

        // Act
        bool result = comparer.Equals(array1, array2);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetHashCode_DoubleArrays_SameContent_ReturnsSameHashCode()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<double>();
        double[] array1 = { 1.0, 2.5, 3.7 };
        double[] array2 = { 1.0, 2.5, 3.7 };

        // Act
        int hash1 = comparer.GetHashCode(array1);
        int hash2 = comparer.GetHashCode(array2);

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void Equals_LongArrays_SameContent_ReturnsTrue()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<long>();
        long[] array1 = { 1L, 2L, 3L };
        long[] array2 = { 1L, 2L, 3L };

        // Act
        bool result = comparer.Equals(array1, array2);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_StringArrays_SameContent_ReturnsTrue()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<string>();
        string[] array1 = { "a", "b", "c" };
        string[] array2 = { "a", "b", "c" };

        // Act
        bool result = comparer.Equals(array1, array2);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetHashCode_StringArrays_SameContent_ReturnsSameHashCode()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<string>();
        string[] array1 = { "a", "b", "c" };
        string[] array2 = { "a", "b", "c" };

        // Act
        int hash1 = comparer.GetHashCode(array1);
        int hash2 = comparer.GetHashCode(array2);

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void Equals_BoolArrays_SameContent_ReturnsTrue()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<bool>();
        bool[] array1 = { true, false, true };
        bool[] array2 = { true, false, true };

        // Act
        bool result = comparer.Equals(array1, array2);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_CharArrays_SameContent_ReturnsTrue()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<char>();
        char[] array1 = { 'a', 'b', 'c' };
        char[] array2 = { 'a', 'b', 'c' };

        // Act
        bool result = comparer.Equals(array1, array2);

        // Assert
        Assert.IsTrue(result);
    }

    #endregion

    #region Edge Cases

    [TestMethod]
    public void GetHashCode_LargeArray_ReturnsConsistentHashCode()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<int>();
        int[] largeArray = new int[1000];
        for (int i = 0; i < largeArray.Length; i++)
        {
            largeArray[i] = i;
        }

        // Act
        int hash1 = comparer.GetHashCode(largeArray);
        int hash2 = comparer.GetHashCode(largeArray);

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void Equals_OrderMatters_ReturnsFalse()
    {
        // Arrange
        var comparer = new ValueArrayEqualityComparer<int>();
        int[] array1 = { 1, 2, 3 };
        int[] array2 = { 3, 2, 1 };

        // Act
        bool result = comparer.Equals(array1, array2);

        // Assert
        Assert.IsFalse(result);
    }

    #endregion
}
