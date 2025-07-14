using System.Net;

namespace LuYao.Net;

[TestClass]
public class IPAddressExtensionsTests
{
    [TestMethod]
    public void IsPrivateIPAddress_LoopbackAddress_ReturnsTrue()
    {
        var ip = IPAddress.Loopback;
        Assert.IsTrue(ip.IsPrivateIPAddress());
    }

    [TestMethod]
    public void IsPrivateIPAddress_IPv4MappedToIPv6_ReturnsTrue()
    {
        var ip = IPAddress.Parse("::ffff:192.168.1.1");
        Assert.IsTrue(ip.IsPrivateIPAddress());
    }

    [TestMethod]
    public void IsPrivateIPAddress_PrivateIPv4Address_ReturnsTrue()
    {
        var ip = IPAddress.Parse("192.168.1.1");
        Assert.IsTrue(ip.IsPrivateIPAddress());
    }

    [TestMethod]
    public void IsPrivateIPAddress_PublicIPv4Address_ReturnsFalse()
    {
        var ip = IPAddress.Parse("8.8.8.8");
        Assert.IsFalse(ip.IsPrivateIPAddress());
    }

    [TestMethod]
    public void IsPrivateIPAddress_PrivateIPv6Address_ReturnsTrue()
    {
        var ip = IPAddress.Parse("fd00::1");
        Assert.IsTrue(ip.IsPrivateIPAddress());
    }

    [TestMethod]
    public void IsPrivateIPAddress_PublicIPv6Address_ReturnsFalse()
    {
        var ip = IPAddress.Parse("2001:4860:4860::8888");
        Assert.IsFalse(ip.IsPrivateIPAddress());
    }

    [TestMethod]
    public void IsPrivateIPAddress_IPv6LinkLocalAddress_ReturnsTrue()
    {
        var ip = IPAddress.Parse("fe80::1");
        Assert.IsTrue(ip.IsPrivateIPAddress());
    }

    [TestMethod]
    public void IsPrivateIPAddress_IPv6MulticastAddress_ReturnsTrue()
    {
        var ip = IPAddress.Parse("ff02::1");
        Assert.IsTrue(ip.IsPrivateIPAddress());
    }
}