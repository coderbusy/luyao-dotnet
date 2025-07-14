using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace LuYao.Net.Http.FakeUserAgent;

/// <summary>
/// 表示一个浏览器的详细信息。
/// </summary>
public class BrowserItem
{
    /// <summary>
    /// 用户代理字符串。
    /// </summary>
    public string UserAgent { get; set; }

    /// <summary>
    /// 用户代理的使用比例（百分比）。
    /// </summary>
    public double Percent { get; set; }

    /// <summary>
    /// 用户代理的类型，例如 "Desktop" 或 "Mobile"。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 设备品牌，例如 "Apple" 或 "Samsung"。
    /// </summary>
    public string DeviceBrand { get; set; }

    /// <summary>
    /// 浏览器名称，例如 "Chrome" 或 "Firefox"。
    /// </summary>
    public string Browser { get; set; }

    /// <summary>
    /// 浏览器版本号，例如 "114.0.0"。
    /// </summary>
    public string BrowserVersion { get; set; }

    /// <summary>
    /// 操作系统名称，例如 "Windows" 或 "Android"。
    /// </summary>
    public string OS { get; set; }

    /// <summary>
    /// 操作系统版本号，例如 "10.0" 或 "13.0"。
    /// </summary>
    public string OSVersion { get; set; }

    /// <summary>
    /// 平台类型，例如 "x86" 或 "ARM"。
    /// </summary>
    public string Platform { get; set; }

    internal void Read(BinaryReader reader)
    {
        this.UserAgent = reader.ReadString();
        this.Percent = reader.ReadDouble();
        this.Type = reader.ReadString();
        this.DeviceBrand = reader.ReadString();
        this.Browser = reader.ReadString();
        this.BrowserVersion = reader.ReadString();
        this.OS = reader.ReadString();
        this.OSVersion = reader.ReadString();
        this.Platform = reader.ReadString();
    }

    /// <summary>
    /// 获取浏览器项的列表。
    /// </summary>
    /// <returns>返回一个包含所有浏览器项的枚举。</returns>
    public static IEnumerable<BrowserItem> List()
    {
        var res = typeof(BrowserItem).Namespace + ".Browsers.dat";
        using (var ms = typeof(BrowserItem).Assembly.GetManifestResourceStream(res))
        using (var gz = new GZipStream(ms, CompressionMode.Decompress))
        using (var reader = new BinaryReader(gz))
        {
            int total = reader.ReadInt32();
            for (int i = 0; i < total; i++)
            {
                var item = new BrowserItem();
                item.Read(reader);
                yield return item;
            }
        }
    }

    ///<inheritdoc/>
    public override string? ToString() => this.UserAgent;
}
