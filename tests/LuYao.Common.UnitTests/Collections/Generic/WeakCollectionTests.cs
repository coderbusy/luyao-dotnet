using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Collections.Generic;

[TestClass]
public class WeakCollectionTests
{
    [TestMethod]
    public void Add_NullItem_ThrowsArgumentNullException()
    {
        // Arrange
        var collection = new WeakCollection<object>();

        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => collection.Add(null));
    }

    [TestMethod]
    public void Add_ValidItem_ItemAddedSuccessfully()
    {
        // Arrange
        var collection = new WeakCollection<object>();
        var item = new object();

        // Act
        collection.Add(item);

        // Assert
        var items = collection.TryGetItems(x => true);
        Assert.AreEqual(1, items.Length);
        Assert.AreSame(item, items[0]);
    }

    [TestMethod]
    public void Remove_ItemExists_ItemRemovedSuccessfully()
    {
        // Arrange
        var collection = new WeakCollection<object>();
        var item = new object();
        collection.Add(item);

        // Act
        var result = collection.Remove(item);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(0, collection.TryGetItems(x => true).Length);
    }

    [TestMethod]
    public void Remove_ItemDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var collection = new WeakCollection<object>();
        var item = new object();

        // Act
        var result = collection.Remove(item);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void TryGetItems_FilterApplied_ReturnsFilteredItems()
    {
        // Arrange
        var collection = new WeakCollection<string>();
        collection.Add("Item1");
        collection.Add("Item2");

        // Act
        var items = collection.TryGetItems(x => x == "Item1");

        // Assert
        Assert.AreEqual(1, items.Length);
        Assert.AreEqual("Item1", items[0]);
    }

    [TestMethod]
    public void TryGetItems_NoFilter_ReturnsAllItems()
    {
        // Arrange
        var collection = new WeakCollection<string>();
        collection.Add("Item1");
        collection.Add("Item2");

        // Act
        var items = collection.TryGetItems();

        // Assert
        Assert.AreEqual(2, items.Length);
        CollectionAssert.Contains(items, "Item1");
        CollectionAssert.Contains(items, "Item2");
    }

    [TestMethod]
    public void GetOrAdd_ItemExists_ReturnsExistingItem()
    {
        // Arrange
        var collection = new WeakCollection<string>();
        collection.Add("ExistingItem");

        // Act
        var item = collection.GetOrAdd(x => x == "ExistingItem", () => "NewItem");

        // Assert
        Assert.AreEqual("ExistingItem", item);
    }

    [TestMethod]
    public void GetOrAdd_ItemDoesNotExist_AddsAndReturnsNewItem()
    {
        // Arrange
        var collection = new WeakCollection<string>();

        // Act
        var item = collection.GetOrAdd(x => x == "NewItem", () => "NewItem");

        // Assert
        Assert.AreEqual("NewItem", item);
        var items = collection.TryGetItems();
        Assert.AreEqual(1, items.Length);
        Assert.AreEqual("NewItem", items[0]);
    }

    [TestMethod]
    public void Clear_CollectionCleared_NoItemsRemain()
    {
        // Arrange
        var collection = new WeakCollection<object>();
        collection.Add(new object());
        collection.Add(new object());

        // Act
        collection.Clear();

        // Assert
        Assert.AreEqual(0, collection.TryGetItems(x => true).Length);
    }

    [TestMethod]
    public void GarbageCollection_WeakReferencesCollected_RemainingItemsCorrect()
    {
        // Arrange
        var collection = new WeakCollection<object>();
        var strongReference = new object();
        collection.Add(new object()); // Weak reference only
        collection.Add(strongReference); // Strong reference

        // Act
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Assert
        var items = collection.TryGetItems(x => true);
        Assert.AreEqual(2, items.Length);
        Assert.AreSame(strongReference, items[1]);
    }
}