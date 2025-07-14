using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Net.Http.FakeUserAgent;

[TestClass]
public class UserAgentSelectorTests
{
    [TestMethod]
    public void Random_NoBrowsers_ReturnsNull()
    {
        // Arrange
        var selector = new UserAgentSelector(Array.Empty<BrowserItem>());

        // Act
        var result = selector.Random();

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Random_WithBrowsers_ReturnsRandomBrowserItem()
    {
        // Arrange
        var browsers = new[] { new BrowserItem { UserAgent = "UA1" }, new BrowserItem { UserAgent = "UA2" } };
        var selector = new UserAgentSelector(browsers);

        // Act
        var result = selector.Random();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(browsers.Contains(result));
    }

    [TestMethod]
    public void Total_WithBrowsers_ReturnsCorrectCount()
    {
        // Arrange
        var browsers = new[] { new BrowserItem { UserAgent = "UA1" }, new BrowserItem { UserAgent = "UA2" } };
        var selector = new UserAgentSelector(browsers);

        // Act
        var total = selector.Total;

        // Assert
        Assert.AreEqual(2, total);
    }

    [TestMethod]
    public void Total_NoBrowsers_ReturnsZero()
    {
        // Arrange
        var selector = new UserAgentSelector(Array.Empty<BrowserItem>());

        // Act
        var total = selector.Total;

        // Assert
        Assert.AreEqual(0, total);
    }

    [TestMethod]
    public void All_CreatesSingletonInstance_ReturnsSameInstance()
    {
        // Act
        var instance1 = UserAgentSelector.All;
        var instance2 = UserAgentSelector.All;

        // Assert
        Assert.AreSame(instance1, instance2);
    }

    [TestMethod]
    public void All_Random_ReturnsRandomBrowserItem()
    {
        // Arrange
        var selector = UserAgentSelector.All;

        // Act
        var result = selector.Random();

        // Assert
        Assert.IsNotNull(result);
    }
}
