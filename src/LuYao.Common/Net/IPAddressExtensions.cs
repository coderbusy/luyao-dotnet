using System.Net;
using System.Net.Sockets;

namespace LuYao.Net;

/// <summary>
/// Provides extension methods for the <see cref="IPAddress"/> class.
/// </summary>
public static class IPAddressExtensions
{
    /// <summary>
    /// 判断IP是否是私有地址
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public static bool IsPrivateIPAddress(this IPAddress ip)
    {
        if (IPAddress.IsLoopback(ip)) return true;

        ip = ip.IsIPv4MappedToIPv6 ? ip.MapToIPv4() : ip;
        byte[] bytes = ip.GetAddressBytes();

        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            return bytes[0] switch
            {
                10 => true,
                100 when bytes[1] >= 64 && bytes[1] <= 127 => true,
                169 when bytes[1] == 254 => true,
                172 when bytes[1] >= 16 && bytes[1] <= 31 => true, // Adjusted for full private range
                192 when bytes[1] == 168 => true,
                _ => false
            };
        }

        if (ip.AddressFamily == AddressFamily.InterNetworkV6)
        {
            return ip.IsIPv6Teredo || ip.IsIPv6LinkLocal || ip.IsIPv6Multicast || ip.IsIPv6SiteLocal || bytes[0] == 0 || bytes[0] >= 252;
        }

        return false;
    }
}
