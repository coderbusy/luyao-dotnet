using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuYao.Collections.Generic;

[TestClass]
public class KeyedListTests
{
    // 测试用模型类
    private class TestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            if (obj is TestItem other)
            {
                return Id == other.Id && Name == other.Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }
    }

    private KeyedList<int, TestItem> _keyedList = null!;
    private List<TestItem> _testItems = null!;

    [TestInitialize]
    public void Initialize()
    {
        _testItems = new List<TestItem>
        {
            new TestItem { Id = 1, Name = "Item 1" },
            new TestItem { Id = 2, Name = "Item 2" },
            new TestItem { Id = 3, Name = "Item 3" }
        };

        _keyedList = new KeyedList<int, TestItem>(item => item.Id);
        foreach (var item in _testItems)
        {
            _keyedList.Add(item);
        }
    }

    #region 构造函数测试

    [TestMethod]
    public void Constructor_WithValidKeySelector_CreatesInstance()
    {
        // Arrange & Act
        var keyedList = new KeyedList<int, TestItem>(item => item.Id);

        // Assert
        Assert.IsNotNull(keyedList);
        Assert.AreEqual(0, keyedList.Count);
    }

    [TestMethod]
    public void Constructor_WithNullKeySelector_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new KeyedList<int, TestItem>(null!));
    }

    #endregion

    #region 索引器测试

    [TestMethod]
    public void Indexer_GetWithValidIndex_ReturnsCorrectItem()
    {
        // Arrange & Act
        var item = _keyedList[1];

        // Assert
        Assert.AreEqual(_testItems[1], item);
    }

    [TestMethod]
    public void Indexer_GetWithInvalidIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => { var item = _keyedList[-1]; });
    }

    [TestMethod]
    public void Indexer_SetWithValidIndex_UpdatesItem()
    {
        // Arrange
        var newItem = new TestItem { Id = 10, Name = "New Item" };

        // Act
        _keyedList[1] = newItem;

        // Assert
        Assert.AreEqual(newItem, _keyedList[1]);
    }

    [TestMethod]
    public void Indexer_SetWithInvalidIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var newItem = new TestItem { Id = 10, Name = "New Item" };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _keyedList[10] = newItem);
    }

    #endregion

    #region Add 方法测试

    [TestMethod]
    public void Add_Item_IncreasesCount()
    {
        // Arrange
        var keyedList = new KeyedList<int, TestItem>(item => item.Id);
        var newItem = new TestItem { Id = 1, Name = "New Item" };
        int initialCount = keyedList.Count;

        // Act
        keyedList.Add(newItem);

        // Assert
        Assert.AreEqual(initialCount + 1, keyedList.Count);
        Assert.AreEqual(newItem, keyedList[0]);
    }

    #endregion

    #region Clear 方法测试

    [TestMethod]
    public void Clear_RemovesAllItems()
    {
        // Arrange
        int initialCount = _keyedList.Count;
        Assert.AreNotEqual(0, initialCount); // 确保初始集合不为空

        // Act
        _keyedList.Clear();

        // Assert
        Assert.AreEqual(0, _keyedList.Count);
    }

    #endregion

    #region ContainsKey 方法测试

    [TestMethod]
    public void ContainsKey_ExistingKey_ReturnsTrue()
    {
        // Arrange & Act
        bool result = _keyedList.ContainsKey(1);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ContainsKey_NonExistingKey_ReturnsFalse()
    {
        // Arrange & Act
        bool result = _keyedList.ContainsKey(999);

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IndexOfKey 方法测试

    [TestMethod]
    public void IndexOfKey_ExistingKey_ReturnsCorrectIndex()
    {
        // Arrange & Act
        int index = _keyedList.IndexOfKey(2);

        // Assert
        Assert.AreEqual(1, index);
    }

    [TestMethod]
    public void IndexOfKey_NonExistingKey_ReturnsNegativeValue()
    {
        // Arrange & Act
        int index = _keyedList.IndexOfKey(999);

        // Assert
        Assert.IsTrue(index < 0);
    }

    [TestMethod]
    public void IndexOfKey_DuplicateKeys_ReturnsFirstOccurrenceIndex()
    {
        // Arrange
        var keyedList = new KeyedList<int, TestItem>(item => item.Id);
        var firstItem = new TestItem { Id = 1, Name = "First Item" };
        var secondItem = new TestItem { Id = 1, Name = "Second Item" };
        var thirdItem = new TestItem { Id = 1, Name = "Third Item" };

        // 按顺序添加三个具有相同键的元素
        keyedList.Add(firstItem);
        keyedList.Add(secondItem);
        keyedList.Add(thirdItem);

        // Act
        int index = keyedList.IndexOfKey(1);

        // Assert
        // 验证返回的是第一个添加的元素的索引
        Assert.AreEqual(0, index);
        Assert.AreEqual(firstItem, keyedList[index]);
    }

    [TestMethod]
    public void IndexOfKey_DuplicateKeysWithRandomOrder_ReturnsFirstOccurrenceIndex()
    {
        // Arrange
        var keyedList = new KeyedList<int, TestItem>(item => item.Id);

        // 创建多个元素，按非递增顺序添加，以测试是否能正确处理非按键排序的情况
        var firstItem = new TestItem { Id = 5, Name = "First Item" };
        var secondItem = new TestItem { Id = 3, Name = "Second Item" };
        var thirdItem = new TestItem { Id = 5, Name = "Third Item" }; // 与第一个元素有相同的键
        var fourthItem = new TestItem { Id = 2, Name = "Fourth Item" };
        var fifthItem = new TestItem { Id = 5, Name = "Fifth Item" }; // 与第一个和第三个元素有相同的键

        keyedList.Add(firstItem);   // 索引 0, 键 5
        keyedList.Add(secondItem);  // 索引 1, 键 3
        keyedList.Add(thirdItem);   // 索引 2, 键 5
        keyedList.Add(fourthItem);  // 索引 3, 键 2
        keyedList.Add(fifthItem);   // 索引 4, 键 5

        // Act
        int index = keyedList.IndexOfKey(5);

        // Assert
        // 验证返回的是第一个具有键 5 的元素的索引
        Assert.AreEqual(0, index);
        Assert.AreEqual(firstItem, keyedList[index]);
    }

    [TestMethod]
    public void IndexOfKey_DuplicateKeysAfterItemRemoval_ReturnsFirstRemainingOccurrence()
    {
        // Arrange
        var keyedList = new KeyedList<int, TestItem>(item => item.Id);
        var firstItem = new TestItem { Id = 1, Name = "First Item" };
        var secondItem = new TestItem { Id = 1, Name = "Second Item" };
        var thirdItem = new TestItem { Id = 1, Name = "Third Item" };

        keyedList.Add(firstItem);
        keyedList.Add(secondItem);
        keyedList.Add(thirdItem);

        // 移除第一个元素
        keyedList.Remove(firstItem);

        // Act
        int index = keyedList.IndexOfKey(1);

        // Assert
        // 验证返回的是剩余的第一个元素的索引
        Assert.AreEqual(keyedList.IndexOf(secondItem), index);
        Assert.AreEqual(secondItem, keyedList[index]);
    }

    [TestMethod]
    public void IndexOfKey_DuplicateKeysAfterCacheInvalidation_ReturnsFirstOccurrence()
    {
        // Arrange
        var keyedList = new KeyedList<int, TestItem>(item => item.Id);
        var firstItem = new TestItem { Id = 1, Name = "First Item" };
        var secondItem = new TestItem { Id = 2, Name = "Second Item" };
        var thirdItem = new TestItem { Id = 1, Name = "Third Item" };

        keyedList.Add(firstItem);
        keyedList.Add(secondItem);

        // 触发一次 IndexOfKey 以构建缓存
        keyedList.IndexOfKey(1);

        // 添加另一个具有相同键的元素，这会使缓存无效
        keyedList.Add(thirdItem);

        // Act
        int index = keyedList.IndexOfKey(1);

        // Assert
        // 验证返回的是第一个添加的元素的索引
        Assert.AreEqual(keyedList.IndexOf(firstItem), index);
        Assert.AreEqual(firstItem, keyedList[index]);
    }

    #endregion

    #region Contains 方法测试

    [TestMethod]
    public void Contains_ExistingItem_ReturnsTrue()
    {
        // Arrange & Act
        bool result = _keyedList.Contains(_testItems[1]);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Contains_NonExistingItem_ReturnsFalse()
    {
        // Arrange
        var nonExistingItem = new TestItem { Id = 999, Name = "Non-existing" };

        // Act
        bool result = _keyedList.Contains(nonExistingItem);

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region CopyTo 方法测试

    [TestMethod]
    public void CopyTo_ValidArray_CopiesAllItems()
    {
        // Arrange
        var array = new TestItem[_keyedList.Count];

        // Act
        _keyedList.CopyTo(array, 0);

        // Assert
        for (int i = 0; i < _keyedList.Count; i++)
        {
            Assert.AreEqual(_keyedList[i], array[i]);
        }
    }

    [TestMethod]
    public void CopyTo_ValidArrayWithOffset_CopiesItemsWithOffset()
    {
        // Arrange
        var array = new TestItem[_keyedList.Count + 2];

        // Act
        _keyedList.CopyTo(array, 2);

        // Assert
        for (int i = 0; i < _keyedList.Count; i++)
        {
            Assert.AreEqual(_keyedList[i], array[i + 2]);
        }
    }

    [TestMethod]
    public void CopyTo_NullArray_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => _keyedList.CopyTo(null!, 0));
    }

    [TestMethod]
    public void CopyTo_NegativeArrayIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var array = new TestItem[_keyedList.Count];

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _keyedList.CopyTo(array, -1));
    }

    [TestMethod]
    public void CopyTo_InsufficientArraySpace_ThrowsArgumentException()
    {
        // Arrange
        var array = new TestItem[_keyedList.Count - 1];

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _keyedList.CopyTo(array, 0));
    }

    #endregion

    #region GetEnumerator 方法测试

    [TestMethod]
    public void GetEnumerator_EnumeratesAllItems()
    {
        // Arrange
        var items = new List<TestItem>();

        // Act
        foreach (var item in _keyedList)
        {
            items.Add(item);
        }

        // Assert
        Assert.AreEqual(_keyedList.Count, items.Count);
        for (int i = 0; i < _keyedList.Count; i++)
        {
            Assert.AreEqual(_keyedList[i], items[i]);
        }
    }

    #endregion

    #region IndexOf 方法测试

    [TestMethod]
    public void IndexOf_ExistingItem_ReturnsCorrectIndex()
    {
        // Arrange & Act
        int index = _keyedList.IndexOf(_testItems[1]);

        // Assert
        Assert.AreEqual(1, index);
    }

    [TestMethod]
    public void IndexOf_NonExistingItem_ReturnsNegativeOne()
    {
        // Arrange
        var nonExistingItem = new TestItem { Id = 999, Name = "Non-existing" };

        // Act
        int index = _keyedList.IndexOf(nonExistingItem);

        // Assert
        Assert.AreEqual(-1, index);
    }

    #endregion

    #region Insert 方法测试

    [TestMethod]
    public void Insert_ValidIndex_InsertsItemAtCorrectPosition()
    {
        // Arrange
        var newItem = new TestItem { Id = 4, Name = "Item 4" };
        int initialCount = _keyedList.Count;

        // Act
        _keyedList.Insert(1, newItem);

        // Assert
        Assert.AreEqual(initialCount + 1, _keyedList.Count);
        Assert.AreEqual(newItem, _keyedList[1]);
        Assert.AreEqual(_testItems[1], _keyedList[2]);
    }

    [TestMethod]
    public void Insert_NegativeIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var newItem = new TestItem { Id = 4, Name = "Item 4" };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _keyedList.Insert(-1, newItem));
    }

    [TestMethod]
    public void Insert_IndexGreaterThanCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var newItem = new TestItem { Id = 4, Name = "Item 4" };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _keyedList.Insert(_keyedList.Count + 1, newItem));
    }

    #endregion

    #region Remove 方法测试

    [TestMethod]
    public void Remove_ExistingItem_RemovesItemAndReturnsTrue()
    {
        // Arrange
        int initialCount = _keyedList.Count;
        var itemToRemove = _testItems[1];

        // Act
        bool result = _keyedList.Remove(itemToRemove);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(initialCount - 1, _keyedList.Count);
        Assert.IsFalse(_keyedList.Contains(itemToRemove));
    }

    [TestMethod]
    public void Remove_NonExistingItem_ReturnsFalse()
    {
        // Arrange
        int initialCount = _keyedList.Count;
        var nonExistingItem = new TestItem { Id = 999, Name = "Non-existing" };

        // Act
        bool result = _keyedList.Remove(nonExistingItem);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(initialCount, _keyedList.Count);
    }

    #endregion

    #region RemoveAt 方法测试

    [TestMethod]
    public void RemoveAt_ValidIndex_RemovesItemAtIndex()
    {
        // Arrange
        int initialCount = _keyedList.Count;
        var itemToRemove = _keyedList[1];

        // Act
        _keyedList.RemoveAt(1);

        // Assert
        Assert.AreEqual(initialCount - 1, _keyedList.Count);
        Assert.IsFalse(_keyedList.Contains(itemToRemove));
    }

    [TestMethod]
    public void RemoveAt_NegativeIndex_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _keyedList.RemoveAt(-1));
    }

    [TestMethod]
    public void RemoveAt_IndexEqualToCount_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _keyedList.RemoveAt(_keyedList.Count));
    }

    #endregion

    #region 属性测试

    [TestMethod]
    public void Count_ReturnsCorrectNumberOfItems()
    {
        // Arrange & Act & Assert
        Assert.AreEqual(_testItems.Count, _keyedList.Count);
    }

    [TestMethod]
    public void IsReadOnly_ReturnsFalse()
    {
        // Arrange & Act & Assert
        Assert.IsFalse(_keyedList.IsReadOnly);
    }

    [TestMethod]
    public void KeySelector_ReturnsCorrectFunction()
    {
        // Arrange
        var item = new TestItem { Id = 5, Name = "Test" };

        // Act
        var key = _keyedList.KeySelector(item);

        // Assert
        Assert.AreEqual(5, key);
    }

    #endregion

    #region Read 方法测试

    [TestMethod]
    public void Read_ExistingKey_ReturnsAllMatchingItems()
    {
        // Arrange
        var keyedList = new KeyedList<int, TestItem>(item => item.Id);
        var item1 = new TestItem { Id = 1, Name = "Item 1A" };
        var item2 = new TestItem { Id = 2, Name = "Item 2" };
        var item3 = new TestItem { Id = 1, Name = "Item 1B" };
        var item4 = new TestItem { Id = 3, Name = "Item 3" };
        var item5 = new TestItem { Id = 1, Name = "Item 1C" };

        keyedList.Add(item1);
        keyedList.Add(item2);
        keyedList.Add(item3);
        keyedList.Add(item4);
        keyedList.Add(item5);

        // Act
        var result = keyedList.Read(1).ToList();

        // Assert
        Assert.AreEqual(3, result.Count);
        CollectionAssert.Contains(result, item1);
        CollectionAssert.Contains(result, item3);
        CollectionAssert.Contains(result, item5);
    }

    [TestMethod]
    public void Read_NonExistingKey_ReturnsEmptyCollection()
    {
        // Arrange
        var nonExistingKey = 999;

        // Act
        var result = _keyedList.Read(nonExistingKey).ToList();

        // Assert
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Read_EmptyList_ReturnsEmptyCollection()
    {
        // Arrange
        var keyedList = new KeyedList<int, TestItem>(item => item.Id);

        // Act
        var result = keyedList.Read(1).ToList();

        // Assert
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Read_PreservesOriginalOrder()
    {
        // Arrange
        var keyedList = new KeyedList<int, TestItem>(item => item.Id);
        var item1 = new TestItem { Id = 1, Name = "First" };
        var item2 = new TestItem { Id = 2, Name = "Second" };
        var item3 = new TestItem { Id = 1, Name = "Third" };
        var item4 = new TestItem { Id = 1, Name = "Fourth" };

        keyedList.Add(item1);
        keyedList.Add(item2);
        keyedList.Add(item3);
        keyedList.Add(item4);

        // Act
        var result = keyedList.Read(1).ToList();

        // Assert
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(item1, result[0]);
        Assert.AreEqual(item3, result[1]);
        Assert.AreEqual(item4, result[2]);
    }

    [TestMethod]
    public void Read_AfterCacheInvalidation_ReturnsCorrectItems()
    {
        // Arrange
        var keyedList = new KeyedList<int, TestItem>(item => item.Id);
        var item1 = new TestItem { Id = 1, Name = "Item 1" };
        var item2 = new TestItem { Id = 2, Name = "Item 2" };

        keyedList.Add(item1);
        keyedList.Add(item2);

        // 触发一次 Read 以构建缓存
        keyedList.Read(1).ToList();

        // 添加另一个具有相同键的元素，这会使缓存无效
        var item3 = new TestItem { Id = 1, Name = "Item 3" };
        keyedList.Add(item3);

        // Act
        var result = keyedList.Read(1).ToList();

        // Assert
        Assert.AreEqual(2, result.Count);
        CollectionAssert.Contains(result, item1);
        CollectionAssert.Contains(result, item3);
    }

    [TestMethod]
    public void Read_AfterModification_ReturnsUpdatedItems()
    {
        // Arrange
        var keyedList = new KeyedList<int, TestItem>(item => item.Id);
        var item1 = new TestItem { Id = 1, Name = "Item 1" };
        var item2 = new TestItem { Id = 1, Name = "Item 2" };
        var item3 = new TestItem { Id = 2, Name = "Item 3" };

        keyedList.Add(item1);
        keyedList.Add(item2);
        keyedList.Add(item3);

        // 初始检查
        Assert.AreEqual(2, keyedList.Read(1).Count());

        // 移除一个元素
        keyedList.Remove(item1);

        // Act
        var result = keyedList.Read(1).ToList();

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(item2, result[0]);
    }

    [TestMethod]
    public void Read_WithCustomComparer_ReturnsMatchingItems()
    {
        // Arrange
        var stringComparer = StringComparer.OrdinalIgnoreCase;
        var keyedList = new KeyedList<string, TestItem>(item => item.Name, stringComparer);

        var item1 = new TestItem { Id = 1, Name = "Test" };
        var item2 = new TestItem { Id = 2, Name = "Different" };
        var item3 = new TestItem { Id = 3, Name = "test" }; // 仅大小写不同

        keyedList.Add(item1);
        keyedList.Add(item2);
        keyedList.Add(item3);

        // Act
        var result = keyedList.Read("TEST").ToList(); // 搜索大写形式

        // Assert
        Assert.AreEqual(2, result.Count);
        CollectionAssert.Contains(result, item1);
        CollectionAssert.Contains(result, item3);
    }

    [TestMethod]
    public void Read_ItemsAddedRandomly_ReturnsSortedByAdditionOrder()
    {
        // Arrange
        var keyedList = new KeyedList<int, TestItem>(item => item.Id);

        // 按随机顺序添加元素
        var item3 = new TestItem { Id = 5, Name = "Item 3" };
        var item1 = new TestItem { Id = 5, Name = "Item 1" };
        var item4 = new TestItem { Id = 2, Name = "Item 4" };
        var item2 = new TestItem { Id = 5, Name = "Item 2" };

        keyedList.Add(item1);  // 索引 0
        keyedList.Add(item4);  // 索引 1
        keyedList.Add(item2);  // 索引 2
        keyedList.Add(item3);  // 索引 3

        // Act
        var result = keyedList.Read(5).ToList();

        // Assert
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(item1, result[0]); // 第一个添加的 ID=5 的元素
        Assert.AreEqual(item2, result[1]); // 第二个添加的 ID=5 的元素
        Assert.AreEqual(item3, result[2]); // 第三个添加的 ID=5 的元素
    }

    #endregion
}
